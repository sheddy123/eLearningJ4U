using eLearningLMS.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace eLearningLMS.Controllers
{
    public class HomeController : Controller
    {
        ApplicationUser CurrentUser = System.Web.HttpContext.Current.GetOwinContext().GetUserManager<ApplicationUserManager>().FindById(
              System.Web.HttpContext.Current.User.Identity.GetUserId());
        ApplicationDbContext context;
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult LoginIndex()
        {

            if (Request.IsAuthenticated)
            {
                return View();
            }
            return RedirectToAction("Login", "Account");
        }

        public ActionResult QuizIndex()
        {
            context = new ApplicationDbContext();
            ViewBag.Tests = context.Tests.Where(t => t.IsActive == true).Select(x => new { x.Id, x.Name }).ToList();
            //ViewBag.Tests = context.Tests.Where(t => t.IsActive == true).SingleOrDefault();
            SessionModel model = null;

            if (Session["SessionModel"] == null)
                model = new SessionModel();
            else
                model = (SessionModel)Session["SessionModel"];

            return View(model);
        }

        public ActionResult Instruction(SessionModel model)
        {
            if (model != null)
            {
                context = new ApplicationDbContext();
                var test = context.Tests.Where(x => x.IsActive == true && x.Id == model.TestId).SingleOrDefault();
                var questionCount = context.TestXQuestions.ToList(); /*.Where(x => x.IsActive == true && x.TestId == model.TestId).FirstOrDefault();*/
                if (test != null)
                {
                    ViewBag.TestName = test.Name;
                    ViewBag.TestDescription = test.Description;
                    ViewBag.QuestionCount = questionCount.Count();
                    ViewBag.TestDuration = test.DurationInMinute;
                }
            }
            return View(model);
        }

        public ActionResult Register(SessionModel model)
        {
            if (model != null)
                Session["SessionModel"] = model;

            if (model == null || string.IsNullOrEmpty(model.UserName) || model.TestId < 1)
            {
                TempData["message"] = "Invalid Registration details. Please try again";
                return RedirectToAction("QuizIndex");
            }
            context = new ApplicationDbContext();
            //to register the user to the system
            //to register the user for test
            /*context.Students.where(x => x.Name.Equals(model.Username,
             * StringComparison.InvariantCultureIgnoreCase) && ((string.IsNullOrEmpty(model.Email)
             * && string.IsNullOrEmpty(x/Email)) || x.Email == model.Email)) && ((string.IsNullOrEmpty(model.Phone)
             * && string.IsNullOrEmpty(x.Phone)) || (x.Phone == model.Phone))).FirstOrDefault();*/
            Student user = context.Students.Where(x => x.FirstName.Equals(model.UserName, StringComparison.InvariantCultureIgnoreCase)
            || x.LastName.Equals(model.UserName, StringComparison.InvariantCultureIgnoreCase)).FirstOrDefault();
            //Instructor instructor = context.Instructors.Where(x => x.FirstName.Equals(model.UserName, StringComparison.InvariantCultureIgnoreCase)).FirstOrDefault();
            if (user == null)
            {
                if (CurrentUser.FirstName != model.UserName || CurrentUser.LastName != model.UserName)
                {
                    TempData["message"] = "Invalid Registration details. Enter your Firstname or LastName";
                    return RedirectToAction("QuizIndex");
                }
                else
                {
                    user = new Student()
                    {
                        FirstName = model.UserName
                    };
                    context.Students.Add(user);
                    context.SaveChanges();
                }
            }

            Registration registration = context.Registrations.Where(x => x.StudentId == user.Id
            && x.TestId == model.TestId
            && x.ExpireTime > DateTime.UtcNow).FirstOrDefault();
            if (registration != null)
            {
                this.Session["TOKEN"] = registration.Token;
                this.Session["EXPIRETIME"] = registration.ExpireTime;
            }
            else
            {
                Test test = context.Tests.Where(x => x.IsActive && x.Id == model.TestId).FirstOrDefault();
                if (test != null)
                {
                    Registration newRegistration = new Registration()
                    {
                        RegistrationDate = DateTime.UtcNow,
                        TestId = model.TestId,
                        Token = Guid.NewGuid(),
                        StudentId = user.Id,
                        ExpireTime = DateTime.UtcNow.AddMinutes(test.DurationInMinute)
                    };
                    //user.Registration.Add(newRegistration);
                    context.Registrations.Add(newRegistration);
                    context.SaveChanges();
                    this.Session["TOKEN"] = newRegistration.Token;
                    this.Session["EXPIRETIME"] = newRegistration.ExpireTime;
                }
            }
            return RedirectToAction("EvalPage", new { @token = Session["TOKEN"] });
        }

        public ActionResult EvalPage(Guid token, int? qno)
        {
            if (token == null)
            {
                TempData["message"] = "You have an Invalid token. Please re-register and try again";
                return RedirectToAction("QuizIndex");
            }

            context = new ApplicationDbContext();
            //verify the user is registered and is allowed to check the question
            var registration = context.Registrations.Where(x => x.Token.Equals(token)).FirstOrDefault();
            if (registration == null)
            {
                TempData["message"] = "This token is invalid";
                return RedirectToAction("QuizIndex");
            }
            if (registration.ExpireTime < DateTime.UtcNow)
            {
                TempData["message"] = "The exam duration has expired at " + registration.ExpireTime.ToString();
                return RedirectToAction("QuizIndex");
            }
            if (qno.GetValueOrDefault() < 1)
                qno = 1;

            var testQuestionId = context.TestXQuestions
                .Where(x => x.TestId == registration.TestId && x.QuestNumbers == qno)
                .Select(x => x.TestXQuestionId).FirstOrDefault();

            if (testQuestionId > 0)
            {
                var model = context.TestXQuestions.Where(x => x.TestXQuestionId == testQuestionId)
                    .Select(x => new QuestionModel()
                    {
                        QuestionType = x.Question.QuestionType,
                        QuestionNumber = x.QuestNumbers,
                        Question = x.Question.QuestionId,
                        Point = x.Question.Points,
                        TestId = x.TestId,
                        TestName = x.Test.Name,
                        Options = context.Choices.Where(y => y.IsActive == true && y.QuestionId == x.Question.Id).Select(y => new QXOModel()
                        {
                            ChoiceId = y.Id,
                            Label = y.Label,
                        }).ToList()
                    }).FirstOrDefault();
                //now if it is already answered earlier, set the choice of the user
                var savedAnswers = context.TestXPapers.Where(x => x.TestXQuestionsId == testQuestionId && x.RegistrationId == registration.Id && x.Choice.IsActive == true)
                    .Select(x => new { x.ChoiceId, x.Answer }).ToList();

                foreach (var savedAnswer in savedAnswers)
                {
                    model.Options.Where(x => x.ChoiceId == savedAnswer.ChoiceId).FirstOrDefault().Answer = savedAnswer.Answer;
                }
                model.TotalQuestionInSet = context.TestXQuestions.Where(x => x.Question.IsActive == true
                && x.TestId == registration.TestId).Count();
                ViewBag.TimeExpire = registration.ExpireTime;
                return View(model);
            }
            else
            {
                return View("Error");
            }

        }

        public ActionResult Calendar()
        {
            return View();
        }

        public ActionResult Email()
        {
            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }

        public FileContentResult UserPhotos()
        {
            String userId = User.Identity.GetUserId();
            if (userId == null)
            {
            }
            // to get the user details to load user Image
            var bdUsers = HttpContext.GetOwinContext().Get<ApplicationDbContext>();
            var userImage = bdUsers.Users.Where(x => x.Id == userId).FirstOrDefault();

            if (userImage == null)
            {
                return null;
            }
            return new FileContentResult(userImage.UserPhoto, "image/jpeg");

        }

        [HttpPost]
        public ActionResult PostAnswer(AnswerModel choices)
        {
            context = new ApplicationDbContext();

            //verify the user is registered and is allowed to check the question
            var registration = context.Registrations.Where(x => x.Token.Equals(choices.Token)).FirstOrDefault();
            if (registration == null)
            {
                TempData["message"] = "This token is invalid";
                return RedirectToAction("QuizIndex");
            }
            if (registration.ExpireTime < DateTime.UtcNow)
            {
                TempData["message"] = "The exam duration has expired at " + registration.ExpireTime.ToString();
                return RedirectToAction("QuizIndex");
            }

            var testQuestionInfo = context.TestXQuestions.Where(x => x.TestId == registration.TestId
            && x.QuestNumbers == choices.QuestionId)
            .Select(x => new
            {
                TQId = x.TestXQuestionId,
                QT = x.Question.QuestionType,
                QID = x.TestXQuestionId,
                POINT = (decimal)x.Question.Points
            }).FirstOrDefault();

            if (testQuestionInfo != null)
            {
                if (choices.UserChoices.Count > 1)
                {
                    var allPointValueOfChoices =
                    (
                        from a in context.Choices.Where(x => x.IsActive)
                        join b in choices.UserSelectedId on a.QuestionId equals b
                        select new { a.Id, Points = (decimal)a.Points }).AsEnumerable()
                        .Select(x => new TestXPaper()
                        {
                            RegistrationId = registration.Id,
                            TestXQuestionsId = testQuestionInfo.QID,
                            ChoiceId = x.Id,
                            Answer = "CHECKED",
                            MarkScored = Convert.ToInt32(Math.Floor((testQuestionInfo.POINT / 100.00M) * x.Points))
                        }
                        ).ToList();
                    context.TestXPapers.AddRange(allPointValueOfChoices);
                }
                else
                {
                    //the answer is of type TEXT
                    context.TestXPapers.Add(new TestXPaper()
                    {
                        RegistrationId = registration.Id,
                        TestXQuestionsId = testQuestionInfo.QID,
                        ChoiceId = choices.UserChoices.FirstOrDefault().ChoiceId,
                        MarkScored = 1,
                        Answer = choices.Answer
                    });
                }
                context.SaveChanges();
            }
            //get the next question depending on the direction
            var nextQuestionNumber = 1;
            if (choices.Direction.Equals("forward", StringComparison.CurrentCultureIgnoreCase))
            {
                nextQuestionNumber = context.TestXQuestions.Where(x => x.TestId == choices.TestId
                && x.QuestNumbers > choices.QuestionId)
                .OrderBy(x => x.QuestNumbers).Take(1).Select(x => x.QuestNumbers).FirstOrDefault();
            }
            else
            {
                nextQuestionNumber = context.TestXQuestions.Where(x => x.TestId == choices.TestId
                && x.QuestNumbers > choices.QuestionId)
                .OrderByDescending(x => x.QuestNumbers).Take(1).Select(x => x.QuestNumbers).FirstOrDefault();
            }

            if (nextQuestionNumber < 1)
                nextQuestionNumber = 1;

            return RedirectToAction("EvalPage", new
            {
                @token = Session["TOKEN"],
                @qno = nextQuestionNumber
            });
        }
    }
}
