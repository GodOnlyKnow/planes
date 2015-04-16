using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Mvc;

namespace Planes.Tools
{
    public class ErrorAttribute : HandleErrorAttribute
    {
        public override void OnException(ExceptionContext filterContext)
        {
            if (filterContext.ExceptionHandled || !filterContext.HttpContext.IsCustomErrorEnabled) return;
            if (new HttpException(null,filterContext.Exception).GetHttpCode() != 500) return;
            if (!ExceptionType.IsInstanceOfType(filterContext.Exception)) return;
            var errors = filterContext.Exception;
            if (filterContext.HttpContext.Request.Headers["X-Requested-With"] == "XMLHttpRequest")
            {
                filterContext.Result = new JsonResult
                {
                    JsonRequestBehavior = JsonRequestBehavior.AllowGet,
                    Data = new
                    {
                        error = true,
                        message = filterContext.Exception.Message
                    }
                };
            }
            else
            {
                var controllerName = filterContext.RouteData.Values["controller"].ToString();
                var actionName = filterContext.RouteData.Values["action"].ToString();
                var model = new HandleErrorInfo(filterContext.Exception,controllerName,actionName);
                filterContext.Result = new ViewResult
                {
                    ViewName = View,
                    MasterName = Master,
                    ViewData = new ViewDataDictionary(model),
                    TempData = filterContext.Controller.TempData
                };
            }
            LogError(string.Format("URL地址：{0}\r\n错误信息：{1}\r\n{2}\r\n", HttpContext.Current.Request.RawUrl, errors.Message, errors.ToString()),filterContext.HttpContext.Server.MapPath("~/Logs"));
            filterContext.ExceptionHandled = true;
            filterContext.HttpContext.Response.Clear();
            filterContext.HttpContext.Response.StatusCode = 500;
            filterContext.HttpContext.Response.TrySkipIisCustomErrors = true;
        }

        private void LogError(string input,string path)
        {
            var str = string.Format("时间：{0}\r\n{1}",DateTime.Now.ToLongTimeString(),input);
            File.AppendText(Path.Combine(path, DateTime.Now.ToString("y-M-d") + ".txt"));
        }
    }
}