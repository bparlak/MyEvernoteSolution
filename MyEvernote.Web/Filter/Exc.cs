using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MyEvernote.Web.Filter
{
    public class Exc : FilterAttribute, IExceptionFilter
    {
        public void OnException(ExceptionContext filterContext)
        {
            filterContext.Controller.TempData["LastError"] = filterContext.Exception;
            filterContext.ExceptionHandled = true;//hatayı biz yöneteceğiz.
            filterContext.Result = new RedirectResult("/Home/HassError");
        }
    }
}