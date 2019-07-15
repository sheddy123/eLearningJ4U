using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace eLearningLMS.Models
{
    public class QuestionXDuration
    {
        public int Id { get; set; }
        public int RegistrationId { get; set; }
        [Key]
        [ForeignKey("TestXQuestion")]
        public int TestXQuestionsId { get; set; }
        public int TestXQuestionId { get; set; }
        public DateTime RequestTime { get; set; }
        public DateTime LeaveTime { get; set; }
        public DateTime AnsweredTime { get; set; }
        public Registration Registration { get; set; }
        public TestXQuestions TestXQuestion { get; set; }
    }
}