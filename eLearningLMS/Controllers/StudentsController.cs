using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Net;
using System.Web;
using System.Web.Mvc;
using eLearningLMS.Models;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.AspNet.Identity;
using System.Data.Entity.Validation;
using System.Diagnostics;
using eLearningLMS.ViewModels;
using eLearningLMS.Models.CustomRoleClass;

namespace eLearningLMS.Controllers
{
    public class StudentsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        ApplicationUser CurrentUser = System.Web.HttpContext.Current.GetOwinContext().GetUserManager<ApplicationUserManager>().FindById(
                System.Web.HttpContext.Current.User.Identity.GetUserId());

        // GET: Students
        //public ActionResult Index()
        //{
        //    PopulateProgrammeDropDownList();
        //    PopulateDepartmentsDropDownList();
        //    return View();
        //}
        [Authorize(Roles = CustomRoles.Administrator)]
        public async Task<ActionResult> Index()
        {
            return View(await db.Students.ToListAsync());
        }

        // GET: Students/Details/5
        [Authorize(Roles = CustomRoles.Administrator)]
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Student student = await db.Students.FindAsync(id);
            if (student == null)
            {
                return HttpNotFound();
            }
            return View(student);
        }

        private void PopulateDepartmentsDropDownList(object selectedDepartment = null)
        {
            var departmentsQuery = from d in db.Departments
                                   orderby d.Name
                                   select d;
            ViewBag.DepartmentID = new SelectList(departmentsQuery, "DepartmentID",
                                    "Name", selectedDepartment);
        }
        private void PopulateProgrammeDropDownList(object selectedDepartment = null)
        {
            var programmeQuery = from p in db.Programmes
                                   orderby p.Programmes
                                   select p;
            ViewBag.ProgrammeID = new SelectList(programmeQuery, "Id",
                                    "Programmes", selectedDepartment);
        }
        [Authorize(Roles = CustomRoles.Student)]
        public ActionResult StudentDetails()
        {
            var Level = new List<int> { 100, 200, 300, 400, 500 };
            var aList = Level.Select((x, i) => new { Value = (x < 600 ? x : x), Data = x }).ToList();
            ViewBag.LevelList = new SelectList(aList, "Value", "Data");
            ViewBag.FirstName = CurrentUser.FirstName;
            ViewBag.LastName = CurrentUser.LastName;
            
            ViewBag.Message = "Add Personal Details";
            PopulateDepartmentsDropDownList();
            PopulateProgrammeDropDownList();
            return PartialView("StudentDetails");
        }
        [Authorize(Roles = CustomRoles.Student)]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> StudentDetails([Bind(Include = "Id,FirstName, LastName,MatriculationNumber,MaritalStat,userId,Genderr,Religions,DOB,Nationality,StateOfOrigin,LGA,level,DepartmentId,ProgrammeId")] Student student)
        {
            student.userId = CurrentUser.Id;
            PopulateDepartmentsDropDownList();
            PopulateProgrammeDropDownList();
            //var check = db.Students.Where(s => s.userId == CurrentUser.Id).Select(c => c.userId).SingleOrDefault();
            
            try
            {
                //if (check == CurrentUser.Id)
                //{
                    if (ModelState.IsValid)
                    {
                        student.FirstName = CurrentUser.FirstName;
                        student.LastName = CurrentUser.LastName;
                        db.Students.Add(student);
                        await db.SaveChangesAsync();
                        return RedirectToAction("Index", "Enrollments");
                    }
                //}
                return RedirectToAction("Index", "Enrollments");
            }
            catch (DbEntityValidationException e)
            {
                foreach (var eve in e.EntityValidationErrors)
                {
                    //Console.WriteLine("Entity of type \"{0}\" in state \"{1}\" has the following validation erros:",
                    //    eve.Entry.Entity.GetType().Name, eve.Entry.State);
                    foreach (var ve in eve.ValidationErrors)
                    {
                        //Console.WriteLine("-Property: \"{0}\", Errpr: \"{1}\", Error: \"{1}\"",
                        //    ve.PropertyName, ve.ErrorMessage);
                        Trace.TraceInformation("Property: {0} Error: {1}", ve.PropertyName, ve.ErrorMessage);
                    }
                }
                throw;
            }
            //return View(student);
        }
        // GET: Students/Edit/5
        //public async Task<ActionResult> Edit(int? id)
        //{
        //    PopulateDepartmentsDropDownList();
        //    PopulateProgrammeDropDownList();
        //    if (id == null)
        //    {
        //        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        //    }
        //    Student student = await db.Students.FindAsync(id);
        //    if (student == null)
        //    {
        //        return HttpNotFound();
        //    }
        //    return View(student);
        //}

        // POST: Students/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public async Task<ActionResult> Edit([Bind(Include = "Id,MatriculationNumber,MaritalStat,userId,Genderr,Religions,DOB,Nationality,StateOfOrigin,LGA,level,DepartmentId,ProgrammeId")] Student student)
        //{
        //    PopulateDepartmentsDropDownList();
        //    PopulateProgrammeDropDownList();
        //    if (ModelState.IsValid)
        //    {
        //        db.Entry(student).State = EntityState.Modified;
        //        await db.SaveChangesAsync();
        //        return RedirectToAction("Index");
        //    }
        //    return View(student);
        //}

        // GET: Students/Delete/5
        [Authorize(Roles = CustomRoles.Administrator)]
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Student student = await db.Students.FindAsync(id);
            if (student == null)
            {
                return HttpNotFound();
            }
            return View(student);
        }

        // POST: Students/Delete/5
        [Authorize(Roles = CustomRoles.Administrator)]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            Student student = await db.Students.FindAsync(id);
            db.Students.Remove(student);
            await db.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
