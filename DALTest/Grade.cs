using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DALTest
{
    [Serializable]
    public class Grade
    {
        public int Id { get; set; }
        public virtual User User { get; set; }
        public virtual Test Test { get; set; }
        public double Mark { get; set; }
    }
}
