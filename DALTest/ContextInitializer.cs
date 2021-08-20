using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DALTest
{
    class ContextInitializer: CreateDatabaseIfNotExists<TestContext>
    {
        protected override void Seed(TestContext context)
        {
            Group group = new Group() { Name = "Default group" };
            context.Groups.Add(group);
            context.SaveChanges();
            context.Users.Add(new User() { Login="admin", Password="1", IsAdmin=true,Group=context.Groups.Where(x=>x.Name=="Default group").First() });
            context.SaveChanges();
        }
    }
}
