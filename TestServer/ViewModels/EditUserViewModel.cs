using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
    class EditUserViewModel:ViewModelBase
    {
        #region Variables
        string selectedOldLogin=string.Empty;
        public string SelectedOldLogin
        {
            get => selectedOldLogin;
            set
            {
                selectedOldLogin = value;
                SelectedOldLoginChanged();
            }
        }
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
                return repo.GetWithInclude(x=>x.Group).Select(x => new { FirstName = x.FirstName ?? "none", LastName = x.LastName ?? "none", IsAdmin = x.IsAdmin, Login = x.Login, Group=x.Group.ToString()}).ToList();

            }
        }

        [SafeForDependencyAnalysis]
        public List<string> LoginList
        {
            get
            {
                GenericUnitOfWork unitOfWork = new GenericUnitOfWork(new TestContext(ConfigurationManager.ConnectionStrings["conStr"].ConnectionString));
                var repo = unitOfWork.Repository<User>();
                return repo.GetAll().Select(x => x.Login).ToList();
            }
        }

        public RelayCommand EditUserCommand { get; }
        #endregion

        public EditUserViewModel()
        {
            EditUserCommand = new RelayCommand(EditUser, CanEditUser, false);
        }

        private void EditUser()
        {
            GenericUnitOfWork unitOfWork = new GenericUnitOfWork(new TestContext(ConfigurationManager.ConnectionStrings["conStr"].ConnectionString));
            var repo = unitOfWork.Repository<User>();
            var repoGroup = unitOfWork.Repository<Group>();


            User user = repo.FindAll(x => x.Login == SelectedOldLogin).First();

            if (repo.FindAll(x => x.Login == LoginText).ToList().Count > 0&&user.Login!=LoginText)
            {
                MessageBox.Show("User with the same login already exists. Choose another one.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                LoginText = string.Empty;
                return;
            }

            user.FirstName = FirstNameText;
            user.LastName = LastNameText;
            user.IsAdmin = IsAdmin;
            user.Login = LoginText;
            user.Password = PasswordText;
            repo.Update(user);

            RaisePropertyChanged("LoginList");
            RaisePropertyChanged("UsersList");
            ClearFields();
        }

        private void SelectedOldLoginChanged()
        {
            GenericUnitOfWork unitOfWork = new GenericUnitOfWork(new TestContext(ConfigurationManager.ConnectionStrings["conStr"].ConnectionString));
            var repo = unitOfWork.Repository<User>();

            User user = repo.FindAll(x => x.Login == SelectedOldLogin).FirstOrDefault();

            if (user != null)
            {
                FirstNameText = user.FirstName ?? string.Empty;
                LastNameText = user.LastName ?? string.Empty;
                PasswordText = user.Password;
                IsAdmin = user.IsAdmin;
            }
        }

        private bool CanEditUser()
        {
            return FirstNameText.Length > 0 && LastNameText.Length > 0 && LoginText.Length > 0 && PasswordText.Length > 0 && SelectedOldLogin.Length > 0 ;
        }

        void ClearFields()
        {
            SelectedOldLogin = null;
            FirstNameText = string.Empty;
            LastNameText = string.Empty;
            IsAdmin = false;
            LoginText = string.Empty;
            PasswordText = string.Empty;
        }
    }
}
