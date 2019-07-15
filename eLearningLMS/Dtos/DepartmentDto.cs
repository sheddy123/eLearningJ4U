using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace eLearningLMS.Dtos
{
    public class DepartmentDto
    {
        public int DepartmentID { get; set; }
        [StringLength(50, MinimumLength = 3)]
        public string Name { get; set; }
        public int InstructorID { get; set; }
        public InstructorDto InstructorDto { get; set; }
        //public ICollection<ProgrammeDto> Programme { get; set; }
        public ICollection<InstructorDto> Instructor { get; set; }
        public ICollection<CourseDto> Courses { get; set; }
    }
}