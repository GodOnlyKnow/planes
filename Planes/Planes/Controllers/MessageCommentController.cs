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
    public class MessageCommentController : Controller
    {
        private planeEntities db = new planeEntities();
        private const int pageSize = 10;

        // GET: MessageComment
        public ActionResult Index(int id, int page = 1)
        {
            var cmts = db.MessageComments.Where(x => x.message_id == id).OrderByDescending(x => x.created_at).ToPagedList(page, pageSize);
            return View(cmts);
        }

        public ActionResult ChangeStatus(int id)
        {
            var tmp = db.MessageComments.Where(x => x.id == id).First();
            var goodId = tmp.message_id;
            tmp.is_lock = tmp.is_lock == 0 ? (short)1 : (short)0;
            db.SaveChanges();
            return RedirectToAction("Index", new { id = goodId });
        }

        public ActionResult ChangeReplyStatus(int id)
        {
            var tmp = db.MessageCommentReplys.Where(x => x.id == id).First();
            var goodId = tmp.MessageComments.message_id;
            tmp.is_lock = tmp.is_lock == 0 ? (short)1 : (short)0;
            db.SaveChanges();
            return RedirectToAction("Index", new { id = goodId });
        }

        public ActionResult Delete(int id)
        {
            var tmp = db.MessageComments.Where(x => x.id == id).First();
            var goodId = tmp.message_id;
            db.MessageComments.Remove(tmp);
            db.SaveChanges();
            return RedirectToAction("Index", new { id = goodId });
        }

        public ActionResult DeleteReply(int id)
        {
            var tmp = db.MessageCommentReplys.Where(x => x.id == id).First();
            var goodId = tmp.MessageComments.message_id;
            db.MessageCommentReplys.Remove(tmp);
            db.SaveChanges();
            return RedirectToAction("Index", new { id = goodId });
        }

    }
}