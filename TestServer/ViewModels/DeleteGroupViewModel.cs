using System;
using System.Collections;
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
    class DeleteGroupViewModel : ViewModelBase
    {
        #region Variables
        public Group SelectedGroup { get; set; } = null;

        [SafeForDependencyAnalysis]
        public IList GroupListModified
        {
            get
            {
                GenericUnitOfWork unitOfWork = new GenericUnitOfWork(new TestContext(ConfigurationManager.ConnectionStrings["conStr"].ConnectionString));
                var repo = unitOfWork.Repository<Group>();
                return repo.GetAll().Select(x => new { Name = x.Name }).ToList();

            }
        }
        [SafeForDependencyAnalysis]
        public IList GroupList
        {
            get
            {
                GenericUnitOfWork unitOfWork = new GenericUnitOfWork(new TestContext(ConfigurationManager.ConnectionStrings["conStr"].ConnectionString));
                var repo = unitOfWork.Repository<Group>();
                return repo.GetAll().ToList();
            }
        }
        public RelayCommand DeleteGroupCommand { get; }
        #endregion

        public DeleteGroupViewModel()
        {
            DeleteGroupCommand = new RelayCommand(DeleteGroup, CanDeleteGroup, false);
        } 

        private void DeleteGroup()
        {
            GenericUnitOfWork unitOfWork = new GenericUnitOfWork(new TestContext(ConfigurationManager.ConnectionStrings["conStr"].ConnectionString));
            var repo = unitOfWork.Repository<Group>();
            var repoUser = unitOfWork.Repository<User>();

            Group group = repo.FindById(SelectedGroup.Id);
            if (repoUser.GetWithInclude(x=>x.Group).Where(x=>x.Group.Id==SelectedGroup.Id).ToList().Count==0)
                repo.Remove(group);
            else
                MessageBox.Show("Sorry, we can`t delete this group, because it`s filled with students!","Error",MessageBoxButton.OK,MessageBoxImage.Error);

            RaisePropertyChanged("GroupList");
            RaisePropertyChanged("GroupListModified");

            ClearFields();
        }

        private bool CanDeleteGroup()
        {
            return SelectedGroup != null;
        }

        void ClearFields()
        {
            SelectedGroup = null;
        }


    }
}
