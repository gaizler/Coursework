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
    class EditGroupViewModel:ViewModelBase
    {
        #region Variables
        public Group SelectedGroup { get; set; } = null;
        public string NewGroupNameText { get; set; } = string.Empty;

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
        public RelayCommand EditGroupCommand { get; }
        #endregion

        public EditGroupViewModel()
        {
            EditGroupCommand = new RelayCommand(EditGroup, CanEditGroup, false);
        }

        private void EditGroup()
        {
            GenericUnitOfWork unitOfWork = new GenericUnitOfWork(new TestContext(ConfigurationManager.ConnectionStrings["conStr"].ConnectionString));
            var repo = unitOfWork.Repository<Group>();

            Group group = repo.FindById(SelectedGroup.Id);
            if (repo.FindAll(x=>x.Name==NewGroupNameText).ToList().Count>0&&NewGroupNameText!=group.Name)
            {
                MessageBox.Show("Sorry, but there already exists group with the same name! Choose anothe one please!", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            group.Name = NewGroupNameText;
            repo.Update(group);

            RaisePropertyChanged("GroupList");
            RaisePropertyChanged("GroupListModified");

            ClearFields();
        }

        private bool CanEditGroup()
        {
            return SelectedGroup != null&&NewGroupNameText.Length>0;

        }

        void ClearFields()
        {
            SelectedGroup = null;
            NewGroupNameText = string.Empty;
        }

    }
}
