using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace eLearningLMS.Models
{
    public class TestXQuestions
    {
        [Key, Column(Order = 0)]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int TestXQuestionId { get; set; }

        public int TestId { get; set; }
        
        public int QuestNumbers { get; set; }
        
        public bool IsActive { get; set; }

     
        public int QuestionId { get; set; }
        
        [Required]
        public virtual Question Question { get; set; }

        public QuestionXDuration QuestionXDuration {get; set;}

        public Test Test { get; set; }
       
        public ICollection<TestXPaper> TestXPaper { get; set; }
    }
}