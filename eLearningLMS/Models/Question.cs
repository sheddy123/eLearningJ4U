using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace eLearningLMS.Models
{
    public class Question
    {
        public int Id { get; set; }
        public int QuestionCategoryId { get; set; }
        public string QuestionType { get; set; }
        public string QuestionId { get; set; }
        public int Points { get; set; }
        public bool IsActive { get; set; }
        public ICollection<Choice> Choices { get; set; }
        public QuestionCategory QuestionCategory { get; set; }
        public TestXQuestions testXQuestions { get; set; }
    }
}