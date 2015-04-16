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
    public class PlaneController : Controller
    {
        private planeEntities db = new planeEntities();
        private const int pageSize = 10;

        public ActionResult IndexA(int airportId, int page = 1)
        {
            var air = db.Airports.Where(x => x.airport_id == airportId).First();
            ViewBag.Data = db.Goods
                                .Where(x => x.type_id == 8 && x.seller_id ==air.seller_id && x.area_id == air.area_id)
                                .OrderBy(x => x.created_at)
                                .ToPagedList(page, pageSize);
            ViewBag.airportId = airportId;
            return View();
        }
        public ActionResult IndexB(int airportId, int page = 1)
        {
            var air = db.Airports.Where(x => x.airport_id == airportId).First();
            ViewBag.Data = db.Goods
                                .Where(x => x.type_id == 6 && x.seller_id == air.seller_id && x.area_id == air.area_id)
                                .OrderBy(x => x.created_at)
                                .ToPagedList(page, pageSize);
            ViewBag.airportId = airportId;
            return View();
        }
        public ActionResult IndexC(int airportId, int page = 1)
        {
            var air = db.Airports.Where(x => x.airport_id == airportId).First();
            ViewBag.Data = db.Goods
                                .Where(x => x.type_id == 7 && x.seller_id == air.seller_id && x.area_id == air.area_id)
                                .OrderBy(x => x.created_at)
                                .ToPagedList(page, pageSize);
            ViewBag.airportId = airportId;
            return View();
        }

        public ActionResult CreateA(int airportId)
        {
            ViewBag.airportId = airportId;
            return View();
        }
        public ActionResult CreateB(int airportId)
        {
            ViewBag.airportId = airportId;
            return View();
        }
        public ActionResult CreateC(int airportId)
        {
            ViewBag.airportId = airportId;
            return View();
        }

        [HttpPost]
        public ActionResult CreateB(PlaneCreateBModel model)
        {
            if (!ModelState.IsValid) return View(model);
            var air = db.Airports.Where(x => x.airport_id == model.AirportId).First();
            var tmp = db.Goods.Add(new Goods()
            {
                col1 = model.Col1,
                col2 = model.Col2,
                model = model.Modelss,
                col3 = model.Col3,
                price = model.Price,
                area_id = air.area_id,
                seller_id = air.seller_id,
                type_id = 6,
                desci = model.Desc,
                collected = 0,
                comments = 0,
                created_at = DateTime.Now,
                is_lock = (short)1,
                name = model.Name,
                saled = 0,
                visited = 0,
                col4 = model.Col4,
                unit = model.Unit ?? "元"
            });
            foreach (var f in model.Imgs)
            {
                db.GoodImages.Add(new GoodImages()
                {
                    created_at = DateTime.Now,
                    good_id = tmp.good_id,
                    img = FileTool.Save(f,"Images/Goods")
                });
            }
            db.SaveChanges();
            ModelState.AddModelError("","添加成功");
            return View(model);
        }

        [HttpPost]
        public ActionResult CreateC(PlaneCreateCModel model)
        {
            if (!ModelState.IsValid) return View(model);
            var air = db.Airports.Where(x => x.airport_id == model.AirportId).First();
            var tmp = db.Goods.Add(new Goods()
            {
                col1 = model.Col1,
                col2 = model.Col2,
                model = model.Modelss,
                col3 = model.Col3,
                price = model.Price,
                area_id = air.area_id,
                seller_id = air.seller_id,
                type_id = 7,
                desci = model.Desc,
                collected = 0,
                comments = 0,
                created_at = DateTime.Now,
                is_lock = (short)1,
                name = model.Name,
                saled = 0,
                visited = 0,
                col4 = model.Col4,
                col5 = model.Col5,
                unit = model.Unit ?? "元",
                col6 = model.Col6
            });
            foreach (var f in model.Imgs)
            {
                db.GoodImages.Add(new GoodImages()
                {
                    created_at = DateTime.Now,
                    good_id = tmp.good_id,
                    img = FileTool.Save(f, "Images/Goods")
                });
            }
            db.SaveChanges();
            ModelState.AddModelError("", "添加成功");
            return View(model);
        }


        public ActionResult DetailB(int id)
        {
            var good = db.Goods.Where(x => x.good_id == id).First();
            var imgs = db.GoodImages.Where(x => x.good_id == good.good_id).Select(x => x.img);
            return View(new PlaneEditBModel() 
            { 
                Id = good.good_id,
                Col1 = good.col1,
                Col2 = good.col2,
                Modelss = good.model,
                Col3 = good.col3,
                Price = good.price ?? 0,
                Desc = good.desci,
                Name = good.name,
                ImgUrls = imgs.ToArray(),
                Col4 = good.col4,
                Unit = good.unit
            });
        }

        [HttpPost]
        public object DetailB(PlaneEditBModel model)
        {
            if (!ModelState.IsValid) return View(model);
            var tmp = db.Goods.Where(x => x.good_id == model.Id).First();
            tmp.col1 = model.Col1;
            tmp.col2 = model.Col2;
            tmp.col3 = model.Col3;
            tmp.model = model.Modelss;
            tmp.price = model.Price;
            tmp.desci = model.Desc;
            tmp.name = model.Name;
            tmp.col4 = model.Col4;
            tmp.unit = model.Unit;
            if (model.Imgs.Count() > 1 || model.Imgs[0] != null)
            {
                var imgs = db.GoodImages.Where(x => x.good_id == tmp.good_id);
                foreach (var s in imgs)
                    FileTool.Delete(s.img);
                db.GoodImages.RemoveRange(imgs);
                foreach (var f in model.Imgs)
                {
                    var ps = FileTool.Save(f,"Images/Goods");
                    db.GoodImages.Add(new GoodImages() { 
                        created_at = DateTime.Now,
                        good_id = tmp.good_id,
                        img = ps
                    });
                }
            }
            db.SaveChanges();
            var imgsf = db.GoodImages.Where(x => x.good_id == tmp.good_id).Select(x => x.img);
            model.ImgUrls = imgsf.ToArray();
            ModelState.AddModelError("","保存成功");
            return View(model);
        }

        public ActionResult DetailC(int id)
        {
            var good = db.Goods.Where(x => x.good_id == id).First();
            var imgs = db.GoodImages.Where(x => x.good_id == good.good_id).Select(x => x.img);
            return View(new PlaneEditCModel()
            {
                Id = good.good_id,
                Col1 = good.col1,
                Col2 = good.col2,
                Modelss = good.model,
                Col3 = good.col3,
                Price = good.price ?? 0,
                Desc = good.desci,
                Name = good.name,
                ImgUrls = imgs.ToArray(),
                Col4 = good.col4,
               Col5 = good.col5,
               Unit = good.unit,
               Col6 = good.col6
            });
        }

        [HttpPost]
        public object DetailC(PlaneEditCModel model)
        {
            if (!ModelState.IsValid) return View(model);
            var tmp = db.Goods.Where(x => x.good_id == model.Id).First();
            tmp.col1 = model.Col1;
            tmp.col2 = model.Col2;
            tmp.col3 = model.Col3;
            tmp.model = model.Modelss;
            tmp.price = model.Price;
            tmp.desci = model.Desc;
            tmp.name = model.Name;
            tmp.col4 = model.Col4;
            tmp.col5 = model.Col5;
            tmp.unit = model.Unit;
            tmp.col6 = model.Col6;
            if (model.Imgs.Count() > 1 || model.Imgs[0] != null)
            {
                var imgs = db.GoodImages.Where(x => x.good_id == tmp.good_id);
                foreach (var s in imgs)
                    FileTool.Delete(s.img);
                db.GoodImages.RemoveRange(imgs);
                foreach (var f in model.Imgs)
                {
                    var ps = FileTool.Save(f, "Images/Goods");
                    db.GoodImages.Add(new GoodImages()
                    {
                        created_at = DateTime.Now,
                        good_id = tmp.good_id,
                        img = ps
                    });
                }
            }
            db.SaveChanges();
            var imgsf = db.GoodImages.Where(x => x.good_id == tmp.good_id).Select(x => x.img);
            model.ImgUrls = imgsf.ToArray();
            ModelState.AddModelError("", "保存成功");
            return View(model);
        }

        public ActionResult DeleteA(int id)
        {
            var gd = db.Goods.Where(x => x.good_id == id).First();
            db.Goods.Remove(gd);
            db.SaveChanges();
            return RedirectToAction("IndexA");
        }
        public ActionResult DeleteB(int id)
        {
            var gd = db.Goods.Where(x => x.good_id == id).First();
            var imgs = db.GoodImages.Where(x => x.good_id == gd.good_id);
            var air = db.Airports.Where(x => x.seller_id == gd.seller_id && x.area_id == gd.area_id).First();
            foreach (var s in imgs)
                FileTool.Delete(s.img);
            db.GoodImages.RemoveRange(imgs);
            db.Goods.Remove(gd);
            db.SaveChanges();
            return RedirectToAction("IndexB", new { airportId = air.airport_id });
        }
        public ActionResult DeleteC(int id)
        {
            var gd = db.Goods.Where(x => x.good_id == id).First();
            var imgs = db.GoodImages.Where(x => x.good_id == gd.good_id);
            var air = db.Airports.Where(x => x.seller_id == gd.seller_id && x.area_id == gd.area_id).First();
            foreach (var s in imgs)
                FileTool.Delete(s.img);
            db.GoodImages.RemoveRange(imgs);
            db.Goods.Remove(gd);
            db.SaveChanges();
            return RedirectToAction("IndexC", new { airportId = air.airport_id });
        }
    }
}