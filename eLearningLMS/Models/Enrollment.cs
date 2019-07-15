using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using eLearningLMS.Models;

namespace eLearningLMS.Models
{
    public class Enrollment
    {
        public int EnrollmentID { get; set; }
        public int CourseID { get; set; }
        public int StudentID { get; set; }
        public Grade? Grades { get; set; }
        public Course Course { get; set; }
        public Student Student { get; set; }
        public ICollection<Course> CoursesEnum { get; set; }
        public enum Grade
        {
            A, B, C, D, F
        }
    }
}