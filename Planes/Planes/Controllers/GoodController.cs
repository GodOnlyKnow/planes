using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PagedList;
using Planes.Models;
using System.Text;
using System.IO;
using Planes.Tools;

namespace Planes.Controllers
{
    [MLogin]
    public class GoodController : Controller
    {
        private planeEntities db = new planeEntities();
        private const int pageSize = 10;

        // GET: Good
        public ActionResult Index()
        {

            ViewBag.Data1 = db.Goods
                                .Where(x => x.seller_id == LUser.Id && x.type_id == 11);
            ViewBag.Data2 = db.Goods
                                .Where(x => x.seller_id == LUser.Id && x.type_id == 12);
            ViewBag.Data3 = db.Goods
                                .Where(x => x.seller_id == LUser.Id && x.type_id == 13);
            ViewBag.Data4 = db.Goods
                                .Where(x => x.seller_id == LUser.Id && x.type_id == 14);
            ViewBag.Data5 = db.Goods
                                .Where(x => x.seller_id == LUser.Id && x.type_id == 15);
            return View();
        }

        public ActionResult Create()
        {
            CreateDropList();
            return View();
        }

        [HttpPost]
        public ActionResult Create(CreateGoodModel model)
        {
            CreateDropList();
            if (!ModelState.IsValid) return View(model);
            var good = db.Goods.Add(new Goods() { 
                type_id = model.TypeId,
                seller_id = LUser.Id,
                name = model.Name,
                price = model.Price,
                img = FileTool.Save(model.Img,"Images/Goods"),
                visited = 0,
                saled = 0,
                collected = 0,
                comments = 0,
                desci = model.Desc,
                unit = model.Unit ?? "元"
            });
            foreach (var f in model.Imgs)
            {
                db.GoodImages.Add(new GoodImages() 
                { 
                    good_id = good.good_id,
                    created_at = DateTime.Now,
                    img = FileTool.Save(f,"Images/Goods/")
                });
            }
            db.SaveChanges();
            ModelState.AddModelError("","新增成功");
            return View(model);
        }

        private void CreateDropList()
        {
            var d = new List<SelectListItem>();
            foreach (var s in db.GoodTypes.Where(x => x.parent == 10))
            {
                d.Add(new SelectListItem() { 
                    Text = s.name,
                    Value = s.type_id.ToString()
                });
            }
            ViewData["Types"] = d;
        }

        public ActionResult IndexOfPlane(int airportId,int page = 1)
        {
            try
            {
                var airport = db.Airports.Where(t => t.airport_id == airportId).First();
                ViewBag.airportId = airportId;
                ViewBag.GroupId = airport.Sellers.SellerGroup.sellerGroup_id;
                ViewBag.Data = db.Goods
                                    .Where(t => t.area_id == airport.area_id && t.seller_id == airport.seller_id && t.type_id != 6 && t.type_id != 7)
                                    .OrderByDescending(x => x.good_id)
                                    .ToPagedList(page, pageSize);
                return View();
            }
            catch (Exception e)
            {
                return HttpNotFound(e.Message);
            }
        }

        private void DropDownListData(int id = 0)
        {
            var d = new List<SelectListItem>();
            foreach (var s in db.GoodTypes.Where(x => x.content == "Plane"))
            {
                d.Add(new SelectListItem() { 
                    Text = s.name,
                    Value = s.type_id.ToString(),
                    Selected = s.type_id == id ? true : false
                });
            }
            ViewData["GoodType"] = d;
        }

        public ActionResult CreateOfPlane(int airportId)
        {
            ViewBag.AirportId = airportId;
            var air = db.Airports.Where(x => x.airport_id == airportId).First();
            ViewBag.GroupId = air.Sellers.SellerGroup.sellerGroup_id;
            return View();
        }

        [HttpPost]
        public ActionResult CreateOfPlane(CreateOfPlaneModel model)
        {
                if (ModelState.IsValid)
                {
                    string fileName = "default.png";
                    if (model.Img != null)
                    {
                        var file = model.Img;
                        fileName = MD5Tool.Encrypt(DateTime.Now.ToString("y-M-d H-m-s")) + Path.GetExtension(file.FileName);
                        file.SaveAs(Path.Combine(HttpContext.Server.MapPath("~/Images/Planes"),fileName));
                    }
                    var airport = db.Airports.Where(x => x.airport_id == model.AirportId).First();
                    var good = db.Goods.Add(new Goods() {
                        type_id = model.GoodType,
                        area_id = airport.area_id,
                        Areas = airport.Areas,
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
                        img = "Images/Planes/" + fileName,
                        created_at = DateTime.Now,
                        GoodTypes = db.GoodTypes.Where(t => t.type_id == model.GoodType).First(),
                        is_lock = 1,
                        market_price = model.MarketPrice,
                        model = model.Modelss,
                        name = model.Name,
                        price = model.Price,
                        production_time = model.ProductionTime,
                        saled = 0,
                        seller_id = airport.seller_id,
                        Sellers = airport.Sellers,
                        visited = 0,
                        desci = model.Desci,
                        col9 = model.Col9,
                        col10 = model.Col10,
                        unit = model.Unit ?? "元",
                        col11 = model.Col11
                    });
                    foreach (var f in model.Imgs)
                    {
                        db.GoodImages.Add(new GoodImages() 
                        { 
                            good_id = good.good_id,
                            created_at = DateTime.Now,
                            img = FileTool.Save(f,"Images/Goods")
                        });
                    }
                    var seller = db.Sellers.Where(x => x.seller_id == airport.seller_id).First();
                    seller.plants++;
                    db.SaveChanges();
                    ModelState.AddModelError("","添加成功");
                    ViewBag.GroupId = airport.Sellers.SellerGroup.sellerGroup_id;
                }
                ViewBag.AirportId = model.AirportId;
                return View();
        }

        public ActionResult ChangeStatus(int id)
        {
            var sql = db.Goods.Where(t => t.good_id == id);
            if (sql.Count() > 0)
            {
                var seller = sql.First();
                seller.is_lock = seller.is_lock == 0 ? (short)1 : (short)0;
                var airport = db.Airports.Where(t => t.area_id == seller.area_id && t.seller_id == seller.seller_id).First();
                db.SaveChanges();
                return RedirectToAction("IndexOfPlane", "Good", new { airportId = airport.airport_id });
            }
            return HttpNotFound();
        }

        public ActionResult DetailOfPlane(int id)
        {
            var sql = db.Goods.Where(t => t.good_id == id);
            if (sql.Count() > 0)
            {
                var good = sql.First();
                var imgs = db.GoodImages.Where(x => x.good_id == good.good_id).Select(x => x.img);
                DropDownListData(good.type_id);
                var airport = db.Airports.Where(t => t.area_id == good.area_id && t.seller_id == good.seller_id).First();
                ViewBag.AirportId = airport.airport_id;
                ViewBag.GroupId = airport.Sellers.SellerGroup.sellerGroup_id;
                return View(new EditOfPlaneModel() { 
                  AirportId = airport.airport_id,
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
                     Col11 = good.col11,
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
            DropDownListData();
            return HttpNotFound();
        }

        [HttpPost]
        public ActionResult DetailOfPlane(EditOfPlaneModel model)
        {
            if (ModelState.IsValid)
            {
                string fileName = "default.png";
                if (model.Img != null)
                {
                    var file = model.Img;
                    fileName = MD5Tool.Encrypt(DateTime.Now.ToString("y-M-d H-m-s")) + Path.GetExtension(file.FileName);
                    file.SaveAs(Path.Combine(HttpContext.Server.MapPath("~/Images/Planes"), fileName));
                }
                var airport = db.Airports.Where(x => x.airport_id == model.AirportId).First();
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
                    good.col11 = model.Col11;
                    model.ImgUrl = good.img = "Images/Planes/" + fileName;
                    good.market_price = model.MarketPrice;
                    good.model = model.Modelss;
                    good.name = model.Name;
                    good.price = model.Price;
                    good.production_time = model.ProductionTime;
                    good.desci = model.Desci;
                    good.unit = model.Unit;
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
                db.SaveChanges();
                ModelState.AddModelError("", "保存成功");
                var imgs = db.GoodImages.Where(x => x.good_id == good.good_id).Select(x => x.img);
                model.ImgUrls = imgs.ToArray();
                ViewBag.GroupId = airport.Sellers.SellerGroup.sellerGroup_id;
            }
            DropDownListData(model.GoodType);
            ViewBag.AirportId = model.AirportId;
            return View(model);
        }

        public ActionResult Detail(int id)
        {
            var good = db.Goods.Where(x => x.good_id == id).First();
            CreateDropList();
            var imgs = db.GoodImages.Where(x => x.good_id == good.good_id).Select(x => x.img);
            return View(new EditGoodModel() { 
                Desc = good.desci,
                Name = good.name,
                ImgUrl = good.img,
                Id = good.good_id,
                Price = good.price ?? 0,
                TypeId = good.type_id,
                ImgUrls = imgs.ToArray(),
                Unit = good.unit
            });
        }

        [HttpPost]
        public ActionResult Detail(EditGoodModel model)
        {
            CreateDropList();
            if (!ModelState.IsValid) return View(model);
            var good = db.Goods.Where(x => x.good_id == model.Id).First();
            good.desci = model.Desc;
            if (model.Img != null)
                good.img = FileTool.Save(model.Img, "Images/Goods");
            if (model.Imgs.Count() > 1 || model.Imgs[0] != null)
            {
                var iss = db.GoodImages.Where(x => x.good_id == good.good_id);
                foreach (var s in iss)
                    FileTool.Delete(s.img);
                db.GoodImages.RemoveRange(iss);
                foreach (var f in model.Imgs)
                {
                    db.GoodImages.Add(new GoodImages() { 
                        good_id = good.good_id,
                        created_at = DateTime.Now,
                        img = FileTool.Save(f,"Images/Goods")
                    });
                }
            }
            good.name = model.Name;
            good.price = model.Price;
            good.unit = model.Unit;
            model.ImgUrl = good.img;
            db.SaveChanges();
            var imgs = db.GoodImages.Where(x => x.good_id == good.good_id).Select(x => x.img);
            model.ImgUrls = imgs.ToArray();
            ModelState.AddModelError("","保存成功");
            return View(model);
        }

        public ActionResult Delete(int id)
        {
            //try
            //{
                var good = db.Goods.Where(t => t.good_id == id).First();
                var imgs = db.GoodImages.Where(x => x.good_id == good.good_id);
                if (imgs.Count() > 0)
                    db.GoodImages.RemoveRange(imgs);
                db.Goods.Remove(good);
                db.SaveChanges();
                if (LUser.LoginUser.group_id == 1)
                    return RedirectToAction("Index");
                var airport = db.Airports.Where(t => t.area_id == good.area_id && t.seller_id == good.seller_id).First();
                return RedirectToAction("IndexOfPlane", new { airportId = airport.airport_id });
            //}
            //catch (Exception e)
            //{
            //    return HttpNotFound(e.Message);
            //}
        }
    }
}