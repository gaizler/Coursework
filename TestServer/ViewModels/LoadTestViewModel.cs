using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Xml.Serialization;
using DALTest;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using Microsoft.Win32;
using PostSharp.Patterns.Model;
using Repository;
using TestLibrary;

namespace TestServer.ViewModels
{
    [NotifyPropertyChanged]
    class LoadTestViewModel : ViewModelBase
    {
        #region Variables
        public static List<int> TimeList { get; } = new List<int>() { 10, 15, 20, 30, 40, 45, 50, 60, 100, 120 };
        [SafeForDependencyAnalysis]
        public IList TestList
        {
            get
            {
                GenericUnitOfWork unitOfWork = new GenericUnitOfWork(new TestContext(ConfigurationManager.ConnectionStrings["conStr"].ConnectionString));
                var repo = unitOfWork.Repository<DALTest.Test>();
                return repo.GetAll().Select(x => new { Title = x.Title, Time = x.Time, Author = x.Author, NumberOfAnswers = x.NumberOfQuestions, Filename = x.Filename }).ToList();

            }
        }
        public int? SelectedTime { get; set; } = null;
        public string FileNameText { get; set; } = string.Empty;
        public string FilePathText { get; set; } = string.Empty;

        private TestExam CurrentTestExam { get; set; }

        public RelayCommand LoadTestCommand { get; }
        public RelayCommand SelectTestCommand { get; }


        #endregion

        public LoadTestViewModel()
        {
            LoadTestCommand = new RelayCommand(LoadTest, CanLoadTest, false);
            SelectTestCommand = new RelayCommand(SelectTest);

        }

        private void LoadTest()
        {
            GenericUnitOfWork unitOfWork = new GenericUnitOfWork(new TestContext(ConfigurationManager.ConnectionStrings["conStr"].ConnectionString));
            var repo = unitOfWork.Repository<DALTest.Test>();

            repo.Add(new DALTest.Test() { Author=CurrentTestExam.Author, Filename=FilePathText, Title=CurrentTestExam.Title, NumberOfQuestions=CurrentTestExam.NumberOfAnswers, Time=new TimeSpan(0,(int)SelectedTime,0), Groups=new List<Group>()});
            RaisePropertyChanged("TestList");
            ClearFields();
        }
        private bool CanLoadTest()
        {
            return SelectedTime != null&&FileNameText.Length>0;
        }

        private void SelectTest()
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "xml files (*.xml)|*.xml|All files (*.*)|*.*";
            if (ofd.ShowDialog() == true)
            {
                XmlSerializer serializer = new XmlSerializer(typeof(TestExam));
                using (FileStream fs = new FileStream(ofd.FileName, FileMode.Open))
                {
                    CurrentTestExam = serializer.Deserialize(fs) as TestExam;
                    FilePathText = ofd.FileName;
                    FileNameText = Path.GetFileName(ofd.FileName);
                }
            }
        }

        private void ClearFields()
        {
            SelectedTime = null;
            FilePathText = string.Empty;
            FileNameText = string.Empty;
        }
    }
}
