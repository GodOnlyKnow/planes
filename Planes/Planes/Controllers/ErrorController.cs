using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Planes.Controllers
{
    public class ErrorController : Controller
    {
        // GET: Error
        public ActionResult Index()
        {
            
            return View("Error");
        }

        public ActionResult Error404(HandleErrorInfo model)
        {
            return View("Error",model);
        }
    }
}