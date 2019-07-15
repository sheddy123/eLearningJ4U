using eLearningLMS.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Web;
using System.Threading.Tasks;
using System.Data.Entity;
using eLearningLMS.ViewModels;

namespace eLearningLMS.Extensions
{
    public static class IdentityExtensions
    {
        public static string GetUserFirstName(this IIdentity identity)
        {
            var db = ApplicationDbContext.Create(); //fetching the name from AspnetUsers Table
            var user = db.Users.FirstOrDefault(u => u.UserName.Equals(identity.Name));

            return user != null ? user.FirstName : String.Empty;
        }


        public static async Task GetUsers(this List<UserViewModel> users) //when you have async that doesn't return anything, it should return task
        {
            var db = ApplicationDbContext.Create();
            users.AddRange(await (from u in db.Users
                                  select new UserViewModel
                                  {
                                      Id = u.Id,
                                      Email = u.Email,
                                      FirstName = u.FirstName,
                                      LastName = u.LastName,
                                      UserRoles = u.UserRoles,
                                  }).OrderBy(o => o.Email).ToListAsync());
        }
    }
   
    
}