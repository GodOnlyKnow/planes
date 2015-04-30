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
    public class UserCommentController : Controller
    {
        private planeEntities db = new planeEntities();
        private const int pageSize = 10;

        // GET: UserComment
        public ActionResult Index(int type,int page = 1)
        {
            var cmts = db.UserComments.Where(x => x.type == type).OrderByDescending(x => x.created_at).ToPagedList(page, pageSize);
            return View(cmts);
        }

        public ActionResult Delete(int id)
        {
            var cmt = db.UserComments.Where(x => x.id == id).First();
            var t = cmt.type;
            db.UserCommentImages.RemoveRange(cmt.UserCommentImages);
            db.UserCommentReplys.RemoveRange(cmt.UserCommentReplys);
            db.UserComments.Remove(cmt);
            db.SaveChanges();
            return RedirectToAction("Index", new { type = t });
        }
    }
}