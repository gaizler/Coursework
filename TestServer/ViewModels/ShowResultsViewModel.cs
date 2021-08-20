using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using DALTest;
using GalaSoft.MvvmLight;
using PostSharp.Patterns.Model;
using Repository;

namespace TestServer.ViewModels
{
    [NotifyPropertyChanged]
    class ShowResultsViewModel:ViewModelBase
    {
        [SafeForDependencyAnalysis]
        public IList ResultsList
        {
            get
            {
                GenericUnitOfWork unitOfWork = new GenericUnitOfWork(new TestContext(ConfigurationManager.ConnectionStrings["conStr"].ConnectionString));
                var repo = unitOfWork.Repository<Grade>();


                return repo.GetWithInclude(t=>t.Test,u=>u.User).Select(x => new { Test = x.Test.Title, User = x.User.Login, Mark = x.Mark }).ToList(); ;
            }
        }
    }
}
