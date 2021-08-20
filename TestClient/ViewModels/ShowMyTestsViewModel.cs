using DALTest;
using DataLibrary;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Messaging;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using TestLibrary;

namespace TestClient.ViewModels
{
    class ShowMyTestsViewModel:ViewModelBase
    {
        #region Variables
        User CurrentUser { get; set; }

        IPAddress groupAddress;
        int localPort = 5679, remotePort = 5998;
        int ttl = 32;
        UdpClient udpClient;
        IPEndPoint remoteEP;
        public IList MyTestsList { get; set; } = null;
        #endregion
        public ShowMyTestsViewModel()
        {
            groupAddress = IPAddress.Parse("234.5.5.10");

            remoteEP = new IPEndPoint(groupAddress, remotePort);

            CurrentUser = Application.Current.Resources["curUser"] as User;

            Task.Factory.StartNew(new Action(() => { ListenTask(); }));

        }
        private void ListenTask()
        {
            List<DataPart> dataParts = new List<DataPart>();
            using (udpClient = new UdpClient(localPort))
            {
                udpClient.JoinMulticastGroup(groupAddress, ttl);
                while (true)
                {
                    IPEndPoint iPEndPoint = null;
                    byte[] buffer = udpClient.Receive(ref iPEndPoint);
                    var binaryFormatter = new BinaryFormatter();
                    DataPart dataPart;
                    using (var memoryStream = new MemoryStream(buffer))
                    {
                        dataPart = binaryFormatter.Deserialize(memoryStream) as DataPart;
                    }
                    if (dataParts.Count == 0)
                        dataParts.Add(dataPart);
                    else if (dataParts[0].Id == dataPart.Id)
                        dataParts.Add(dataPart);
                    else
                    {
                        if (dataParts.Count == dataPart.PartCount)
                        {
                            dataParts = dataParts.OrderBy(d => d.PartNum).ToList();
                            byte[] data = dataParts[0].Buffer;
                            for (int i = 1; i < dataParts.Count; i++)
                                data = data.Concat(dataParts[i].Buffer).ToArray();
                            List<SendableExam> exams;
                            using (var memoryStream = new MemoryStream(data))
                            {
                                exams = binaryFormatter.Deserialize(memoryStream) as List<SendableExam>;
                            }
                            MyTestsList = exams.Where(x=>x.Group.Id==CurrentUser.Group.Id).Select(x => new { Title = x.TestExam.Title, Time = x.Time, Group = x.Group }).ToList();
                            RaisePropertyChanged("MyTestsList");
                            break;
                        }
                        dataParts.Clear();
                        dataParts.Add(dataPart);
                    }
                }
            }
        }
    }
}
