using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;

namespace eLearningLMS.Models
{
    // You can add profile data for the user by adding more properties to your ApplicationUser class, please visit https://go.microsoft.com/fwlink/?LinkID=317594 to learn more.
    public class ApplicationUser : IdentityUser
    {
        public bool IsGoogleAuthenticatorEnabled { get; set; }

        public string GoogleAuthenticatorSecretKey { get; set; }
        [Required]
        [Display(Name ="First Name")]
        public string FirstName { get; set; }
        [Required]
        [Display(Name ="Last Name")]
        public string LastName { get; set; }
        public DateTime Registered { get; set; }
        [Required]
        [Display(Name = "User Roles")]
        public string UserRoles { get; set; }
        // Here we add a byte to Save the user Profile Pictuer
        [Required]
        public byte[] UserPhoto { get; set; }
        
        public async Task<ClaimsIdentity> GenerateUserIdentityAsync(UserManager<ApplicationUser> manager)
        {
            // Note the authenticationType must match the one defined in CookieAuthenticationOptions.AuthenticationType
            var userIdentity = await manager.CreateIdentityAsync(this, DefaultAuthenticationTypes.ApplicationCookie);
            // Add custom user claims here
            return userIdentity;
        }
    }

    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        
        public ApplicationDbContext()
            : base("DefaultConnection", throwIfV1Schema: false)
        {
        }
        //DbSet<Student> Students { get; set; }
        //DbSet<Enrollment> Enrollments { get; set; }
        ////DbSet<Course> Courses { get; set; }
        //DbSet<Department> Department { get; set; }
        //DbSet<Programme> Programmes { get; set; }
        
        public static ApplicationDbContext Create()
        {
            return new ApplicationDbContext();
        }
        

        public System.Data.Entity.DbSet<eLearningLMS.Models.Instructor> Instructors { get; set; }

        public System.Data.Entity.DbSet<eLearningLMS.Models.OfficeAssignment> OfficeAssignments { get; set; }

        public System.Data.Entity.DbSet<eLearningLMS.Models.Department> Departments { get; set; }

        public System.Data.Entity.DbSet<eLearningLMS.Models.Course> Courses { get; set; }

        public System.Data.Entity.DbSet<eLearningLMS.Models.Student> Students { get; set; }

        public System.Data.Entity.DbSet<eLearningLMS.Models.Enrollment> Enrollments { get; set; }

        public System.Data.Entity.DbSet<eLearningLMS.Models.Programme> Programmes { get; set; }

        public System.Data.Entity.DbSet<eLearningLMS.Models.Registration> Registrations { get; set; }

        public System.Data.Entity.DbSet<eLearningLMS.Models.Test> Tests { get; set; }

        public System.Data.Entity.DbSet<eLearningLMS.Models.QuestionCategory> QuestionCategories { get; set; }

        public System.Data.Entity.DbSet<eLearningLMS.Models.Question> Questions { get; set; }

        public System.Data.Entity.DbSet<eLearningLMS.Models.TestXQuestions> TestXQuestions { get; set; }

        public System.Data.Entity.DbSet<eLearningLMS.Models.Choice> Choices { get; set; }

        public System.Data.Entity.DbSet<eLearningLMS.Models.QuestionXDuration> QuestionXDurations { get; set; }

        public System.Data.Entity.DbSet<eLearningLMS.Models.TestXPaper> TestXPapers { get; set; }
        
        public System.Data.Entity.DbSet<eLearningLMS.Models.FileDetai> FileDetai { get; set; }
        public System.Data.Entity.DbSet<eLearningLMS.Models.Support> Supports { get; set; }
        public System.Data.Entity.DbSet<eLearningLMS.Models.YouTube> YouTubes { get; set; }
    }
}