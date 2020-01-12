using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SCE_Website.Models;
using SCE_Website.Dal;
using SCE_Website.ViewModel;
using System.Web.Routing;

namespace SCE_Website.Controllers
{
    public class LoginController : Controller
    {
        public ActionResult Login()
        {
            Session.Abandon();
            return View("Login", new User()) ;
        }

        public ActionResult Submit()
        {
            var userDal = new UserDal();
            var userId = Request.Form["Username"].ToString();
            var password = Request.Form["Password"].ToString();
            var objUsers = (from x
                                  in userDal.Users
                                  where x.ID.Equals(userId)
                                  select x).ToList<User>();

            if (!objUsers.Any()) 
            {
                TempData["PromptMessage"] = "No users found with ID " + userId;
                return RedirectToAction("Login");
            };
            var user = objUsers[0];
            Session["UserID"] = user.ID;
            Session["Name"] = user.Name;
            Session["Permission"] = user.PermissionType;
            if (!user.Password.Equals(password))
            {
                TempData["PromptMessage"] = "Password not matched for ID " + userId;
                return RedirectToAction("Login");
            }
            return RedirectToAction("Menu", Session["Permission"].ToString());
        }

        public ActionResult Stam()
        {
            return View();
        }
    }
}