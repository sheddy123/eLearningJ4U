using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace eLearningLMS.Models
{
    public class YouTube
    {
        public int Id { get; set; }
        [Required]
        [StringLength(50)]
        public string Name { get; set; }
        [Required]
        public string Link { get; set; }
        public int InstructorId { get; set; }
        [ForeignKey("Course")]
        public int CourseId { get; set; }
        
        public Instructor Instructor { get; set; }
        public Course Course { get; set; }
    }
}