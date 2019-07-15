using eLearningLMS.Models;
using eLearningLMS.Models.CustomRoleClass;
using eLearningLMS.ViewModels;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace eLearningLMS.Controllers
{

    public class SupportController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        ApplicationUser CurrentUser = System.Web.HttpContext.Current.GetOwinContext().GetUserManager<ApplicationUserManager>().FindById(
               System.Web.HttpContext.Current.User.Identity.GetUserId());

        [Authorize(Roles = CustomRoles.Student)]
        public ActionResult Index()
        {
            var getCurrentStudent = db.Students.Where(j => j.userId == CurrentUser.Id).Select(c => c.Id).SingleOrDefault();
            var instructors = db.Instructors.FirstOrDefault();//gets the Instructors
            var fileDetail = db.FileDetai.Where(s => s.InstructorId == instructors.id).FirstOrDefault();//Checks to see the file the instructor Uploaded

            var studentCourse = db.Enrollments.Where(x => x.StudentID == getCurrentStudent).Select(c => c.CourseID).ToList();

            var getCourseId = db.FileDetai.Select(x => x.CourseCode).ToList().Distinct();

            List<Support> sup = new List<Support>();
            IEnumerable<Support> gets = new List<Support>();
            List<FileDetai> addFileDetail = new List<FileDetai>();
            IEnumerable<FileDetai> getss = new List<FileDetai>();
            //int ch;
            int numss = 0;
            int i = 0;
            int itr = 0;
            foreach (var item in getCourseId.Distinct())
            {

                if (studentCourse.Contains(item))
                {
                    var supportId = db.Supports.Where(c => c.CoursesId == item).Select(d => d.SupportId).ToList();
                    var check = supportId;
                    //var enumerator = check.GetEnumerator();
                    var count = check.Count;
                    //var ResultList = check.Select(x => check.IndexOf(x));
                    //ch = check.GetEnumerator().Current;//ResultList;

                    for (int k = 0; k <= count; k++)
                    {  if (itr - 1 < count)
                        {
                            if (itr - 1 == count - 1)
                            {
                            }
                            else
                            {
                                numss = check[itr];
                            }
                        }
                        else
                        {
                            itr = 0;
                            numss = check[itr];
                        }
                        Support support = db.Supports.Include(s => s.FileDetai).OrderByDescending(x => x.dateUploaded).FirstOrDefault(x => x.SupportId.Equals(numss) && x.CoursesId == item);

                        var df = support.FileDetai.Distinct().ToList();
                        addFileDetail.AddRange(df.Distinct());
                        getss = addFileDetail.Distinct();
                        var studentCourses = db.Enrollments.Where(x => x.StudentID == getCurrentStudent && x.CourseID == item).Select(c => c.CourseID).ToList();

                        if (studentCourses.Contains(item))
                        {
                            var supports = db.Supports.Where(x => x.CoursesId == item && x.SupportId.Equals(numss)).Distinct().ToList();
                            //sup = supports;
                            i++;
                            sup.AddRange(supports.Distinct());
                            gets = sup.Distinct();
                        }
                        itr++;
                    }
                }
            }
            if (i > 0)
            {
                ViewBag.DownloadCourse = getss;
                return View(gets.ToList());
            }
            //PopulateCoursesDropDownList();

            return View(/*db.Supports.ToList()*/);
        }

        List<FileDetai> addFileDetail = new List<FileDetai>();
        IEnumerable<FileDetai> getss = new List<FileDetai>();

        [Authorize(Roles = CustomRoles.Instructor)]
        public ActionResult UploadFile()
        {
            var getCurrentInstructor = db.Instructors.Where(j => j.UserId == CurrentUser.Id).Select(c => c.id).SingleOrDefault();
            var instructorId = db.Instructors.Where(x => x.id == getCurrentInstructor).Select(c => c.id).FirstOrDefault();
            var fileDetail = db.FileDetai.Where(s => s.InstructorId == instructorId).Select(c => c.CourseCode).ToList();//Checks to see the file the instructor Uploaded
            var filesDetail = db.FileDetai.Where(s => s.InstructorId == instructorId).Select(c => c.SupportId).ToList();
            var getFileName = db.FileDetai.Where(s => s.InstructorId == instructorId).Select(c => c.FileName).ToList();
            //var filesDetail = db.FileDetai.Where(s => s.InstructorId == instructorId).Select(c => new { c.CourseCode, c.SupportId }).ToList();//Checks to see the file the instructor Uploaded
            var check = fileDetail;
            var count = fileDetail.Count;
            var chk = filesDetail;
            int it = 0;
            int nums = 0;
            int num1 = 0;
            
            for (int k = 0; k <= count; k++)
            {
                if (it - 1 < count)
                {
                    if (it - 1 == count - 1)
                    {
                    }
                    else
                    {
                        nums = check[it];//CourseCode
                        num1 = chk[it];//SUpportId
                    }
                }
                else
                {
                    it = 0;
                    nums = check[it];
                    num1 = chk[it];
                }
                if (it - 1 == count - 1)
                {
                }
                else
                {
                    //var support = db.Supports.Include(f => f.FileDetai).OrderByDescending(x => x.dateUploaded).Where(x => x.CoursesId.Equals(nums) && x.SupportId == num1);
                    var support = db.FileDetai.OrderByDescending(x => x.dateUploaded).Where(x => x.CourseCode.Equals(nums) && x.SupportId == num1);
                    //var supportId = db.Supports.Where(c => c.CoursesId == support).Select(d => d.SupportId).ToList();            
                    var df = support.ToList();
                    addFileDetail.AddRange(df);
                    getss = addFileDetail.Distinct();
                }
                
                Instructor instructor = db.Instructors
                .Include(i => i.Courses)
                .Where(i => i.id == getCurrentInstructor)
                .Single();
                PopulateAssignedCourseData(instructor);
                it++;
            }
            ViewBag.Details = getss;
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

        [Authorize(Roles = CustomRoles.Instructor)]
        [HttpPost]
        public ActionResult UploadFile(Support support)
        {
            var getInstructorName = db.Instructors.Where(j => j.UserId == CurrentUser.Id).Select(c => c.id).SingleOrDefault();
            var instructorsName = db.Instructors.Where(i => i.UserId == CurrentUser.Id).Select(c => c.FirstName).SingleOrDefault();
            support.Name = instructorsName;


            List<FileDetai> fileDetails = new List<FileDetai>();
            for (int i = 0; i < Request.Files.Count; i++)
            {
                var file = Request.Files[i];

                if (file != null && file.ContentLength > 0)
                {
                    var fileName = Path.GetFileName(file.FileName);
                    FileDetai fileDetail = new FileDetai()
                    {
                        FileName = fileName,
                        Extension = Path.GetExtension(fileName),
                        Id = Guid.NewGuid()
                    };

                    fileDetail.InstructorId = getInstructorName;
                    fileDetail.CourseCode = support.CoursesId;
                    fileDetail.dateUploaded = DateTime.Now;
                    support.dateUploaded = fileDetail.dateUploaded;
                    fileDetails.Add(fileDetail);
                    var path = Path.Combine(Server.MapPath("~/App_Data/Upload/"), fileDetail.Id + fileDetail.Extension);
                    file.SaveAs(path);
                    db.Entry(fileDetail).State = EntityState.Added;
                }

                support.FileDetai = fileDetails;

                db.Supports.Add(support);
                db.SaveChanges();
                return RedirectToAction("UploadFile", "Support");
            }

            return View(support);
        }

        [Authorize(Roles = CustomRoles.Student + "," + CustomRoles.Instructor)]
        public FileResult Download(String p, String d)
        {
            return File(Path.Combine(Server.MapPath("~/App_Data/Upload/"), p), System.Net.Mime.MediaTypeNames.Application.Octet, d);
        }

        [Authorize(Roles = CustomRoles.Student)]
        public ActionResult Edit(int? id)
        {
            var getCurrentStudent = db.Students.Where(j => j.userId == CurrentUser.Id).Select(c => c.Id).SingleOrDefault();
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Support support = db.Supports.Include(s => s.FileDetai).SingleOrDefault(x => x.SupportId == id);
            var instructors = db.Instructors.FirstOrDefault();//gets the Instructors
            var fileDetail = db.FileDetai.Where(s => s.InstructorId == instructors.id).FirstOrDefault();//Checks to see the file the instructor Uploaded
            var getInstructorName = db.Instructors.Where(i => i.id == fileDetail.InstructorId).Select(c => c.id).SingleOrDefault();
            PopulateCoursesDropDownList();

            //viewModel.Courses = viewModel.Instructors.Where(i => i.id == getInstructorName).FirstOrDefault().Courses;//gets the instructors courses
            var instructorPostedCoursecode = db.FileDetai.Where(s => s.InstructorId == instructors.id && s.SupportId == id.Value).Select(c => c.CourseCode).FirstOrDefault();
            var instructorCourseCode = db.Courses.Where(x => x.Id == instructorPostedCoursecode).Select(c => c.CourseCode).First();

            var studentEnrollment = db.Enrollments.Where(x => x.StudentID == getCurrentStudent).Select(c => c.Course.CourseCode).ToList();//Gets the students enrolled courses
            if (studentEnrollment.Contains(instructorCourseCode))
            {
                var courses = db.Courses.Where(x => x.CourseCode == instructorCourseCode).Select(c => c.Id).First();
                ViewBag.Course = courses;
            }
            else
            {
                ViewBag.Course = null;
            }

            if (support == null)
            {
                return HttpNotFound();
            }
            return View(support);
        }

        private void PopulateCoursesDropDownList(object selectedCourse = null)
        {
            var departmentsQuery = from d in db.Courses
                                   orderby d.Title
                                   select d;
            ViewBag.CourseId = new SelectList(departmentsQuery, "Id",
                                    "CourseCode", selectedCourse);

            //ViewBag.DepartmentID = new SelectList(db.Courses, "Id", "CourseCode");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Support support)
        {
            if (ModelState.IsValid)
            {

                //New Files
                for (int i = 0; i < Request.Files.Count; i++)
                {
                    var file = Request.Files[i];

                    if (file != null && file.ContentLength > 0)
                    {
                        var fileName = Path.GetFileName(file.FileName);
                        FileDetai fileDetail = new FileDetai()
                        {
                            FileName = fileName,
                            Extension = Path.GetExtension(fileName),
                            Id = Guid.NewGuid(),
                            SupportId = support.SupportId,
                            CourseCode = support.CoursesId,
                            dateUploaded = DateTime.Now
                        };
                        var path = Path.Combine(Server.MapPath("~/App_Data/Upload/"), fileDetail.Id + fileDetail.Extension);
                        file.SaveAs(path);

                        db.Entry(fileDetail).State = EntityState.Added;
                    }
                }

                db.Entry(support).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(support);
        }

        [HttpPost]
        public JsonResult DeleteFile(string id)
        {
            if (String.IsNullOrEmpty(id))
            {
                Response.StatusCode = (int)HttpStatusCode.BadRequest;
                return Json(new { Result = "Error" });
            }
            try
            {
                Guid guid = new Guid(id);
                FileDetai fileDetail = db.FileDetai.Find(guid);
                var getSupportId = db.Supports.Where(x => x.SupportId == fileDetail.SupportId).FirstOrDefault();
                if (fileDetail == null)
                {
                    Response.StatusCode = (int)HttpStatusCode.NotFound;
                    return Json(new { Result = "Error" });
                }

                //Remove from database
                db.FileDetai.Remove(fileDetail);
                db.Supports.Remove(getSupportId);
                db.SaveChanges();

                //Delete file from the file system
                var path = Path.Combine(Server.MapPath("~/App_Data/Upload/"), fileDetail.Id + fileDetail.Extension);
                if (System.IO.File.Exists(path))
                {
                    System.IO.File.Delete(path);
                }
                return Json(new { Result = "OK" });
            }
            catch (Exception ex)
            {
                return Json(new { Result = "ERROR", Message = ex.Message });
            }
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