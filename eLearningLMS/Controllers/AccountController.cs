using System;
using System.Globalization;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using eLearningLMS.Models;
using System.Data.Entity.Validation;
using System.Diagnostics;
using System.Collections.Generic;
using eLearningLMS.ViewModels;
using eLearningLMS.Extensions;
using System.Net;
using Microsoft.AspNet.Identity.EntityFramework;
using System.IO;
using System.Data.Entity;
using Google.Authenticator;
using OtpSharp;
using Base32;

namespace eLearningLMS.Controllers
{
    [Authorize]
    public class AccountController : Controller
    {
        private ApplicationSignInManager _signInManager;
        private ApplicationUserManager _userManager;
        ApplicationDbContext db = new ApplicationDbContext();
        private const string key = "qaz123!@@)(*"; // any 10-12 char string for use as private key in google authenticator
        public static readonly DateTime UNIX_EPOCH = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        public AccountController()
        {
            var CurrentUser = System.Web.HttpContext.Current.GetOwinContext().GetUserManager<ApplicationUserManager>().FindById(
                System.Web.HttpContext.Current.User.Identity.GetUserId());

        }

        public AccountController(ApplicationUserManager userManager, ApplicationSignInManager signInManager)
        {
            UserManager = userManager;
            SignInManager = signInManager;
        }

        public ApplicationSignInManager SignInManager
        {
            get
            {
                return _signInManager ?? HttpContext.GetOwinContext().Get<ApplicationSignInManager>();
            }
            private set
            {
                _signInManager = value;
            }
        }

        public ApplicationUserManager UserManager
        {
            get
            {
                return _userManager ?? HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
            private set
            {
                _userManager = value;
            }
        }

        //
        // GET: /Account/Login
        [AllowAnonymous]
        public ActionResult Login(string returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl;
            return View();
        }

        public ActionResult EmailNotConfirmed()
        {
            return View();
        }


        //
        // POST: /Account/Login
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Login(LoginViewModel model, string returnUrl)
        {

            if (!ModelState.IsValid)
            {
                return View(model);
            }
            var userid = db.Users.Where(c => c.Email.Equals(model.Email)).Select(x => x.Id).SingleOrDefault();
            //var userid = UserManager.FindByEmail(model.Email).Id;//.SingleOrDefault().ToString();
            Session["userId"] = userid;
            if (userid != null){
                if (!UserManager.IsEmailConfirmed(userid))
                {
                    return View("EmailNotConfirmed");
                }
            }

            // This doesn't count login failures towards account lockout
            // To enable password failures to trigger account lockout, change to shouldLockout: true
            var result = await SignInManager.PasswordSignInAsync(model.Email, model.Password, model.RememberMe, shouldLockout: false);
            switch (result)
            {
                case SignInStatus.Success:
                    return RedirectToLocal(returnUrl);
                case SignInStatus.LockedOut:
                    return View("Lockout");
                case SignInStatus.RequiresVerification:
                    return RedirectToAction("SendCode", new { ReturnUrl = returnUrl, RememberMe = model.RememberMe });
                case SignInStatus.Failure:
                default:
                    ModelState.AddModelError("", "Invalid login attempt.");
                    return View(model);
            }
            //  return View();
        }


        //
        // GET: /Account/VerifyCode
        [AllowAnonymous]
        public async Task<ActionResult> VerifyCode(string provider, string returnUrl, bool rememberMe)
        {
            // Require that the user has already logged in via username/password or external login
            if (!await SignInManager.HasBeenVerifiedAsync())
            {
                return View("Error");
            }

            return View(new VerifyCodeViewModel { Provider = provider, ReturnUrl = returnUrl, RememberMe = rememberMe });
        }

        [AllowAnonymous]
        [HttpGet]
        public async Task<ActionResult> EnableGoogleAuthenticator(string provider, string returnUrl, bool rememberMe)
        {
            var CurrentUser = System.Web.HttpContext.Current.GetOwinContext().GetUserManager<ApplicationUserManager>().FindById(
              System.Web.HttpContext.Current.User.Identity.GetUserId());

            byte[] secretKey = KeyGeneration.GenerateRandomKey(20);
            //string userName = User.Identity.GetUserName();
            var userss = Session["userId"];
            var userName = db.Users.Where(c => c.Id.Equals(userss.ToString())).Select(d => d.Email).First();
            string sessionID = HttpContext.Session.SessionID;
            string barcodeUrl = KeyUrl.GetTotpUrl(secretKey, userName) + "&issuer=MySuperApplication";

            var secret = Base32Encoder.Encode(secretKey);
            var barcode = HttpUtility.UrlEncode(barcodeUrl);
            var ff = Base32Encoder.Encode(secretKey);



            var model = new VerifyCodeViewModel
            {
                SecretKey = Base32Encoder.Encode(secretKey),
                BarcodeUrl = HttpUtility.UrlEncode(barcodeUrl)
            };
            //ViewBag.Status = model;

            return View(new VerifyCodeViewModel { Provider = provider, ReturnUrl = returnUrl, RememberMe = rememberMe, SecretKey = secret, BarcodeUrl = barcode });
            //return View(model);
        }

        [AllowAnonymous]
        [HttpPost]
        public async Task<ActionResult> EnableGoogleAuthenticator(VerifyCodeViewModel model)
        {

            byte[] secretKey = Base32Encoder.Decode(model.SecretKey);

            long timeStepMatched = 0;
            var otp = new Totp(secretKey);
            if (otp.VerifyTotp(model.Code, out timeStepMatched, new VerificationWindow(2, 2)))
            {
                //var user = await UserManager.FindByIdAsync(User.Identity.GetUserId());
                var userd = Session["userId"];
                var user = await UserManager.FindByIdAsync(userd.ToString());
                //var userName = db.Users.Where(c => c.Id.Equals(user)).Select(d => d.Email).First();
                user.IsGoogleAuthenticatorEnabled = true;
                user.GoogleAuthenticatorSecretKey = model.SecretKey;
                await UserManager.UpdateAsync(user);
                //var provider = Session["Provider"];
                //var returnUrl = Session["ReturnUrl"];
                //var RememberMe = Session["RememberMe"];
                //model.Provider = provider.ToString();
                var result = await SignInManager.TwoFactorSignInAsync(model.Provider, model.Code, isPersistent: model.RememberMe, rememberBrowser: model.RememberBrowser);
                switch (result)
                {
                    case SignInStatus.Success:
                        return RedirectToLocal(model.ReturnUrl);
                    case SignInStatus.LockedOut:
                        return View("Lockout");
                    case SignInStatus.Failure:
                    default:
                        ModelState.AddModelError("", "Invalid code.");
                        return View(model);
                }
            }
            else
                ModelState.AddModelError("Code", "The Code is not valid");


            return View(model);
        }


        //
        // POST: /Account/VerifyCode
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> VerifyCode(VerifyCodeViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            // The following code protects for brute force attacks against the two factor codes. 
            // If a user enters incorrect codes for a specified amount of time then the user account 
            // will be locked out for a specified amount of time. 
            // You can configure the account lockout settings in IdentityConfig
            var result = await SignInManager.TwoFactorSignInAsync(model.Provider, model.Code, isPersistent: model.RememberMe, rememberBrowser: model.RememberBrowser);
            switch (result)
            {
                case SignInStatus.Success:
                    return RedirectToLocal(model.ReturnUrl);
                case SignInStatus.LockedOut:
                    return View("Lockout");
                case SignInStatus.Failure:
                default:
                    ModelState.AddModelError("", "Invalid code.");
                    return View(model);
            }
        }

        //
        // GET: /Account/Register
        [AllowAnonymous]
        public ActionResult Register()
        {
            PopulateUsersRoles();
            return View();
        }

        //
        // POST: /Account/Register
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Register([Bind(Exclude = "ProfileImage")]RegisterViewModel model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    // To convert the user uploaded Photo as Byte Array before save to DB
                    byte[] imageData = null;
                    if (Request.Files.Count > 0)
                    {
                        HttpPostedFileBase poImgFile = Request.Files["ProfileImage"];

                        using (var binary = new BinaryReader(poImgFile.InputStream))
                        {
                            imageData = binary.ReadBytes(poImgFile.ContentLength);
                        }
                    }
                    var user = new ApplicationUser
                    {
                        UserName = model.Email,
                        Email = model.Email,
                        FirstName = model.FirstName,
                        LastName = model.LastName,
                        UserRoles = model.UserRoles,
                        EmailConfirmed = false,
                        Registered = DateTime.Now,
                        TwoFactorEnabled = true
                    };

                    //Here we pass the byte array to user context to store in db
                    user.UserPhoto = imageData;
                    var result = await UserManager.CreateAsync(user, model.Password);
                    if (result.Succeeded)
                    {
                        if (user.UserRoles == "Instructor")
                        {
                            db.Instructors.Add(new Instructor
                            {
                                FirstName = user.FirstName,
                                LastName = user.LastName,
                                HireDate = user.Registered,
                                UserId = user.Id
                            });
                            await db.SaveChangesAsync();
                        }
                        //Session["userId"] = model.Email;
                        await SignInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);
                        await this.UserManager.AddToRoleAsync(user.Id, model.UserRoles);
                        // For more information on how to enable account confirmation and password reset please visit https://go.microsoft.com/fwlink/?LinkID=320771
                        // Send an email with this link
                        string code = await UserManager.GenerateEmailConfirmationTokenAsync(user.Id);
                        var callbackUrl = Url.Action("ConfirmEmail", "Account", new { userId = user.Id, code = code }, protocol: Request.Url.Scheme);
                        await UserManager.SendEmailAsync(user.Id, "Confirm your account", "Please confirm your account by clicking <a href=\"" + callbackUrl + "\">here</a>");
                        await db.SaveChangesAsync();
                        return RedirectToAction("EmailNotConfirmed", "Account", new { Email = "" });
                        //   return RedirectToAction("Index", "Home");
                    }
                    PopulateUsersRoles();
                    AddErrors(result);
                }
                var errorList = ModelState.Values.SelectMany(v => v.Errors);
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
            PopulateUsersRoles();
            // If we got this far, something failed, redisplay form
            return View(model);
        }

        private void PopulateUsersRoles(object user = null)
        {
            var userInRoles = from users in db.Roles
                              orderby users.Name
                              select users.Name;
            ViewBag.Name = new SelectList(from roles in db.Roles
                                          orderby roles.Name
                                          select roles, "Name", "Name", user);
        }

        //
        // GET: /Account/ConfirmEmail
        [AllowAnonymous]
        public async Task<ActionResult> ConfirmEmail(string userId, string code)
        {

            if (userId == null || code == null)
            {
                return View("Error");
            }
            var result = await UserManager.ConfirmEmailAsync(userId, code);
            return View(result.Succeeded ? "ConfirmEmail" : "Error");
            //return View();
        }

        //
        // GET: /Account/ForgotPassword
        [AllowAnonymous]
        public ActionResult ForgotPassword()
        {
            return View();
        }

        //
        // POST: /Account/ForgotPassword
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ForgotPassword(ForgotPasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await UserManager.FindByNameAsync(model.Email);
                if (user == null || !(await UserManager.IsEmailConfirmedAsync(user.Id)))
                {
                    // Don't reveal that the user does not exist or is not confirmed
                    return View("ForgotPasswordConfirmation");
                }

                // For more information on how to enable account confirmation and password reset please visit https://go.microsoft.com/fwlink/?LinkID=320771
                // Send an email with this link
                // string code = await UserManager.GeneratePasswordResetTokenAsync(user.Id);
                // var callbackUrl = Url.Action("ResetPassword", "Account", new { userId = user.Id, code = code }, protocol: Request.Url.Scheme);		
                // await UserManager.SendEmailAsync(user.Id, "Reset Password", "Please reset your password by clicking <a href=\"" + callbackUrl + "\">here</a>");
                // return RedirectToAction("ForgotPasswordConfirmation", "Account");
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        //
        // GET: /Account/ForgotPasswordConfirmation
        [AllowAnonymous]
        public ActionResult ForgotPasswordConfirmation()
        {
            return View();
        }

        //
        // GET: /Account/ResetPassword
        [AllowAnonymous]
        public ActionResult ResetPassword(string code)
        {
            return code == null ? View("Error") : View();
        }

        //
        // POST: /Account/ResetPassword
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ResetPassword(ResetPasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            var user = await UserManager.FindByNameAsync(model.Email);
            if (user == null)
            {
                // Don't reveal that the user does not exist
                return RedirectToAction("ResetPasswordConfirmation", "Account");
            }
            var result = await UserManager.ResetPasswordAsync(user.Id, model.Code, model.Password);
            if (result.Succeeded)
            {
                return RedirectToAction("ResetPasswordConfirmation", "Account");
            }
            AddErrors(result);
            return View();
        }

        //
        // GET: /Account/ResetPasswordConfirmation
        [AllowAnonymous]
        public ActionResult ResetPasswordConfirmation()
        {
            return View();
        }

        //
        // POST: /Account/ExternalLogin
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult ExternalLogin(string provider, string returnUrl)
        {
            Session["Workaround"] = 0;
            // Request a redirect to the external login provider
            return new ChallengeResult(provider, Url.Action("ExternalLoginCallback", "Account", new { ReturnUrl = returnUrl }));
        }

        //
        // GET: /Account/SendCode
        [AllowAnonymous]
        public async Task<ActionResult> SendCode(string returnUrl, bool rememberMe)
        {
            var userId = await SignInManager.GetVerifiedUserIdAsync();
            if (userId == null)
            {
                return View("Error");
            }
            var userFactors = await UserManager.GetValidTwoFactorProvidersAsync(userId);
            var factorOptions = userFactors.Select(purpose => new SelectListItem { Text = purpose, Value = purpose }).ToList();
            return View(new SendCodeViewModel { Providers = factorOptions, ReturnUrl = returnUrl, RememberMe = rememberMe });
        }

        //
        // POST: /Account/SendCode
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> SendCode(SendCodeViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }


            // Generate the token and send it
            if (!await SignInManager.SendTwoFactorCodeAsync(model.SelectedProvider))
            {
                return View("Error");
            }

            if (model.SelectedProvider == "GoogleAuthenticator")
            {
                Session["Provider"] = model.SelectedProvider;
                Session["ReturnUrl"] = model.ReturnUrl;
                Session["RememberMe"] = model.RememberMe;
                return RedirectToAction("EnableGoogleAuthenticator", new { Provider = model.SelectedProvider, ReturnUrl = model.ReturnUrl, RememberMe = model.RememberMe });
            }
            return RedirectToAction("VerifyCode", new { Provider = model.SelectedProvider, ReturnUrl = model.ReturnUrl, RememberMe = model.RememberMe });
        }


        //
        // GET: /Account/ExternalLoginCallback
        [AllowAnonymous]
        public async Task<ActionResult> ExternalLoginCallback(string returnUrl)
        {
            var loginInfo = await AuthenticationManager.GetExternalLoginInfoAsync();
            if (loginInfo == null)
            {
                return RedirectToAction("Login");
            }
            
            if (loginInfo.Email != null)
            {
                var userid = db.Users.Where(c => c.Email.Equals(loginInfo.Email)).Select(x => x.EmailConfirmed).SingleOrDefault();
                if (!userid)
                {
                    return View("EmailNotConfirmed");
                }
            }

            PopulateUsersRoles();
            // Sign in the user with this external login provider if the user already has a login
            var result = await SignInManager.ExternalSignInAsync(loginInfo, isPersistent: false);
            switch (result)
            {
                case SignInStatus.Success:
                    return RedirectToLocal(returnUrl);
                case SignInStatus.LockedOut:
                    return View("Lockout");
                case SignInStatus.RequiresVerification:
                    return RedirectToAction("SendCode", new { ReturnUrl = returnUrl, RememberMe = false });
                case SignInStatus.Failure:
                default:
                    // If the user does not have an account, then prompt the user to create an account
                    ViewBag.ReturnUrl = returnUrl;
                    ViewBag.LoginProvider = loginInfo.Login.LoginProvider;
                    return View("ExternalLoginConfirmation", new ExternalLoginConfirmationViewModel { Email = loginInfo.Email });
            }
        }

        //
        // POST: /Account/ExternalLoginConfirmation
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ExternalLoginConfirmation([Bind(Exclude = "ProfileImage")]ExternalLoginConfirmationViewModel model, string returnUrl)
        {
            if (User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "Manage");
            }

            if (ModelState.IsValid)
            {
                // Get the information about the user from the external login provider
                var info = await AuthenticationManager.GetExternalLoginInfoAsync();
                if (info == null)
                {
                    return View("ExternalLoginFailure");
                }
                byte[] imageData = null;
                if (Request.Files.Count > 0)
                {
                    HttpPostedFileBase poImgFile = Request.Files["ProfileImage"];

                    using (var binary = new BinaryReader(poImgFile.InputStream))
                    {
                        imageData = binary.ReadBytes(poImgFile.ContentLength);
                    }
                }
                var user = new ApplicationUser
                {
                    UserName = model.Email,
                    Email = model.Email,
                    FirstName = model.FirstName,
                    LastName = model.LastName,
                    UserRoles = model.UserRoles,
                    EmailConfirmed = false,
                    Registered = DateTime.Now,
                    TwoFactorEnabled = true
                };

                //Here we pass the byte array to user context to store in db
                user.UserPhoto = imageData;
                //var user = new ApplicationUser { UserName = model.Email, Email = model.Email };
                var result = await UserManager.CreateAsync(user);
                if (result.Succeeded)
                {
                    result = await UserManager.AddLoginAsync(user.Id, info.Login);
                    if (result.Succeeded)
                    {
                        if (user.UserRoles == "Instructor")
                        {
                            db.Instructors.Add(new Instructor
                            {
                                FirstName = user.FirstName,
                                LastName = user.LastName,
                                HireDate = user.Registered,
                                UserId = user.Id
                            });
                            await db.SaveChangesAsync();
                        }
                        await SignInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);
                        await this.UserManager.AddToRoleAsync(user.Id, model.UserRoles);
                        // For more information on how to enable account confirmation and password reset please visit https://go.microsoft.com/fwlink/?LinkID=320771
                        // Send an email with this link
                        string code = await UserManager.GenerateEmailConfirmationTokenAsync(user.Id);
                        var callbackUrl = Url.Action("ConfirmEmail", "Account", new { userId = user.Id, code = code }, protocol: Request.Url.Scheme);
                        await UserManager.SendEmailAsync(user.Id, "Confirm your account", "Please confirm your account by clicking <a href=\"" + callbackUrl + "\">here</a>");
                        await db.SaveChangesAsync();
                        return RedirectToAction("EmailNotConfirmed", "Account", new { Email = "" });
                        //   return RedirectToAction("Index", "Home");
                        
                    }
                }
                PopulateUsersRoles();
                AddErrors(result);
            }

            ViewBag.ReturnUrl = returnUrl;
            return View(model);
        }

        //
        // POST: /Account/LogOff
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult LogOff()
        {
            AuthenticationManager.SignOut(DefaultAuthenticationTypes.ApplicationCookie);
            return RedirectToAction("Index", "Home");
        }

        //
        // GET: /Account/ExternalLoginFailure
        [AllowAnonymous]
        public ActionResult ExternalLoginFailure()
        {
            return View();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (_userManager != null)
                {
                    _userManager.Dispose();
                    _userManager = null;
                }

                if (_signInManager != null)
                {
                    _signInManager.Dispose();
                    _signInManager = null;
                }
            }

            base.Dispose(disposing);
        }

        #region Helpers
        // Used for XSRF protection when adding external logins
        private const string XsrfKey = "XsrfId";

        private IAuthenticationManager AuthenticationManager
        {
            get
            {
                return HttpContext.GetOwinContext().Authentication;
            }
        }

        private void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error);
            }
        }

        private ActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }

            return RedirectToAction("LoginIndex", "Home");
            //return RedirectToAction("Index", "Home");
        }

        internal class ChallengeResult : HttpUnauthorizedResult
        {
            public ChallengeResult(string provider, string redirectUri)
                : this(provider, redirectUri, null)
            {
            }

            public ChallengeResult(string provider, string redirectUri, string userId)
            {
                LoginProvider = provider;
                RedirectUri = redirectUri;
                UserId = userId;
            }

            public string LoginProvider { get; set; }
            public string RedirectUri { get; set; }
            public string UserId { get; set; }

            public override void ExecuteResult(ControllerContext context)
            {
                var properties = new AuthenticationProperties { RedirectUri = RedirectUri };
                if (UserId != null)
                {
                    properties.Dictionary[XsrfKey] = UserId;
                }
                context.HttpContext.GetOwinContext().Authentication.Challenge(properties, LoginProvider);
            }
        }
        #endregion

        //return users from the database
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> Index()
        {
            var users = new List<UserViewModel>();
            await users.GetUsers();

            return View(users);
        }

        //
        // GET: /Account/Create
        [Authorize(Roles = "Admin")]
        public ActionResult Create()
        {
            PopulateUsersRoles();
            return View();
        }

        //
        // POST: /Account/Create
        [HttpPost]
        [Authorize(Roles = "Admin")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Exclude = "ProfileImage")]UserViewModel model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    // To convert the user uploaded Photo as Byte Array before save to DB
                    byte[] imageData = null;
                    if (Request.Files.Count > 0)
                    {
                        HttpPostedFileBase poImgFile = Request.Files["ProfileImage"];

                        using (var binary = new BinaryReader(poImgFile.InputStream))
                        {
                            imageData = binary.ReadBytes(poImgFile.ContentLength);
                        }
                    }
                    var user = new ApplicationUser
                    {
                        UserName = model.Email,
                        Email = model.Email,
                        FirstName = model.FirstName,
                        LastName = model.LastName,
                        UserRoles = model.UserRoles,
                        EmailConfirmed = false,
                        Registered = DateTime.Now,
                        TwoFactorEnabled = true
                    };
                    user.UserPhoto = imageData;
                    //object Discriminator = null;
                    PopulateUsersRoles();
                    var result = await UserManager.CreateAsync(user, model.Password);
                    if (result.Succeeded)
                    {
                        if (user.UserRoles == "Instructor")
                        {
                            db.Instructors.Add(new Instructor
                            {
                                FirstName = user.FirstName,
                                LastName = user.LastName,
                                HireDate = user.Registered,
                                UserId = user.Id
                            });
                            await db.SaveChangesAsync();
                        }
                        await this.UserManager.AddToRoleAsync(user.Id, model.UserRoles);
                        //used to sign in the user automatically after registration
                        //await SignInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);

                        // For more information on how to enable account confirmation and password reset please visit https://go.microsoft.com/fwlink/?LinkID=320771
                        // Send an email with this link
                        string code = await UserManager.GenerateEmailConfirmationTokenAsync(user.Id);
                        var callbackUrl = Url.Action("ConfirmEmail", "Account", new { userId = user.Id, codes = code }, protocol: Request.Url.Scheme);
                        await UserManager.SendEmailAsync(user.Id, "Confirm your account", "Please confirm your account by clicking <a href=\"" + callbackUrl + "\">here</a>");
                        await db.SaveChangesAsync();
                        // return RedirectToAction("Index", "Account");
                    }
                    AddErrors(result);
                }
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
            PopulateUsersRoles();
            // If we got this far, something failed, redisplay form
            return View(model);
        }
        string id;
        //GET: /Account/Edit
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> Edit(string userId)
        {
            if (userId == null || userId.Equals(String.Empty))
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            ApplicationUser user = await UserManager.FindByIdAsync(userId);
            id = userId;
            if (userId == null)
            {
                return HttpNotFound();
            }
            PopulateUsersRoles(user.UserRoles);
            var model = new UserViewModel
            {
                Email = user.Email,
                FirstName = user.FirstName,
                LastName = user.LastName,
                UserRoles = user.UserRoles,
                Password = user.PasswordHash,
                ProfileImage = user.UserPhoto
            };
            return View(model);
        }


        //POST: /Account/Edit
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> Edit([Bind(Exclude = "ProfileImage")]UserViewModel modelUser, string userId)
        {
            try
            {
                if (modelUser == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                var EditUserId = db.Users.Where(n => n.Email == modelUser.Email).Select(m => m.Id).SingleOrDefault();
                if (ModelState.IsValid)
                {
                    // To convert the user uploaded Photo as Byte Array before save to DB
                    byte[] imageData = null;
                    if (Request.Files.Count > 0)
                    {
                        HttpPostedFileBase poImgFile = Request.Files["ProfileImage"];
                        using (var binary = new BinaryReader(poImgFile.InputStream))
                        {
                            imageData = binary.ReadBytes(poImgFile.ContentLength);
                        }
                    }
                    ApplicationUser user = await UserManager.FindByIdAsync(EditUserId);
                    byte[] a = { };
                    if (imageData.Length != a.Length)
                    {
                        user.UserPhoto = imageData;
                    }
                    if (user != null)
                    {
                        user.Email = modelUser.Email;
                        user.FirstName = modelUser.FirstName;
                        user.UserName = modelUser.Email;
                        user.LastName = modelUser.LastName;
                        user.UserRoles = modelUser.UserRoles;
                        if (!user.PasswordHash.Equals(modelUser.Password))
                            user.PasswordHash = UserManager
                                .PasswordHasher
                                .HashPassword(modelUser.Password);
                        var oldUser = await UserManager.FindByIdAsync(user.Id);
                        var oldRoleId = oldUser.Roles.SingleOrDefault().RoleId;
                        var oldRoleName = db.Roles.SingleOrDefault(r => r.Id == oldRoleId).Name;
                        if (oldRoleName != user.UserRoles)
                        {
                            await UserManager.RemoveFromRoleAsync(user.Id, oldRoleName);
                            await this.UserManager.AddToRoleAsync(user.Id, modelUser.UserRoles);
                        }
                        PopulateUsersRoles();
                        var getInstructor = db.Instructors.SingleOrDefault(i => i.UserId == user.Id);
                        if (getInstructor != null)
                        {
                            getInstructor.FirstName = user.FirstName;
                            getInstructor.LastName = user.LastName;
                            getInstructor.UserId = user.Id;
                            getInstructor.HireDate = user.Registered;

                            db.Entry(getInstructor).State = EntityState.Modified;
                            db.SaveChanges();
                        }
                        var result = await UserManager.UpdateAsync(user); //to check if the update was successful or not
                        if (result.Succeeded)
                        {
                            return RedirectToAction("Index", "Account");
                        }
                        AddErrors(result);
                    }
                }
                var errorList = ModelState.Values.SelectMany(v => v.Errors);
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
            PopulateUsersRoles();
            return View(modelUser);
        }

        //GET: /Account/Delete
        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> Delete(string userId)
        {
            if (userId == null || userId.Equals(String.Empty))
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ApplicationUser user = await UserManager.FindByIdAsync(userId);
            if (user == null /*|| !user.Equals(string.Empty)*/)
            {
                return HttpNotFound();
            }

            var model = new UserViewModel
            {
                Email = user.Email,
                FirstName = user.FirstName,
                LastName = user.LastName,
                UserRoles = user.UserRoles,
                Id = user.Id,
                Password = "Fake Password"
            };
            PopulateUsersRoles();
            return View(model);

        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Delete(UserViewModel model, string userId)
        {
            try
            {
                var EditUserId = db.Users.Where(n => n.Email == model.Email).Select(m => m.Id).SingleOrDefault();
                if (model == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                if (ModelState.IsValid)
                {
                    ApplicationUser user = await UserManager.FindByIdAsync(EditUserId);
                    if (user != null)
                    {
                        var Instructors = db.Instructors.Where(c => c.UserId == user.Id).Select(c => c.id).SingleOrDefault();
                        if (Instructors > 1)
                        {
                            //Instructor instructor = await db.Instructors.FindAsync(Instructors);
                            Instructor instructor = db.Instructors.Include(i => i.OfficeAssignment).FirstOrDefault(d => d.id == Instructors);
                            db.Instructors.Remove(instructor);
                            await db.SaveChangesAsync();
                        }
                        var result = await UserManager.DeleteAsync(user);
                        if (result.Succeeded)
                        {
                            var db = new ApplicationDbContext();
                            //var deleteUser = UserManager.Users.SingleOrDefault(u => u.Id.Equals(userId));
                            //await UserManager.DeleteAsync(deleteUser);
                            await db.SaveChangesAsync();
                            return RedirectToAction("Index");
                        }
                        AddErrors(result);
                    }
                }
                var errorList = ModelState.Values.SelectMany(v => v.Errors);
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
            return View(model);
        }
    }
}