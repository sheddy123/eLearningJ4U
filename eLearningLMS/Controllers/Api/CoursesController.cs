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

namespace eLearningLMS.Controllers.Api
{
    public class CoursesController : ApiController
    {
        private ApplicationDbContext _context;
        public CoursesController()
        {
            _context = new ApplicationDbContext();
        }

        // GET /api/courses
        public IEnumerable<CourseDto> GetCourses()
        {
            return _context.Courses
                .Include(d => d.Department)
                .ToList()
                .Select(Mapper.Map<Course, CourseDto>);
        }

        //GET /api/courses/1
        public IHttpActionResult GetCourses(int id)
        {
            var course = _context.Courses.SingleOrDefault(c => c.Id == id);

            if (course == null)
                return NotFound();

            return Ok(Mapper.Map<Course, CourseDto>(course));
        }

        //POST /api/course
        [HttpPost]
        public IHttpActionResult CreateCourse(CourseDto courseDto)
        {
            if (!ModelState.IsValid)
                return BadRequest();
            
            var course = Mapper.Map<CourseDto, Course>(courseDto);
            _context.Courses.Add(course);
            _context.SaveChanges();

            courseDto.Id = course.Id;

            return Created(new Uri(Request.RequestUri + "/" + course.Id), courseDto);
        }

        //PUT /api/courses/1
       [HttpPut, Route("api/courses/{id}")]
        public void Put(int id, [FromBody]CourseDto courseDto)
        {
            if (!ModelState.IsValid)
                throw new HttpResponseException(HttpStatusCode.BadRequest);

            var courseInDb = _context.Courses.SingleOrDefault(p => p.Id == id);

            if (courseInDb == null)
                throw new HttpResponseException(HttpStatusCode.NotFound);

            Mapper.Map(courseDto, courseInDb);

            _context.SaveChanges();
        }

        //DELETE /api/customers/1
        [HttpDelete, Route("api/courses/{id}")]
        public void Delete(int id)
        {
            var courseInDb = _context.Courses.SingleOrDefault(p => p.Id == id);

            if (courseInDb == null)
                throw new HttpResponseException(HttpStatusCode.NotFound);

            _context.Courses.Remove(courseInDb);
            _context.SaveChanges();
            
        }
    }
}
