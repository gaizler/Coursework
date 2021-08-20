using DALTest;
using DataLibrary;
using GalaSoft.MvvmLight.CommandWpf;
using Repository;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;
using System.Xml.Serialization;
using TestLibrary;

namespace TestServer.ViewModels
{
    public class MainServerViewModel
    {
        #region Variables
        IPAddress groupAddress;
        int localPort = 5998, remotePort = 5679, ttl = 32;
        UdpClient udpClient;
        IPEndPoint remoteEP;
        DispatcherTimer timer = new DispatcherTimer();

        private const int passwordPort = 8888;


        public Uri frameSource { get; set; }

        public RelayCommand ClosingCommand { get; }
        #endregion
        public MainServerViewModel()
        {
            frameSource = new Uri(string.Format("../Pages/DefaultPage.xaml"), UriKind.RelativeOrAbsolute);

            ClosingCommand = new RelayCommand(OnClosing);

            groupAddress = IPAddress.Parse("234.5.5.10");
            udpClient = new UdpClient(localPort);
            udpClient.JoinMulticastGroup(groupAddress, ttl);
            remoteEP = new IPEndPoint(groupAddress, remotePort);

            timer.Start();
            timer.Interval = new TimeSpan(0, 0, 1);
            timer.Tick += Timer_Tick;

            Task.Factory.StartNew(new Action(() => { ListenTask(); }));

            Task.Factory.StartNew(new Action(() => { ListenForPassword(); }));
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            GenericUnitOfWork unitOfWork = new GenericUnitOfWork(new TestContext(ConfigurationManager.ConnectionStrings["conStr"].ConnectionString));
            var repo = unitOfWork.Repository<Group>();
            
            List<SendableExam> sendableExams = new List<SendableExam>();

            foreach (var group in repo.GetWithInclude(x=>x.Tests.Count>0,t=>t.Tests))
            {
                foreach (var test in group.Tests)
                {
                    SendableExam currentExam = new SendableExam();
                    currentExam.Group = group;
                    currentExam.Time = test.Time;


                    XmlSerializer serializer = new XmlSerializer(typeof(TestExam));
                    using (FileStream fs = new FileStream(test.Filename, FileMode.Open))
                    {
                        currentExam.TestExam = serializer.Deserialize(fs) as TestExam;
                    }
                    sendableExams.Add(currentExam);
                }
            }

            SendData(sendableExams);
        }

        private void SendData(List<SendableExam> exams)
        {
            byte[] data;
            var binaryFormatter = new BinaryFormatter();
            using (var ms = new MemoryStream())
            {
                binaryFormatter.Serialize(ms, exams);

                data = ms.ToArray();
            }

            byte[][] bufferArray = BufferSplit(data, 61440);
            string id = GenerateId();
            for (int i = 0; i < bufferArray.Length; i++)
            {
                DataPart dataPart = new DataPart() { Id = id, PartCount = bufferArray.Length, PartNum = i, Buffer = bufferArray[i] };
                byte[] dataPartArr;
                using (var memoryStream = new MemoryStream())
                {
                    binaryFormatter.Serialize(memoryStream, dataPart);
                    dataPartArr = memoryStream.ToArray();
                }
                udpClient.Send(dataPartArr, dataPartArr.Length, remoteEP);

            }
        }
        private void ListenTask()
        {
            List<DataPart> dataParts = new List<DataPart>();
            while (true)
            {
                IPEndPoint iPEndPoint = null;
                byte[] buffer = udpClient.Receive(ref iPEndPoint);
                var binaryFormatter = new BinaryFormatter();
                Grade grade;
                using (var memoryStream = new MemoryStream(buffer))
                {
                    grade = binaryFormatter.Deserialize(memoryStream) as Grade;
                }

                GenericUnitOfWork unitOfWork = new GenericUnitOfWork(new TestContext(ConfigurationManager.ConnectionStrings["conStr"].ConnectionString));
                var repo = unitOfWork.Repository<Grade>();
                var repoUser = unitOfWork.Repository<User>();
                var repoTest = unitOfWork.Repository<DALTest.Test>();

                Grade gradeToAdd = new Grade() { Mark = grade.Mark, User = repoUser.FindById(grade.User.Id), Test = repoTest.FindAll(x => x.Title == grade.Test.Title).First() };
                repo.Add(gradeToAdd);

            }
        }
        private byte[][] BufferSplit(byte[] buffer, int blockSize)
        {
            byte[][] blocks = new byte[(buffer.Length + blockSize - 1) / blockSize][];
            for (int i = 0, j = 0; i < blocks.Length; i++, j += blockSize)
            {
                blocks[i] = new byte[Math.Min(blockSize, buffer.Length - j)];
                Array.Copy(buffer, j, blocks[i], 0, blocks[i].Length);
            }
            return blocks;
        }
        private string GenerateId()
        {
            return Guid.NewGuid().ToString("N");
        }

        private void ListenForPassword()
        {
            TcpListener server = null;
            try
            {
                IPAddress localAddress = IPAddress.Parse("127.0.0.1");
                server = new TcpListener(localAddress, passwordPort);
                server.Start();
                while (true)
                {
                    TcpClient client = server.AcceptTcpClient();
                    NetworkStream stream = client.GetStream();
                    byte[] data = new byte[64];

                    stream.Read(data, 0, data.Length);
                    string login = Encoding.Unicode.GetString(data, 0, data.Length).Replace("\0",string.Empty);

                    GenericUnitOfWork unitOfWork = new GenericUnitOfWork(new TestContext(ConfigurationManager.ConnectionStrings["conStr"].ConnectionString));
                    var repo = unitOfWork.Repository<User>();
                    User user = repo.GetWithInclude(x=>x.Login==login,g => g.Group).FirstOrDefault();

                    if (user == null)
                    {
                        data = Encoding.Unicode.GetBytes("error");
                    }
                    else
                    {
                        using (var ms = new MemoryStream())
                        {
                            BinaryFormatter binaryFormatter = new BinaryFormatter();
                            binaryFormatter.Serialize(ms, user);
                            data = ms.ToArray();
                        }
                    }

                    stream.Write(data, 0, data.Length);

                    stream.Close();
                    client.Close();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            finally
            {
                if (server != null)
                    server.Stop();
            }
        }

        private void OnClosing()
        {
            timer.Stop();
            udpClient.Close();
        }
    }
}

