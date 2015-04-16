﻿using System;
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
    public class AdsController : Controller
    {
        private planeEntities db = new planeEntities();
        private const int pageSize = 10;

        // GET: Ads
        public ActionResult Index(int page = 1)
        {
            ViewBag.Data = db.Ads.OrderByDescending(x => x.created_at).ToPagedList(page,pageSize);
            return View();
        }

        public ActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Create(CreateAdsModel model)
        {
            if (!ModelState.IsValid) return View(model);

            db.Ads.Add(new Ads() { 
                name = model.Name,
                desci = model.Desc,
                link = model.Link,
                img = FileTool.Save(model.Img,"Images/Ads"),
                created_at = DateTime.Now,
                position = model.Position
            });
            db.SaveChanges();
            ModelState.AddModelError("","添加成功");
            return View(model);
        }

        public ActionResult Detail(int id)
        {
            var ad = db.Ads.Where(x => x.id == id).First();
            return View(new EditAdsModel() { 
                Name = ad.name,
                ImgUrl = ad.img,
                Link = ad.link,
                Desc = ad.desci,
                Id = ad.id,
                Position = ad.position
            });
        }

        [HttpPost]
        public ActionResult Detail(EditAdsModel model)
        {
            if (!ModelState.IsValid) return View(model);

            var ad = db.Ads.Where(x => x.id == model.Id).First();
            ad.name = model.Name;
            ad.desci = model.Desc;
            ad.link = model.Link;
            ad.position = model.Position;
            if (model.Img != null)
                ad.img = FileTool.Save(model.Img,"Images/Ads");
            model.ImgUrl = ad.img;
            db.SaveChanges();
            ModelState.AddModelError("","修改成功");
            return View(model);
        }

        public ActionResult Delete(int id)
        {
            db.Ads.Remove(db.Ads.Where(x => x.id == id).First());
            db.SaveChanges();
            return RedirectToAction("Index");
        }
    }
}