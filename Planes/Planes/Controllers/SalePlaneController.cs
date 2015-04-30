using Planes.Tools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Planes.Models;
using PagedList;
using System.IO;

namespace Planes.Controllers
{
    [NLogin]
    public class SalePlaneController : Controller
    {
        private planeEntities db = new planeEntities();
        private const int pageSize = 10;

        // GET: SalePlane
        public ActionResult Index(int page = 1)
        {
            ViewBag.Data = db.Goods.Where(x => x.type_id == 9).OrderByDescending(x => x.created_at).ToPagedList(page, pageSize);
            return View();
        }

        public ActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Create(CreateOfPlaneModel model)
        {
            model.GoodType = 9;
            if (ModelState.IsValid)
            {
                var good = db.Goods.Add(new Goods()
                {
                    type_id = model.GoodType,
                    col1 = model.Col1,
                    col2 = model.Col2,
                    col3 = model.Col3,
                    col4 = model.Col4,
                    col5 = model.Col5,
                    col6 = model.Col6,
                    col7 = model.Col7,
                    col8 = model.Col8,
                    collected = 0,
                    comments = 0,
                    img = FileTool.Save(model.Img,"Images/Planes"),
                    created_at = DateTime.Now,
                    GoodTypes = db.GoodTypes.Where(t => t.type_id == model.GoodType).First(),
                    is_lock = 1,
                    market_price = model.MarketPrice,
                    model = model.Modelss,
                    name = model.Name,
                    price = model.Price,
                    production_time = model.ProductionTime,
                    saled = 0,
                    seller_id = LUser.Id,
                    visited = 0,
                    desci = model.Desci,
                    col9 = model.Col9,
                    col10 = model.Col10,
                    unit = model.Unit ?? "元"
                });
                foreach (var f in model.Imgs)
                {
                    db.GoodImages.Add(new GoodImages()
                    {
                        good_id = good.good_id,
                        created_at = DateTime.Now,
                        img = FileTool.Save(f, "Images/Goods/")
                    });
                }
                LUser.LoginUser.plants++;
                db.SaveChanges();
                ModelState.AddModelError("", "添加成功");
            }
            return View();
        }

        public ActionResult ChangeStatus(int id)
        {
            var sql = db.Goods.Where(t => t.good_id == id);
            if (sql.Count() > 0)
            {
                var seller = sql.First();
                seller.is_lock = seller.is_lock == 0 ? (short)1 : (short)0;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return HttpNotFound();
        }

        public ActionResult Detail(int id)
        {
            var sql = db.Goods.Where(t => t.good_id == id);
            if (sql.Count() > 0)
            {
                var good = sql.First();
                var imgs = db.GoodImages.Where(x => x.good_id == good.good_id).Select(x => x.img);
                return View(new EditOfPlaneModel()
                {
                    Col1 = good.col1,
                    Col2 = good.col2,
                    Col3 = good.col3,
                    Col4 = good.col4,
                    Col5 = good.col5,
                    Col6 = good.col6,
                    Col7 = good.col7,
                    Col8 = good.col8,
                    Col9 = good.col9,
                    Col10 = good.col10,
                    Desci = good.desci,
                    GoodType = good.GoodTypes.type_id,
                    ImgUrl = good.img,
                    MarketPrice = good.market_price ?? 0,
                    Modelss = good.model,
                    Name = good.name,
                    Price = good.price ?? 0,
                    ProductionTime = good.production_time,
                    Id = good.good_id,
                    Collection = good.collected ?? 0,
                    Saled = good.saled ?? 0,
                    ImgUrls = imgs.ToArray(),
                    Unit = good.unit
                });
            }
            return HttpNotFound();
        }

        [HttpPost]
        public ActionResult Detail(EditOfPlaneModel model)
        {
            model.GoodType = 9;
            if (ModelState.IsValid)
            {
                string fileName = "default.png";
                if (model.Img != null)
                {
                    var file = model.Img;
                    fileName = MD5Tool.Encrypt(DateTime.Now.ToString("y-M-d H-m-s")) + Path.GetExtension(file.FileName);
                    file.SaveAs(Path.Combine(HttpContext.Server.MapPath("~/Images/Planes"), fileName));
                }
                var good = db.Goods.Where(x => x.good_id == model.Id).First();
                good.type_id = model.GoodType;
                good.col1 = model.Col1;
                good.col2 = model.Col2;
                good.col3 = model.Col3;
                good.col4 = model.Col4;
                good.col5 = model.Col5;
                good.col6 = model.Col6;
                good.col7 = model.Col7;
                good.col8 = model.Col8;
                good.col9 = model.Col9;
                good.col10 = model.Col10;
                good.unit = model.Unit;
                model.ImgUrl = good.img = "Images/Planes/" + fileName;
                if (model.Imgs.Count() > 1 || model.Imgs[0] != null)
                {
                    db.GoodImages.RemoveRange(db.GoodImages.Where(x => x.good_id == good.good_id));
                    foreach (var f in model.Imgs)
                    {
                        db.GoodImages.Add(new GoodImages()
                        {
                            good_id = good.good_id,
                            created_at = DateTime.Now,
                            img = FileTool.Save(f, "Images/Goods")
                        });
                    }
                }
                good.market_price = model.MarketPrice;
                good.model = model.Modelss;
                good.name = model.Name;
                good.price = model.Price;
                good.production_time = model.ProductionTime;
                good.desci = model.Desci;
                db.SaveChanges();
                var imgs = db.GoodImages.Where(x => x.good_id == good.good_id).Select(x => x.img);
                model.ImgUrls = imgs.ToArray();
                ModelState.AddModelError("", "保存成功");
            }
            return View(model);
        }

        public ActionResult Delete(int id)
        {
            var good = db.Goods.Where(t => t.good_id == id).First();
            var imgs = db.GoodImages.Where(x => x.good_id == good.good_id);
            if (imgs.Count() > 0)
                db.GoodImages.RemoveRange(imgs);
            db.Goods.Remove(good);
            db.SaveChanges();
            return RedirectToAction("Index");
        }
    }
}