using eLearningLMS.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace eLearningLMS.Dtos
{
    public class ProgrammeDto
    {
        public int Id { get; set; }
        
        [StringLength(30, MinimumLength = 3)]
        
        public string Programmes { get; set; }

        public DepartmentDto Department { get; set; }

        
        public int DepartmentId { get; set; }

        public Student Student { get; set; }
    }
}