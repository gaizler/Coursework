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
    class ShowUsersByGroupViewModel:ViewModelBase
    {
        #region Variables
        public Group SelectedGroup { get; set; } = null;

        [SafeForDependencyAnalysis]
        public IList UsersList
        {
            get
            {
                if (SelectedGroup != null)
                {
                    GenericUnitOfWork unitOfWork = new GenericUnitOfWork(new TestContext(ConfigurationManager.ConnectionStrings["conStr"].ConnectionString));
                    var repo = unitOfWork.Repository<User>();
                    return repo.GetWithInclude(x=>x.Group.Id==SelectedGroup.Id,g=>g.Group).Select(y => new { FirtsName = y.FirstName, LastName = y.LastName, Login = y.Login, IsAdmin = y.IsAdmin, Group = y.Group.ToString() }).ToList();
                }
                return null;
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
        #endregion

    }
}
