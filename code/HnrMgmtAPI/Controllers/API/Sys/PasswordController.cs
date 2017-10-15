using HnrMgmtAPI.Common;
using HnrMgmtAPI.Models;
using HnrMgmtAPI.Models.API;
using HnrMgmtAPI.Models.API.Sys;
using System.Linq;
using System.Web.Http;

namespace HnrMgmtAPI.Controllers.API.Sys
{
    [RoutePrefix("api/password")]
    public class PasswordController : BaseApiController
    {
        /// <summary>
        /// 修改密码接口、只能修改令牌本人密码
        /// </summary>
        /// <param name=""></param>
        /// <returns></returns>
        [HttpPost, Route("modify")]
        public ApiResult Modify([FromBody]PasswordModify model)
        {
            //此处调用的是Check的重写方法，因为需验证是否操作的是本人的账户
            result = AccessToken.Check(model.access_token, "api/account/addteacher", model.ID);
            if (result == null)
            {
                #region 参数验证
                result = ParameterCheck.CheckParameters(model);
                if (result != null)
                {
                    return result;
                }
                #endregion

                #region 逻辑操作
                string old_pwd = model.OldPwd.ToString().Trim();
                string new_pwd = model.NewPwd.ToString().Trim();

                var accountlist = from T_Account in db.T_Account where (T_Account.AccountID == model.ID && T_Account.Password == old_pwd) select T_Account;
                if (accountlist.Any())
                {
                    try
                    {
                        T_Account accountmodel = db.T_Account.Find(model.ID);
                        accountmodel.Password = new_pwd;
                        db.SaveChanges();

                        return Success("修改密码成功");
                    }
                    catch
                    {
                        return Error("修改密码失败");
                    }
                }
                else
                {
                    return Error("原密码输入错误");
                }
                #endregion

            }
            return result;
        }
    }
}