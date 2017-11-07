using HnrMgmtAPI.Common;
using HnrMgmtAPI.Models;
using HnrMgmtAPI.Models.API;
using HnrMgmtAPI.Models.API.TB;
using System;
using System.Collections.Generic;
using System.Data.Entity.Validation;
using System.Web.Http;

namespace HnrMgmtAPI.Controllers.API.TB
{
    [RoutePrefix("api/record")]
    public class RecordController : BaseApiController
    {
        #region 荣誉信息填报
        /// <summary>
        /// 荣誉奖项申请填报
        /// </summary>
        /// <param name="model">参数参考TB_HnrRecord</param>
        /// <returns></returns>
        [HttpPost, Route("honor")]
        public ApiResult HnrRecord([FromBody]TB_HnrRecord model)
        {
            result = AccessToken.Check(model.access_token, "api/record/honor");
            if (result == null)
            {
                #region 参数验证
                result = ParameterCheck.CheckParameters(model);
                if (result != null)
                {
                    return result;
                }

                //验证荣誉类别ID是否正确
                if (db.T_Honor.Find(model.HonorID) == null)
                {
                    return Error("此荣誉奖项ID不存在");
                }

                //验证部门ID是否正确
                if (db.T_Organization.Find(model.OrgID) == null)
                {
                    return Error("此部门ID不存在");
                }

                //暂未实现上传文件接口
                if (model.FileUrl != "-1")
                {
                    return Error("暂不支持上传文件");
                }
                #endregion

                #region 逻辑操作
                T_HnrRecord hnrRecord = new T_HnrRecord();
                T_ExmRecord exmRecord = new T_ExmRecord();
                string ID = Guid.NewGuid().ToString();
                UserInfo userInfo = AccessToken.GetUserInfo(model.access_token);
                if (userInfo == null)
                {
                    //缓存中不存在此用户信息、说明令牌已过期，返回错误信息
                    return Error("access_token已过期，请重新登录！");
                }

                #region 获奖记录

                hnrRecord.HnrRecID = ID;
                hnrRecord.HonorID = model.HonorID.ToString().Trim();
                hnrRecord.Annual = model.Annual.ToString().Trim();
                hnrRecord.Time = model.HnrTime.ToString().Trim();
                hnrRecord.AwdeeID = model.AwdeeID.ToString().Trim();
                hnrRecord.OrgID = model.OrgID.ToString().Trim();
                hnrRecord.Branch = model.Branch.ToString().Trim();
                hnrRecord.FileUrl = "";

                if (model.FileUrl != "-1")
                {
                    //此处暂存上传文件名字
                    hnrRecord.FileUrl = model.FileUrl;
                }
                db.T_HnrRecord.Add(hnrRecord);
                #endregion

                #region 个人信息
                T_Awardee awardee = db.T_Awardee.Find(model.AwdeeID);
                if (awardee != null)
                {
                    //数据库中已存在此学生信息 更新原有记录
                    awardee.Name = model.AwdeeName;
                    awardee.OrgID = model.OrgID;
                    awardee.Branch = model.Branch;
                }
                else
                {
                    //数据库中不存在此获奖学生信息 添加新纪录
                    T_Awardee awardeeModel = new T_Awardee();
                    awardeeModel.AwdeeID = model.AwdeeID.ToString().Trim();
                    awardeeModel.Name = model.AwdeeName.ToString().Trim();
                    awardeeModel.OrgID = model.OrgID.ToString().Trim();
                    awardeeModel.Branch = model.Branch.ToString().Trim();
                    //添加到数据库
                    db.T_Awardee.Add(awardeeModel);
                }
                #endregion

                #region 审核记录
                exmRecord.RecordID = ID;//记录ID
                exmRecord.ApplyID = userInfo.userID;//申请人ID
                exmRecord.ApplyTime = DateTime.Now;//申请时间
                exmRecord.ExmID = null;//审核人ID
                exmRecord.State = "0";//审核状态
                exmRecord.ExmTime = null;
                db.T_ExmRecord.Add(exmRecord);

                try
                {
                    db.SaveChanges();
                    return Success("提交成功");
                }
                catch (DbEntityValidationException dbEx)
                {
                    return Error("提交失败");
                }
                #endregion

                #endregion
            }
            return result;
        }
        #endregion

        #region 竞赛获奖填报
        /// <summary>
        /// 竞赛奖项申请填报
        /// </summary>
        /// <param name="model">参数参考TB_AwdRecord</param>
        /// <returns></returns>
        [HttpPost, Route("award")]
        public ApiResult AwdRecord([FromBody]TB_AwdRecord model)
        {
            result = AccessToken.Check(model.access_token, "api/record/award");
            if (result == null)
            {
                #region 参数验证
                //可为空参数列表
                List<string> nullPropertyList = new List<string>();
                nullPropertyList.Add("Term");
                nullPropertyList.Add("OrgId");
                nullPropertyList.Add("Teacher");

                result = ParameterCheck.CheckIsNullParameters(model, nullPropertyList);
                if (result != null)
                {
                    return result;
                }

                //验证荣誉类别ID是否正确
                if (db.T_Award.Find(model.AwardID) == null)
                {
                    return Error("此竞赛奖项ID不存在");
                }

                //验证团队所属部门ID是否正确
                if (model.IsTeam == "1")
                {
                    if (model.OrgID == null || model.OrgID.ToString() == "")
                    {
                        return Error("参数错误，团队获奖项目必须填写团队所属部门");
                    }
                    else
                    {
                        if (db.T_Organization.Find(model.OrgID) == null)
                        {
                            return Error("团队所属部门ID不存在");
                        }
                    }
                }

                //验证 非团队获奖时，获奖成员只能为1人
                if (model.IsTeam == "0")
                {
                    if (model.Members.Count != 1)
                    {
                        return Error("参数错误，非团队获奖时，项目成员只能为一人");
                    }
                }

                //验证部门ID是否正确
                if (model.Members.Count >= 1)
                {
                    foreach (Awardee item in model.Members)
                    {
                        if (db.T_Organization.Find(item.OrgID) == null)
                        {
                            return Error("部分成员部门ID不存在");
                        }
                    }
                }

                //暂未实现上传文件接口
                if (model.FileUrl != "-1")
                {
                    return Error("暂不支持上传文件");
                }
                #endregion

                #region 逻辑操作
                T_AwdRecord awdRecord = new T_AwdRecord();
                T_ExmRecord exmRecord = new T_ExmRecord();
                string RecordID = Guid.NewGuid().ToString();
                string TeamID = Guid.NewGuid().ToString();
                UserInfo userInfo = AccessToken.GetUserInfo(model.access_token);
                if (userInfo == null)
                {
                    return Error("access_token已过期，请重新登录！");
                }

                #region 加入记录信息

                awdRecord.AwdRecID = RecordID;
                awdRecord.AwdID = model.AwardID.ToString().Trim();
                awdRecord.Year = model.Year.ToString().Trim();
                awdRecord.Time = model.AwdTime.ToString().Trim();
                awdRecord.Term = (model.Term == null) ? null : model.Term.ToString().Trim();
                awdRecord.IsTeam = model.IsTeam;
                awdRecord.ProName = (model.ProjectName == null) ? null : model.ProjectName;
                awdRecord.FileUrl = "";
                if (model.IsTeam == "0")
                {
                    //非团队获奖
                    awdRecord.AwdeeID = model.Members[0].AwdeeID;
                    awdRecord.OrgID = (model.OrgID == null) ? null : model.OrgID;
                    awdRecord.Teacher = "";
                    if (model.Teacher != null && model.Teacher.Count > 0)
                    {
                        foreach (string item in model.Teacher)
                        {
                            awdRecord.Teacher += item;
                            awdRecord.Teacher += "#";
                        }
                    }

                    if (model.FileUrl != "-1")
                    {
                        //此处暂存上传文件名字
                        awdRecord.FileUrl = model.FileUrl;
                    }

                    db.T_AwdRecord.Add(awdRecord);
                }
                else
                {
                    //团队获奖
                    awdRecord.AwdeeID = TeamID;
                    awdRecord.OrgID = (model.OrgID == null) ? null : model.OrgID;
                    awdRecord.Teacher = "";
                    if (model.Teacher != null && model.Teacher.Count > 0)
                    {
                        foreach (string item in model.Teacher)
                        {
                            awdRecord.Teacher += item;
                            awdRecord.Teacher += "#";
                        }
                    }

                    if (model.FileUrl != "-1")
                    {
                        //此处暂存上传文件名字
                        awdRecord.FileUrl = model.FileUrl;
                    }

                    db.T_AwdRecord.Add(awdRecord);
                }
                #endregion

                #region 加入获奖人信息

                if (model.IsTeam == "0")
                {
                    //非团队获奖
                    T_Awardee awardee = db.T_Awardee.Find(model.Members[0].AwdeeID);
                    if (awardee != null)
                    {
                        //数据库中已存在此学生信息 更新原有记录
                        awardee.Name = model.Members[0].AwdeeName;
                        awardee.OrgID = model.Members[0].OrgID;
                        awardee.Branch = model.Members[0].Branch;
                    }
                    else
                    {
                        //数据库中不存在此获奖学生信息 添加新纪录
                        T_Awardee awardeeModel = new T_Awardee();
                        awardeeModel.AwdeeID = model.Members[0].AwdeeID;
                        awardeeModel.Name = model.Members[0].AwdeeName;
                        awardeeModel.OrgID = model.Members[0].OrgID;
                        awardeeModel.Branch = model.Members[0].Branch;
                        //添加到数据库
                        db.T_Awardee.Add(awardeeModel);
                    }
                }
                else
                {
                    //团队获奖
                    int flag = 1;
                    foreach (Awardee item in model.Members)
                    {
                        T_Awardee awardee = db.T_Awardee.Find(item.AwdeeID);
                        if (awardee != null)
                        {
                            //数据库中已存在此学生信息 更新原有记录
                            awardee.Name = item.AwdeeName;
                            awardee.OrgID = item.OrgID;
                            awardee.Branch = item.Branch;
                        }
                        else
                        {
                            //数据库中不存在此获奖学生信息 添加新纪录
                            T_Awardee awardeeModel = new T_Awardee();
                            awardeeModel.AwdeeID = item.AwdeeID;
                            awardeeModel.Name = item.AwdeeName;
                            awardeeModel.OrgID = item.OrgID;
                            awardeeModel.Branch = item.Branch;
                            //添加到数据库
                            db.T_Awardee.Add(awardeeModel);
                        }

                        T_Team teamMember = new T_Team();
                        teamMember.TeamID = TeamID;
                        teamMember.AwdeeID = item.AwdeeID;
                        teamMember.Rank = flag.ToString();
                        db.T_Team.Add(teamMember);

                        flag++;
                    }
                }
                #endregion

                #region 加入审核记录
                exmRecord.RecordID = RecordID;//记录ID
                exmRecord.ApplyID = userInfo.userID;//申请人ID
                exmRecord.ApplyTime = DateTime.Now;//申请时间
                exmRecord.ExmID = null;//审核人ID
                exmRecord.State = "0";//审核状态
                exmRecord.ExmTime = null;
                db.T_ExmRecord.Add(exmRecord);

                //db.SaveChanges();
                try
                {
                    db.SaveChanges();
                    return Success("提交成功");
                }
                catch (DbEntityValidationException dbEx)
                {
                    return Error("提交失败");
                }
                #endregion

                #endregion
            }
            return result;
        }
        #endregion

        #region 记录查询
        [HttpGet, Route("get")]
        public ApiResult GetRecord(string access_token)
        {
            result = AccessToken.Check(access_token, "api/record/get");
            if (result == null)
            {
                #region 参数验证
                //无参数 无需验证
                #endregion

                #region 逻辑操作
                int _page = -1;
                int _limit = -1;
                string _sortField = "";

                return GetData(access_token, _page, _limit, _sortField);
                #endregion
            }
            return result;
        }

        [HttpGet, Route("get")]
        public ApiResult GetRecord(string access_token, int page, int limit)
        {

            result = AccessToken.Check(access_token, "api/record/get");
            if (result == null)
            {
                #region 参数验证
                if (page == 0 || limit == 0)
                {
                    return Error("参数存在错误");
                }
                #endregion

                #region 逻辑操作
                int _page = page;
                int _limit = limit;
                string _sortField = "";

                return GetData(access_token, _page, _limit, _sortField);
                #endregion
            }
            return result;
        }

        [HttpGet, Route("get")]
        public ApiResult GetRecord(string access_token, int page, int limit, string sortField)
        {
            result = AccessToken.Check(access_token, "api/record/get");
            if (result == null)
            {
                #region 参数验证
                if (page == 0 || limit == 0 || sortField == null || sortField == "")
                {
                    return Error("参数存在错误");
                }
                #endregion

                #region 逻辑操作
                int _page = page;
                int _limit = limit;
                string _sortField = sortField;

                return GetData(access_token, _page, _limit, _sortField);
                #endregion
            }
            return result;
        }
        #endregion

        #region 数据库搜索操作
        private ApiResult GetData(string access_token, int page, int limit, string sortField)
        {
            UserInfo userInfo = AccessToken.GetUserInfo(access_token);
            if (userInfo.userRoleID == "4")
            {
                //学生账号只能看到自己的账号记录
                //此块学生模块暂时未做
                return Error("对不起，当前系统不支持学生查询记录");
            }
            else if (userInfo.userRoleID == "3")
            {
                //学院账号可以看到所属学院的账号记录账号记录

            }
            else if (userInfo.userRoleID == "2" || userInfo.userRoleID == "1")
            {
                //校团委助理账号及校团委老师账号可以看到所有记录
            }
            else
            {
                //没有角色ID信息
                return Error("发生未知错误。请联系系统维护人员");
            }
            return result;
        }
        #endregion
    }
}