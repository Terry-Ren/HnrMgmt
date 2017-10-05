using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using HnrMgmtAPI.Models.API;
using HnrMgmtAPI.Models.API.JC;
using HnrMgmtAPI.Models;
using HnrMgmtAPI.Common;
using System;

namespace HnrMgmtAPI.Controllers.API.Sys
{
    [RoutePrefix("api/account")]
    public class AccountController : BaseApiController
    {
        [HttpGet, Route("teacher")]
        public ApiResult GetTeacherList(string access_token, int page, int size)
        {
            result = AccessToken.Check(access_token, "api/account/teacher");
            if (result == null)
            {

                result = new ApiResult();

                #region 参数验证
                if (page == 0 || size == 0)
                {
                    result.status = "error";
                    result.messages = "参数格式错误";
                    return result;
                }
                #endregion

                Return_GetList<T_Account> returnData = new Return_GetList<T_Account>();

                result.status = "success";
                result.messages = "获取数据成功";

                //T_Account.RoleID == "1"  1 代表校团委老师
                var accountList = from T_Account in db.T_Account where (T_Account.RoleID == "1") orderby T_Account.Name select T_Account;
                if (accountList.Any())
                {
                    returnData.count = accountList.Count();
                    returnData.list = (List<T_Account>)(accountList.Skip((page - 1) * size).Take(size).ToList());
                }
                else
                {
                    returnData.count = 0;
                    returnData.list = null;
                }

                result.data = returnData;
                return result;
            }
            return result;
        }

        [HttpPost, Route("addteacher")]
        public ApiResult AddTeacher(string access_token)
        {
            return null;
        }

        [HttpGet, Route("delteacher")]
        public ApiResult DeleteTeacher(string access_token, string accountID)
        {
            return null;
        }

        [HttpPost, Route("modteacher")]
        public ApiResult ModifyTeacher(string access_token)
        {
            return null;
        }
    }
}
