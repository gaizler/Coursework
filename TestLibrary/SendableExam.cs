using DALTest;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestLibrary
{
    [Serializable]
    public class SendableExam
    {
        public TestExam TestExam { get; set; }
        public TimeSpan Time { get; set; }
        public Group Group { get; set; }

        public override string ToString()
        {
            return $"{TestExam.Title}, time: {Time.TotalMinutes} min";
        }
    }
}
