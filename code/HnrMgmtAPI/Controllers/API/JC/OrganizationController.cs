using HnrMgmtAPI.Common;
using HnrMgmtAPI.Models;
using HnrMgmtAPI.Models.API;
using HnrMgmtAPI.Models.API.JC;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;

namespace HnrMgmtAPI.Controllers.API.JC
{
    [RoutePrefix("api/org")]
    public class OrganizationController : BaseApiController
    {
        /// <summary>
        ///  获取T_Organization表中全部数据
        /// </summary>
        /// <param name="access_token">授权令牌</param>
        /// <returns></returns>
        [HttpGet, Route("get")]
        public ApiResult Get(string access_token)
        {
            result = AccessToken.Check(access_token, "api/org/get");
            if (result == null)
            {
                result = new ApiResult();

                #region
                Return_GetList<T_Organization> returnData = new Return_GetList<T_Organization>();

                var orgList = from T_Organization in db.T_Organization orderby T_Organization.Name select T_Organization;
                if (orgList.Any())
                {
                    returnData.count = orgList.Count();
                    returnData.list = (List<T_Organization>)(orgList.ToList());
                }
                else
                {
                    returnData.count = 0;
                    returnData.list = null;
                }

                #endregion

                return Success("获取数据成功", returnData);
            }
            return result;
        }

        /// <summary>
        ///  获取T_Organization表中第page页数据 每页数据为limit条
        /// </summary>
        /// <param name="access_token">授权令牌</param>
        /// <param name="page">页码</param>
        /// <param name="limit">每页条数</param>
        /// <returns></returns>
        [HttpGet, Route("get")]
        public ApiResult Get(string access_token, int page, int limit)
        {
            result = AccessToken.Check(access_token, "api/org/get");
            if (result == null)
            {

                result = new ApiResult();

                #region 参数验证
                if (page == 0 || limit == 0)
                {
                    result.status = "error";
                    result.messages = "参数格式错误";
                    return result;
                }
                #endregion

                Return_GetList<T_Organization> returnData = new Return_GetList<T_Organization>();

                result.status = "success";
                result.messages = "获取数据成功";

                var orgList = from T_Organization in db.T_Organization orderby T_Organization.Name select T_Organization;
                if (orgList.Any())
                {
                    returnData.count = orgList.Count();
                    returnData.list = (List<T_Organization>)(orgList.Skip((page - 1) * limit).Take(limit).ToList());
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
        ///  添加到一条数据 T_Organization
        /// </summary>
        /// <param name="model">参数参考OrganizationAdd数据模型</param>
        /// <returns></returns>
        [HttpPost, Route("add")]
        public ApiResult Add([FromBody]OrganizationAdd model)
        {
            result = AccessToken.Check(model.access_token, "api/org/add");
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
                var orgModel = from T_Organization in db.T_Organization where (T_Organization.Name == model.Name) select T_Organization;
                if (orgModel.Any())
                {
                    return Error("该条记录已存在");
                }
                else
                {
                    try
                    {
                        T_Organization org = new T_Organization();
                        org.OrgID = Guid.NewGuid().ToString();
                        org.Name = model.Name;
                        db.T_Organization.Add(org);
                        db.SaveChanges();

                        return Success("添加成功");
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
        ///  删除一条记录 T_Organization
        /// </summary>
        /// <param name="access_token">授权令牌</param>
        /// <param name="awardID">部门ID</param>
        /// <returns></returns>
        [HttpGet, Route("delete")]
        public ApiResult Delete(string access_token, string orgID)
        {
            result = AccessToken.Check(access_token, "api/org/delete");
            if (result == null)
            {

                #region 参数验证
                if (orgID == null || orgID == "")
                {
                    Dictionary<string, string> errorFields = new Dictionary<string, string>();
                    errorFields.Add("orgID", "orgID");
                    return Error("参数格式错误", errorFields);
                }
                #endregion

                #region 逻辑操作
                var orgModel = from T_Organization in db.T_Organization where (T_Organization.OrgID == orgID) select T_Organization;
                if (orgModel.Any())
                {
                    var awardList = from T_AwdRecord in db.T_AwdRecord where (T_AwdRecord.OrgID == orgID) select T_AwdRecord;
                    var honorList = from T_HnrRecord in db.T_HnrRecord where (T_HnrRecord.OrgID == orgID) select T_HnrRecord;

                    if (awardList.Any() || honorList.Any())
                    {
                        return Error("数据库中包含此奖项获奖记录，不能删除");
                    }
                    else
                    {
                        try
                        {
                            T_Organization model = db.T_Organization.Find(orgID);
                            db.T_Organization.Remove(model);
                            db.SaveChanges();

                            return Success("删除成功");
                        }
                        catch
                        {
                            return Error("删除失败");
                        }
                    }
                }
                else
                {
                    return Error("数据库中不包含此竞赛项目ID");
                }
                #endregion

            }
            return result;
        }

        /// <summary>
        ///  修改一条记录 T_Organization
        /// </summary>
        /// <param name="model">参数参考OrganizationModify数据模型</param>
        /// <returns></returns>
        [HttpPost, Route("modify")]
        public ApiResult Modify([FromBody]OrganizationModify model)
        {
            result = AccessToken.Check(model.access_token, "api/org/modify");
            if (result == null)
            {
                result = ParameterCheck.CheckParameters(model);
                if (result == null)
                {
                    #region 参数验证
                    #endregion

                    #region 逻辑操作
                    T_Organization orgModel = db.T_Organization.Find(model.OrgID);
                    if (orgModel != null)
                    {
                        try
                        {
                            orgModel.Name = model.Name;
                            db.SaveChanges();
                            return Success("修改成功");
                        }
                        catch
                        {
                            return Success("修改失败，请检查参数是否正确");
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
    }
}
