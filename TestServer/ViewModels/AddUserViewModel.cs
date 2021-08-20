using DALTest;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using PostSharp.Patterns.Model;
using Repository;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Windows;

namespace TestServer.ViewModels
{

    [NotifyPropertyChanged]
    public class AddUserViewModel:ViewModelBase
    {
        #region Variables
        public string FirstNameText { get; set; } = string.Empty;
        public string LastNameText { get; set; } = string.Empty;
        public string LoginText { get; set; } = string.Empty;
        public string PasswordText { get; set; } = string.Empty;
        public bool IsAdmin { get; set; }

        [SafeForDependencyAnalysis]
        public IList UsersList
        {
            get
            {
                GenericUnitOfWork unitOfWork = new GenericUnitOfWork(new TestContext(ConfigurationManager.ConnectionStrings["conStr"].ConnectionString));
                var repo = unitOfWork.Repository<User>();
                return repo.GetWithInclude(x=>x.Group).Select(x => new { FirstName = x.FirstName ?? "none", LastName = x.LastName ?? "none", IsAdmin = x.IsAdmin, Login = x.Login, Group = x.Group.ToString() }).ToList();

            }
        }

        public RelayCommand AddUserCommand { get; }
        #endregion

        public AddUserViewModel()
        {
            AddUserCommand = new RelayCommand(AddUser,CanAddUser,false);
        }

        private void AddUser()
        {
            GenericUnitOfWork unitOfWork = new GenericUnitOfWork(new TestContext(ConfigurationManager.ConnectionStrings["conStr"].ConnectionString));
            var repo = unitOfWork.Repository<User>();
            var repoGroup = unitOfWork.Repository<Group>();
            if (repo.FindAll(x => x.Login == LoginText).ToList().Count > 0)
            {
                MessageBox.Show("User with the same login already exists. Choose another one.","Error",MessageBoxButton.OK,MessageBoxImage.Error);
                return;
            }
            repo.Add(new User() { FirstName=FirstNameText,LastName=LastNameText, Login=LoginText, Password=PasswordText, Group= repoGroup.FindAll(x=>x.Name=="Default group").First(), IsAdmin=IsAdmin });

            RaisePropertyChanged("UsersList");
            ClearFields();
        }

        private bool CanAddUser()
        {
            return FirstNameText.Length > 0 && LastNameText.Length > 0 && LoginText.Length > 0 && PasswordText.Length > 0;
        }

        void ClearFields()
        {
            FirstNameText = string.Empty;
            LastNameText = string.Empty;
            IsAdmin = false;
            LoginText = string.Empty;
            PasswordText = string.Empty;
        }
    }
}
