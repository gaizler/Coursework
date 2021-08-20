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
    class AddUsersToGroupViewModel:ViewModelBase
    {
        #region Variables
        Group selectedGroup = null;
        public Group SelectedGroup
        {
            get => selectedGroup;
            set
            {
                selectedGroup = value;
                SelectedGroupChanged();
            }
        }
        public string GroupLabelTitle { get; set; } = "Users from selected group:";

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


        [SafeForDependencyAnalysis]
        public IList UsersList
        {
            get
            {
                GenericUnitOfWork unitOfWork = new GenericUnitOfWork(new TestContext(ConfigurationManager.ConnectionStrings["conStr"].ConnectionString));
                var repo = unitOfWork.Repository<User>();
                return repo.GetAll().ToList();

            }
        }


        [SafeForDependencyAnalysis]
        public List<User> UsersByGroupList
        {
            get
            {
                if (SelectedGroup != null)
                {
                    GenericUnitOfWork unitOfWork = new GenericUnitOfWork(new TestContext(ConfigurationManager.ConnectionStrings["conStr"].ConnectionString));
                    var repo = unitOfWork.Repository<User>();
                    return repo.FindAll(x => x.Group.Id == SelectedGroup.Id).ToList();
                }
                return null;
            }
        }

        public RelayCommand<object> AddUsersToGroupCommand { get; set; }
        #endregion
        public AddUsersToGroupViewModel()
        {
            AddUsersToGroupCommand = new RelayCommand<object>(AddUsersToGroup, CanAddUsersToGroup,false);
        }
        void AddUsersToGroup(object SelectedItems)
        {
            foreach(User user in (IList)SelectedItems)
            {
                if (UsersByGroupList.Where(x => x.Id == user.Id).ToList().Count == 0)
                {
                    if (MessageBox.Show($"User {user.Login} already is in a group. Do you want to change it?","Group changing",MessageBoxButton.YesNo,MessageBoxImage.Question)==MessageBoxResult.Yes)
                    {
                        GenericUnitOfWork unitOfWork = new GenericUnitOfWork(new TestContext(ConfigurationManager.ConnectionStrings["conStr"].ConnectionString));
                        var repoUser = unitOfWork.Repository<User>();
                        var repoGroup = unitOfWork.Repository<Group>();

                        User userToUpdate = repoUser.FindById(user.Id);
                        userToUpdate.Group = repoGroup.FindById(SelectedGroup.Id);
                        repoUser.Update(userToUpdate);

                    }
                }
            }
            RaisePropertyChanged("UsersByGroupList");
        }
        bool CanAddUsersToGroup(object SelectedItems)
        {
            return SelectedGroup != null;
        }
        void SelectedGroupChanged()
        {
            GroupLabelTitle = $"Users from {SelectedGroup}:";
            RaisePropertyChanged("UsersByGroupList");
        }
    }
}
