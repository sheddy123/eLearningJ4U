using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace eLearningLMS.Models
{
    public class DetailFile
    {
        public Guid Id { get; set; }
        public string FileName { get; set; }
        public string Extension { get; set; }
        public int InstructorId { get; set; }
        public virtual Instructor Instructor { get; set; }
        public int SupportId { get; set; }
        public  Support Support { get; set; }
        public int CourseCode { get; set; }
    }
}