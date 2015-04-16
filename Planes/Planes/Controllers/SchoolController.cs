using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Planes.Models;
using PagedList;

namespace Planes.Controllers
{
    public class SchoolController : Controller
    {
        private planeEntities db = new planeEntities();
        private const int pageSize = 10;

        // GET: School
        public ActionResult Index(int page = 1)
        {
            ViewBag.Data = db.Sellers.Where(x => x.group_id == 2 || x.SellerGroup.parent == 2)
                            .OrderBy(x => x.name).ToPagedList(page, pageSize);
            return View();
        }
    }
}