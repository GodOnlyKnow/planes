using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Planes.Models;
using PagedList;
using Planes.Tools;


namespace Planes.Controllers
{
    [NLogin]
    public class FeedbackController : Controller
    {
        private planeEntities db = new planeEntities();
        private const int pageSize = 10;

        public ActionResult Index(int page = 1)
        {
            ViewBag.Data = db.Feedback.OrderByDescending(x => x.id).ToPagedList(page, pageSize);
            return View();
        }

        public ActionResult Delete(int id)
        {
            var tmp = db.Feedback.Where(x => x.id == id).First();
            db.Feedback.Remove(tmp);
            db.SaveChanges();
            return RedirectToAction("Index");
        }
    }
}