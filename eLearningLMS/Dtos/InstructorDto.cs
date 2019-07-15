using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace eLearningLMS.Dtos
{
    public class InstructorDto
    {
        public int id { get; set; }
        [Required]
        [StringLength(50)]
        public string LastName { get; set; }
        [Required]
        
        [StringLength(50)]
        public string FirstName { get; set; }
        public string UserId { get; set; }
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime HireDate { get; set; }
        [Required]
        public int DepartmentId { get; set; }
        
        public virtual DepartmentDto Department { get; set; }
        
        public string FullName
        {
            get { return LastName + ", " + FirstName; }
        }
    }
}