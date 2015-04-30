using Planes.Tools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Planes.Models;
using PagedList;

namespace Planes.Controllers
{
    [NLogin]
    public class GoodCommentController : Controller
    {
        private planeEntities db = new planeEntities();
        private const int pageSize = 10;

        // GET: GoodComment
        public ActionResult Index(int id,int page = 1)
        {
            var cmts = db.GoodComments.Where(x => x.good_id == id).OrderByDescending(x => x.created_at).ToPagedList(page, pageSize);
            return View(cmts);
        }

        public ActionResult ChangeStatus(int id)
        {
            var tmp = db.GoodComments.Where(x => x.id == id).First();
            var goodId = tmp.good_id;
            tmp.is_lock = tmp.is_lock == 0 ? (short)1 : (short)0;
            db.SaveChanges();
            return RedirectToAction("Index", new { id = goodId });
        }

        public ActionResult ChangeReplyStatus(int id)
        {
            var tmp = db.GoodCommentReplys.Where(x => x.id == id).First();
            var goodId = tmp.GoodComments.good_id;
            tmp.is_lock = tmp.is_lock == 0 ? (short)1 : (short)0;
            db.SaveChanges();
            return RedirectToAction("Index", new { id = goodId });
        }

        public ActionResult Delete(int id)
        {
            var tmp = db.GoodComments.Where(x => x.id == id).First();
            var goodId = tmp.good_id;
            db.GoodCommentImages.RemoveRange(tmp.GoodCommentImages);
            db.GoodCommentReplys.RemoveRange(tmp.GoodCommentReplys);
            db.GoodComments.Remove(tmp);
            db.SaveChanges();
            return RedirectToAction("Index", new { id = goodId });
        }

        public ActionResult DeleteReply(int id)
        {
            var tmp = db.GoodCommentReplys.Where(x => x.id == id).First();
            var goodId = tmp.GoodComments.good_id;
            db.GoodCommentReplys.Remove(tmp);
            db.SaveChanges();
            return RedirectToAction("Index", new { id = goodId });
        }

    }
}