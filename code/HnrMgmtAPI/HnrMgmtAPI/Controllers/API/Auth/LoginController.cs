using HnrMgmtAPI.Common;
using HnrMgmtAPI.Models;
using HnrMgmtAPI.Models.API;
using HnrMgmtAPI.Models.API.Auth;
using System;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Http;

namespace HnrMgmtAPI.Controllers.API.Auth
{
    [RoutePrefix("api/auth")]
    public class LoginController : BaseApiController
    {
        /// <summary>
        /// 登录接口
        /// </summary>
        /// <param name="loginIngo">参数类型参考 LoginModel</param>
        /// <returns></returns>
        [HttpPost, Route("login")]
        public ApiResult Login([FromBody]LoginModel loginIngo)
        {
            #region 参数验证
            result = ParameterCheck.CheckParameters(loginIngo);
            if (result != null)
            {
                return result;
            }
            #endregion

            string AccountID = loginIngo.ID.Trim();
            string AccountPwd = loginIngo.Password.Trim();
            string AccountRoleID = loginIngo.RoleID.Trim();

            #region 超级管理员登录
            if (AccountRoleID == "0")
            {
                string adminID = ConfigurationManager.AppSettings["adminID"].ToString().Trim();
                string adminPwd = ConfigurationManager.AppSettings["adminPwd"].ToString().Trim();
                string adminName = ConfigurationManager.AppSettings["adminName"].ToString().Trim();

                if (AccountID == adminID && AccountPwd == adminPwd)
                {
                    AccountInfo adminModel = new AccountInfo();
                    adminModel.access_token = Guid.NewGuid().ToString();
                    adminModel.ID = adminID;
                    adminModel.Name = adminName;
                    adminModel.OrgName = "校团委";
                    adminModel.RoleID = AccountRoleID;
                    adminModel.RoleName = adminName;

                    return Success("登陆成功", adminModel);
                }
            }
            #endregion

            var account = from T_Account in db.T_Account where (T_Account.AccountID == AccountID && T_Account.Password == AccountPwd && T_Account.RoleID == AccountRoleID) select T_Account;

            #region 登录验证
            if (account.Any())
            {
                vw_Account accountModel = (from vw_Account in db.vw_Account where (vw_Account.AccountID == AccountID) select vw_Account).ToList().First();
                if (accountModel != null)
                {
                    //验证账号是否处于冻结状态  0代表冻结状态，1代表使用状态
                    if (accountModel.State.ToString().Trim() == "0")
                    {
                        return Error("该账户处于冻结状态！");
                    }

                    AccountInfo accountInfo = new AccountInfo();

                    //string accessToken = Guid.NewGuid().ToString();
                    string accessToken = "11";

                    accountInfo.access_token = accessToken;
                    accountInfo.ID = accountModel.AccountID;
                    accountInfo.Name = accountModel.AccountName;
                    accountInfo.OrgName = accountModel.OrgName;
                    accountInfo.RoleID = accountModel.RoleID;
                    accountInfo.RoleName = accountModel.RoleName;

                    #region 提取用户信息、生成加密令牌、权限写入缓存 -- 权限尚未写入缓存
                    UserInfo model = new UserInfo();
                    model.access_token = accessToken;
                    model.userID = accountModel.AccountID;
                    model.userName = accountModel.AccountName;
                    model.userOrgID = accountModel.OrgID;
                    model.userOrgName = accountModel.OrgName;
                    model.userRoleID = accountModel.RoleID;
                    model.userRoleName = accountModel.RoleName;
                    model.Tel = accountModel.Tel;

                    //此处用于测试、、完成代码开发后 此处数据来自数据库中的 角色功能视图
                    model.permissionList = null;

                    HttpRuntime.Cache.Insert(accessToken, model, null, DateTime.MaxValue, TimeSpan.FromMinutes(Convert.ToInt32(ConfigurationManager.AppSettings["CacheSpanTime"].ToString())));
                    #endregion

                    return Success("登陆成功", accountInfo);
                }
            }
            #endregion

            return Error("登录失败，账号或密码错误！");
        }

        /// <summary>
        /// 注销接口
        /// </summary>
        /// <param name="access_token">授权令牌</param>
        /// <returns></returns>
        [HttpGet, Route("logout")]
        public ApiResult Logout(string access_token)
        {
            return AccessToken.Clear(access_token);
        }

        /// <summary>
        /// 根据access_token从Cache中获取信息 用于测试使用
        /// </summary>
        /// <param name="access_token"></param>
        /// <returns></returns>
        [HttpGet, Route("getuserinfofromcache")]
        public ApiResult GetUserInfoFromCache(string access_token)
        {
            UserInfo userInfo = (UserInfo)HttpRuntime.Cache.Get(access_token);
            if (userInfo != null)
            {
                return Success(DateTime.Now.ToString() + "缓存中存在此令牌信息", userInfo);
            }
            else
            {
                return Error(DateTime.Now.ToString() + "缓存中不存在此令牌信息");
            }
        }
    }
}