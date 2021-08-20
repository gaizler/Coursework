using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DALTest;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using PostSharp.Patterns.Model;
using Repository;

namespace TestServer.ViewModels
{
    [NotifyPropertyChanged]
    class DeleteUserViewModel:ViewModelBase
    {
        #region Variables
        public string SelectedLogin { get; set; } = null;
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
        public RelayCommand DeleteUserCommand { get; } 
        #endregion
        public DeleteUserViewModel()
        {
            DeleteUserCommand = new RelayCommand(DeleteUser,CanDeleteUser,false);
        }

        private void DeleteUser()
        {
            GenericUnitOfWork unitOfWork = new GenericUnitOfWork(new TestContext(ConfigurationManager.ConnectionStrings["conStr"].ConnectionString));
            var repo = unitOfWork.Repository<User>();

            User user = repo.FindAll(x=>x.Login==SelectedLogin).FirstOrDefault();
            repo.Remove(user);

            RaisePropertyChanged("LoginList");
            RaisePropertyChanged("UsersList");

            ClearFields();
        }

        private bool CanDeleteUser()
        {
            return SelectedLogin != null;
        }

        void ClearFields()
        {
            SelectedLogin = null;
        }



    }
}
