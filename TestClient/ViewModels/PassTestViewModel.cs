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
using System.Windows.Input;
using DALTest;
using DataLibrary;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using PostSharp.Patterns.Model;
using TestLibrary;
using System.Reflection;

namespace TestClient.ViewModels
{
    [NotifyPropertyChanged]
    class PassTestViewModel:ViewModelBase
    {
        #region Variables

        IPAddress groupAddress;
        int localPort = 5679, remotePort = 5998;
        int ttl = 32;
        UdpClient udpClient;
        IPEndPoint remoteEP;

        User CurrentUser { get; set; }
        public SendableExam SelectedExam { get; set; } = null;
        public bool SelectTestIsEnabled { get; set; } = true;

        [SafeForDependencyAnalysis]
        public List<SendableExam> MyTestsList { get; set; } = null;

        public string Answer1 { get; set; }
        public string Answer2 { get; set; }
        public string Answer3 { get; set; }
        public string Answer4 { get; set; }
        public string Answer5 { get; set; }

        public Visibility Answer1Visibility { get; set; } = Visibility.Hidden;
        public Visibility Answer2Visibility { get; set; } = Visibility.Hidden;
        public Visibility Answer3Visibility { get; set; } = Visibility.Hidden;
        public Visibility Answer4Visibility { get; set; } = Visibility.Hidden;
        public Visibility Answer5Visibility { get; set; } = Visibility.Hidden;

        public bool? IsChecked1 { get; set; } = false;
        public bool? IsChecked2 { get; set; } = false;
        public bool? IsChecked3 { get; set; } = false;
        public bool? IsChecked4 { get; set; } = false;
        public bool? IsChecked5 { get; set; } = false;


        private int CurrentTestIndex = 0;
        private Answer SelectedAnswer=null;
        public string QuestionText { get; set; }

        private int maxPoint, curPoint;


        public ICommand PassCommand { get; set; }
        public ICommand NextCommand { get; set; }
        public RelayCommand<int> CheckedChangedCommand { get; set; }
        public RelayCommand ClosingCommand { get; }

        #endregion

        public PassTestViewModel()
        {
            CurrentUser = Application.Current.Resources["curUser"] as User;

            InitializeUdpClient();

            Task.Factory.StartNew(new Action(() => { ListenTask(); }));

            PassCommand = new RelayCommand(PassExam, CanPassExam, false);
            NextCommand = new RelayCommand(Next, CanGoNext, false);
            CheckedChangedCommand = new RelayCommand<int>(CheckedTestChanged);
            ClosingCommand = new RelayCommand(OnClosing);

        }

        private void PassExam()
        {
            SelectTestIsEnabled = false;

            for (int i = 1; i <= SelectedExam.TestExam.NumberOfAnswers; i++)
            {
                this.GetType().GetProperty($"Answer{i}Visibility").SetValue(this,Visibility.Visible);
            }

            FillTest();

            maxPoint = CountMaxPoint();
            curPoint = 0;
        }
        private bool CanPassExam()
        {
            return SelectedExam != null;
        }

        private void CheckedTestChanged(int index)
        {
            SelectedAnswer = SelectedExam.TestExam.Tests[CurrentTestIndex].Answers[index - 1];
        }

        private void Next()
        {
            if (SelectedAnswer.isCorrect)
            {
                IncrementPoint();
            }
            CurrentTestIndex++;
            FillTest();
            SelectedAnswer = null;
            for (int i = 1; i <= SelectedExam.TestExam.NumberOfAnswers; i++)
            {
                this.GetType().GetProperty($"IsChecked{i}").SetValue(this, false);
            }
        }

        private void FillTest()
        {
            if (CheckEnd())
                return;
          
            QuestionText = SelectedExam.TestExam.Tests[CurrentTestIndex].Question;
            for (int i = 1; i <= SelectedExam.TestExam.NumberOfAnswers; i++)
            {
                this.GetType().GetProperty($"Answer{i}").SetValue(this, SelectedExam.TestExam.Tests[CurrentTestIndex].Answers[i - 1].ToString());
            }
        }

        private bool CanGoNext()
        {
            return SelectedAnswer != null;
        }

        private void ListenTask()
        {
            List<DataPart> dataParts = new List<DataPart>();
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
                        MyTestsList = exams.Where(x => x.Group.Id == CurrentUser.Group.Id).ToList();
                        RaisePropertyChanged("MyTestsList");

                        udpClient.Close();
                        break;
                    }
                    dataParts.Clear();
                    dataParts.Add(dataPart);
                }
            }
        }

        private void ClearFields()
        {
            CurrentTestIndex = 0;
            SelectTestIsEnabled = true;
            QuestionText = string.Empty;
            for (int i = 1; i <= SelectedExam.TestExam.NumberOfAnswers; i++)
            {
                this.GetType().GetProperty($"Answer{i}Visibility").SetValue(this, Visibility.Hidden);
            }
        }

        private int CountMaxPoint()
        {
            int max = 0;
            for (int i = 0; i < SelectedExam.TestExam.Tests.Count; i++)
            {
                switch (SelectedExam.TestExam.Tests[i].Difficulty)
                {
                    case TestDifficulty.Easy:
                        max += 1;
                        break;
                    case TestDifficulty.Medium:
                        max += 2;
                        break;
                    case TestDifficulty.Hard:
                        max += 3;
                        break;
                }
            }
            return max;
        }

        private void IncrementPoint()
        {
            switch (SelectedExam.TestExam.Tests[CurrentTestIndex].Difficulty)
            {
                case TestDifficulty.Easy:
                    curPoint += 1;
                    break;
                case TestDifficulty.Medium:
                    curPoint += 2;
                    break;
                case TestDifficulty.Hard:
                    curPoint += 3;
                    break;
            }
        }

        private bool CheckEnd()
        {
            if (CurrentTestIndex == SelectedExam.TestExam.Tests.Count)
            {
                MessageBox.Show($"You got {curPoint} points from {maxPoint}. Your result is {Math.Round((double)curPoint / maxPoint * 100, 2)}%", "Finish", MessageBoxButton.OK, MessageBoxImage.Asterisk);
                ClearFields();

                Grade grade = new Grade() { Mark = Math.Round((double)curPoint / maxPoint * 100, 2), Test = new DALTest.Test() { Author=SelectedExam.TestExam.Author, Title=SelectedExam.TestExam.Title }, User = CurrentUser };
                SendData(grade);
                return true;
            }
            return false;
        }

        private void SendData(Grade grade)
        {
            InitializeUdpClient();

            byte[] data;
            var binaryFormatter = new BinaryFormatter();
            using (var ms = new MemoryStream())
            {
                binaryFormatter.Serialize(ms,grade);

                data = ms.ToArray();
            }

           udpClient.Send(data, data.Length, remoteEP);
           udpClient.Close();

        }

        private void InitializeUdpClient()
        {
            groupAddress = IPAddress.Parse("234.5.5.10");
            remoteEP = new IPEndPoint(groupAddress, remotePort);
            udpClient = new UdpClient(localPort);
            udpClient.JoinMulticastGroup(groupAddress, ttl);
        }
        private void OnClosing()
        {
            udpClient.Close();
        }



    }
}
