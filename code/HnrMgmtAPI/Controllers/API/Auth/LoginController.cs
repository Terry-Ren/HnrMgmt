using System;
using System.Linq;
using System.Web.Http;
using HnrMgmtAPI.Models;
using HnrMgmtAPI.Models.API;
using HnrMgmtAPI.Models.API.Auth;

namespace HnrMgmtAPI.Controllers.API.Auth
{
    [RoutePrefix("api/auth")]
    public class LoginController : BaseApiController
    {
        [HttpPost, Route("login")]
        public ApiResult Post([FromBody]LoginModel loginIngo)
        {
            ApiResult result = new ApiResult();

            string AccountID = loginIngo.id.Trim();
            string AccountPwd = loginIngo.pwd.Trim();
            string AccountRoleID = loginIngo.roleID.Trim();
            var account = from T_Account in db.T_Account where (T_Account.AccountID == AccountID && T_Account.Password == AccountPwd && T_Account.RoleID == AccountRoleID) select T_Account;
            if (account.Any())
            {
                vw_Account accountModel = (from vw_Account in db.vw_Account where (vw_Account.AccountID == AccountID) select vw_Account).ToList().First();
                if (accountModel != null)
                {
                    //登陆成功：提取用户信息、生成加密令牌、权限写入缓存，现只实现了前两个，尚未写入缓存

                    AccountInfo accountInfo = new AccountInfo();

                    string accessToken = Guid.NewGuid().ToString();
                    accountInfo.access_token = accessToken;
                    accountInfo.id = accountModel.AccountID;
                    accountInfo.name = accountModel.AccountName;
                    accountInfo.orgName = accountModel.OrgName;
                    accountInfo.roleID = accountModel.RoleID;
                    accountInfo.roleName = accountModel.RoleName;

                    return Success("登陆成功", accountInfo);
                }
            }

            return Error("登录失败");
        }
    }
}
