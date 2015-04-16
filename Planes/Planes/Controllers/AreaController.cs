using Planes.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PagedList;
using Planes.Tools;

namespace Planes.Controllers
{
    [NLogin]
    public class AreaController : Controller
    {
        private planeEntities db = new planeEntities();
        private const int pageSize = 10;

        // GET: Area
        public ActionResult Index(int page = 1)
        {
            ViewBag.Data = db.Areas.OrderBy(x => x.name).ToPagedList(page, pageSize);
            return View();
        }
        [HttpPost]
        public ActionResult Index(AreaSearchModel model,int page = 1)
        {
            if (model.Address == null && model.Name == null )
                ViewBag.Data = db.Areas.OrderBy(x => x.name).ToPagedList(page, pageSize);
            else
                ViewBag.Data = (from r in db.Areas
                                where r.name.Contains(model.Name) || r.address.Contains(model.Address)
                                select r).OrderBy(x => x.name).ToPagedList(page, pageSize);
            return View();
        }
        // GET: Area/Details/5
        public ActionResult Details(int id)
        {
            var area = db.Areas.Where(t => t.area_id == id).First();
            var loc = area.location.Split('|');
            var model = new AreaEditModel() { 
                Name = area.name,
                Address = area.address,
                Lng = loc[0],
                Lat = loc[1],
                Id = area.area_id
            };
            return View(model);
        }

        // GET: Area/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Area/Create
        [HttpPost]
        public ActionResult Create(AreaCreateModel model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    db.Areas.Add(new Areas()
                    {
                        address = model.Address,
                        name = model.Name,
                        location = model.Lng + "|" + model.Lat
                    });
                    db.SaveChanges();
                    ModelState.AddModelError("", "保存成功");
                }
                return View();
            }
            catch (Exception e)
            {
                ModelState.AddModelError("",e.Message);
                return View();
            }
           
        }

        // GET: Area/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: Area/Edit/5
        [HttpPost]
        public ActionResult Details(AreaEditModel model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var area = db.Areas.Where(x => x.area_id == model.Id).First();
                    area.name = model.Name;
                    area.address = model.Address;
                    area.location = model.Lng + "|" + model.Lat;
                    db.SaveChanges();
                    ModelState.AddModelError("","保存成功");
                }
                return View();
            }
            catch (Exception e)
            {
                ModelState.AddModelError("", e.Message);
                return View();
            }
        }

        // GET: Area/Delete/5
        public RedirectResult Delete(int id)
        {
            try
            {
                
                db.Areas.Remove(db.Areas.Where(t => t.area_id == id).First());
                db.SaveChanges();
                return Redirect("/Area/Index");
            }
            catch 
            {
                return Redirect("/Area/Index");
            }
        }
    }
}
