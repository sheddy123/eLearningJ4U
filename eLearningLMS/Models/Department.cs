using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace eLearningLMS.Models
{
    public class Department
    {
        public int DepartmentID { get; set; }
        [StringLength(50, MinimumLength = 3)]
        public string Name { get; set; }
        public int InstructorID { get; set; }
        public Instructor GetInstructor { get; set; }
        //public ICollection<Programme> Programme { get; set; }
        public ICollection<Instructor> Instructor { get; set; }
        //public ICollection<Course> Courses { get; set; }
        
    }
}