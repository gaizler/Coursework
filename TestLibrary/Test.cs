using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestLibrary
{
    public enum TestDifficulty { Easy,Medium,Hard}

    [Serializable]
    public class Test
    {
        public Test()
        {
            Answers = new List<Answer>();
        }
        public string Question { get; set; }
        public List<Answer> Answers { get; set; }
        public TestDifficulty Difficulty { get; set; }
        public override string ToString() => Question;
    }
}
