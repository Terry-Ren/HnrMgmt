using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using HnrMgmtAPI.Models;
using System.Web.Http.Controllers;
using System.Web;

namespace HnrMgmtAPI.Controllers.API.JC
{
    [RoutePrefix("api/honor")]
    public class HonorController : ApiController
    {
        ApiResult result = new ApiResult();

        public ApiResult Get()
        {
            try
            {
                string value = HttpRuntime.Cache.Get("key").ToString();
                result.status = "success";
                result.messages = "读取Cache成功  key:" + value;

                HttpRuntime.Cache.Remove("key");
            }
            catch
            {
                result.status = "error";
                result.messages = "读取Cache失败  key:";
            }

            return result;
        }

        [HttpGet, Route("set")]
        public ApiResult Set(string value)
        {
            ApiResult result = new ApiResult();

            HttpRuntime.Cache.Insert("key", value);

            result.status = "success";
            result.messages = "设置Cache成功  key:" + value;

            return result;
        }
    }
}
