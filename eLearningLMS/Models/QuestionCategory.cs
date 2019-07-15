using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace eLearningLMS.Models
{
    public class QuestionCategory
    {
        public int Id { get; set; }
        public string Category { get; set; }
        public ICollection<Question> Questions { get; set; }
    }
}