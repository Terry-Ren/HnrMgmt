using System.Net;
using System.Web.Mvc;

namespace HnrMgmtAPI.Common
{
    public static class ForceHttpStatusCodeResult
    {
        public const string ForceHttpUnauthorizedHeaderName = "ForceHttpUnauthorizedHeader";
        public const string ForceHttpUnauthorizedHeaderValue = "true";

        public static void SetForceHttpUnauthorizedHeader()
        {
            System.Web.HttpContext.Current.Response.AddHeader(ForceHttpUnauthorizedHeaderName, ForceHttpUnauthorizedHeaderValue);
        }
    }
}