using GalaSoft.MvvmLight.CommandWpf;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Xml.Serialization;
using TestLibrary;
using PostSharp.Patterns.Model;
using GalaSoft.MvvmLight;

namespace TestDesigner.ViewModels
{
    [NotifyPropertyChanged]
    public class CreateTestViewModel:ViewModelBase
    {
        #region Variables
        public static ObservableCollection<int> NumberOfQuestions { get; } = new ObservableCollection<int>() { 2, 3, 4, 5 };
        public static Array Difficulties { get; } = Enum.GetValues(typeof(TestDifficulty));

        TestExam TestExam { get; set; } = new TestExam();
        public Test CurrentTest { get; set; }

        [SafeForDependencyAnalysis]
        public ObservableCollection<Answer> AnswersForCurrentTest
        {
            get
            {
                ObservableCollection<Answer> answers = new ObservableCollection<Answer>();
                if (CurrentTest != null)
                {
                    foreach (var item in CurrentTest.Answers)
                        answers.Add(item);
                }
                return answers;
            }
        }


        public string AuthorText { get; set; } = string.Empty;
        public string TitleText { get; set; } = string.Empty;
        public int? SelectedQuantityOfAnswers { get; set; }

        public string QuestionText { get; set; } = string.Empty;
        public TestDifficulty? SelectedDifficulty { get; set; }

        public string AnswerText { get; set; } = string.Empty;
        public bool IsCorrectAnswer { get; set; }

        Answer selectedAnswer;
        public Answer SelectedAnswer
        {
            get => selectedAnswer;
            set
            {
                selectedAnswer = value;
                SelectedAnswerChanged();
            }
        }

        public string FileName { get; set; } = string.Empty;


        public bool IsEnabledQuestionGroupBox { get; set; } = false;
        public bool IsEnabledSaveGroupBox { get; set; } = false;
        public bool AreEnabledAnswerGroupBoxes { get; set; } = false;
        public bool IsEnabledInfoGroupBox { get; set; } = true; 
        #endregion

        #region Commands
        public RelayCommand AddQuestionCommand { get; }
        public RelayCommand NextQuestionCommand { get; }
        public RelayCommand AddAnswerCommand { get; }
        public RelayCommand AddAuthorCommand { get; }
        public RelayCommand SaveCommand { get; }
        public RelayCommand SelectedAnswerChangedCommand { get; }
        #endregion


        public CreateTestViewModel()
        {
            AddQuestionCommand = new RelayCommand(AddQuestion,AddQuestionCanExecute,false);
            NextQuestionCommand = new RelayCommand(NextQuestion,NextQuestionCanExecute,false);
            AddAnswerCommand = new RelayCommand(AddAnswer,AddAnswerCanExecute,false);
            AddAuthorCommand = new RelayCommand(AddAuthor,AddAuthorCanExecute,false);
            SaveCommand = new RelayCommand(SaveFile, SaveFileCanExecute, false);
        }

        private void AddAuthor()
        {
            TestExam.Author = AuthorText;
            TestExam.Title = TitleText;
            TestExam.NumberOfAnswers = (int)SelectedQuantityOfAnswers;

            MoveToQuestionMode();
        }
        private bool AddAuthorCanExecute()
        {
            return AuthorText.Length>0 &&TitleText.Length>0 && SelectedQuantityOfAnswers != null;
        }
        private void AddQuestion()
        {
            CurrentTest = new Test() { Question=QuestionText, Difficulty=(TestDifficulty)SelectedDifficulty};

            MoveToAnswersMode();
        }
        private bool AddQuestionCanExecute()
        {
            return QuestionText.Length != 0 && SelectedDifficulty != null;
        }
        private void NextQuestion()
        {
            if (CurrentTest.Answers.Count(x => x.isCorrect == true) != 1)
            {
                MessageBox.Show($"Sorry, but your test can`t have {CurrentTest.Answers.Count(x => x.isCorrect == true)} correct answers." +
                                    $" There has to be only 1 correct answer. You can choose answers from the list and edit them.", "Wrong quantity" +
                                        " of correct answers", MessageBoxButton.OK,MessageBoxImage.Error);
                return;
            }

            TestExam.Tests.Add(CurrentTest);

            ClearQuestionSection();
            ClearAnswersSection();
            MoveToQuestionMode();

        }
        private bool NextQuestionCanExecute()
        {
            return CurrentTest!=null&& CurrentTest.Answers.Count==SelectedQuantityOfAnswers;
        }
        private void AddAnswer()
        {
            if (SelectedAnswer != null)
                CurrentTest.Answers.Remove(SelectedAnswer);

            CurrentTest.Answers.Add(new Answer() { AnswerText=AnswerText, isCorrect=IsCorrectAnswer });

            ClearAnswersSection();

            RaisePropertyChanged("AnswersForCurrentTest");
            RaisePropertyChanged("SelectedAnswer");
        }
        private bool AddAnswerCanExecute()
        {
            return CurrentTest!=null&&SelectedQuantityOfAnswers!=null&&(CurrentTest.Answers.Count < SelectedQuantityOfAnswers||SelectedAnswer!=null) && AnswerText.Length > 0;
        }
        private void SelectedAnswerChanged()
        {
            if (SelectedAnswer != null)
            {
                AnswerText = SelectedAnswer.AnswerText;
                IsCorrectAnswer = SelectedAnswer.isCorrect;
            }
        }
        private void SaveFile()
        {
            XmlSerializer serializer = new XmlSerializer(typeof(TestExam));
            using (FileStream fs=new FileStream($"Tests/{FileName}.xml", FileMode.Create))
            {
                serializer.Serialize(fs,TestExam);
                ClearAllSections();
                MoveToAuthorIdentificationMode();
                MessageBox.Show("File was created successfuly!","Success",MessageBoxButton.OK,MessageBoxImage.Asterisk);
            }
        }
        private bool SaveFileCanExecute()
        {
            return FileName.Length>0;
        }

        private void MoveToQuestionMode()
        {
            IsEnabledQuestionGroupBox = true;
            IsEnabledSaveGroupBox = true;
            IsEnabledInfoGroupBox = false;
            AreEnabledAnswerGroupBoxes = false;
        }
        private void MoveToAnswersMode()
        {
            AreEnabledAnswerGroupBoxes = true;
            IsEnabledQuestionGroupBox = false;
            IsEnabledSaveGroupBox = false;
            IsEnabledInfoGroupBox = false;
        }
        private void MoveToAuthorIdentificationMode()
        {
            IsEnabledSaveGroupBox = false;
            IsEnabledInfoGroupBox = true;
            IsEnabledQuestionGroupBox = false;
            AreEnabledAnswerGroupBoxes = false;
        }

        private void ClearAllSections()
        {
            TestExam = new TestExam();
            AuthorText = string.Empty;
            TitleText = string.Empty;
            SelectedQuantityOfAnswers = null;
            FileName = string.Empty;
            SelectedDifficulty = null;
            QuestionText = string.Empty;
        }
        private void ClearAnswersSection()
        {
            SelectedAnswer = null;
            AnswerText = string.Empty;
            IsCorrectAnswer = false;
        }
        private void ClearQuestionSection()
        {
            CurrentTest = new Test();
            QuestionText = string.Empty;
            SelectedDifficulty = null;
        }
    }
}
