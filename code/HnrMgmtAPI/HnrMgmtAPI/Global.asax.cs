using System;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using HnrMgmtAPI.Common;
using System.Net;
using System.Linq;

namespace HnrMgmtAPI
{
    public class WebApiApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            GlobalConfiguration.Configure(WebApiConfig.Register);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
        }

        protected void Application_PreSendRequestHeaders(object sender, EventArgs e)
        {
            //处理401响应被多个框架重写的问题
            var values = Response.Headers.GetValues(ForceHttpStatusCodeResult.ForceHttpUnauthorizedHeaderName);
            if (values != null && values.Contains(ForceHttpStatusCodeResult.ForceHttpUnauthorizedHeaderValue))
            {
                //清空响应头，返回401错误
                Response.ClearHeaders();
                Response.StatusCode = (int)HttpStatusCode.Unauthorized;
            }
        }
    }
}
