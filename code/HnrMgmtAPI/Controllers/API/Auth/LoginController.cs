using HnrMgmtAPI.Models;
using HnrMgmtAPI.Models.API;
using HnrMgmtAPI.Models.API.Auth;
using System;
using System.Configuration;
using System.Linq;
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

                    #region 提取用户信息、生成加密令牌、权限写入缓存，现只实现了前两个，尚未写入缓存
                    #endregion

                    AccountInfo accountInfo = new AccountInfo();

                    string accessToken = Guid.NewGuid().ToString();
                    accountInfo.access_token = accessToken;
                    accountInfo.ID = accountModel.AccountID;
                    accountInfo.Name = accountModel.AccountName;
                    accountInfo.OrgName = accountModel.OrgName;
                    accountInfo.RoleID = accountModel.RoleID;
                    accountInfo.RoleName = accountModel.RoleName;

                    return Success("登陆成功", accountInfo);
                }
            }
            #endregion

            return Error("登录失败，账号或密码错误！");
        }
    }
}
