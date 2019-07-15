using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace eLearningLMS.Models
{
    public class FileDetai
    {
        public Guid Id { get; set; }
        public string FileName { get; set; }
        public string Extension { get; set; }
        public int InstructorId { get; set; }
        public virtual Instructor Instructor { get; set; }
        public int SupportId { get; set; }
        public  Support Support { get; set; }
        public int CourseCode { get; set; }
        [DisplayFormat(DataFormatString = "{0:dd MMM yyyy}")]
        public DateTime dateUploaded { get; set; }
    }
}