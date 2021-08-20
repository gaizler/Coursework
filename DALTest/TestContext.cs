using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DALTest
{
    public class TestContext:DbContext
    {
        public DbSet<User> Users { get; set; }
        public DbSet<Test> Tests { get; set; }
        public DbSet<Grade> Grades{ get; set; }
        public DbSet<Group> Groups { get; set; }
        public TestContext() { }
        public TestContext(string conStr) : base(conStr)
        {
            Database.SetInitializer<TestContext>(new ContextInitializer());
            this.Configuration.ProxyCreationEnabled = false;

        }
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Group>().HasMany<Test>(g => g.Tests).WithMany(t => t.Groups).Map(gt => { gt.MapLeftKey("groupId"); gt.MapRightKey("testId"); gt.ToTable("GroupTests"); });
        }


    }
}
