using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ProductManagement.Controllers
{
    [Authorize]
    [AllowAnonymous]
    public class HomeController : Controller
    {
        // Tracing the logs by log4net variable
        private static readonly log4net.ILog logger = log4net.LogManager.GetLogger(typeof(HomeController));
        // GET : Home/Index
        public ActionResult Index()
        {
            try
            {
                return View();
            }
            catch(Exception ex)
            {
                logger.Error(ex.ToString());
            }
            return View();
        }

        [Authorize]
        // GET : Home/Api
        public ActionResult Api()
        {
            try
            {
                return View();
            }
            catch (Exception ex)
            {
                logger.Error(ex.ToString());
            }
            return View();
        }
    }
}