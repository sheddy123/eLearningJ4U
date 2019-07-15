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
using System.Data.Entity.Infrastructure;
using eLearningLMS.ViewModels;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.AspNet.Identity;
using eLearningLMS.Models.CustomRoleClass;
using System.IO;

namespace eLearningLMS.Controllers
{
    [Authorize(Roles = CustomRoles.Instructor + "," + CustomRoles.Administrator)]
    public class InstructorsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        ApplicationUser CurrentUser = System.Web.HttpContext.Current.GetOwinContext().GetUserManager<ApplicationUserManager>().FindById(
                System.Web.HttpContext.Current.User.Identity.GetUserId());


        // GET: Instructors
        public ActionResult Index(int? ID, int? courseID)
        {
            var getInstructorName = db.Instructors.Where(i => i.UserId == CurrentUser.Id).Select(c => c.id).SingleOrDefault();
            var getInstructorNames = db.Departments.Where(i => i.InstructorID == getInstructorName).Select(c => c.DepartmentID).FirstOrDefault();
            var getHOD = db.Departments.Where(i => i.InstructorID == getInstructorName).Select(c => c.InstructorID).FirstOrDefault();
            ViewBag.HOD = getHOD;
            ViewBag.InstructorName = getInstructorName;
            var viewModel = new InstructorIndexData();
            if (getHOD == getInstructorName)
            {
                viewModel.Instructors = db.Instructors.Where(i => i.DepartmentId == getInstructorNames)
                                        .Include(i => i.OfficeAssignment).Include(i => i.Department)
                                        .Include(i => i.Courses.Select(c => c.Department))
                                        .OrderBy(i => i.LastName);

                if (ID != null)
                {
                    ViewBag.InstructorID = ID.Value;
                    viewModel.Courses = viewModel.Instructors.Where(i => i.id == ID.Value).Single().Courses;
                }
                //var instructors = db.Instructors.Include(i => i.OfficeAssignment);
                if (courseID != null)
                {
                    ViewBag.CourseId = courseID.Value;
                    var selectedCourse = viewModel.Courses.Where(i => i.Id == courseID.Value).Single();
                    db.Entry(selectedCourse).Collection(x => x.Enrollments).Load();
                    foreach (var enrollment in selectedCourse.Enrollments)
                    {
                        db.Entry(enrollment).Reference(x => x.Student).Load();
                    }
                    viewModel.Enrollments = selectedCourse.Enrollments;
                    //viewModel.Enrollments = viewModel.Courses.Where(i => i.CourseID.Equals(courseID.Value)).Single().Enrollments;
                }
            }
            else
            {
                return RedirectToAction("InstructorIndexes");
            }

            return View(viewModel);
        }


        public ActionResult InstructorIndexes(int? ID, int? courseID)
        {
            var getInstructorName = db.Instructors.Where(i => i.UserId == CurrentUser.Id).Select(c => c.id).SingleOrDefault();
            ViewBag.InstructorName = getInstructorName;
            var viewModel = new InstructorIndexData();

            viewModel.Instructors = db.Instructors.Where(i => i.id == getInstructorName)
                                   .Include(i => i.OfficeAssignment).Include(i => i.Department)
                                   .Include(i => i.Courses.Select(c => c.Department))
                                   .OrderBy(i => i.LastName);
            if (ID != null)
            {
                ViewBag.InstructorID = ID.Value;
                viewModel.Courses = viewModel.Instructors.Where(i => i.id == ID.Value).Single().Courses;
            }
            List<string> add = new List<string>();
            //var instructors = db.Instructors.Include(i => i.OfficeAssignment);
            if (courseID != null)
            {
                ViewBag.CourseId = courseID.Value;
                var selectedCourse = viewModel.Courses.Where(i => i.Id == courseID.Value).Single();
                var courseEnrolled = db.Enrollments.Where(x => x.CourseID == selectedCourse.Id).Select(c => c.StudentID).ToList();
                db.Entry(selectedCourse).Collection(x => x.Enrollments).Load();
                foreach (var enrollment in courseEnrolled) //selectedCourse.Enrollments)
                {
                    var studentName = db.Students.Where(c => c.Id == enrollment).Select(d => d.FirstName).ToList();

                    add.AddRange(studentName);
                    //  db.Entry(enrollment).Reference(x => x.Student).Load();
                }
                ViewBag.StudentName = add;
                //viewModel.Enrollments = selectedCourse.Enrollments;
                //viewModel.Enrollments = viewModel.Courses.Where(i => i.Id.Equals(courseID.Value)).Single().Enrollments;
            }

            return View(viewModel);
        }


        public ActionResult InstructorIndex(int? id)
        {
            var CurrentUser = System.Web.HttpContext.Current.GetOwinContext().GetUserManager<ApplicationUserManager>().FindById(
                System.Web.HttpContext.Current.User.Identity.GetUserId());

            var instructor = db.Instructors.Where(i => i.UserId == CurrentUser.Id).Select(i => i.id).SingleOrDefault();
            id = instructor;

            Instructor instructore = db.Instructors
            .Include(i => i.OfficeAssignment).Include(i => i.Courses)
            .Where(i => i.id == id)
            .Single();
            PopulateDepartmentsDropDownList();

            if (instructore == null)
            {
                return HttpNotFound();
            }
            //ViewBag.id = new SelectList(db.OfficeAssignments, "InstructorID", "Location", instructor.id);
            return View(instructore);

        }

        // GET: Instructors/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Instructor instructor = db.Instructors.Include(i => i.OfficeAssignment).Where(i => i.id == id).Single();
            if (instructor == null)
            {
                return HttpNotFound();
            }
            return View(instructor);
        }

        // GET: Instructors/Create
        public ActionResult Create()
        {
            ViewBag.id = new SelectList(db.OfficeAssignments, "InstructorID", "Location");
            PopulateDepartmentsDropDownList();
            return View();
        }

        // POST: Instructors/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "id,LastName,FirstName,UserId,HireDate,DepartmentId")] Instructor instructor)
        {
            if (ModelState.IsValid)
            {
                db.Instructors.Add(instructor);
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            ViewBag.id = new SelectList(db.OfficeAssignments, "InstructorID", "Location", instructor.id);
            return View(instructor);
        }

        private void PopulateDepartmentsDropDownList(object selectedDepartment = null)
        {
            var departmentsQuery = from d in db.Departments
                                   orderby d.Name
                                   select d;
            ViewBag.DepartmentID = new SelectList(departmentsQuery, "DepartmentID",
                                    "Name", selectedDepartment);
        }

        private void PopulateAssignedCourseData(Instructor instructor)
        {
            var getInstructor = db.Instructors.Find(instructor.id).DepartmentId;
            var allCourses = db.Courses.Where(c => c.DepartmentID == getInstructor);
            var instructorCourses = new HashSet<int>(instructor.Courses.Select(c => c.Id));
            var viewModel = new List<AssignedCourseData>();
            foreach (var course in allCourses)
            {
                viewModel.Add(new AssignedCourseData
                {
                    CourseCode = course.CourseCode,
                    Title = course.Title,
                    Assigned = instructorCourses.Contains(course.Id)
                });
            }
            ViewBag.Courses = viewModel;
        }

        // GET: Instructors/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            Instructor instructor = db.Instructors
            .Include(i => i.OfficeAssignment).Include(i => i.Courses)
            .Where(i => i.id == id)
            .Single();
            PopulateAssignedCourseData(instructor);
            PopulateDepartmentsDropDownList(instructor.DepartmentId);
            if (instructor == null)
            {
                return HttpNotFound();
            }
            return View(instructor);
        }

        // POST: Instructors/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int? Id, string[] selectedCourses, Instructor instru)
        {
            if (Id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var instructorToUpdate = db.Instructors
            .Include(i => i.OfficeAssignment)
            .Include(i => i.Courses)
            .Where(i => i.id == Id)
            .Single();

            if (TryUpdateModel(instructorToUpdate, "",
                new string[] { "DepartmentId", "LastName", "FirstName", "HireDate", "OfficeAssignment" }))
            {
                try
                {
                    if (String.IsNullOrWhiteSpace(instructorToUpdate.OfficeAssignment.Location))
                    {
                        instructorToUpdate.OfficeAssignment = null;
                    }
                    instructorToUpdate.DepartmentId = instru.DepartmentId;
                    UpdateInstructorCourses(selectedCourses, instructorToUpdate);
                    db.Entry(instructorToUpdate).State = EntityState.Modified;
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }
                catch (RetryLimitExceededException /* dex */)
                {
                    //Log the error (uncomment dex variable name and add a line here to write a log.
                    ModelState.AddModelError("", "Unable to save changes. Try again, and if the " +
                        "problem persists, see your system administrator.");
                }
            }
            //ViewBag.id = new SelectList(db.OfficeAssignments, "InstructorID", "Location", instructor.id);
            PopulateAssignedCourseData(instructorToUpdate);
            PopulateDepartmentsDropDownList();
            return View(instructorToUpdate);
        }

        private void UpdateInstructorCourses(string[] selectedCourses, Instructor instructorToUpdate)
        {
            if (selectedCourses == null)
            {
                instructorToUpdate.Courses = new List<Course>();
                return;
            }
            var selectedCoursesHS = new HashSet<string>(selectedCourses);
            var instructorCourses = new HashSet<int>(instructorToUpdate.Courses.Select(c => c.Id));
            foreach (var course in db.Courses)
            {
                if (selectedCoursesHS.Contains(course.CourseCode.ToString()))
                {
                    if (!instructorCourses.Contains(course.Id))
                    {
                        instructorToUpdate.Courses.Add(course);
                    }
                }
                else
                {
                    if (instructorCourses.Contains(course.Id))
                    {
                        instructorToUpdate.Courses.Remove(course);
                    }
                }
            }
        }

        // GET: Instructors/Delete/5
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Instructor instructor = await db.Instructors.FindAsync(id);
            if (instructor == null)
            {
                return HttpNotFound();
            }
            return View(instructor);
        }

        // POST: Instructors/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            Instructor instructor = await db.Instructors.FindAsync(id);
            db.Instructors.Remove(instructor);
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
