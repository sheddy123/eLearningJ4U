using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace eLearningLMS.Models
{
    public class Registration
    {
        public int Id { get; set; }
    
        public int StudentId { get; set; }
 
        public int InstructorId { get; set; }
        public int TestId { get; set; }
        public DateTime RegistrationDate { get; set; }
        public Guid Token { get; set; }
        public DateTime ExpireTime { get; set; }
        public ICollection<QuestionXDuration> questionXDuration { get; set; }
        public Student Student { get; set; }
        public Test Test { get; set; }
        public ICollection<Instructor> Instructor { get; set; }
     
        public ICollection<TestXPaper> TestXPaper { get; set; }
    }
}