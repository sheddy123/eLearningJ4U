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
using eLearningLMS.Models.CustomRoleClass;

namespace eLearningLMS.Controllers.Api
{
    [Authorize(Roles = CustomRoles.Administrator)]
    public class ProgrammesController : ApiController
    {
        private ApplicationDbContext _context;
        public ProgrammesController()
        {
            _context = new ApplicationDbContext();
        }

     

        // GET /api/programmes
        public IEnumerable<ProgrammeDto> GetProgrammes()
        {
            return _context.Programmes
                .Include(d => d.Department)
                .ToList()
                .Select(Mapper.Map<Programme, ProgrammeDto>);
        }


        //GET /api/programmes/1
        public IHttpActionResult GetProgramme(int id)
        {
            var programme = _context.Programmes.SingleOrDefault(c => c.Id == id);

            if (programme == null)
                return NotFound();

            return Ok(Mapper.Map<Programme, ProgrammeDto>(programme));
        }

        //POST /api/programmes
        [HttpPost]
        public IHttpActionResult CreateProgramme(ProgrammeDto programmeDto)
        {
            if (!ModelState.IsValid)
                return BadRequest();
            
            var programme = Mapper.Map<ProgrammeDto, Programme>(programmeDto);
            _context.Programmes.Add(programme);
            _context.SaveChanges();

            programmeDto.Id = programme.Id;

            return Created(new Uri(Request.RequestUri + "/" + programme.Id), programmeDto);
        }

        //PUT /api/programmes/1
       [HttpPut, Route("api/programmes/{id}")]
        public void Put(int id, [FromBody]ProgrammeDto programmeDto)
        {
            if (!ModelState.IsValid)
                throw new HttpResponseException(HttpStatusCode.BadRequest);

            var programmeInDb = _context.Programmes.SingleOrDefault(p => p.Id == id);

            if (programmeInDb == null)
                throw new HttpResponseException(HttpStatusCode.NotFound);

            Mapper.Map(programmeDto, programmeInDb);

            _context.SaveChanges();
        }

        //DELETE /api/customers/1
        [HttpDelete, Route("api/programmes/{id}")]
        public void Delete(int id)
        {
            var programmeInDb = _context.Programmes.SingleOrDefault(p => p.Id == id);

            if (programmeInDb == null)
                throw new HttpResponseException(HttpStatusCode.NotFound);

            _context.Programmes.Remove(programmeInDb);
            _context.SaveChanges();
            
        }
    }
}
