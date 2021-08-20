using DALTest;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using PostSharp.Patterns.Model;
using Repository;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace TestServer.ViewModels
{
    [NotifyPropertyChanged]
    class AddGroupViewModel:ViewModelBase
    {
        #region Variables
        public string GroupNameText { get; set; } = string.Empty;
   
        [SafeForDependencyAnalysis]
        public IList GroupsList
        {
            get
            {
                GenericUnitOfWork unitOfWork = new GenericUnitOfWork(new TestContext(ConfigurationManager.ConnectionStrings["conStr"].ConnectionString));
                var repo = unitOfWork.Repository<Group>();
                return repo.GetAll().Select(x=> new { Group=x.Name}).ToList();
            }
        }

        public RelayCommand AddGroupCommand { get; }
        #endregion

        public AddGroupViewModel()
        {
            AddGroupCommand = new RelayCommand(AddGroup, CanAddGroup, false);
        }

        private void AddGroup()
        {
            GenericUnitOfWork unitOfWork = new GenericUnitOfWork(new TestContext(ConfigurationManager.ConnectionStrings["conStr"].ConnectionString));
            var repo = unitOfWork.Repository<Group>();
            if (repo.FindAll(x => x.Name == GroupNameText).ToList().Count > 0)
            {
                MessageBox.Show("Group with the same name already exists. Choose another one.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            repo.Add(new Group() { Name=GroupNameText});

            RaisePropertyChanged("GroupsList");
            ClearFields();
        }

        private bool CanAddGroup()
        {
            return GroupNameText.Length > 0;
        }

        void ClearFields()
        {
            GroupNameText =string.Empty;
        }
    }
}
