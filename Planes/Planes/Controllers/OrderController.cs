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
    [MLogin]
    public class OrderController : Controller
    {
        private planeEntities db = new planeEntities();
        private const int pageSize = 10;

        public ActionResult Index(int page = 1)
        {
            if (LUser.LoginUser.group_id == 1)
                ViewBag.Data = db.Orders.OrderByDescending(x => x.created_at).ToPagedList(page, pageSize);
            else
                ViewBag.Data = db.Orders.Where(x => x.Goods.seller_id == LUser.Id).OrderByDescending(x => x.created_at).ToPagedList(page, pageSize);
            return View();
        }

        public ActionResult Search(string UserName,int Status,int page = 1)
        {
            if ( string.IsNullOrWhiteSpace(UserName) && Status == -1)
                return RedirectToAction("Index");
            else if (string.IsNullOrWhiteSpace(UserName))
            {
                if (LUser.LoginUser.group_id == 1)
                    ViewBag.Data = db.Orders.Where(x => x.status == Status).OrderByDescending(x => x.created_at).ToPagedList(page, pageSize);
                else
                    ViewBag.Data = db.Orders.Where(x => x.status == Status && x.Goods.seller_id == LUser.Id).OrderByDescending(x => x.created_at).ToPagedList(page, pageSize);
            }
            else if (Status == -1)
            {
                if (LUser.LoginUser.group_id == 1)
                    ViewBag.Data = db.Orders.Where(x => x.Users.username.Contains(UserName)).OrderByDescending(x => x.created_at).ToPagedList(page, pageSize);
                else
                    ViewBag.Data = db.Orders.Where(x => x.Users.username.Contains(UserName) && x.Goods.seller_id == LUser.Id).OrderByDescending(x => x.created_at).ToPagedList(page, pageSize);
            }
            else
            {
                if (LUser.LoginUser.group_id == 1)
                    ViewBag.Data = db.Orders.Where(x => x.status == Status && x.Users.username.Contains(UserName)).OrderByDescending(x => x.created_at).ToPagedList(page, pageSize);
                else
                    ViewBag.Data = db.Orders.Where(x => x.status == Status && x.Users.username.Contains(UserName) && x.Goods.seller_id == LUser.Id).OrderByDescending(x => x.created_at).ToPagedList(page, pageSize);
            }
            ViewData["UserName"] = UserName;
            ViewData["Status"] = Status;
            return View();
        }

        public ActionResult Details(int id)
        {
            if (LUser.LoginUser.group_id == 1)
                return View(db.Orders.Where(x => x.order_id == id).First());
            else
                return View(db.Orders.Where(x => x.order_id == id && x.Goods.seller_id == LUser.Id).First());
        }

        public ActionResult Delete(int id)
        {
            if (LUser.LoginUser.group_id == 1)
                db.Orders.Remove(db.Orders.Where(x => x.order_id == id).First());
            else
                db.Orders.Remove(db.Orders.Where(x => x.order_id == id && x.Goods.seller_id == LUser.Id).First());
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        public JsonResult ChangeStatus(int id)
        {
            try
            {
                Orders order = null;
                if (LUser.LoginUser.group_id == 1)
                    order = db.Orders.Where(x => x.order_id == id).First();
                else
                    order = db.Orders.Where(x => x.order_id == id && x.Goods.seller_id == LUser.Id).First();
                order.status = order.status == 0 ? (short)1 : (short)0;
                db.SaveChanges();
                return Json(new { Status = order.status, Info = "更改成功", Code = 200 });
            }
            catch (Exception e)
            {
                return Json(new { Info = "更改失败，请将以下信息发给技术人员：" + e.Message,Code = -1});
            }

        }
    }
}