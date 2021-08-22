using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Xml.Serialization;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using Microsoft.Win32;
using PostSharp.Patterns.Model;
using TestLibrary;

namespace TestDesigner.ViewModels
{
    [NotifyPropertyChanged]
    class EditTestViewModel:ViewModelBase
    {

        #region Variables
        TestExam currentExam;

        public static ObservableCollection<int> NumberOfAnswers { get; } = new ObservableCollection<int>() { 2, 3, 4, 5 };
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

        [SafeForDependencyAnalysis]
        public ObservableCollection<Test> CurrentExamQuestions
        {
            get
            {
                ObservableCollection<Test> tests = new ObservableCollection<Test>();
                foreach (Test test in TestExam.Tests)
                    tests.Add(test);
                return tests;
            }
        }

        public Test CurrentExamSelectedQuestion { get; set; }
        public bool IsEnabledEditQuestionMode { get; set; } = false;

        public string QuestionEditText { get; set; } = string.Empty;
        public TestDifficulty? SelectedEditDifficulty { get; set; }
        bool edit_flag = false;


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


        Window currentWindow;


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
        public RelayCommand SelectedQuestionToEditChangedCommand { get; set; }
        public RelayCommand EditQuestionCommand { get; set; }
        public RelayCommand DeleteQuestionCommand { get; }
        public RelayCommand DeleteAnswerCommand { get; set; }
        public RelayCommand<Window> LoadedCommand { get; }

        #endregion
        public EditTestViewModel()
        {
            LoadedCommand = new RelayCommand<Window>(LoadTest);
            AddQuestionCommand = new RelayCommand(AddQuestion, AddQuestionCanExecute, false);
            NextQuestionCommand = new RelayCommand(NextQuestion, NextQuestionCanExecute, false);
            AddAnswerCommand = new RelayCommand(AddAnswer, AddAnswerCanExecute, false);
            AddAuthorCommand = new RelayCommand(AddAuthor, AddAuthorCanExecute, false);
            SaveCommand = new RelayCommand(SaveFile, SaveFileCanExecute, false);
            SelectedQuestionToEditChangedCommand = new RelayCommand(SelectedQuestionToEditChanged);
            EditQuestionCommand = new RelayCommand(EditQuestion,CanEditQuestion,false);
            DeleteAnswerCommand = new RelayCommand(DeleteAnswer, CanDeleteAnswer, false);
            DeleteQuestionCommand = new RelayCommand(DeleteQuestion,CanDeleteQuestion,false);

        }

        private void LoadTest(Window window)
        {
            currentWindow = window;
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "xml files (*.xml)|*.xml|All files (*.*)|*.*";
            if (ofd.ShowDialog() == true)
            {
                XmlSerializer serializer = new XmlSerializer(typeof(TestExam));
                try
                {
                    using (FileStream fs = new FileStream(ofd.FileName, FileMode.Open))
                    {
                        currentExam = serializer.Deserialize(fs) as TestExam;
                        FileName = Path.GetFileNameWithoutExtension(ofd.FileName);
                        FillFields();
                    }
                }
                catch
                {
                    MessageBox.Show("Problem with accessing to this file. Try in a few seconds", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    LoadTest(currentWindow);
                }
            }
            else
                window.Close();
        }

        private void FillFields()
        {
            TestExam = currentExam;
            AuthorText = currentExam.Author;
            TitleText = currentExam.Title;
            SelectedQuantityOfAnswers = currentExam.NumberOfAnswers;
            
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
            return AuthorText.Length > 0 && TitleText.Length > 0 && SelectedQuantityOfAnswers != null;
        }
        private void AddQuestion()
        {
            CurrentTest = new Test() { Question = QuestionText, Difficulty = (TestDifficulty)SelectedDifficulty };

            MoveToAnswersMode();
        }
        private bool AddQuestionCanExecute()
        {
            return QuestionText.Length != 0 && SelectedDifficulty != null;
        }

        private void EditQuestion()
        {
            //CurrentExamSelectedQuestion = null;
            edit_flag = true;
            //RaisePropertyChanged("CurrentExamQuestions");
            MoveToAnswersMode();
        }

        private bool CanEditQuestion()
        {
            return QuestionEditText.Length > 0 && SelectedEditDifficulty != null;
        }
        private void DeleteQuestion()
        {
            TestExam.Tests.Remove(CurrentExamSelectedQuestion);
            RaisePropertyChanged("CurrentExamQuestions");
            ClearQuestionSection();

        }

        private bool CanDeleteQuestion()
        {
            return CurrentExamSelectedQuestion != null;
        }
        private void NextQuestion()
        {
            if (CurrentTest.Answers.Count(x => x.isCorrect == true) != 1)
            {
                MessageBox.Show($"Sorry, but your test can`t have {CurrentTest.Answers.Count(x => x.isCorrect == true)} correct answers." +
                                    $" There has to be only 1 correct answer. You can choose answers from the list and edit them.", "Wrong quantity" +
                                        " of correct answers", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }



            if (edit_flag)
            {
                CurrentTest.Difficulty = (TestDifficulty)SelectedEditDifficulty;
                CurrentTest.Question = QuestionEditText;
                TestExam.Tests.Remove(CurrentExamSelectedQuestion);
            }

            edit_flag = false;
            TestExam.Tests.Add(CurrentTest);

            RaisePropertyChanged("CurrentExamQuestions");
            ClearQuestionSection();
            ClearAnswersSection();
            MoveToQuestionMode();

        }

        private void SelectedQuestionToEditChanged()
        {
            if (CurrentExamSelectedQuestion == null)
                return;
            IsEnabledEditQuestionMode = true;
            QuestionEditText = CurrentExamSelectedQuestion.Question;
            SelectedEditDifficulty = CurrentExamSelectedQuestion.Difficulty;
            CurrentTest = CurrentExamSelectedQuestion;
            RaisePropertyChanged("AnswersForCurrentTest");
        }

        private bool NextQuestionCanExecute()
        {
            return CurrentTest != null && CurrentTest.Answers.Count == SelectedQuantityOfAnswers;
        }
        private void AddAnswer()
        {
            if (SelectedAnswer != null)
                CurrentTest.Answers.Remove(SelectedAnswer);

            CurrentTest.Answers.Add(new Answer() { AnswerText = AnswerText, isCorrect = IsCorrectAnswer });

            ClearAnswersSection();

            RaisePropertyChanged("AnswersForCurrentTest");
            RaisePropertyChanged("SelectedAnswer");
        }
        private bool AddAnswerCanExecute()
        {
            return CurrentTest != null && SelectedQuantityOfAnswers != null && (CurrentTest.Answers.Count < SelectedQuantityOfAnswers || SelectedAnswer != null) && AnswerText.Length > 0;
        }
        private void DeleteAnswer()
        {
            CurrentTest.Answers.Remove(SelectedAnswer);
            AnswerText = string.Empty;
            IsCorrectAnswer = false;
            RaisePropertyChanged("AnswersForCurrentTest");
        }
        private bool CanDeleteAnswer()
        {
            return SelectedAnswer != null;
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
            using (FileStream fs = new FileStream($"Tests/{FileName}.xml", FileMode.Create))
            {
                serializer.Serialize(fs, TestExam);
                ClearAllSections();
                MessageBox.Show("File was created successfuly!", "Success", MessageBoxButton.OK, MessageBoxImage.Asterisk);
                MoveToAuthorIdentificationMode();

            }
        }
        private bool SaveFileCanExecute()
        {
            return FileName.Length > 0;
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
            LoadTest(currentWindow);
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
            QuestionEditText = string.Empty;
            SelectedEditDifficulty = null;
        }
    }
}
