using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace eLearningLMS.Dtos
{
    public class EnrollmentDto
    {
        public int EnrollmentID { get; set; }
        public int CourseID { get; set; }
        public int StudentID { get; set; }
        public Grade? Grades { get; set; }
        public CourseDto Course { get; set; }
        public StudentDto Student { get; set; }

        public enum Grade
        {
            A, B, C, D, F
        }
    }
}