using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace eLearningLMS.Models
{
    public class Programme
    {
        public int Id { get; set; }
        [Display(Name="Programme")]
        [StringLength(30, MinimumLength =3)]
        [Required]
        public string Programmes { get; set; }
        [Required]
        [Display(Name ="Department")]
        public int DepartmentId { get; set; }
        public Department Department { get; set; }
        public Student Student { get; set; }
    }
}