using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DALTest
{
    [Serializable]
    public class Group
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public virtual List<User> Users{get;set;}
        public virtual List<Test> Tests { get; set; }
        public Group()
        {
            Users = new List<User>();
            Tests = new List<Test>();
        }
        public override string ToString() => Name;

    }
}
