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
    class ShowGroupsViewModel
    {
        public IList GroupsList
        {
            get
            {
                GenericUnitOfWork unitOfWork = new GenericUnitOfWork(new TestContext(ConfigurationManager.ConnectionStrings["conStr"].ConnectionString));
                var repo = unitOfWork.Repository<Group>();
                return repo.GetAll().Select(x => new { Group = x.Name }).ToList();
            }
        }
    }
}
