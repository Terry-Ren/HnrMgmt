using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace HnrMgmtAPI.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            ViewBag.Title = "Home Page";

            return View();
        }

        /// <summary>
        /// 基础数据 API 接口
        /// </summary>
        /// <returns></returns>
        public ActionResult Api_JC()
        {
            return View();
        }
        
        /// <summary>
        /// 系统管理 API 接口
        /// </summary>
        /// <returns></returns>
        public ActionResult Api_Sys()
        {
            return View();
        }
    }
}
