using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestLibrary
{
    [Serializable]
    public class Answer
    {
        public string AnswerText { get; set; }
        public bool isCorrect { get; set; }
        public override string ToString() => AnswerText;
    }
}
