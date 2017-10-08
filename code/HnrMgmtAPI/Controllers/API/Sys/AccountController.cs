using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using HnrMgmtAPI.Models.API;
using HnrMgmtAPI.Models.API.Sys;
using HnrMgmtAPI.Models;
using HnrMgmtAPI.Common;

namespace HnrMgmtAPI.Controllers.API.Sys
{
    [RoutePrefix("api/account")]
    public class AccountController : BaseApiController
    {
        #region 系统管理员可访问接口 用于管理 校团委管理员账号
        /// <summary>
        /// 获取校团委老师信息
        /// </summary>
        /// <param name="access_token">授权令牌</param>
        /// <param name="page">页码</param>
        /// <param name="size">每页条数</param>
        /// <returns></returns>
        [HttpGet, Route("teacher")]
        public ApiResult GetTeacherList(string access_token, int page, int size)
        {
            //若第一次请求第十页 每页20条数据 如何处理？是否报错？   此处不会报错，返回的count是总的数据条数、list中为空
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

                Return_GetList<vw_Account> returnData = new Return_GetList<vw_Account>();

                result.status = "success";
                result.messages = "获取数据成功";

                //vw_Account.RoleID == "1"  1 代表校团委老师
                //此处需验证  若其他字段为null时的 返回结果，在前面测试中，若包含字段结果类型为null，则不能返回值
                var accountList = from vw_Account in db.vw_Account where (vw_Account.RoleID == "1") orderby vw_Account.AccountName select vw_Account;
                if (accountList.Any())
                {
                    returnData.count = accountList.Count();
                    returnData.list = accountList.Skip((page - 1) * size).Take(size).ToList();
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

        /// <summary>
        /// 添加老师信息
        /// </summary>
        /// <param name="model">参数参考TeacherAdd</param>
        /// <returns></returns>
        [HttpPost, Route("addteacher")]
        public ApiResult AddTeacher([FromBody]TeacherAdd model)
        {
            //此处的参数中，OrgID 和 RoleID  不能为空，且必须是数据库中包含的数据
            //需要对OrgID  和  RoleID 对验证处理 验证处理部分代码尚未实现
            //此处在数据库中添加了 触发器 做数据验证，输入的ID不存在时 rollback
            result = AccessToken.Check(model.access_token, "api/account/addteacher");
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
                var accountModel = from T_Account in db.T_Account where (T_Account.AccountID == model.AccountID) select T_Account;
                if (accountModel.Any())
                {
                    return Error("该账号已存在");
                }
                else
                {
                    try
                    {
                        T_Account account = new T_Account();
                        account.AccountID = model.AccountID.Trim();
                        account.Name = model.Name.Trim();
                        account.OrgID = model.OrgID.Trim();
                        account.Tel = model.Tel.Trim();
                        account.RoleID = "1";
                        account.Password = model.AccountID.Substring(model.AccountID.Length - 6, 6);
                        account.State = "1";//1代表可使用

                        db.T_Account.Add(account);
                        db.SaveChanges();

                        return Success("添加账号成功，默认密码为账号后六位");
                    }
                    catch
                    {
                        return Error("添加失败，请检查参数是否正确");
                    }
                }
                #endregion

            }
            return result;
        }

        /// <summary>
        /// 删除老师信息
        /// </summary>
        /// <param name="access_token">授权令牌</param>
        /// <param name="accountID">账号ID</param>
        /// <returns></returns>
        [HttpGet, Route("delteacher")]
        public ApiResult DeleteTeacher(string access_token, string accountID)
        {
            result = AccessToken.Check(access_token, "api/account/delteacher");
            if (result == null)
            {

                #region 参数验证
                if (accountID == null || accountID == "")
                {
                    Dictionary<string, string> errorFields = new Dictionary<string, string>();
                    errorFields.Add("accountID", "accountID错误");
                    return Error("参数格式错误", errorFields);
                }
                #endregion

                #region 逻辑操作
                var accountModel = from T_Account in db.T_Account where (T_Account.AccountID == accountID) select T_Account;
                if (accountModel.Any())
                {
                    if (accountModel.ToList().First().RoleID == "1")
                    {
                        //仅能删除角色为1的账号
                        try
                        {
                            T_Account model = db.T_Account.Find(accountID);
                            db.T_Account.Remove(model);
                            db.SaveChanges();

                            return Success("删除成功");
                        }
                        catch
                        {
                            return Error("删除失败");
                        }
                    }
                    else
                    {
                        //不具备删除其他账号权限
                        return Error("您不具备删除此账号权限");
                    }
                }
                else
                {
                    return Error("数据库中不包含此账号ID");
                }
                #endregion

            }
            return result;
        }

        /// <summary>
        /// 修改老师信息
        /// </summary>
        /// <param name="model">参数参考TeacherAdd</param>
        /// <returns></returns>
        [HttpPost, Route("modteacher")]
        public ApiResult ModifyTeacher([FromBody]TeacherModify model)
        {
            result = AccessToken.Check(model.access_token, "api/account/modteacher");
            if (result == null)
            {
                result = ParameterCheck.CheckParameters(model);
                if (result == null)
                {
                    #region 参数验证
                    #endregion

                    #region 逻辑操作
                    T_Account accountModel = db.T_Account.Find(model.AccountID);
                    if (accountModel != null)
                    {
                        try
                        {
                            accountModel.Name = model.Name;
                            accountModel.OrgID = model.OrgID;
                            accountModel.Tel = model.Tel;
                            db.SaveChanges();
                            return Success("修改成功");
                        }
                        catch
                        {
                            return Error("修改失败，请检查参数是否正确");
                        }
                    }
                    else
                    {
                        return Error("数据错误，无法查找到此条记录");
                    }
                    #endregion
                }
                return result;
            }
            return result;
        }

        /// <summary>
        /// 系统管理员 -> 校团委老师账号 重置账户密码 操作T_Account表
        /// </summary>
        /// <param name="access_token">授权令牌</param>
        /// <param name="accountID">账户ID</param>
        /// <returns></returns>
        [HttpGet, Route("resetteacher")]
        public ApiResult ResetTeacherPwd(string access_token, string accountID)
        {
            result = AccessToken.Check(access_token, "api/account/resetteacher");
            if (result == null)
            {
                #region 参数验证
                if (accountID == null || accountID == "")
                {
                    return Error("accuntID参数错误");
                }
                #endregion

                #region 逻辑操作
                T_Account accountModel = db.T_Account.Find(accountID);
                if (accountModel != null)
                {
                    try
                    {
                        accountModel.Password = accountModel.AccountID.Substring(accountModel.AccountID.Length - 6, 6);
                        db.SaveChanges();
                        return Success("重置密码成功，初始密码为账号后六位");
                    }
                    catch
                    {
                        return Error("修改失败，请检查参数是否正确");
                    }
                }
                else
                {
                    return Error("数据错误，无法查找到此条记录");
                }
                #endregion
            }
            return result;
        }
        #endregion

        #region 校团委管理员可访问接口 用于管理 校团委助理账号、各二级单位 等具有审核权限的账号
        /// <summary>
        /// 获取 角色类型为2 或 3 的账户信息
        /// </summary>
        /// <param name="access_token">授权令牌</param>
        /// <param name="page">页码</param>
        /// <param name="size">页面数量</param>
        /// <returns></returns>
        [HttpGet, Route("admin")]
        public ApiResult GetAdminList(string access_token, int page, int size)
        {
            result = AccessToken.Check(access_token, "api/account/admin");
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

                Return_GetList<vw_Account> returnData = new Return_GetList<vw_Account>();

                result.status = "success";
                result.messages = "获取数据成功";

                //T_Account.RoleID == "2/3"  2 代表校团委助理 3 代表学院账号
                var accountList = from vw_Account in db.vw_Account where (vw_Account.RoleID == "2" || vw_Account.RoleID == "3") orderby vw_Account.RoleName, vw_Account.AccountName select vw_Account;
                if (accountList.Any())
                {
                    returnData.count = accountList.Count();
                    returnData.list = (List<vw_Account>)(accountList.Skip((page - 1) * size).Take(size).ToList());
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

        /// <summary>
        /// 添加 角色类型为2 或 3 的账户信息
        /// </summary>
        /// <param name="model">参数参考AdminAdd</param>
        /// <returns></returns>
        [HttpPost, Route("addadmin")]
        public ApiResult AddAdmin([FromBody]AdminAdd model)
        {
            result = AccessToken.Check(model.access_token, "api/account/addadmin");
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
                var accountModel = from T_Account in db.T_Account where (T_Account.AccountID == model.AccountID) select T_Account;
                if (accountModel.Any())
                {
                    return Error("该账号已存在");
                }
                else
                {
                    if (model.RoleID == "2" || model.RoleID == "3")
                    {
                        try
                        {
                            T_Account account = new T_Account();
                            account.AccountID = model.AccountID.Trim();
                            account.Name = model.Name.Trim();
                            account.OrgID = model.OrgID.Trim();
                            account.Tel = model.Tel.Trim();
                            account.RoleID = model.RoleID;
                            account.Password = model.AccountID.Substring(model.AccountID.Length - 6, 6);
                            account.State = "1";//1代表可使用

                            db.T_Account.Add(account);
                            db.SaveChanges();

                            return Success("添加账号成功，默认密码为账号后六位");
                        }
                        catch
                        {
                            return Error("添加失败，请检查参数是否正确");
                        }
                    }
                    else
                    {
                        return Error("此接口只能设置校团委助理账号或学院账号");
                    }
                }
                #endregion

            }
            return result;
        }

        /// <summary>
        /// 删除 角色类型为2 或 3 的账户信息
        /// </summary>
        /// <param name="access_token">授权令牌</param>
        /// <param name="accountID">账户ID</param>
        /// <returns></returns>
        [HttpGet, Route("deladmin")]
        public ApiResult DeleteAdmin(string access_token, string accountID)
        {
            result = AccessToken.Check(access_token, "api/account/deladmin");
            if (result == null)
            {

                #region 参数验证
                if (accountID == null || accountID == "")
                {
                    Dictionary<string, string> errorFields = new Dictionary<string, string>();
                    errorFields.Add("accountID", "accountID错误");
                    return Error("参数格式错误", errorFields);
                }
                #endregion

                #region 逻辑操作
                var accountModel = from T_Account in db.T_Account where (T_Account.AccountID == accountID) select T_Account;
                if (accountModel.Any())
                {
                    if (accountModel.ToList().First().RoleID == "2" || accountModel.ToList().First().RoleID == "3")
                    {
                        try
                        {
                            T_Account model = db.T_Account.Find(accountID);
                            db.T_Account.Remove(model);
                            db.SaveChanges();

                            return Success("删除成功");
                        }
                        catch
                        {
                            return Error("删除失败");
                        }
                    }
                    else
                    {
                        //不具备删除其他账号权限
                        return Error("您不具备删除此账号权限");
                    }
                }
                else
                {
                    return Error("数据库中不包含此账号ID");
                }
                #endregion

            }
            return result;
        }

        /// <summary>
        /// 修改 角色类型为2 或 3 的账户信息
        /// </summary>
        /// <param name="model">参数参考AdminModify</param>
        /// <returns></returns>
        [HttpPost, Route("modadmin")]
        public ApiResult ModifyAdmin([FromBody]AdminModify model)
        {
            result = AccessToken.Check(model.access_token, "api/account/modadmin");
            if (result == null)
            {
                result = ParameterCheck.CheckParameters(model);
                if (result == null)
                {
                    #region 参数验证
                    #endregion

                    #region 逻辑操作
                    T_Account accountModel = db.T_Account.Find(model.AccountID);
                    if (accountModel != null)
                    {
                        if (accountModel.RoleID == "2" || accountModel.RoleID == "3")
                        {
                            try
                            {
                                accountModel.Name = model.Name;
                                accountModel.OrgID = model.OrgID;
                                accountModel.Tel = model.Tel;
                                accountModel.State = model.State;
                                db.SaveChanges();
                                return Success("修改成功");
                            }
                            catch
                            {
                                return Error("修改失败，请检查参数是否正确");
                            }
                        }
                        else
                        {
                            return Error("您不具备修改此账号权限");
                        }
                    }
                    else
                    {
                        return Error("数据错误，无法查找到此条记录");
                    }
                    #endregion
                }
                return result;
            }
            return result;
        }

        /// <summary>
        /// 校团委老师 -> 校团委助理账号/学院账号 重置账户密码 操作T_Account表
        /// </summary>
        /// <param name="access_token">授权令牌</param>
        /// <param name="accuntID">账户ID</param>
        /// <returns></returns>
        [HttpGet, Route("resetadmin")]
        public ApiResult ResetAdminPwd(string access_token, string accountID)
        {
            result = AccessToken.Check(access_token, "api/account/resetadmin");
            if (result == null)
            {
                #region 参数验证
                if (accountID == null || accountID == "")
                {
                    return Error("accuntID参数错误");
                }
                #endregion

                #region 逻辑操作
                T_Account accountModel = db.T_Account.Find(accountID);
                if (accountModel != null)
                {
                    try
                    {
                        accountModel.Password = accountModel.AccountID.Substring(accountModel.AccountID.Length - 6, 6);
                        db.SaveChanges();
                        return Success("重置密码成功，初始密码为账号后六位");
                    }
                    catch
                    {
                        return Error("修改失败，请检查参数是否正确");
                    }
                }
                else
                {
                    return Error("数据错误，无法查找到此条记录");
                }
                #endregion
            }
            return result;
        }
        #endregion

        #region 各二级单位使用 用于管理 学生账号   暂未开发
        //暂时先不开发
        #endregion
    }
}