using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using DALTest;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using PostSharp.Patterns.Model;
using Repository;

namespace TestServer.ViewModels
{
    [NotifyPropertyChanged]
    class AssignTestViewModel:ViewModelBase
    {
        #region Varibles
        [SafeForDependencyAnalysis]
        public List<Group> GroupList
        {
            get
            {
                GenericUnitOfWork unitOfWork = new GenericUnitOfWork(new TestContext(ConfigurationManager.ConnectionStrings["conStr"].ConnectionString));
                var repo = unitOfWork.Repository<Group>();
                return repo.GetAll().ToList();
            }
        }

        [SafeForDependencyAnalysis]
        public List<Test> TestList
        {
            get
            {
                GenericUnitOfWork unitOfWork = new GenericUnitOfWork(new TestContext(ConfigurationManager.ConnectionStrings["conStr"].ConnectionString));
                var repo = unitOfWork.Repository<Test>();
                return repo.GetAll().ToList();
            }
        }
        public Test SelectedTest { get; set; } = null;
        public Group SelectedGroup { get; set; } = null;

        public RelayCommand AssignTestCommand { get; }
        #endregion

        public AssignTestViewModel()
        {
            AssignTestCommand = new RelayCommand(AssignTest,CanAssignTest,false);
        }

        private void AssignTest()
        {
            GenericUnitOfWork unitOfWork = new GenericUnitOfWork(new TestContext(ConfigurationManager.ConnectionStrings["conStr"].ConnectionString));
            var repoTest = unitOfWork.Repository<Test>();
            var repoGroup = unitOfWork.Repository<Group>();

            Group group = repoGroup.FindById(SelectedGroup.Id);
            Test test = repoTest.FindById(SelectedTest.Id);


            try
            {
                group.Tests.Add(test);
                repoGroup.Update(group);
                MessageBox.Show("The test has been assigned", "Success", MessageBoxButton.OK, MessageBoxImage.Asterisk);
            }
            catch
            {
                MessageBox.Show("It seems that this test has already been assigned for this group", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            ClearFields();

        }
        private bool CanAssignTest()
        {
            return SelectedGroup != null && SelectedTest != null;
        }
        private void ClearFields()
        {
            SelectedTest = null;
            SelectedGroup = null;
        }

    }
}
