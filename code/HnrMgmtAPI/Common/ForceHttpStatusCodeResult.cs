namespace HnrMgmtAPI.Common
{
    public static class ForceHttpStatusCodeResult
    {
        public const string ForceHttpUnauthorizedHeaderName = "ForceHttpUnauthorizedHeader";
        public const string ForceHttpUnauthorizedHeaderValue = "true";

        public static void SetForceHttpUnauthorizedHeader()
        {
            //添加响应头 返回401错误
            System.Web.HttpContext.Current.Response.AddHeader(ForceHttpUnauthorizedHeaderName, ForceHttpUnauthorizedHeaderValue);
            System.Web.HttpContext.Current.Response.AddHeader("Access-Control-Allow-Origin", "*");
        }
    }
}