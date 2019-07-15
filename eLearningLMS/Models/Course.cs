using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace eLearningLMS.Models
{
    public class Course
    {
        
        public int Id { get; set; }
        [Display(Name = "Course Code")]
        [Required]
        [StringLength(20, MinimumLength =2)]
        public string CourseCode { get; set; }
        [StringLength(50, MinimumLength = 3)]
        public string Title { get; set; }
        [Range(0, 5)]
        public int Credits { get; set; }
        public int DepartmentID { get; set; }
        public Department Department { get; set; }
        public virtual ICollection<Enrollment> Enrollments { get; set; }
        public ICollection<Instructor> Instructors { get; set; }
        public ICollection<Support> Supports { get; set; }
        

    }
}