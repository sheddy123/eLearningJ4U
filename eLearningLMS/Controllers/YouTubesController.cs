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
using eLearningLMS.ViewModels;
using eLearningLMS.Models.CustomRoleClass;

namespace eLearningLMS.Controllers
{
    
    public class YouTubesController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        ApplicationUser CurrentUser = System.Web.HttpContext.Current.GetOwinContext().GetUserManager<ApplicationUserManager>().FindById(
                System.Web.HttpContext.Current.User.Identity.GetUserId());
        // GET: YouTubes
        [Authorize(Roles = CustomRoles.Student)]
        public ActionResult Index()
        {
            var studentId = db.Students.Where(i => i.userId == CurrentUser.Id).Select(c => c.Id).FirstOrDefault();

            var enrolled = db.Enrollments.Include(c => c.Course).Where(c => c.StudentID == studentId).ToList();

            //var courses = db.Courses.Where(c => c.Id == enrolled);
            var enrolledCourses = new HashSet<int>(enrolled.Select(c => c.CourseID));
            List<YouTube> Youtubes = new List<YouTube>();
            foreach (var course in enrolledCourses)
            {
                var postedVideos = db.YouTubes.Where(c => c.CourseId == course).ToList();
                Youtubes.AddRange(postedVideos);
            }
            //ViewBag.Youtube = Youtubes;

            
            //var youTubes = db.YouTubes.Include(y => y.Course).Include(y => y.Instructor);
            return View(Youtubes);
        }

        // GET: YouTubes/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            YouTube youTube = await db.YouTubes.FindAsync(id);
            if (youTube == null)
            {
                return HttpNotFound();
            }
            return View(youTube);
        }

        // GET: YouTubes/Create
        [Authorize(Roles = CustomRoles.Instructor)]
        public ActionResult Create()
        {
            var InstructorId = db.Instructors.Where(i => i.UserId == CurrentUser.Id).Select(i => i.id).FirstOrDefault();
            Instructor instructor = db.Instructors
            .Include(i => i.Courses)
            .Where(i => i.id == InstructorId)
            .Single();
            PopulateAssignedCourseData(instructor);
            GetInstructorPostedVideos(instructor);
            //ViewBag.CourseId = new SelectList(db.Courses, "Id", "CourseCode");
            ViewBag.InstructorId = new SelectList(db.Instructors.Where(c => c.id == InstructorId), "id", "FirstName");
            return View();
        }

        private void PopulateAssignedCourseData(Instructor instructor)
        {
            var instructorCourses = new HashSet<int>(instructor.Courses.Select(c => c.Id));
            List<Course> courseId = new List<Course>();
            foreach (var course in instructorCourses)
            {
                var Courses = db.Courses.Where(c => c.Id == course).ToList();
                courseId.AddRange(Courses);
            }
            ViewBag.CourseId = new SelectList(courseId, "Id", "CourseCode");
        }

        private void GetInstructorPostedVideos(Instructor instructor)
        {
            var checkInstructorVideo = db.YouTubes.Where(c => c.InstructorId == instructor.id).Select(d => d.InstructorId);
            if (checkInstructorVideo == null)
            {

            }
            else
            {
                var instructorCourses = new HashSet<int>(instructor.Courses.Select(c => c.Id));
                List<YouTube> Youtubes = new List<YouTube>();
                foreach (var course in instructorCourses)
                {
                    var postedVideos = db.YouTubes.Where(c => c.CourseId == course && c.InstructorId == instructor.id).ToList();
                    Youtubes.AddRange(postedVideos);
                }
                ViewBag.Youtube = Youtubes;
            }
        }

        // POST: YouTubes/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = CustomRoles.Instructor)]
        public async Task<ActionResult> Create([Bind(Include = "Id,Name,Link,InstructorId,CourseId")] YouTube youTube)
        {
            if (ModelState.IsValid)
            {
                string second_string = string.Empty;
                if (youTube.Link.Contains(string.Format("https://www.youtube.com/watch?v=")))
                {
                    second_string = youTube.Link.Remove(youTube.Link.IndexOf(string.Format("https://www.youtube.com/watch?v=")), string.Format("https://www.youtube.com/watch?v=").Length);
                }
                else if (youTube.Link.Contains(string.Format("www.youtube.com/watch?v=")))
                {
                    second_string = youTube.Link.Remove(youTube.Link.IndexOf(string.Format("www.youtube.com/watch?v=")), string.Format("www.youtube.com/watch?v=").Length);
                }
                else
                {
                    ViewBag.Link = "The Link is not valid";
                }
                youTube.Link = second_string;
                db.YouTubes.Add(youTube);
                await db.SaveChangesAsync();
                return RedirectToAction("Create", "YouTubes");
            }


            ViewBag.CourseId = new SelectList(db.Courses, "Id", "CourseCode", youTube.CourseId);
            ViewBag.InstructorId = new SelectList(db.Instructors, "id", "LastName", youTube.InstructorId);
            return View(youTube);
        }

        // GET: YouTubes/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            YouTube youTube = await db.YouTubes.FindAsync(id);
            if (youTube == null)
            {
                return HttpNotFound();
            }
            ViewBag.CourseId = new SelectList(db.Courses, "Id", "CourseCode", youTube.CourseId);
            ViewBag.InstructorId = new SelectList(db.Instructors, "id", "LastName", youTube.InstructorId);
            return View(youTube);
        }

        // POST: YouTubes/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "Id,Name,Link,InstructorId,CourseId")] YouTube youTube)
        {
            if (ModelState.IsValid)
            {
                db.Entry(youTube).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            ViewBag.CourseId = new SelectList(db.Courses, "Id", "CourseCode", youTube.CourseId);
            ViewBag.InstructorId = new SelectList(db.Instructors, "id", "LastName", youTube.InstructorId);
            return View(youTube);
        }

        // GET: YouTubes/Delete/5
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            YouTube youTube = await db.YouTubes.FindAsync(id);
            if (youTube == null)
            {
                return HttpNotFound();
            }
            return View(youTube);
        }

        // POST: YouTubes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            YouTube youTube = await db.YouTubes.FindAsync(id);
            db.YouTubes.Remove(youTube);
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
