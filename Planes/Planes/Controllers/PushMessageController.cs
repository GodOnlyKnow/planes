using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PagedList;
using Planes.Models;
using Planes.Tools;

namespace Planes.Controllers
{
    [NLogin]
    public class PushMessageController : Controller
    {
        private planeEntities db = new planeEntities();
        private const int pageSize = 10;

        // GET: PushMessage
        public ActionResult Index(int page = 1)
        {
            ViewBag.Data = db.PushMessage.OrderByDescending(x => x.created_at).ToPagedList(page,pageSize);
            return View();
        }

        public ActionResult Delete(int id)
        {
            var tmp = db.PushMessage.Where(x => x.id == id).First();
            db.PushMessage.Remove(tmp);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        public ActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Create(PushMessageCreate model)
        {
            if (!ModelState.IsValid) return View(model);

            db.PushMessage.Add(new PushMessage() { 
                title = model.Title,
                body = model.Body,
                created_at = DateTime.Now
            });
            db.SaveChanges();

            try
            {
                switch (model.Type)
                {
                    case 0:
                        Push.PushToAll(model.Title,model.Body);
                        break;
                    case 1:
                        Push.PushToAndroid(model.Title, model.Body);
                        break;
                    case 2:
                        Push.PushToIOS(model.Body);
                        break;
                }
            }
            catch
            {
                ModelState.AddModelError("","推送失败，请重试");
                return View(model);
            }

            ModelState.AddModelError("","推送成功");
            return View(model);
        }
    }
}