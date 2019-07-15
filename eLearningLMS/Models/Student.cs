using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace eLearningLMS.Models
{
    public class Student
    {
        
        public int Id { get; set; }


        [Required]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        [Display(Name = "Matriculation Number")]
        public string MatriculationNumber { get; set; }

        [Required]
        [Display(Name ="Marital Status")]
        public MaritalStatus MaritalStat{get; set;}

        public string userId { get; set; }

        [Display(Name ="First Name")]
        [StringLength(20, MinimumLength =3)]
        public string FirstName { get; set; }

        [Display(Name = "Last Name")]
        [StringLength(20, MinimumLength = 3)]
        public string LastName { get; set; }

        [Required]
        [Display(Name ="Gender")]
        public Gender Genderr { get; set; }

        [Required]
        [Display(Name ="Religion")]
        public Religion Religions { get; set; }

        [Required]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString ="{0:dd MMM yyyy}")]
        [Display(Name ="Date of Birth")]
        public DateTime DOB { get; set; }

        [Required]
        [Display(Name ="Nationality")]
        [StringLength(30, MinimumLength =3)]
        public string Nationality { get; set; }

        [Required]
        [Display(Name = "State of Origin")]
        [StringLength(30, MinimumLength = 3)]
        public string StateOfOrigin { get; set; }

        [Required]
        [Display(Name = "Local Government")]
        [StringLength(30, MinimumLength = 3)]
        public string LGA { get; set; }


        [Required]
        [Display(Name ="Level")]
        public int level  { get; set; }

        [Required]
        [Display(Name ="Department")]
        public int DepartmentId { get; set; }
        public ICollection<Department> Department { get; set; }

        [Required]
        [Display(Name ="Programme")]
        public int ProgrammeId { get; set; }
        public ICollection<Programme> Programme { get; set; }

        public ICollection<Enrollment> Enrollments { get; set; }
        
        public ICollection<Registration> Registration { get; set; }
        

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