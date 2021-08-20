using DALTest;
using Repository;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestServer.ViewModels
{
    public class ShowUsersViewModel
    {
        public IList UsersList
        {
            get
            {
                GenericUnitOfWork unitOfWork = new GenericUnitOfWork(new TestContext(ConfigurationManager.ConnectionStrings["conStr"].ConnectionString));
                var repo = unitOfWork.Repository<User>();
                return repo.GetWithInclude(x=>x.Group).Select(x=>new { FirstName=x.FirstName??"none", LastName=x.LastName??"none", IsAdmin=x.IsAdmin, Login=x.Login,Group=x.Group.ToString()}).ToList();
              
            }
        }
    }
}
