using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace eLearningLMS.Models
{
    public class QuestionModel
    {
       public int TotalQuestionInSet { get; set; }
        public int QuestionNumber { get; set; }
        public int TestId { get; set; }
        public string TestName { get; set; }
        public string Question { get; set; }
        public string QuestionType { get; set; }
        public int Point { get; set; }
        public List<QXOModel> Options { get; set; }
    }

    public class QXOModel
    {
        public int ChoiceId { get; set; }
        public string Label { get; set; }
        public string Answer { get; set; }
    }
}