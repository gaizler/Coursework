using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestLibrary
{
    [Serializable]
    public class TestExam
    {
        public string Author { get; set; }
        public string Title { get; set; }
        public int NumberOfAnswers { get; set; }
        public List<Test> Tests { get; set; }
        public TestExam()
        {
            Tests = new List<Test>();
        }
    }
}
