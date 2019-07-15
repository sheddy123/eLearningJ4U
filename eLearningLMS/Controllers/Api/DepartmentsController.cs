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
    public class DepartmentsController : ApiController
    {
        private ApplicationDbContext _context;
        public DepartmentsController()
        {
            _context = new ApplicationDbContext();
        }

        // GET /api/courses
        public IEnumerable<DepartmentDto> GetDepartments()
        {

            var dept = _context.Departments.Include(i => i.GetInstructor)
               .ToList()
               .Select(Mapper.Map<Department, DepartmentDto>);
            return dept;
        }

        //GET /api/courses/1
        public IHttpActionResult GetDepartments(int id)
        {
            var department = _context.Departments.SingleOrDefault(c => c.DepartmentID == id);

            if (department == null)
                return NotFound();

            return Ok(Mapper.Map<Department, DepartmentDto>(department));
        }

        //POST /api/course
        [HttpPost]
        public IHttpActionResult CreateDepartment(DepartmentDto departmentDto)
        {
            if (!ModelState.IsValid)
                return BadRequest();
            
            var department = Mapper.Map<DepartmentDto, Department>(departmentDto);
            _context.Departments.Add(department);
            _context.SaveChanges();

            department.DepartmentID = department.DepartmentID;

            return Created(new Uri(Request.RequestUri + "/" + department.DepartmentID), department);
        }

        //PUT /api/courses/1
       [HttpPut, Route("api/courses/{id}")]
        public void Put(int id, [FromBody]DepartmentDto departmentDto)
        {
            if (!ModelState.IsValid)
                throw new HttpResponseException(HttpStatusCode.BadRequest);

            var departmentInDb = _context.Departments.SingleOrDefault(p => p.DepartmentID == id);

            if (departmentInDb == null)
                throw new HttpResponseException(HttpStatusCode.NotFound);

            Mapper.Map(departmentDto, departmentInDb);

            _context.SaveChanges();
        }

        //DELETE /api/customers/1
        [HttpDelete, Route("api/departments/{id}")]
        public void Delete(int id)
        {
            var departmentInDb = _context.Departments.SingleOrDefault(p => p.DepartmentID == id);

            if (departmentInDb == null)
                throw new HttpResponseException(HttpStatusCode.NotFound);

            _context.Departments.Remove(departmentInDb);
            _context.SaveChanges();
            
        }
    }
}
