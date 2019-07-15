using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Web;

namespace eLearningLMS.Models
{
    public class TestXPaper
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int TestXPaperId { get; set; }
  
    
        public string Answer { get; set; }
        public int MarkScored { get; set; }
        
        public int ChoiceId { get; set; }
        [ForeignKey("ChoiceId")]
        public Choice Choice { get; set; }
        
        public int RegistrationId { get; set; }
        [ForeignKey("RegistrationId")]
        public Registration Registration { get; set; }

        public int TestXQuestionsId { get; set; }
        public TestXQuestions TestXQuestions { get; set; }

     
    }
}