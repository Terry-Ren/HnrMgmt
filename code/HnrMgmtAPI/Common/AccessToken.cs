using System;
using System.Collections.Generic;
using System.Web;
using HnrMgmtAPI.Models.API;

namespace HnrMgmtAPI.Common
{
    public class AccessToken
    {
        /// <summary>
        /// 令牌验证
        /// 验证是否存在该令牌
        /// 验证该令牌是否具有该操作权限
        /// </summary>
        /// <param name="access_token"></param>
        /// <param name="apiPath"></param>
        /// <returns></returns>
        public static ApiResult Check(string access_token, string apiPath)
        {
            if (System.Configuration.ConfigurationManager.AppSettings["IsTest"].ToString() == "1")
            {
                //强制返回401错误
                //ForceHttpStatusCodeResult.SetForceHttpUnauthorizedHeader();
                return null;
            }
            ApiResult result = new ApiResult();
            try
            {
                UserInfo userInfo = (UserInfo)HttpRuntime.Cache.Get(access_token);
                if (HttpRuntime.Cache.Get(access_token) != null)
                {
                    List<string> permissionList = userInfo.permissionList;
                    if (permissionList.IndexOf(apiPath) == -1)
                    {
                        Dictionary<string, string> rolePermission = (Dictionary<string, string>)HttpRuntime.Cache.Get("rolePermission");
                        throw new Exception("该账户缺少权限，权限名称：" + rolePermission[apiPath].ToString());
                    }
                    else
                    {
                        //返回null 代表令牌验证成功
                        return null;
                    }
                }
                else
                {
                    throw new Exception("AccessToken错误，尚未授权或授权已过期");
                }
            }
            catch (Exception e)
            {
                //强制返回401错误
                ForceHttpStatusCodeResult.SetForceHttpUnauthorizedHeader();
                result.status = "error";
                result.messages = e.Message;
                return result;
            }
        }
    }
}