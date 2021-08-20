using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DALTest
{
    [Serializable]
    public class Test
    {
        public int Id { get; set; }
        public string Author { get; set; }
        public string Title { get; set; }
        public string Filename { get; set; }
        public int NumberOfQuestions { get; set; }
        public TimeSpan Time { get; set; }
        public virtual List<Group> Groups { get; set; }
        public Test()
        {
            Groups = new List<Group>();
        }
        public override string ToString() => Title;


    }
}
