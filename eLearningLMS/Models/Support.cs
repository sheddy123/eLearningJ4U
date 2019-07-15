﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace eLearningLMS.Models
{
    public class Support
    {
        public int SupportId { get; set; }

        [Required(ErrorMessage = "Please Enter Your Name")]
        [Display(Name = "Name")]
        [MaxLength(100)]
        public string Name { get; set; }

        [Required(ErrorMessage = "Please Enter Summary")]
        [Display(Name = "Summary")]
        [MaxLength(500)]
        public string Summary { get; set; }

        [Required]
        public int CoursesId { get; set; }
        [Display(Name = "Course")]
        public virtual Course Course { get; set; }

        [DisplayFormat(DataFormatString = "{0:dd MMM yyyy}")]
        public DateTime dateUploaded { get; set; }

        public virtual ICollection<FileDetai> FileDetai { get; set; }

        
    }
}