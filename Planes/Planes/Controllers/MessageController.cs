using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Planes.Models;
using PagedList;
using Planes.Tools;
using System.IO;

namespace Planes.Controllers
{
    [MLogin]
    public class MessageController : Controller
    {
        private planeEntities db = new planeEntities();
        private const int pageSize = 10;

        public ActionResult Index(int page = 1)
        {
            ViewData["Id"] = LUser.Id;
            ViewBag.Data = db.SellerMessage.Where(x => x.seller_id == LUser.Id).OrderByDescending(x => x.created_at)
                                .ToPagedList(page,pageSize);
            return View();
        }

        public ActionResult Create(int id)
        {
            return View(new MessageCreateModel() { 
                SellerId = LUser.Id
            });
        }

        [HttpPost]
        public ActionResult Create(MessageCreateModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            string fileName = "";
            var good = db.Goods.Where(x => x.good_id == model.GoodId).First();
            if (model.Img != null)
            {
                fileName = FileTool.Save(model.Img,"Images/SellerMessage");
            }
            else
            {
                fileName = good.img;
            }

            db.SellerMessage.Add(new SellerMessage() { 
                good_id = model.GoodId,
                seller_id = model.SellerId,
                content = model.Content,
                img = fileName,
                created_at = DateTime.Now
            });
            db.SaveChanges();
            ModelState.AddModelError("","添加成功");
            return View(model);
        }

        public ActionResult Details(int id)
        {
            var mes = db.SellerMessage.Where(x => x.id == id).First();
            if (mes.seller_id != LUser.Id) return HttpNotFound();
            return View(new MessageCreateModel() { 
                SellerId = mes.seller_id,
                GoodId = mes.good_id,
                Content = mes.content,
                ImgUrl = mes.img,
                Id = mes.id,
                GoodName = mes.Goods.name
            });
        }

        [HttpPost]
        public ActionResult Details(MessageCreateModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            string fileName = "";
            var mes = db.SellerMessage.Where(x => x.id == model.Id).First();
            if (model.Img != null)
            {
                fileName = FileTool.Save(model.Img,"Images/SellerMessage");
            }
            else
            {
                fileName = mes.img;
            }
            mes.content = model.Content;
            mes.img = fileName;
            db.SaveChanges();
            ModelState.AddModelError("","保存成功");
            return View(model);
        }

        public ActionResult Delete(int id)
        {
            var mes = db.SellerMessage.Where(x => x.id == id).First();
            if (mes.seller_id != LUser.Id) return HttpNotFound();
            db.SellerMessage.Remove(mes);
            db.SaveChanges();
            return RedirectToAction("Index", new { id = mes.seller_id });
        }

        public JsonResult GetPlanes(int sellerId)
        {
            var d = new List<MessagePlanesModel>();
            foreach( var s in db.Goods.Where(x => x.seller_id == sellerId) )
            {
                d.Add(new MessagePlanesModel() { 
                    Id = s.good_id,
                    Name = s.name,
                    Address = s.Areas.name,
                    Img = s.img
                });
            }
            return Json(d);
        }
    }
}