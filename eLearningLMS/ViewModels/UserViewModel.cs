using eLearningLMS.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace eLearningLMS.ViewModels
{
    public class UserViewModel
    {
        [Display(Name ="User Id")]
        public string Id { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [Display(Name ="First Name")]
        [StringLength(30, ErrorMessage ="Please enter First Name", MinimumLength =2)]
        public string FirstName { get; set; }

        [Required]
        [Display(Name = "Last Name")]
        [StringLength(30, ErrorMessage = "Please enter Last Name", MinimumLength = 2)]
        public string LastName { get; set; }

        [Required]
        [StringLength(100)]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        
        [Required]
        [Display(Name = "UserRoles")]
        public string UserRoles { get; set; }


        [Display(Name = "Profile Photo")]
        public byte[] ProfileImage { get; set; }
    }
}