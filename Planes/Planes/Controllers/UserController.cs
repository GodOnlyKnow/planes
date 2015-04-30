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
    public class UserController : Controller
    {
        private planeEntities db = new planeEntities();
        private const int pageSize = 10;

        // GET: User
        public ActionResult Index(int page = 1)
        {
            ViewBag.Data = db.Users.OrderBy(x => x.user_id).ToPagedList(page, pageSize);
            return View();
        }
        [HttpPost]
        public ActionResult Index(string Name,int page = 1)
        {
            ViewBag.Data = db.Users.Where(x => x.username.Contains(Name)).OrderBy(x => x.user_id).ToPagedList(page,pageSize);
            return View();
        }

        public ActionResult ChangeStatus(int id)
        {
            var user = db.Users.Where(x => x.user_id == id).First();
            user.is_lock = user.is_lock == 0 ? (short)1 : (short)0;
            db.SaveChanges();
            return RedirectToAction("Index");
        }
        
        public ActionResult Details(int id)
        {
            return View(db.Users.Where(x => x.user_id == id).First());
        }

        public ActionResult ChangeLevel(int id)
        {
            DropDownListData();
            var user = db.Users.Where(x => x.user_id == id).First();
            return View(new UserViewModel() { 
                Id = user.user_id,
                LevelId = user.level_id
            });
        }
        [HttpPost]
        public ActionResult ChangeLevel(UserViewModel model)
        {
            DropDownListData();
            if (ModelState.IsValid)
            {
                var user = db.Users.Where(x => x.user_id == model.Id).First();
                user.level_id = model.LevelId;
                db.SaveChanges();
                ModelState.AddModelError("","保存成功");
            }
            return View();
        }

        private void DropDownListData()
        {
            var d = new List<SelectListItem>();
            foreach (var s in db.Levels)
            {
                d.Add(new SelectListItem() { 
                    Text = s.name,
                    Value = s.level_id.ToString()
                });
            }
            ViewData["Levels"] = d;
        }

        public ActionResult Delete(int id)
        {
            var user = db.Users.Where(x => x.user_id == id).First();
            
            db.GoodCommentReplys.RemoveRange(user.GoodCommentReplys);
            db.GoodComments.RemoveRange(user.GoodComments);
            db.Orders.RemoveRange(user.Orders);
            db.UserCommentReplys.RemoveRange(user.UserCommentReplys);
            db.UserCommentReplys.RemoveRange(user.UserCommentReplys1);
            db.UserComments.RemoveRange(user.UserComments);
            db.MessageCommentReplys.RemoveRange(user.MessageCommentReplys);
            db.MessageComments.RemoveRange(user.MessageComments);
            db.Entrys.RemoveRange(user.Entrys);
            db.Feedback.RemoveRange(user.Feedback);

            db.Users.Remove(user);
            db.SaveChanges();
            return RedirectToAction("Index");
        }
    }
}