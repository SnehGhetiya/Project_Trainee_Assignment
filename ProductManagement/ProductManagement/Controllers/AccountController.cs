using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using ProductManagement.Models;

namespace ProductManagement.Controllers
{
    [AllowAnonymous]
    public class AccountController : Controller
    {
        //Entity variable for database connectivity
        private readonly db_PMEntities _db = new db_PMEntities();
        // Tracing the logs by log4net variable
        private static readonly log4net.ILog logger = log4net.LogManager.GetLogger(typeof(AccountController));
        // GET : Account/Register
        public ActionResult Register()
        {
            return View();
        }

        // POST : Account/Register
        [HttpPost]
        [AllowAnonymous]
        public ActionResult Register(tbl_User model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    // Adding the new user to the database and redirecting to the products page
                    _db.tbl_User.Add(model);
                    _db.SaveChanges();
                    ModelState.Clear();
                    return this.RedirectToAction("List", "Product");
                }
                else
                {
                    ModelState.AddModelError("", "Somethind went wrong.");
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex.ToString());
            }
            return View(model);
        }

        // GET : Account/Login
        public ActionResult Login()
        {
            return View();
        }

        // POST : Account/Login
        [HttpPost]
        [AllowAnonymous]
        public ActionResult Login(tbl_Login model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    // Findindg user by submitted email and password
                    var obj = _db.tbl_User.Where(x => x.Email.Equals(model.Email) && x.Password.Equals(model.Password)).FirstOrDefault();
                    if (obj != null)
                    {
                        // If user found then set cookies and redirecting to the products list page
                        FormsAuthentication.SetAuthCookie(obj.Email.ToString(), false);
                        Session["Email"] = obj.Email.ToString();
                        return this.RedirectToAction("Index", "Home");
                    }
                    else
                    {
                        // If user not found then giving error message and redirecting to registration page
                        ModelState.AddModelError("", "Incorrect username or password");
                        return this.RedirectToAction("Register", "Account");
                    }
                }
                else
                {
                    ModelState.AddModelError("", "Something went wrong");
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex.ToString());
            }
            return View(model);
        }

        // GET : Account/Logout
        public ActionResult Logout()
        {
            try
            {
                // Signing out user
                FormsAuthentication.SignOut();
            }
            catch (Exception ex)
            {
                logger.Error(ex.ToString());
            }
            return RedirectToAction("Register", "Account");
        }
    }
}