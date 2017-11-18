using HnrMgmtAPI.Models.API;
using System;
using System.Collections.Generic;
using System.Web;

namespace HnrMgmtAPI.Common
{
    public class AccessToken
    {
        /// <summary>
        /// 令牌验证
        /// 验证是否存在该令牌
        /// 验证该令牌是否具有该操作权限
        /// </summary>
        /// <param name="access_token">授权令牌</param>
        /// <param name="ApiPath">Api接口路径</param>
        /// <returns></returns>
        public static ApiResult Check(string access_token, string ApiPath)
        {
            #region 测试代码--测试401错误，默认返回ApiResult
            //判断是否处于测试状态，1代表测试状态，其他代表非测试状态
            if (System.Configuration.ConfigurationManager.AppSettings["IsTest401"].ToString() == "1")
            {
                //强制返回401错误
                ForceHttpStatusCodeResult.SetForceHttpUnauthorizedHeader();

                ApiResult result_401 = new ApiResult();
                result_401.status = "error";
                result_401.messages = "发生401错误（处于测试模式）";

                return result_401;
            }
            #endregion

            #region 测试代码--跳过令牌验证，返回null
            //判断是否处于测试状态，1代表测试状态，其他代表非测试状态
            if (System.Configuration.ConfigurationManager.AppSettings["IsTest"].ToString() == "1")
            {
                //返回null  代表验证通过
                return null;
            }
            #endregion

            #region 令牌验证代码
            ApiResult result = new ApiResult();
            try
            {
                UserInfo userInfo = (UserInfo)HttpRuntime.Cache.Get(access_token);
                if (HttpRuntime.Cache.Get(access_token) != null)
                {
                    List<string> permissionList = userInfo.permissionList;
                    if (permissionList.IndexOf(ApiPath) == -1)
                    {
                        Dictionary<string, string> rolePermission = (Dictionary<string, string>)HttpRuntime.Cache.Get("rolePermission");
                        throw new Exception("该账户缺少权限，接口名称：" + rolePermission[ApiPath].ToString());
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
            #endregion
        }

        /// <summary>
        /// 验证是否为本人操作
        /// 令牌验证
        /// 验证是否存在该令牌
        /// 验证该令牌是否具有该操作权限
        /// </summary>
        /// <param name="access_token">授权令牌</param>
        /// <param name="ApiPath">Api接口路径</param>
        /// <param name="AccountID">操作人ID</param>
        /// <returns></returns>
        public static ApiResult Check(string access_token, string ApiPath, string AccountID)
        {
            #region 测试代码--测试401错误，默认返回ApiResult
            //判断是否处于测试状态，1代表测试状态，其他代表非测试状态
            if (System.Configuration.ConfigurationManager.AppSettings["IsTest401"].ToString() == "1")
            {
                //强制返回401错误
                ForceHttpStatusCodeResult.SetForceHttpUnauthorizedHeader();

                ApiResult result_401 = new ApiResult();
                result_401.status = "error";
                result_401.messages = "发生401错误（处于测试模式）";

                return result_401;
            }
            #endregion

            #region 测试代码
            //判断是否处于测试状态，1代表测试状态，其他代表非测试状态
            if (System.Configuration.ConfigurationManager.AppSettings["IsTest"].ToString() == "1")
            {
                //返回null  代表验证通过
                return null;
            }
            #endregion

            #region 令牌验证代码
            ApiResult result = new ApiResult();
            try
            {
                UserInfo userInfo = (UserInfo)HttpRuntime.Cache.Get(access_token);
                if (userInfo != null)
                {
                    List<string> permissionList = userInfo.permissionList;
                    if (permissionList.IndexOf(ApiPath) == -1)
                    {
                        Dictionary<string, string> rolePermission = (Dictionary<string, string>)HttpRuntime.Cache.Get("rolePermission");
                        throw new Exception("该账户缺少权限，权限名称：" + rolePermission[ApiPath].ToString());
                    }
                    else
                    {
                        if (AccountID == userInfo.userID)
                        {
                            return null;
                        }
                        else
                        {
                            throw new Exception("AccessToken错误，此接口不能修改他人账户");
                        }
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
            #endregion
        }

        /// <summary>
        /// 注销AccessToken
        /// </summary>
        /// <param name="access_token">授权令牌</param>
        /// <returns></returns>
        public static ApiResult Clear(string access_token)
        {
            ApiResult result = new ApiResult();
            if (HttpRuntime.Cache.Remove(access_token) != null)
            {
                result.status = "success";
                result.messages = "注销成功";
            }
            else
            {
                result.status = "error";
                result.messages = "令牌无效";
            }
            return result;
        }

        /// <summary>
        /// 获取令牌帐号信息
        /// </summary>
        /// <param name="access_token"></param>
        /// <returns></returns>
        public static UserInfo GetUserInfo(string access_token)
        {
            #region 测试代码
            //判断是否处于测试状态，1代表测试状态，其他代表非测试状态
            if (System.Configuration.ConfigurationManager.AppSettings["IsTest"].ToString() == "1")
            {
                //返回测试信息  代表验证通过
                UserInfo _userInfo = (UserInfo)HttpRuntime.Cache.Get(access_token);
                return _userInfo;
            }
            #endregion

            #region 根据令牌获取用户信息
            UserInfo userInfo = new UserInfo();
            userInfo = (UserInfo)HttpRuntime.Cache.Get(access_token);
            if (HttpRuntime.Cache.Get(access_token) != null)
            {
                return userInfo;
            }
            else
            {
                return null;
            }
            #endregion
        }
    }
}