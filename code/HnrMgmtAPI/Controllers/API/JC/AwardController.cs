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
    [RoutePrefix("api/award")]
    public class AwardController : BaseApiController
    {
        /// <summary>
        ///  获取T_Award表中全部数据
        /// </summary>
        /// <param name="access_token">授权令牌</param>
        /// <returns></returns>
        [HttpGet, Route("get")]
        public ApiResult Get(string access_token)
        {
            result = AccessToken.Check(access_token, "api/award/get");
            if (result == null)
            {
                result = new ApiResult();

                #region
                Return_GetList<T_Award> returnData = new Return_GetList<T_Award>();

                var awardList = from T_Award in db.T_Award orderby T_Award.Name, T_Award.GradeName select T_Award;
                if (awardList.Any())
                {
                    returnData.count = awardList.Count();
                    returnData.list = (List<T_Award>)(awardList.ToList());
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
        ///  获取T_Award表中第page页数据 每页数据为limit条
        /// </summary>
        /// <param name="access_token">授权令牌</param>
        /// <param name="page">页码</param>
        /// <param name="limit">每页条数</param>
        /// <returns></returns>
        [HttpGet, Route("get")]
        public ApiResult Get(string access_token, int page, int limit)
        {
            result = AccessToken.Check(access_token, "api/award/get");
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

                Return_GetList<T_Award> returnData = new Return_GetList<T_Award>();

                result.status = "success";
                result.messages = "获取数据成功";

                var awardList = from T_Award in db.T_Award orderby T_Award.Name, T_Award.GradeName select T_Award;
                if (awardList.Any())
                {
                    returnData.count = awardList.Count();
                    returnData.list = (List<T_Award>)(awardList.Skip((page - 1) * limit).Take(limit).ToList());
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
        ///  添加到一条数据 T_Award
        /// </summary>
        /// <param name="model">参数参考AwardAdd数据模型</param>
        /// <returns></returns>
        [HttpPost, Route("add")]
        public ApiResult Add([FromBody]AwardAdd model)
        {
            result = AccessToken.Check(model.access_token, "api/award/add");
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
                var awardModel = from T_Award in db.T_Award where (T_Award.Name == model.Name && T_Award.GradeName == model.GradeName && T_Award.Grade == model.Grade) select T_Award;
                if (awardModel.Any())
                {
                    return Error("该条记录已存在");
                }
                else
                {
                    try
                    {
                        T_Award award = new T_Award();
                        award.AwdID = Guid.NewGuid().ToString();
                        award.Name = model.Name;
                        award.GradeName = model.GradeName;
                        award.Grade = model.Grade;
                        db.T_Award.Add(award);
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
        ///  删除一条记录 T_Award
        /// </summary>
        /// <param name="access_token">授权令牌</param>
        /// <param name="awardID">奖项ID</param>
        /// <returns></returns>
        [HttpGet, Route("delete")]
        public ApiResult Delete(string access_token, string awardID)
        {
            result = AccessToken.Check(access_token, "api/award/delete");
            if (result == null)
            {

                #region 参数验证
                if (awardID == null || awardID == "")
                {
                    Dictionary<string, string> errorFields = new Dictionary<string, string>();
                    errorFields.Add("awardID", "awardID");
                    return Error("参数格式错误", errorFields);
                }
                #endregion

                #region 逻辑操作
                var awardModel = from T_Award in db.T_Award where (T_Award.AwdID == awardID) select T_Award;
                if (awardModel.Any())
                {
                    var awardList = from T_AwdRecord in db.T_AwdRecord where (T_AwdRecord.AwdID == awardID) select T_AwdRecord;
                    if (awardList.Any())
                    {
                        return Error("数据库中包含此奖项获奖记录，不能删除");
                    }
                    else
                    {
                        try
                        {
                            T_Award model = db.T_Award.Find(awardID);
                            db.T_Award.Remove(model);
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
        ///  修改一条记录 T_Award
        /// </summary>
        /// <param name="model">参数参考AwardModify数据模型</param>
        /// <returns></returns>
        [HttpPost, Route("modify")]
        public ApiResult Modify([FromBody]AwardModify model)
        {
            result = AccessToken.Check(model.access_token, "api/award/modify");
            if (result == null)
            {
                result = ParameterCheck.CheckParameters(model);
                if (result == null)
                {
                    #region 参数验证
                    #endregion

                    #region 逻辑操作
                    T_Award awardModel = db.T_Award.Find(model.AwdID);
                    if (awardModel != null)
                    {
                        try
                        {
                            awardModel.Name = model.Name;
                            awardModel.GradeName = model.GradeName;
                            awardModel.Grade = model.Grade;
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

    }
}
