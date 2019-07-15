using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace eLearningLMS.Dtos
{
    public class StudentDto
    {
        public int Id { get; set; }


        [Required]
        
        public string MatriculationNumber { get; set; }

        [Required]
        public MaritalStatus MaritalStat { get; set; }

        public string userId { get; set; }
        
        [StringLength(20, MinimumLength = 3)]
        public string FirstName { get; set; }
        
        [StringLength(20, MinimumLength = 3)]
        public string LastName { get; set; }

        [Required]
        public Gender Genderr { get; set; }

        [Required]
        public Religion Religions { get; set; }

        [Required]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd MMM yyyy}")]
        public DateTime DOB { get; set; }

        [Required]
        [StringLength(30, MinimumLength = 3)]
        public string Nationality { get; set; }

        [Required]
        [StringLength(30, MinimumLength = 3)]
        public string StateOfOrigin { get; set; }

        [Required]
        [StringLength(30, MinimumLength = 3)]
        public string LGA { get; set; }


        [Required]
        public int level { get; set; }

        [Required]
        public int DepartmentId { get; set; }
        public ICollection<DepartmentDto> Department { get; set; }

        [Required]
        public int ProgrammeId { get; set; }
        public ICollection<ProgrammeDto> Programme { get; set; }

        public ICollection<EnrollmentDto> Enrollments { get; set; }

        public enum MaritalStatus
        {
            Single,
            Married,
            Divorce
        }

        public enum Gender
        {
            Male,
            Female
        }

        [Display(Name = "Full Name")]
        public string FullName
        {
            get { return LastName + ", " + FirstName; }
        }

        public enum Religion
        {
            Christianity,
            Muslim,
            Other
        }
    }
}