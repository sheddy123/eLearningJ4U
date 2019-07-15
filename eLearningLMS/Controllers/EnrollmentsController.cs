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
using eLearningLMS.Models.CustomRoleClass;

namespace eLearningLMS.Controllers
{
    [Authorize(Roles = CustomRoles.Student)]
    public class EnrollmentsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        ApplicationUser CurrentUser = System.Web.HttpContext.Current.GetOwinContext().GetUserManager<ApplicationUserManager>().FindById(
                System.Web.HttpContext.Current.User.Identity.GetUserId());

        // GET: Enrollments
        public async Task<ActionResult> Index()

        {
            ViewBag.CourseID = new SelectList(db.Courses, "Id", "CourseCode");
            ViewBag.StudentID = new SelectList(db.Students, "Id", "userId");
            var enroll = db.Students.Where(i => i.userId == CurrentUser.Id).SingleOrDefault();
            ViewBag.CheckId = enroll;
            if (enroll == null)
            {
                ViewBag.Message = "Please enter your Personal Details";
            }
            else
            {
                var enrollments = db.Enrollments.Include(e => e.Course).Include(e => e.Student).Where(i => i.StudentID == enroll.Id);
                //if (enrollments.Count > 0)
                return View(await enrollments.ToListAsync());
            }

            return View();
        }

        
        // GET: Enrollments/Details/5
        //public async Task<ActionResult> Details(int? id)
        //{
        //    if (id == null)
        //    {
        //        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        //    }
        //    Enrollment enrollment = await db.Enrollments.FindAsync(id);
        //    if (enrollment == null)
        //    {
        //        return HttpNotFound();
        //    }
        //    return View(enrollment);
        //}

        // POST: Enrollments/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "EnrollmentID,CourseID,StudentID,Grades")] Enrollment enrollment)
        {
            var enroll = db.Students.Where(i => i.userId == CurrentUser.Id).Select(s => s.Id).SingleOrDefault();
            enrollment.StudentID = enroll;
            if (ModelState.IsValid)
            {
                db.Enrollments.Add(enrollment);
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            ViewBag.CourseID = new SelectList(db.Courses, "Id", "CourseCode", enrollment.CourseID);
            ViewBag.StudentID = new SelectList(db.Students, "Id", "userId", enrollment.StudentID);
            return View(enrollment);
        }

        // GET: Enrollments/Edit/5
        //public async Task<ActionResult> Edit(int? id)
        //{
        //    if (id == null)
        //    {
        //        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        //    }
        //    Enrollment enrollment = await db.Enrollments.FindAsync(id);
        //    if (enrollment == null)
        //    {
        //        return HttpNotFound();
        //    }
        //    ViewBag.CourseID = new SelectList(db.Courses, "Id", "CourseCode", enrollment.CourseID);
        //    ViewBag.StudentID = new SelectList(db.Students, "Id", "userId", enrollment.StudentID);
        //    return View(enrollment);
        //}

        // POST: Enrollments/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public async Task<ActionResult> Edit([Bind(Include = "EnrollmentID,CourseID,StudentID,Grades")] Enrollment enrollment)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        db.Entry(enrollment).State = EntityState.Modified;
        //        await db.SaveChangesAsync();
        //        return RedirectToAction("Index");
        //    }
        //    ViewBag.CourseID = new SelectList(db.Courses, "Id", "CourseCode", enrollment.CourseID);
        //    ViewBag.StudentID = new SelectList(db.Students, "Id", "userId", enrollment.StudentID);
        //    return View(enrollment);
        //}

        // GET: Enrollments/Delete/5
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Enrollment enrollment = await db.Enrollments.FindAsync(id);
            if (enrollment == null)
            {
                return HttpNotFound();
            }
            return View(enrollment);
        }

        // POST: Enrollments/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            Enrollment enrollment = await db.Enrollments.FindAsync(id);
            db.Enrollments.Remove(enrollment);
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
