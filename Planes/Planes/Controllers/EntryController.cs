using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Planes.Models;
using Planes.Tools;
using PagedList;

namespace Planes.Controllers
{
    [NLogin]
    public class EntryController : Controller
    {
        private planeEntities db = new planeEntities();
        private const int pageSize = 10;

        // GET: Entry
        public ActionResult Index(int page = 1)
        {
            ViewBag.Data = db.Entrys.OrderByDescending(x => x.created_at).ToPagedList(page,pageSize);
            return View();
        }

        public ActionResult Delete(int id)
        {
            var tmp = db.Entrys.Where(x => x.id == id).First();
            db.Entrys.Remove(tmp);
            db.SaveChanges();
            return RedirectToAction("Index");
        }
    }
}