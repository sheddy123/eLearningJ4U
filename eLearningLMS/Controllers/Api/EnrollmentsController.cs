using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Data.Entity;
using System.Net.Http;
using System.Web.Http;
using AutoMapper;
using eLearningLMS.Dtos;
using eLearningLMS.Models;
using System.Web;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.AspNet.Identity;

namespace eLearningLMS.Controllers.Api
{
    public class EnrollmentsController : ApiController
    {
        private ApplicationDbContext _context;
        ApplicationUser CurrentUser = System.Web.HttpContext.Current.GetOwinContext().GetUserManager<ApplicationUserManager>().FindById(
                System.Web.HttpContext.Current.User.Identity.GetUserId());
        public EnrollmentsController()
        {
            _context = new ApplicationDbContext();
        }

        //// GET /api/enrollments
        //public IEnumerable<EnrollmentDto> GetEnrollments()
        //{
        //    var CurrentuserId = "5b60c24e-7f74-4926-ab14-c728ab8796d8";
        //    var enroll = _context.Students.Where(i => i.userId == CurrentuserId).SingleOrDefault();
        //    return _context.Enrollments
        //        .Include(c => c.Course).Include(s => s.Student).Where(i => i.StudentID == enroll.Id)
        //        .ToList()
        //        .Select(Mapper.Map<Enrollment, EnrollmentDto>);
        //}

        //POST /api/enrollments
        [HttpPost]
        public IHttpActionResult CreateEnrollment(EnrollmentDto enrollmentDto)
        {
            if (!ModelState.IsValid)
                return BadRequest();

            var enrollment = Mapper.Map<EnrollmentDto, Enrollment>(enrollmentDto);
            _context.Enrollments.Add(enrollment);
            _context.SaveChanges();

            enrollmentDto.EnrollmentID = enrollment.EnrollmentID;

            return Created(new Uri(Request.RequestUri + "/" + enrollment.EnrollmentID), enrollmentDto);
        }


        //DELETE /api/enrollments/1
        [HttpDelete, Route("api/enrollments/{id}")]
        public void Delete(int id)
        {
            var enrollmentInDb = _context.Enrollments.Find(id);

            if (enrollmentInDb == null)
                throw new HttpResponseException(HttpStatusCode.NotFound);

            _context.Enrollments.Remove(enrollmentInDb);
            _context.SaveChanges();
            
        }
    }
}
