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
    public class SellerMessageController : Controller
    {
        private planeEntities db = new planeEntities();
        private const int pageSize = 10;

        // GET: SellerMessage
        public ActionResult Index(int id,int page = 1)
        {
            ViewBag.Data = db.SellerMessage.Where(x => x.seller_id == id).OrderByDescending(x => x.created_at).ToPagedList(page, pageSize);
            return View();
        }

        public ActionResult Delete(int id)
        {
            var tmp = db.SellerMessage.Where(x => x.id == id).First();
            db.SellerMessage.Remove(tmp);
            db.SaveChanges();
            return RedirectToAction("Index", new { id = tmp.seller_id });
        }
        
    }
}