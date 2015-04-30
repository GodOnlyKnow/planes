using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Planes.Models;
using Planes.Tools;
using PagedList;
using System.IO;

namespace Planes.Controllers
{
    [MLogin]
    public class AirportController : Controller
    {
        private planeEntities db = new planeEntities();
        private const int pageSize = 10;
        // GET: Seller
        public object Index(int page = 1)
        {
            if (LUser.LoginUser.group_id == 1)
                ViewBag.Data = db.Airports.OrderBy(x => x.Sellers.name).ToPagedList(page, pageSize);
            else
                ViewBag.Data = db.Airports.Where(x => x.seller_id == LUser.Id).OrderBy(x => x.Sellers.name).ToPagedList(page, pageSize);
            return View();
        }
        [HttpPost]
        public ActionResult Index(AirportSearchModel model,int page = 1)
        {
            if (LUser.LoginUser.group_id == 1)
                ViewBag.Data = (from r in db.Airports
                                where r.Sellers.name.Contains(model.Name) || r.Sellers.username.Contains(model.UserName)
                            select r).OrderBy(x => x.Sellers.name).ToPagedList(page, pageSize);
            return View();
        }
        // GET: Seller/Details/5
        public ActionResult Details(int id)
        {
            var sql = db.Airports.Where(t => t.airport_id == id);
            if (sql.Count() > 0)
            {
                var airport = sql.First();
                var imgs = db.SellerImage.Where(x => x.seller_id == airport.seller_id).Select(x => x.img);
                DropDownListData(airport.area_id);
                var loc = airport.location.Split('|');
                return View(new AirportEditModel() { 
                    Address = airport.Sellers.address,
                    AreaId = airport.area_id,
                    Collection = airport.Sellers.collected ?? 0,
                    Id = airport.airport_id,
                    ImgUrl = "http://" + Request.Url.Host + ":" + Request.Url.Port + "/" + airport.Sellers.img,
                    IsLock = airport.Sellers.is_lock ?? 0,
                    Lat = loc[1],
                    Lng = loc[0],
                    Name = airport.Sellers.name,
                    Password = airport.Sellers.password,
                    Phone = airport.Sellers.phone,
                    Planes = airport.Sellers.plants ?? 0,
                    UserName = airport.Sellers.username,
                    WX = airport.Sellers.wx,
                    AirportAddress = airport.address,
                    GroupId = airport.Sellers.group_id ?? 3,
                    ImgUrls = imgs.ToArray(),
                    Desc = airport.Sellers.desci
                });
            }
            return HttpNotFound();
        }
        [HttpPost]
        public ActionResult Details(AirportEditModel model)
        {
            Airports airport = null;
            if (ModelState.IsValid)
            {
                airport = db.Airports.Where(x => x.airport_id == model.Id).First();
                var seller = db.Sellers.Where(x => x.seller_id == airport.seller_id).First();
                seller.address = model.Address;
                seller.username = model.UserName;
                seller.name = model.Name;
                if (model.Password != "Default")
                    seller.password = MD5Tool.Encrypt(model.Password);
                if (model.Imgs.Count() > 1 || model.Imgs[0] != null)
                {
                    db.SellerImage.RemoveRange(db.SellerImage.Where(x => x.seller_id == seller.seller_id));
                    foreach (var f in model.Imgs)
                    {
                        db.SellerImage.Add(new SellerImage() { 
                            created_at = DateTime.Now,
                            seller_id = seller.seller_id,
                            img = FileTool.Save(f,"Images/Sellers")
                        });
                    }
                }
                seller.is_lock = model.IsLock;
                seller.wx = model.WX;
                seller.phone = model.Phone;
                if (model.Img != null)
                    seller.img = FileTool.Save(model.Img,"Images/Sellers");
                model.ImgUrl = seller.img;
                seller.group_id = model.GroupId;
                seller.desci = model.Desc;
                airport.area_id = model.AreaId;
                airport.location = model.Lng + "|" + model.Lat;
                airport.address = model.AirportAddress;
                db.SaveChanges();
                ModelState.AddModelError("","修改成功");
            }
            if (airport == null) DropDownListData();
            else DropDownListData(airport.area_id);
            model.ImgUrls = new string[0];
            if (airport != null)
            {
                var sss = db.SellerImage.Where(x => x.seller_id == airport.seller_id).Select(x => x.img);
                model.ImgUrls = sss.ToArray();
            }
            return View(model);
        }

        // GET: Seller/Create
        public ActionResult Create()
        {
            DropDownListData();
            return View();
        }

        private void DropDownListData(int def = 1)
        {
            IsLockData();
            AreaData(def);
        }

        public JsonResult GetAreas()
        {
            var list = new List<AreaListModel>();
            foreach (var s in db.Areas)
            {
                list.Add(new AreaListModel() { 
                    Id = s.area_id,
                    Name = s.name,
                    Address = s.address
                });
            }
            return Json(list);
        }

        private void IsLockData()
        {
            var d = new List<SelectListItem>();
            d.Add(new SelectListItem() { Text = "启用", Value = "1" });
            d.Add(new SelectListItem() { Text = "禁用", Value = "0" });
            ViewData["IsLock"] = d;
        }

        private void AreaData(int def)
        {
            var d = new List<SelectListItem>();
            foreach (var a in db.Areas)
            {
                d.Add(new SelectListItem()
                {
                    Text = a.name,
                    Value = a.area_id.ToString()
                });
            }
            ViewData["Areas"] = d;
        }

        // POST: Seller/Create
        [HttpPost]
        public ActionResult Create(AirportCreateModel model)
        {
            DropDownListData();
            try
            {
                if (model.UserName != null)
                {
                    var sql = db.Sellers.Where(t => t.username == model.UserName);
                    Sellers seller = null;
                    if (sql.Count() < 1)
                    {
                        if (!ModelState.IsValid) return View();
                        
                        seller = db.Sellers.Add(new Sellers()
                            {
                                name = model.Name,
                                username = model.UserName,
                                password = MD5Tool.Encrypt(model.Password),
                                address = model.Address,
                                is_lock = model.IsLock,
                                wx = model.WX,
                                phone = model.Phone,
                                collected = 0,
                                desci = model.Desc,
                                img = FileTool.Save(model.Img,"Images/Sellers"),
                                plants = 0,
                                qq = model.WX,
                                saled = 0,
                                visited = 0,
                                created_at = DateTime.Now,
                                group_id = model.GroupId
                            });
                        foreach (var f in model.Imgs)
                        {
                            db.SellerImage.Add(new SellerImage() 
                            { 
                                seller_id = seller.seller_id,
                                created_at = DateTime.Now,
                                img = FileTool.Save(f,"Images/Sellers")
                            });
                        }
                    }
                    else seller = sql.First();
                    if (model.Lat == null || model.Lng == null) {
                        ModelState.Clear();
                        ModelState.AddModelError("","经纬度不能为空");
                        return View();
                    }
                    var area = db.Areas.Where(t => t.area_id == model.AreaId).First();
                    db.Airports.Add(new Airports() { 
                         area_id = area.area_id,
                         Areas = area,
                         Sellers = seller,
                         seller_id = seller.seller_id,
                         created_at = DateTime.Now,
                         location = model.Lng + "|" + model.Lat,
                         address = model.AirportAddress
                    });
                    db.SaveChanges();
                    ModelState.Clear();
                    ModelState.AddModelError("", "保存成功");
                }
                else
                {
                    ModelState.Clear();
                    ModelState.AddModelError("","请填写公司账户");
                }
                return View();
            }
            catch (Exception e)
            {
                ModelState.AddModelError("", e.Message);
                return View();
            }
        }

        // GET: Seller/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: Seller/Edit/5
        [HttpPost]
        public ActionResult Edit(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add update logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        public ActionResult ChangeStatus(int id)
        {
            var sql = db.Sellers.Where(t => t.seller_id == id);
            if (sql.Count() > 0)
            {
                var seller = sql.First();
                seller.is_lock = seller.is_lock == 0 ? (short)1 : (short)0;
                db.SaveChanges();
            }
            return RedirectToAction("Index");
        }

        // GET: Seller/Delete/5
        public ActionResult Delete(int id)
        {
                var airport = db.Airports.Where(t => t.airport_id == id).First();
                var goods = db.Goods.Where(t => t.area_id == airport.area_id && t.seller_id == airport.seller_id);
                if (goods.Count() > 0)
                {
                    foreach (var g in goods)
                    {
                        var imgs = db.GoodImages.Where(x => x.good_id == g.good_id);
                        if (imgs.Count() > 0)
                            db.GoodImages.RemoveRange(imgs);
                    }
                    db.Goods.RemoveRange(goods);
                }
                var seller = airport.Sellers;
                db.Goods.RemoveRange(seller.Goods);
                db.SellerMessage.RemoveRange(seller.SellerMessage);
                db.SellerImage.RemoveRange(seller.SellerImage);
                db.Airports.Remove(airport);
                db.Sellers.Remove(seller);
                db.SaveChanges();
                return RedirectToAction("Index");
            
        }

        // POST: Seller/Delete/5
        [HttpPost]
        public ActionResult Delete(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add delete logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }
    }
}
