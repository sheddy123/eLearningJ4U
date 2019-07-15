using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace eLearningLMS.Models
{
    public class Choice
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
      
        [ForeignKey("Question")]
        public int QuestionId { get; set; }
        public string Label { get; set; }
        public int Points { get; set; }
        public bool IsActive { get; set; }

        public Question Question { get; set; }

        public virtual ICollection<TestXPaper> TestXPaper { get; set; }
    }
}