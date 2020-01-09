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
            return View();
        }

        public ActionResult Submit()
        {
            var userDal = new UserDal();
            var username = Request.Form["Username"].ToString();
            var password = Request.Form["Password"].ToString();
            var objUsers = (from x
                                  in userDal.Users
                                  where x.ID.Equals(username)
                                  select x).ToList<User>();
            
            if (!objUsers.Any()) return View("Login");
            var user = objUsers[0];
            Session["UserID"] = user.ID;
            Session["Name"] = user.Name;
            Session["Permission"] = user.PermissionType;
            if (!user.Password.Equals(password)) return View("Login");
            return RedirectToAction("Menu", Session["Permission"].ToString());
        }
    }
}