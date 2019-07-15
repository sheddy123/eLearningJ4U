using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using eLearningLMS.Models;

namespace eLearningLMS.Models
{
    public class Test
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public bool IsActive { get; set; }
        public double DurationInMinute { get; set; }
        public ICollection<Registration> registration { get; set; }
        public ICollection<TestXQuestions> testXQuestions { get; set; }
    }
}