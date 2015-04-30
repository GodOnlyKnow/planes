using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Com.Alipay;
using Planes.Models;
using System.Web.Script.Serialization;

namespace Planes.Controllers
{
    public class CallbackController : Controller
    {
        private planeEntities db = new planeEntities();

        public ActionResult Test()
        {
            return View();
        }

        public object Alipay(Alipay model)
        {
            //{"id":0,"notify_time":"\/Date(1429799390000)\/","notify_type":"trade_status_sync","sign_type":"RSA","notify_id":"972c01f67ef5c5a932e65a8a9b47fa3452","sign":"WSX00DptG6Qfd/gfo6crtnXJd9qeFV/eIe5HZf4bcgS3EFGfpveawlWyaVdS1b1YHwMAymyMeOpvZXCBRLw8I47zNmfFYVbKnaAW8Vf25VPCPncrJp8dzKYy3otsY18kO99SzernebkBShvOudpVzkic6OIlkeH+VsBXVHq7y2c=","out_trade_no":"0423222854-1038","subject":"测试的商品","payment_type":"1","trade_no":"2015042300001000550050307869","trade_status":"TRADE_FINISHED","seller_id":"2088002197126523","seller_email":"globalwings.cn@gmail.com","buyer_id":"2088502246557552","buyer_email":"s771002022@sohu.com","total_fee":0.01,"quantity":1,"price":0.01,"gmt_create":"\/Date(1429799390000)\/","gmt_payment":"\/Date(1429799390000)\/","is_total_fee_adjust":"N","use_coupon":"N","discount":"0.00","refund_status":null,"gmt_refund":null}
            var js = new JavaScriptSerializer();
            System.IO.File.WriteAllText(Server.MapPath("~/Images/logs.txt"), js.Serialize(model));
            SortedDictionary<string, string> sPara = new SortedDictionary<string, string>();
            System.Collections.Specialized.NameValueCollection coll = Request.Form;
            
            if (coll.Count == 0) return "failA";
            for (int i = 0;i < coll.Count;i++)
            {
                sPara.Add(coll.GetKey(i),coll[i]);
            }
            Notify notify = new Notify();

            if (notify.Verify(sPara,model.notify_id,model.sign))
            {
                if (db.Orders.Where(x => x.orderId == model.out_trade_no).Count() < 1)
                    return "failB";
                db.Alipay.Add(model);
                if (db.Orders.Where(x => x.orderId == model.out_trade_no).Count() > 0)
                {
                    var order = db.Orders.Where(x => x.orderId == model.out_trade_no).First();
                    if (order.status == 1) return "success";
                    if (model.trade_status == "TRADE_FINISHED")
                    {
                        order.status = 1;
                    }
                    else if (model.trade_status == "TRADE_SUCCESS")
                    {
                        order.status = 1;
                    }
                    else
                    {
                        order.status = 2;
                    }
                }
                db.SaveChanges();

                return "success";
            }
            else
            {
                return "failC";
            }

        }

        public object AlipayJson(string json)
        {
            var serializer = new JavaScriptSerializer();
            var model = serializer.Deserialize<Alipay>(json);
            return Alipay(model);
        }
    }
}