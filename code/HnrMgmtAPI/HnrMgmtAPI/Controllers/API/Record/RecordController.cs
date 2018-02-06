﻿using HnrMgmtAPI.Common;
using HnrMgmtAPI.Models;
using HnrMgmtAPI.Models.API;
using HnrMgmtAPI.Models.API.Record;
using System;
using System.Collections.Generic;
using System.Data.Entity.Validation;
using System.Linq;
using System.Web.Http;

namespace HnrMgmtAPI.Controllers.API.Record
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
                #endregion

                #region 逻辑操作
                T_HnrRecord hnrRecord = new T_HnrRecord();
                T_ExmRecord exmRecord = new T_ExmRecord();
                string ID = Guid.NewGuid().ToString();
                UserInfo userInfo = AccessToken.GetUserInfo(model.access_token);
                if (userInfo == null)
                {
                    //缓存中不存在此用户信息、说明令牌已过期，返回错误信息
                    return Error();
                }

                #region 信息检查 主要面向学院账号和学生账号
                if (userInfo.userRoleID == "3")
                {
                    if (model.OrgID != userInfo.userOrgID)
                    {
                        return Error("荣誉信息填报申请需由本人账号、或本人所属单位账号、或校级账号填写");
                    }
                }
                if (userInfo.userRoleID == "4")
                {
                    if (model.AwdeeID != userInfo.userID)
                    {
                        return Error("荣誉信息填报申请需由本人账号、或本人所属单位账号、或校级账号填写");
                    }
                }
                #endregion

                #region 获奖记录

                hnrRecord.HnrRecID = ID;
                hnrRecord.HonorID = model.HonorID.ToString().Trim();
                hnrRecord.Annual = model.HnrAnnual.ToString().Trim();
                hnrRecord.Time = model.HnrTime.ToString().Trim();
                hnrRecord.AwdeeID = model.AwdeeID.ToString().Trim();
                hnrRecord.OrgID = model.OrgID.ToString().Trim();
                hnrRecord.Branch = model.Branch.ToString().Trim();
                hnrRecord.FileUrl = "";

                if (model.FileUrl != "-1" && model.FileUrl != "")
                {
                    //此处存储 七牛云 url地址 到数据库中 文件不存在服务器上
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
                nullPropertyList.Add("AwdTerm");
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
                    if (model.AwdOrgID == null || model.AwdOrgID.ToString() == "")
                    {
                        return Error("参数错误，团队获奖项目必须填写团队所属部门");
                    }
                    else
                    {
                        if (db.T_Organization.Find(model.AwdOrgID) == null)
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
                        return Error("参数错误，非团队获奖时，项目成员只能为1人");
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

                #region 信息检查 主要面向学院账号和学生账号
                if (userInfo.userRoleID == "3")
                {
                    if (model.AwdOrgID != userInfo.userOrgID)
                    {
                        return Error("竞赛获奖信息填报申请需由该项目所属单位账号、或项目负责人账号、或校级账号填写");
                    }
                }
                if (userInfo.userRoleID == "4")
                {
                    if (model.Members.ToList().First().AwdeeID != userInfo.userID)
                    {
                        return Error("竞赛获奖信息填报申请需由该项目所属单位账号、或项目负责人账号、或校级账号填写");
                    }
                }
                #endregion

                #region 加入记录信息

                awdRecord.AwdRecID = RecordID;
                awdRecord.AwdID = model.AwardID.ToString().Trim();
                awdRecord.Year = model.AwdYear.ToString().Trim();
                awdRecord.Time = model.AwdTime.ToString().Trim();
                awdRecord.Term = (model.AwdTerm == null) ? null : model.AwdTerm.ToString().Trim();
                awdRecord.IsTeam = model.IsTeam;
                awdRecord.ProName = (model.AwdProName == null) ? null : model.AwdProName;
                awdRecord.FileUrl = "";
                if (model.IsTeam == "0")
                {
                    //非团队获奖
                    awdRecord.AwdeeID = TeamID;
                    awdRecord.OrgID = (model.AwdOrgID == null) ? null : model.AwdOrgID;
                    awdRecord.Teacher = "";
                    if (model.Teacher != null && model.Teacher.Count > 0)
                    {
                        foreach (TeacherInfo item in model.Teacher)
                        {
                            awdRecord.Teacher += item.TchName.ToString().Trim();
                            awdRecord.Teacher += "#";
                            awdRecord.Teacher = awdRecord.Teacher.Substring(0, awdRecord.Teacher.Length - 1);//去掉最后一个 # 
                        }
                    }

                    if (model.FileUrl != "-1" && model.FileUrl != "")
                    {
                        //此处存储 七牛云 url地址 到数据库中 文件不存在服务器上
                        awdRecord.FileUrl = model.FileUrl;
                    }

                    db.T_AwdRecord.Add(awdRecord);
                }
                else
                {
                    //团队获奖
                    awdRecord.AwdeeID = TeamID;
                    awdRecord.OrgID = (model.AwdOrgID == null) ? null : model.AwdOrgID;
                    awdRecord.Teacher = "";
                    if (model.Teacher != null && model.Teacher.Count > 0)
                    {
                        foreach (TeacherInfo item in model.Teacher)
                        {
                            awdRecord.Teacher += item.TchName.ToString().Trim();
                            awdRecord.Teacher += "#";
                        }
                        awdRecord.Teacher = awdRecord.Teacher.Substring(0, awdRecord.Teacher.Length - 1);//去掉最后一个 # 
                    }

                    if (model.FileUrl != "-1" && model.FileUrl != "")
                    {
                        //此处存储 七牛云 url地址 到数据库中 文件不存在服务器上
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

                    T_Team teamMember = new T_Team();
                    teamMember.TeamID = TeamID;
                    teamMember.AwdeeID = model.Members[0].AwdeeID;
                    teamMember.Rank = "0";
                    db.T_Team.Add(teamMember);

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

        #region 记录查询功能
        /// <summary>
        /// 
        /// </summary>
        /// <param name="access_token">授权令牌</param>
        /// <param name="type">记录数据类型 0代表全部 1代表荣誉记录 2代表竞赛获奖记录</param>
        /// <returns></returns>
        [HttpGet, Route("get")]
        public ApiResult GetRecord(string access_token, string type)
        {
            result = AccessToken.Check(access_token, "api/record/get");
            if (result == null)
            {
                #region 参数验证
                if (type != "0" && type != "1" && type != "2")
                {
                    return Error("type参数存在错误");
                }
                #endregion

                #region 逻辑操作
                int _page = -1;
                int _limit = -1;
                string _sortDirection = "";
                string _sortField = "";

                return GetData(access_token, type, _page, _limit, _sortDirection, _sortField);
                #endregion
            }
            return result;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="access_token">授权令牌</param>
        /// <param name="type">记录数据类型 0代表全部 1代表荣誉记录 2代表竞赛获奖记录</param>
        /// <param name="page">页码</param>
        /// <param name="limit">每页条数</param>
        /// <returns></returns>
        [HttpGet, Route("get")]
        public ApiResult GetRecord(string access_token, string type, int page, int limit)
        {

            result = AccessToken.Check(access_token, "api/record/get");
            if (result == null)
            {
                #region 参数验证
                if (page <= 0 || limit <= 0)
                {
                    return Error("参数存在错误");
                }
                if (type != "0" && type != "1" && type != "2")
                {
                    return Error("type参数存在错误");
                }
                #endregion

                #region 逻辑操作
                int _page = page;
                int _limit = limit;
                string _sortDirection = "";
                string _sortField = "";

                return GetData(access_token, type, _page, _limit, _sortDirection, _sortField);
                #endregion
            }
            return result;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="access_token">授权令牌</param>
        /// <param name="type">记录数据类型 0代表全部 1代表荣誉记录 2代表竞赛获奖记录</param>
        /// <param name="page">页码</param>
        /// <param name="limit">每页条数</param>
        /// <param name="sortDirection">ASC 或 DESC</param>
        /// <param name="sortField">排序字段名称</param>
        /// <returns></returns>
        [HttpGet, Route("get")]
        public ApiResult GetRecord(string access_token, string type, int page, int limit, string sortDirection, string sortField)
        {
            result = AccessToken.Check(access_token, "api/record/get");
            if (result == null)
            {
                #region 参数验证
                if (page <= 0 || limit <= 0 || sortDirection == null || sortDirection == "" || sortField == null || sortField == "")
                {
                    return Error("参数存在错误");
                }
                if (sortDirection.ToUpper() != "ASC" && sortDirection.ToUpper() != "DESC")
                {
                    return Error("参数 sortDirection 格式错误");
                }
                if (type != "0" && type != "1" && type != "2")
                {
                    return Error("type参数存在错误");
                }
                #endregion

                #region 逻辑操作
                int _page = page;
                int _limit = limit;
                string _sortDirection = sortDirection;
                string _sortField = sortField;

                return GetData(access_token, type, _page, _limit, _sortDirection, _sortField);
                #endregion
            }
            return result;
        }

        /// <summary>
        /// 获取竞赛获奖团队信息 接口返回数据模型参考TeamInfo
        /// </summary>
        /// <param name="access_token">授权令牌</param>
        /// <param name="AwdRecordID">记录ID</param>
        /// <returns></returns>
        [HttpGet, Route("teaminfo")]
        public ApiResult GetTeamInfo(string access_token, string AwdRecordID)
        {
            result = AccessToken.Check(access_token, "api/record/teaminfo");
            if (result == null)
            {
                #region 参数验证
                if (AwdRecordID == "" || AwdRecordID == null)
                {
                    return Error("AwdRecordID参数不能为空");
                }
                #endregion

                #region 逻辑操作
                //vw_AwdRecord_Rec awdRecord = db.vw_AwdRecord_Rec.Find();
                var awdRecord = from vw_AwdRecord_Rec in db.vw_AwdRecord_Rec where (vw_AwdRecord_Rec.AwdRecID == AwdRecordID) select vw_AwdRecord_Rec.TeamID;
                if (awdRecord.Any())
                {
                    string teamID = awdRecord.ToList().First();
                    var teamMember = from vw_TeamMembers in db.vw_TeamMembers where (vw_TeamMembers.TeamID == teamID) orderby vw_TeamMembers.AwdeeRank select vw_TeamMembers;
                    if (teamMember.Any())
                    {
                        TeamInfo data = new TeamInfo();
                        data.Num = teamMember.Count();
                        data.Members = new List<Member>();

                        foreach (vw_TeamMembers item in teamMember.ToList())
                        {
                            Member model = new Member();

                            model.AwdeeID = item.AwdeeID;
                            model.AwdeeName = item.AwdeeName;
                            model.OrgID = item.AwdeeOrgID;
                            model.OrgName = item.AwdeeOrgName;
                            model.Branch = item.AwdeeBranch;
                            model.Rank = item.AwdeeRank;

                            data.Members.Add(model);
                        }

                        return Success("获取团队成员数据成功", data);
                    }
                    else
                    {
                        return Error("团队成员信息不存在");
                    }
                }
                else
                {
                    return Error("此竞赛获奖记录ID不存在");
                }
                #endregion
            }
            return null;
        }

        /// <summary>
        /// 排序、分页、多条件查询
        /// 参数type值为0时，可搜索字段为：RecordID、Time、OrgID、OrgName、AwdeeName、AwdeeOrgName、State
        /// 参数type值不为0时，可搜索字段为 视图中所有字段
        /// </summary>
        /// <param name="model">参数参考 SelectCondition</param>
        /// <returns></returns>
        [HttpPost, Route("multiconditionquery")]
        public ApiResult GetRecord([FromBody]SelectCondition model)
        {
            result = AccessToken.Check(model.access_token, "api/record/multiconditionquery");
            if (result == null)
            {
                #region 参数验证
                //可为空参数列表
                List<string> nullPropertyList = new List<string>();
                nullPropertyList.Add("page");
                nullPropertyList.Add("limit");
                nullPropertyList.Add("sortDirection");
                nullPropertyList.Add("sortField");

                result = ParameterCheck.CheckIsNullParameters(model, nullPropertyList);
                if (result != null)
                {
                    return result;
                }

                if (model.sortDirection != null && model.sortDirection != "ASC" && model.sortDirection != "DESC")
                {
                    return Error("sortDirection参数取值错误");
                }

                if (model.conditions != null && model.conditions.Count > 0)
                {
                    foreach (ConditionModel conditionItem in model.conditions)
                    {
                        if (conditionItem.fieldName == "State")
                        {
                            foreach (var item in conditionItem.fieldValues)
                            {
                                if (item.item.ToString().Trim() == "-1" || item.item.ToString().Trim() == "-2" || item.item.ToString().Trim() == "-3")
                                {
                                    return Error("不能搜索已删除记录");
                                }
                            }
                        }
                        else
                        {
                            continue;
                        }
                    }
                }

                int _page = (model.page == 0) ? 0 : model.page;
                int _limit = (model.limit == 0) ? 0 : model.limit;
                string _sortDirection = (model.sortDirection == null) ? "" : model.sortDirection;
                string _sortField = (model.sortField == null) ? "" : model.sortField;
                #endregion

                #region 逻辑操作
                try
                {
                    //var list = db.vw_HnrRecord.Where(ConditionExpressions.GetConditionExpression<vw_HnrRecord>(model.conditions));
                    //var _list = GetList(list, _page, _limit, _sortDirection, _sortField);

                    return GetData(model);
                }
                catch
                {
                    return Error("操作失败");
                }

                #endregion
            }
            return result;
        }
        #endregion

        #region 编辑功能
        /// <summary>
        /// 荣誉记录修改
        /// </summary>
        /// <param name="model">参数参考ModifyHnrRecord</param>
        /// <returns></returns>
        [HttpPost, Route("hnrmodify")]
        public ApiResult HnrRecordModify([FromBody]ModifyHnrRecord model)
        {
            result = AccessToken.Check(model.access_token, "api/record/modify");
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
                #endregion

                #region 逻辑操作
                T_HnrRecord hnrRecord = new T_HnrRecord();
                T_ExmRecord exmRecord = new T_ExmRecord();
                string ID = Guid.NewGuid().ToString();
                UserInfo userInfo = AccessToken.GetUserInfo(model.access_token);
                if (userInfo == null)
                {
                    //缓存中不存在此用户信息、说明令牌已过期，返回错误信息
                    return Error();
                }

                var _recordList = from vw_HnrRecord in db.vw_HnrRecord where (vw_HnrRecord.HnrRecID == model.HnrRecordID) select vw_HnrRecord;
                if (_recordList.Any())
                {
                    vw_HnrRecord _recordModel = _recordList.ToList().First();
                    if (_recordModel.State == "0")
                    {
                        T_ExmRecord _exmRecord = db.T_ExmRecord.Find(model.HnrRecordID);
                        if (_exmRecord != null)
                        {
                            _exmRecord.State = "-3";
                            _exmRecord.ExmID = userInfo.userID;
                            _exmRecord.ExmTime = DateTime.Now;
                        }
                        else
                        {
                            return SystemError();
                        }
                    }
                    else
                    {
                        return Error("该记录已审核，不能执行修改操作");
                    }
                }
                else
                {
                    return Error("此记录ID不存在");
                }

                #region 信息检查 主要面向学院账号和学生账号
                if (userInfo.userRoleID == "3")
                {
                    if (model.OrgID != userInfo.userOrgID)
                    {
                        return Error("荣誉信息填报申请只有本人账号、或本人所属单位账号、或校级账号可以修改");
                    }
                }
                if (userInfo.userRoleID == "4")
                {
                    if (model.AwdeeID != userInfo.userID)
                    {
                        return Error("荣誉信息填报申请只有本人账号、或本人所属单位账号、或校级账号可以修改");
                    }
                }
                #endregion

                #region 获奖记录
                hnrRecord.HnrRecID = ID;
                hnrRecord.HonorID = model.HonorID.ToString().Trim();
                hnrRecord.Annual = model.HnrAnnual.ToString().Trim();
                hnrRecord.Time = model.HnrTime.ToString().Trim();
                hnrRecord.AwdeeID = model.AwdeeID.ToString().Trim();
                hnrRecord.OrgID = model.OrgID.ToString().Trim();
                hnrRecord.Branch = model.Branch.ToString().Trim();
                hnrRecord.FileUrl = "";

                if (model.FileUrl != "-1" && model.FileUrl != "")
                {
                    //此处存储 七牛云 url地址 到数据库中 文件不存在服务器上
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
                    return Success("修改成功");
                }
                catch (DbEntityValidationException dbEx)
                {
                    return Error("修改失败");
                }
                #endregion

                #endregion
            }
            return result;
        }

        /// <summary>
        /// 竞赛获奖记录修改
        /// </summary>
        /// <param name="model">参数参考ModifyAwdRecord</param>
        /// <returns></returns>
        [HttpPost, Route("awdmodify")]
        public ApiResult AwdRecordModify([FromBody]ModifyAwdRecord model)
        {
            result = AccessToken.Check(model.access_token, "api/record/award");
            if (result == null)
            {
                #region 参数验证
                //可为空参数列表
                List<string> nullPropertyList = new List<string>();
                nullPropertyList.Add("AwdTerm");
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
                    if (model.AwdOrgID == null || model.AwdOrgID.ToString() == "")
                    {
                        return Error("参数错误，团队获奖项目必须填写团队所属部门");
                    }
                    else
                    {
                        if (db.T_Organization.Find(model.AwdOrgID) == null)
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
                        return Error("参数错误，非团队获奖时，项目成员只能为1人");
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

                var _recordList = from vw_AwdRecord_Rec in db.vw_AwdRecord_Rec where (vw_AwdRecord_Rec.AwdRecID == model.AwdRecordID) select vw_AwdRecord_Rec;
                if (_recordList.Any())
                {
                    vw_AwdRecord_Rec _recordModel = _recordList.ToList().First();
                    if (_recordModel.State == "0")
                    {
                        T_ExmRecord _exmRecord = db.T_ExmRecord.Find(model.AwdRecordID);
                        if (_exmRecord != null)
                        {
                            _exmRecord.State = "-3";
                            _exmRecord.ExmID = userInfo.userID;
                            _exmRecord.ExmTime = DateTime.Now;
                        }
                        else
                        {
                            return SystemError();
                        }
                    }
                    else
                    {
                        return Error("该记录已审核，不能执行修改操作");
                    }
                }
                else
                {
                    return Error("此记录ID不存在");
                }

                #region 信息检查 主要面向学院账号和学生账号
                if (userInfo.userRoleID == "3")
                {
                    if (model.AwdOrgID != userInfo.userOrgID)
                    {
                        return Error("竞赛获奖信息填报申请需由该项目所属单位账号、或项目负责人账号、或校级账号填写");
                    }
                }
                if (userInfo.userRoleID == "4")
                {
                    if (model.Members.ToList().First().AwdeeID != userInfo.userID)
                    {
                        return Error("竞赛获奖信息填报申请需由该项目所属单位账号、或项目负责人账号、或校级账号填写");
                    }
                }
                #endregion

                #region 加入记录信息

                awdRecord.AwdRecID = RecordID;
                awdRecord.AwdID = model.AwardID.ToString().Trim();
                awdRecord.Year = model.AwdYear.ToString().Trim();
                awdRecord.Time = model.AwdTime.ToString().Trim();
                awdRecord.Term = (model.AwdTerm == null) ? null : model.AwdTerm.ToString().Trim();
                awdRecord.IsTeam = model.IsTeam;
                awdRecord.ProName = (model.AwdProName == null) ? null : model.AwdProName;
                awdRecord.FileUrl = "";
                if (model.IsTeam == "0")
                {
                    //非团队获奖
                    awdRecord.AwdeeID = TeamID;
                    awdRecord.OrgID = (model.AwdOrgID == null) ? null : model.AwdOrgID;
                    awdRecord.Teacher = "";
                    if (model.Teacher != null && model.Teacher.Count > 0)
                    {
                        foreach (string item in model.Teacher)
                        {
                            awdRecord.Teacher += item;
                            awdRecord.Teacher += "#";
                        }
                        awdRecord.Teacher = awdRecord.Teacher.Substring(0, awdRecord.Teacher.Length - 1);//去掉最后一个 # 
                    }

                    if (model.FileUrl != "-1" && model.FileUrl != "")
                    {
                        //此处存储 七牛云 url地址 到数据库中 文件不存在服务器上
                        awdRecord.FileUrl = model.FileUrl;
                    }

                    db.T_AwdRecord.Add(awdRecord);
                }
                else
                {
                    //团队获奖
                    awdRecord.AwdeeID = TeamID;
                    awdRecord.OrgID = (model.AwdOrgID == null) ? null : model.AwdOrgID;
                    awdRecord.Teacher = "";
                    if (model.Teacher != null && model.Teacher.Count > 0)
                    {
                        foreach (string item in model.Teacher)
                        {
                            awdRecord.Teacher += item;
                            awdRecord.Teacher += "#";
                        }
                        awdRecord.Teacher = awdRecord.Teacher.Substring(0, awdRecord.Teacher.Length - 1);//去掉最后一个 # 
                    }

                    if (model.FileUrl != "-1" && model.FileUrl != "")
                    {
                        //此处存储 七牛云 url地址 到数据库中 文件不存在服务器上
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

                    T_Team teamMember = new T_Team();
                    teamMember.TeamID = TeamID;
                    teamMember.AwdeeID = model.Members[0].AwdeeID;
                    teamMember.Rank = "0";
                    db.T_Team.Add(teamMember);

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

        #region 删除功能
        /// <summary>
        /// 删除荣誉获奖填报记录
        /// </summary>
        /// <param name="access_token"></param>
        /// <param name="HnrRecordID"></param>
        /// <returns></returns>
        [HttpGet, Route("delete")]
        public ApiResult DeleteHnrRecord(string access_token, string HnrRecordID)
        {
            result = AccessToken.Check(access_token, "api/record/delete");
            if (result == null)
            {
                #region 参数验证
                if (HnrRecordID == "" || HnrRecordID == null)
                {
                    return Error("HnrRecordID参数不能为空");
                }
                #endregion

                #region 逻辑操作
                var recordList = from vw_HnrRecord in db.vw_HnrRecord where (vw_HnrRecord.HnrRecID == HnrRecordID) select vw_HnrRecord;//此处 State == -1 代表记录已被删除

                if (recordList.Any())
                {
                    vw_HnrRecord recordModel = recordList.ToList().First();
                    if (recordModel.State != "0")
                    {
                        return Error("该记录已审核，不能执行删除操作");
                    }
                    UserInfo userInfo = AccessToken.GetUserInfo(access_token);
                    if (userInfo != null)
                    {
                        if (userInfo.userRoleID == "4")
                        {
                            #region 删除操作
                            if (recordModel.ApplyAccountID == userInfo.userID)
                            {
                                //可删除
                                T_ExmRecord exmRecord = db.T_ExmRecord.Find(recordModel.HnrRecID);
                                if (exmRecord != null)
                                {
                                    exmRecord.State = "-1";
                                    exmRecord.ExmID = userInfo.userID;
                                    exmRecord.ExmTime = DateTime.Now;
                                    try
                                    {
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
                                    return SystemError();
                                }
                            }
                            else
                            {
                                //不可删除
                                return Error("您不具备删除该记录权限，学生账号只能删除本人申请记录");
                            }
                            #endregion
                        }
                        else if (userInfo.userRoleID == "3")
                        {
                            #region 删除操作
                            if (recordModel.ApplyAccountOrgID == userInfo.userOrgID)
                            {
                                //可删除
                                T_ExmRecord exmRecord = db.T_ExmRecord.Find(recordModel.HnrRecID);
                                if (exmRecord != null)
                                {
                                    exmRecord.State = "-1";
                                    exmRecord.ExmID = userInfo.userID;
                                    exmRecord.ExmTime = DateTime.Now;
                                    try
                                    {
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
                                    return SystemError();
                                }
                            }
                            else
                            {
                                return Error("您不具备删除该记录权限，学院账号只能删除本院学生申请记录");
                            }
                            #endregion
                        }
                        else if (userInfo.userRoleID == "2" || userInfo.userRoleID == "1")
                        {
                            #region 删除操作
                            //可删除
                            T_ExmRecord exmRecord = db.T_ExmRecord.Find(recordModel.HnrRecID);
                            if (exmRecord != null)
                            {
                                exmRecord.State = "-1";
                                exmRecord.ExmID = userInfo.userID;
                                exmRecord.ExmTime = DateTime.Now;
                                try
                                {
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
                                return SystemError();
                            }
                            #endregion
                        }
                        else
                        {
                            return Error("此账号不具备删除数据库记录权限");
                        }
                    }
                    else
                    {
                        return Error();
                    }
                }
                else
                {
                    return Error("此记录ID不存在！");
                }
                #endregion
            }
            return result;
        }

        /// <summary>
        /// 删除竞赛获奖填报记录
        /// </summary>
        /// <param name="access_token"></param>
        /// <param name="AwdRecordID"></param>
        /// <returns></returns>
        [HttpGet, Route("delete")]
        public ApiResult DeleteAwdRecord(string access_token, string AwdRecordID)
        {
            result = AccessToken.Check(access_token, "api/record/delete");
            if (result == null)
            {
                #region 参数验证
                if (AwdRecordID == "" || AwdRecordID == null)
                {
                    return Error("HnrRecordID参数不能为空");
                }
                #endregion

                #region 逻辑操作
                var recordList = from vw_AwdRecord_Rec in db.vw_AwdRecord_Rec where (vw_AwdRecord_Rec.AwdRecID == AwdRecordID) select vw_AwdRecord_Rec;//此处 State == -1 代表记录已被删除

                if (recordList.Any())
                {
                    vw_AwdRecord_Rec recordModel = recordList.ToList().First();
                    if (recordModel.State != "0")
                    {
                        return Error("该记录已审核，不能执行删除操作");
                    }
                    UserInfo userInfo = AccessToken.GetUserInfo(access_token);
                    if (userInfo != null)
                    {
                        if (userInfo.userRoleID == "4")
                        {
                            #region 删除操作
                            if (recordModel.ApplyAccountID == userInfo.userID)
                            {
                                //可删除
                                T_ExmRecord exmRecord = db.T_ExmRecord.Find(recordModel.AwdRecID);
                                if (exmRecord != null)
                                {
                                    exmRecord.State = "-1";
                                    exmRecord.ExmID = userInfo.userID;
                                    exmRecord.ExmTime = DateTime.Now;
                                    try
                                    {
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
                                    return SystemError();
                                }
                            }
                            else
                            {
                                //不可删除
                                return Error("您不具备删除该记录权限，学生账号只能删除本人申请记录");
                            }
                            #endregion
                        }
                        else if (userInfo.userRoleID == "3")
                        {
                            #region 删除操作
                            if (recordModel.ApplyAccountOrgID == userInfo.userOrgID)
                            {
                                //可删除
                                T_ExmRecord exmRecord = db.T_ExmRecord.Find(recordModel.AwdRecID);
                                if (exmRecord != null)
                                {
                                    exmRecord.State = "-1";
                                    exmRecord.ExmID = userInfo.userID;
                                    exmRecord.ExmTime = DateTime.Now;
                                    try
                                    {
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
                                    return SystemError();
                                }
                            }
                            else
                            {
                                return Error("您不具备删除该记录权限，学院账号只能删除本院学生申请记录");
                            }
                            #endregion
                        }
                        else if (userInfo.userRoleID == "2" || userInfo.userRoleID == "1")
                        {
                            #region 删除操作
                            //可删除
                            T_ExmRecord exmRecord = db.T_ExmRecord.Find(recordModel.AwdRecID);
                            if (exmRecord != null)
                            {
                                exmRecord.State = "-1";
                                exmRecord.ExmID = userInfo.userID;
                                exmRecord.ExmTime = DateTime.Now;
                                try
                                {
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
                                return SystemError();
                            }
                            #endregion
                        }
                        else
                        {
                            return Error("此账号不具备删除数据库记录权限");
                        }
                    }
                    else
                    {
                        return Error();
                    }
                }
                else
                {
                    return Error("此记录ID不存在！");
                }
                #endregion
            }
            return result;
        }

        /// <summary>
        /// 强制删除荣誉获奖填报记录
        /// </summary>
        /// <param name="access_token"></param>
        /// <param name="RecordID"></param>
        /// <returns></returns>
        [HttpGet, Route("forcedelete")]
        public ApiResult ForceDeleteHnrRecord(string access_token, string HnrRecordID)
        {
            result = AccessToken.Check(access_token, "api/record/forcedelete");
            if (result == null)
            {
                #region 参数验证
                if (HnrRecordID == "" || HnrRecordID == null)
                {
                    return Error("HnrRecordID参数不能为空");
                }
                #endregion

                #region 逻辑操作
                var recordList = from vw_HnrRecord in db.vw_HnrRecord where (vw_HnrRecord.HnrRecID == HnrRecordID) select vw_HnrRecord;//此处 State == -1 代表记录已被删除
                if (recordList.Any())
                {
                    vw_HnrRecord recordModel = recordList.ToList().First();
                    UserInfo userInfo = AccessToken.GetUserInfo(access_token);
                    T_ExmRecord exmRecord = db.T_ExmRecord.Find(recordModel.HnrRecID);
                    if (exmRecord != null)
                    {
                        exmRecord.State = "-2";//此处代表强制删除
                        exmRecord.ExmID = userInfo.userID;
                        exmRecord.ExmTime = DateTime.Now;
                        try
                        {
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
                        return SystemError();
                    }
                }
                else
                {
                    return Error("此记录ID不存在！");
                }
                #endregion
            }
            return result;
        }

        /// <summary>
        /// 强制删除竞赛获奖填报记录
        /// </summary>
        /// <param name="access_token"></param>
        /// <param name="AwdRecordID"></param>
        /// <returns></returns>
        [HttpGet, Route("forcedelete")]
        public ApiResult ForceDeleteAwdRecord(string access_token, string AwdRecordID)
        {
            result = AccessToken.Check(access_token, "api/record/forcedelete");
            if (result == null)
            {
                #region 参数验证
                if (AwdRecordID == "" || AwdRecordID == null)
                {
                    return Error("AwdRecordID参数不能为空");
                }
                #endregion

                #region 逻辑操作
                var recordList = from vw_AwdRecord_Rec in db.vw_AwdRecord_Rec where (vw_AwdRecord_Rec.AwdRecID == AwdRecordID) select vw_AwdRecord_Rec;//此处 State == -1 代表记录已被删除
                if (recordList.Any())
                {
                    vw_AwdRecord_Rec recordModel = recordList.ToList().First();
                    UserInfo userInfo = AccessToken.GetUserInfo(access_token);
                    T_ExmRecord exmRecord = db.T_ExmRecord.Find(recordModel.AwdRecID);
                    if (exmRecord != null)
                    {
                        exmRecord.State = "-2";//此处代表强制删除
                        exmRecord.ExmID = userInfo.userID;
                        exmRecord.ExmTime = DateTime.Now;
                        try
                        {
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
                        return SystemError();
                    }
                }
                else
                {
                    return Error("此记录ID不存在！");
                }
                #endregion
            }
            return result;
        }
        #endregion

        #region 审核功能
        /// <summary>
        /// 审核通过荣誉奖项记录
        /// </summary>
        /// <param name="access_token">授权令牌</param>
        /// <param name="HnrRecordID">荣誉奖项记录ID</param>
        /// <returns></returns>
        [HttpGet, Route("auditpass")]
        public ApiResult AuditPassHnrRecord(string access_token, string HnrRecordID)
        {
            result = AccessToken.Check(access_token, "api/record/auditpass");
            if (result == null)
            {
                #region 参数验证
                if (HnrRecordID == null || HnrRecordID == "")
                {
                    return Error("HnrRecordID参数不能为空");
                }
                #endregion

                #region 逻辑操作
                UserInfo userInfo = AccessToken.GetUserInfo(access_token);
                if (userInfo != null)
                {
                    var recordList = from vw_HnrRecord in db.vw_HnrRecord where (vw_HnrRecord.HnrRecID == HnrRecordID) select vw_HnrRecord;
                    if (recordList.Any())
                    {
                        vw_HnrRecord recordModel = recordList.ToList().First();
                        if (userInfo.userRoleID == "4")
                        {
                            return Error("学生账号不能审核记录");
                        }
                        else if (userInfo.userRoleID == "3")
                        {
                            //只能审核本学院的申请记录 审核状态 0 -> 1
                            if (recordModel.State == "0")
                            {
                                if (recordModel.AwardeeOrgID == userInfo.userOrgID)
                                {
                                    try
                                    {
                                        T_ExmRecord exmRecord = db.T_ExmRecord.Find(HnrRecordID);
                                        exmRecord.State = "1";
                                        exmRecord.ExmTime = DateTime.Now;
                                        db.SaveChanges();
                                        return Success("已通过");
                                    }
                                    catch
                                    {
                                        return SystemError();
                                    }
                                }
                                else
                                {
                                    return Error("荣誉奖项记录只能由本学院账号");
                                }
                            }
                            else
                            {
                                return Error("此记录已学院审核完成");
                            }
                        }
                        else
                        {
                            //任何记录 审核状态 0,1 -> 2
                            if (recordModel.State == "0" || recordModel.State == "1")
                            {
                                try
                                {
                                    T_ExmRecord exmRecord = db.T_ExmRecord.Find(HnrRecordID);
                                    exmRecord.State = "2";
                                    exmRecord.ExmID = userInfo.userID;
                                    exmRecord.ExmTime = DateTime.Now;
                                    db.SaveChanges();
                                    return Success("已通过");
                                }
                                catch
                                {
                                    return SystemError();
                                }
                            }
                            else
                            {
                                return Error("此记录校团委已审核");
                            }
                        }
                    }
                    else
                    {
                        return Error("此荣誉奖项ID不存在");
                    }
                }
                else
                {
                    //令牌过期 返回
                    return Error();
                }
                #endregion
            }
            return result;
        }

        /// <summary>
        /// 审核通过竞赛奖项记录
        /// </summary>
        /// <param name="access_token">授权令牌</param>
        /// <param name="AwdRecordID">竞赛奖项记录ID</param>
        /// <returns></returns>
        [HttpGet, Route("auditpass")]
        public ApiResult AuditPassAwdRecord(string access_token, string AwdRecordID)
        {
            result = AccessToken.Check(access_token, "api/record/auditpass");
            if (result == null)
            {
                #region 参数验证
                if (AwdRecordID == null || AwdRecordID == "")
                {
                    return Error("HnrRecordID参数不能为空");
                }
                #endregion

                #region 逻辑操作
                UserInfo userInfo = AccessToken.GetUserInfo(access_token);
                if (userInfo != null)
                {
                    var recordList = from vw_AwdRecord_Rec in db.vw_AwdRecord_Rec where (vw_AwdRecord_Rec.AwdRecID == AwdRecordID) select vw_AwdRecord_Rec;
                    if (recordList.Any())
                    {
                        vw_AwdRecord_Rec recordModel = recordList.ToList().First();
                        if (userInfo.userRoleID == "4")
                        {
                            return Error("学生账号不能审核记录");
                        }
                        else if (userInfo.userRoleID == "3")
                        {
                            //只能审核本学院的申请记录 审核状态 0 -> 1
                            if (recordModel.State == "0")
                            {
                                if (recordModel.OrgID == userInfo.userOrgID)
                                {
                                    try
                                    {
                                        T_ExmRecord exmRecord = db.T_ExmRecord.Find(AwdRecordID);
                                        exmRecord.State = "1";
                                        exmRecord.ExmTime = DateTime.Now;
                                        db.SaveChanges();
                                        return Success("已通过");
                                    }
                                    catch
                                    {
                                        return SystemError();
                                    }
                                }
                                else
                                {
                                    return Error("荣誉奖项记录只能由本学院账号");
                                }
                            }
                            else
                            {
                                return Error("此记录已学院审核完成");
                            }
                        }
                        else
                        {
                            //任何记录 审核状态 0,1 -> 2
                            if (recordModel.State == "0" || recordModel.State == "1")
                            {
                                try
                                {
                                    T_ExmRecord exmRecord = db.T_ExmRecord.Find(AwdRecordID);
                                    exmRecord.State = "2";
                                    exmRecord.ExmID = userInfo.userID;
                                    exmRecord.ExmTime = DateTime.Now;
                                    db.SaveChanges();
                                    return Success("已通过");
                                }
                                catch
                                {
                                    return SystemError();
                                }
                            }
                            else
                            {
                                return Error("此记录校团委已审核");
                            }
                        }
                    }
                    else
                    {
                        return Error("此荣誉奖项ID不存在");
                    }
                }
                else
                {
                    //令牌过期 返回
                    return Error();
                }
                #endregion
            }
            return result;
        }

        /// <summary>
        /// 驳回荣誉奖项记录
        /// </summary>
        /// <param name="access_token">授权令牌</param>
        /// <param name="HnrRecordID">荣誉奖项记录ID</param>
        /// <param name="RejectReason">驳回理由</param>
        /// <returns></returns>
        [HttpGet, Route("auditreject")]
        public ApiResult AuditRejectHnrRecord(string access_token, string HnrRecordID, string RejectReason)
        {
            result = AccessToken.Check(access_token, "api/record/auditrejcet");
            if (result == null)
            {
                #region 参数验证
                if (HnrRecordID == null || HnrRecordID == "")
                {
                    return Error("HnrRecordID参数不能为空");
                }
                if (RejectReason == null || RejectReason == "")
                {
                    return Error("RejectReason参数不能为空");
                }
                #endregion

                #region 逻辑操作
                UserInfo userInfo = AccessToken.GetUserInfo(access_token);
                if (userInfo != null)
                {
                    var recordList = from vw_HnrRecord in db.vw_HnrRecord where (vw_HnrRecord.HnrRecID == HnrRecordID) select vw_HnrRecord;
                    if (recordList.Any())
                    {
                        vw_HnrRecord recordModel = recordList.ToList().First();
                        if (userInfo.userRoleID == "4")
                        {
                            return Error("学生账号不能审核记录");
                        }
                        else if (userInfo.userRoleID == "3")
                        {
                            //只能审核本学院的申请记录 审核状态 0 -> 1
                            if (recordModel.State == "0")
                            {
                                if (recordModel.AwardeeOrgID == userInfo.userOrgID)
                                {
                                    try
                                    {
                                        T_ExmRecord exmRecord = db.T_ExmRecord.Find(HnrRecordID);
                                        exmRecord.State = "3";
                                        exmRecord.ExmID = userInfo.userID;
                                        exmRecord.Reason = RejectReason;
                                        exmRecord.ExmTime = DateTime.Now;
                                        db.SaveChanges();
                                        return Success("已驳回");
                                    }
                                    catch
                                    {
                                        return SystemError();
                                    }
                                }
                                else
                                {
                                    return Error("荣誉奖项记录只能由本学院账号");
                                }
                            }
                            else
                            {
                                return Error("此记录已学院审核完成");
                            }
                        }
                        else
                        {
                            //任何记录 审核状态 0,1 -> 2
                            if (recordModel.State == "0" || recordModel.State == "1")
                            {
                                try
                                {
                                    T_ExmRecord exmRecord = db.T_ExmRecord.Find(HnrRecordID);
                                    exmRecord.State = "3";
                                    exmRecord.ExmID = userInfo.userID;
                                    exmRecord.Reason = RejectReason;
                                    exmRecord.ExmTime = DateTime.Now;
                                    db.SaveChanges();
                                    return Success("已驳回");
                                }
                                catch
                                {
                                    return SystemError();
                                }
                            }
                            else
                            {
                                return Error("此记录校团委已审核");
                            }
                        }
                    }
                    else
                    {
                        return Error("此荣誉奖项ID不存在");
                    }
                }
                else
                {
                    //令牌过期 返回
                    return Error();
                }
                #endregion
            }
            return result;
        }

        /// <summary>
        /// 驳回竞赛奖项记录
        /// </summary>
        /// <param name="access_token">授权令牌</param>
        /// <param name="AwdRecordID">竞赛奖项记录ID</param>
        /// <param name="RejectReason">驳回理由</param>
        /// <returns></returns>
        [HttpGet, Route("auditreject")]
        public ApiResult AuditRejectAwdRecord(string access_token, string AwdRecordID, string RejectReason)
        {
            result = AccessToken.Check(access_token, "api/record/auditpass");
            if (result == null)
            {
                #region 参数验证
                if (AwdRecordID == null || AwdRecordID == "")
                {
                    return Error("HnrRecordID参数不能为空");
                }
                if (RejectReason == null || RejectReason == "")
                {
                    return Error("RejectReason参数不能为空");
                }
                #endregion

                #region 逻辑操作
                UserInfo userInfo = AccessToken.GetUserInfo(access_token);
                if (userInfo != null)
                {
                    var recordList = from vw_AwdRecord_Rec in db.vw_AwdRecord_Rec where (vw_AwdRecord_Rec.AwdRecID == AwdRecordID) select vw_AwdRecord_Rec;
                    if (recordList.Any())
                    {
                        vw_AwdRecord_Rec recordModel = recordList.ToList().First();
                        if (userInfo.userRoleID == "4")
                        {
                            return Error("学生账号不能审核记录");
                        }
                        else if (userInfo.userRoleID == "3")
                        {
                            //只能审核本学院的申请记录 审核状态 0 -> 1
                            if (recordModel.State == "0")
                            {
                                if (recordModel.OrgID == userInfo.userOrgID)
                                {
                                    try
                                    {
                                        T_ExmRecord exmRecord = db.T_ExmRecord.Find(AwdRecordID);
                                        exmRecord.State = "3";
                                        exmRecord.ExmID = userInfo.userID;
                                        exmRecord.Reason = RejectReason;
                                        exmRecord.ExmTime = DateTime.Now;
                                        db.SaveChanges();
                                        return Success("已驳回");
                                    }
                                    catch
                                    {
                                        return SystemError();
                                    }
                                }
                                else
                                {
                                    return Error("荣誉奖项记录只能由本学院账号");
                                }
                            }
                            else
                            {
                                return Error("此记录已学院审核完成");
                            }
                        }
                        else
                        {
                            //任何记录 审核状态 0,1 -> 2
                            if (recordModel.State == "0" || recordModel.State == "1")
                            {
                                try
                                {
                                    T_ExmRecord exmRecord = db.T_ExmRecord.Find(AwdRecordID);
                                    exmRecord.State = "3";
                                    exmRecord.ExmID = userInfo.userID;
                                    exmRecord.Reason = RejectReason;
                                    exmRecord.ExmTime = DateTime.Now;
                                    db.SaveChanges();
                                    return Success("已驳回");
                                }
                                catch
                                {
                                    return SystemError();
                                }
                            }
                            else
                            {
                                return Error("此记录校团委已审核");
                            }
                        }
                    }
                    else
                    {
                        return Error("此荣誉奖项ID不存在");
                    }
                }
                else
                {
                    //令牌过期 返回
                    return Error();
                }
                #endregion
            }
            return result;
        }
        #endregion

        #region 数据库搜索操作
        private ApiResult GetData(string access_token, string type, int page, int limit, string sortDirection, string sortField)
        {
            #region
            UserInfo userInfo = AccessToken.GetUserInfo(access_token);

            if (userInfo == null)
            {
                return Error();
            }

            List<vw_HnrRecord> hnrRecordList = new List<vw_HnrRecord>();
            List<vw_AwdRecord_Rec> awdRecordList = new List<vw_AwdRecord_Rec>();

            var hnrRecord = from vw_HnrRecord in db.vw_HnrRecord where (vw_HnrRecord.State != "-1" && vw_HnrRecord.State != "-2" && vw_HnrRecord.State != "-3") orderby vw_HnrRecord.ApplyTime descending select vw_HnrRecord;
            var awdRecord = from vw_AwdRecord_Rec in db.vw_AwdRecord_Rec where (vw_AwdRecord_Rec.State != "-1" && vw_AwdRecord_Rec.State != "-2" && vw_AwdRecord_Rec.State != "-3") orderby vw_AwdRecord_Rec.ApplyTime descending select vw_AwdRecord_Rec;
            #endregion

            if (userInfo.userRoleID == "4")
            {
                //学生账号只能看到自己的账号记录
                //此块学生模块暂时未做
                return Error("对不起，当前系统不支持学生查询记录");
            }
            else if (userInfo.userRoleID == "3")
            {
                //学院账号可以看到所属学院的账号记录账号记录
                hnrRecord = from vw_HnrRecord in db.vw_HnrRecord where (vw_HnrRecord.AwardeeOrgID == userInfo.userOrgID && vw_HnrRecord.State != "-1" && vw_HnrRecord.State != "-2" && vw_HnrRecord.State != "-3") orderby vw_HnrRecord.ApplyTime descending select vw_HnrRecord;
                awdRecord = from vw_AwdRecord_Rec in db.vw_AwdRecord_Rec where (vw_AwdRecord_Rec.OrgID == userInfo.userOrgID && vw_AwdRecord_Rec.State != "-1" && vw_AwdRecord_Rec.State != "-2" && vw_AwdRecord_Rec.State != "-3") orderby vw_AwdRecord_Rec.ApplyTime descending select vw_AwdRecord_Rec;
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

            hnrRecordList = GetList(hnrRecord, page, limit, sortDirection, sortField);
            awdRecordList = GetList(awdRecord, page, limit, sortDirection, sortField);

            #region 数据处理
            RecordList data = new RecordList();
            data.hnrListNum = hnrRecord.Count();
            data.awdListNum = awdRecord.Count();
            data.hnrList = new List<returnHnrRecord>();
            data.awdList = new List<returnAwdRecord>();

            foreach (vw_HnrRecord item in hnrRecordList)
            {
                returnHnrRecord model = new returnHnrRecord();
                model.HnrRecordID = item.HnrRecID;
                model.AwdeeID = item.AwardeeID;
                model.AwdeeName = item.AwardeeName;
                model.AwdeeOrgID = item.AwardeeOrgID;
                model.AwdeeOrgName = item.AwardeeOrgName;
                model.AwdeeBranch = item.AwardeeBranch;
                model.HnrName = item.HnrName;
                model.GradeName = item.HnrGradeName;
                model.HnrAnnual = item.HnrAnnual;
                model.HnrTime = item.HnrTime;
                model.ApplyAccountName = item.ApplyAccountName;
                model.ApplyAccountOrg = item.ApplyAccountOrgName;
                model.ApplyAccountRole = item.ApplyAccountRoleName;
                model.ApplyTime = item.ApplyTime;
                model.RejectReason = item.Reason;
                model.FileUrl = item.FileUrl;
                model.State = item.State;

                data.hnrList.Add(model);
            }

            foreach (vw_AwdRecord_Rec item in awdRecordList)
            {
                returnAwdRecord model = new returnAwdRecord();
                model.AwdRecordID = item.AwdRecID;
                model.AwdeeName = item.TeamAwdeeName;
                model.AwdeeOrgID = item.TeamAwdeeOrgID;
                model.AwdeeOrgName = item.TeamAwdeeOrgName;
                model.AwdeeBranch = item.TeamAwdeeBranch;
                model.AwdID = item.AwdID;
                model.AwdName = item.AwdName;
                model.AwdOrgID = item.OrgID;
                model.AwdOrgName = item.OrgName;
                model.AwdProName = item.ProName;
                model.Grade = item.AwdGrade;
                model.GradeName = item.AwdGradeName;
                model.AwdYear = item.Year;
                model.AwdTerm = item.Term;
                model.AwdTime = item.Time;
                model.IsTeam = item.IsTeam;
                model.Teacher = item.Teacher;
                model.ApplyAccountName = item.ApplyAccountName;
                model.ApplyAccountOrg = item.ApplyAccountOrgName;
                model.ApplyAccountRole = item.ApplyAccountRoleName;
                model.ApplyTime = item.ApplyTime;
                model.RejectReason = item.Reason;
                model.FileUrl = item.FileUrl;
                model.State = item.State;

                data.awdList.Add(model);
            }
            if (type == "1")
            {
                data.awdList = null;
                data.awdListNum = 0;
            }
            else if (type == "2")
            {
                data.hnrList = null;
                data.hnrListNum = 0;
            }
            else
            {
                //type == 0  不需要任何操作
            }
            #endregion

            return Success("获取数据成功", data);
        }

        private ApiResult GetData(SelectCondition conditionModel)
        {
            List<ConditionModel> HnrRecordSearchConditions = new List<ConditionModel>();
            List<ConditionModel> AwdRecordSearchConditions = new List<ConditionModel>();

            #region 查询条件处理
            if (conditionModel.type.ToString().Trim() == "0")
            {
                foreach (var item in conditionModel.conditions)
                {
                    if (item.fieldName == "RecordID")
                    {
                        HnrRecordSearchConditions.Add(new ConditionModel("HnrRecID", item.fieldValues));
                        AwdRecordSearchConditions.Add(new ConditionModel("AwdRecID", item.fieldValues));
                    }
                    else if (item.fieldName == "Time")
                    {
                        HnrRecordSearchConditions.Add(new ConditionModel("HnrTime", item.fieldValues));
                        AwdRecordSearchConditions.Add(new ConditionModel("Time", item.fieldValues));
                    }
                    else if (item.fieldName == "OrgID")
                    {
                        HnrRecordSearchConditions.Add(new ConditionModel("AwardeeOrgID", item.fieldValues));
                        AwdRecordSearchConditions.Add(new ConditionModel("OrgID", item.fieldValues));
                    }
                    else if (item.fieldName == "OrgName")
                    {
                        HnrRecordSearchConditions.Add(new ConditionModel("AwardeeOrgName", item.fieldValues));
                        AwdRecordSearchConditions.Add(new ConditionModel("OrgName", item.fieldValues));
                    }
                    else if (item.fieldName == "AwdeeName")
                    {
                        HnrRecordSearchConditions.Add(new ConditionModel("AwardeeName", item.fieldValues));
                        AwdRecordSearchConditions.Add(new ConditionModel("TeamMembers", item.fieldValues));
                    }
                    else if (item.fieldName == "AwdeeOrgName")
                    {
                        HnrRecordSearchConditions.Add(new ConditionModel("AwardeeOrgName", item.fieldValues));
                        AwdRecordSearchConditions.Add(new ConditionModel("TeamMembersOrgName", item.fieldValues));
                    }
                    else if (item.fieldName == "State")
                    {
                        HnrRecordSearchConditions.Add(new ConditionModel("State", item.fieldValues));
                        AwdRecordSearchConditions.Add(new ConditionModel("State", item.fieldValues));
                    }
                    else
                    {
                        return Error("参数type值为0时，可搜索字段为：RecordID、Time、OrgID、OrgName、AwdeeName、AwdeeOrgName、State");
                    }
                }

                bool _stateFlag = false;
                foreach (ConditionModel conditionItem in HnrRecordSearchConditions)
                {
                    if (conditionItem.fieldName == "State")
                    {
                        _stateFlag = true;
                    }
                }
                if (!_stateFlag)
                {
                    ConditionModel _condition = new ConditionModel();
                    _condition.fieldValues = new List<FieldValue>();
                    _condition.fieldName = "State";
                    FieldValue item_01 = new FieldValue();
                    item_01.item = "0";
                    FieldValue item_02 = new FieldValue();
                    item_02.item = "1";
                    FieldValue item_03 = new FieldValue();
                    item_03.item = "2";
                    FieldValue item_04 = new FieldValue();
                    item_04.item = "3";
                    _condition.fieldValues.Add(item_01);
                    _condition.fieldValues.Add(item_02);
                    _condition.fieldValues.Add(item_03);
                    _condition.fieldValues.Add(item_04);

                    HnrRecordSearchConditions.Add(_condition);
                    _stateFlag = false;
                }
                foreach (ConditionModel conditionItem in AwdRecordSearchConditions)
                {
                    if (conditionItem.fieldName == "State")
                    {
                        _stateFlag = true;
                    }
                }
                if (!_stateFlag)
                {
                    ConditionModel _condition = new ConditionModel();
                    _condition.fieldValues = new List<FieldValue>();
                    _condition.fieldName = "State";
                    FieldValue item_01 = new FieldValue();
                    item_01.item = "0";
                    FieldValue item_02 = new FieldValue();
                    item_02.item = "1";
                    FieldValue item_03 = new FieldValue();
                    item_03.item = "2";
                    FieldValue item_04 = new FieldValue();
                    item_04.item = "3";
                    _condition.fieldValues.Add(item_01);
                    _condition.fieldValues.Add(item_02);
                    _condition.fieldValues.Add(item_03);
                    _condition.fieldValues.Add(item_04);

                    AwdRecordSearchConditions.Add(_condition);
                    _stateFlag = false;
                }
            }
            else
            {
                bool stateFlag = false;
                foreach (ConditionModel conditionItem in conditionModel.conditions)
                {
                    if (conditionItem.fieldName == "State")
                    {
                        stateFlag = true;
                    }
                }
                if (!stateFlag)
                {
                    ConditionModel _condition = new ConditionModel();
                    _condition.fieldValues = new List<FieldValue>();
                    _condition.fieldName = "State";
                    FieldValue item_01 = new FieldValue();
                    item_01.item = "0";
                    FieldValue item_02 = new FieldValue();
                    item_02.item = "1";
                    FieldValue item_03 = new FieldValue();
                    item_03.item = "2";
                    FieldValue item_04 = new FieldValue();
                    item_04.item = "3";
                    _condition.fieldValues.Add(item_01);
                    _condition.fieldValues.Add(item_02);
                    _condition.fieldValues.Add(item_03);
                    _condition.fieldValues.Add(item_04);

                    conditionModel.conditions.Add(_condition);
                }
            }
            #endregion

            #region 执行查询条件
            IQueryable<vw_HnrRecord> hnrRecordList = db.vw_HnrRecord;
            IQueryable<vw_AwdRecord_Rec> awdRecordList = db.vw_AwdRecord_Rec;
            if (conditionModel.type.ToString().Trim() == "0")
            {
                foreach (var item in HnrRecordSearchConditions)
                {
                    hnrRecordList = hnrRecordList.Where(ConditionExpressions.GetConditionExpression<vw_HnrRecord>(item));

                }
                foreach (var item in AwdRecordSearchConditions)
                {
                    awdRecordList = awdRecordList.Where(ConditionExpressions.GetConditionExpression<vw_AwdRecord_Rec>(item));
                }
            }
            else
            {
                foreach (var item in conditionModel.conditions)
                {
                    hnrRecordList = hnrRecordList.Where(ConditionExpressions.GetConditionExpression<vw_HnrRecord>(item));
                    awdRecordList = awdRecordList.Where(ConditionExpressions.GetConditionExpression<vw_AwdRecord_Rec>(item));
                }
            }
            #endregion

            #region 角色权限控制（存在不能进行角色权限控制问题）
            //if (userInfo.userRoleID == "4")
            //{
            //    //学生账号只能看到自己的账号记录
            //    //此块学生模块暂时未做
            //    return Error("对不起，当前系统不支持学生查询记录");
            //}
            //else if (userInfo.userRoleID == "3")
            //{
            //    //学院账号可以看到所属学院的账号记录账号记录
            //    #region 添加搜索记录所属学院条件
            //    foreach ()
            //    {

            //    }
            //    #endregion
            //}
            //else if (userInfo.userRoleID == "2" || userInfo.userRoleID == "1")
            //{
            //    //校团委助理账号及校团委老师账号可以看到所有记录
            //}
            //else
            //{
            //    //没有角色ID信息
            //    return Error("发生未知错误。请联系系统维护人员");
            //}
            #endregion

            var _hnrRecordList = GetList(hnrRecordList, conditionModel.page, conditionModel.limit, conditionModel.sortDirection, conditionModel.sortField);
            var _awdRecordList = GetList(awdRecordList, conditionModel.page, conditionModel.limit, conditionModel.sortDirection, conditionModel.sortField);

            #region 数据处理
            RecordList data = new RecordList();
            data.hnrListNum = _hnrRecordList.Count();
            data.awdListNum = _awdRecordList.Count();
            data.hnrList = new List<returnHnrRecord>();
            data.awdList = new List<returnAwdRecord>();

            foreach (vw_HnrRecord item in hnrRecordList)
            {
                returnHnrRecord model = new returnHnrRecord();
                model.HnrRecordID = item.HnrRecID;
                model.AwdeeID = item.AwardeeID;
                model.AwdeeName = item.AwardeeName;
                model.AwdeeOrgID = item.AwardeeOrgID;
                model.AwdeeOrgName = item.AwardeeOrgName;
                model.AwdeeBranch = item.AwardeeBranch;
                model.HnrName = item.HnrName;
                model.GradeName = item.HnrGradeName;
                model.HnrAnnual = item.HnrAnnual;
                model.HnrTime = item.HnrTime;
                model.ApplyAccountName = item.ApplyAccountName;
                model.ApplyAccountOrg = item.ApplyAccountOrgName;
                model.ApplyAccountRole = item.ApplyAccountRoleName;
                model.ApplyTime = item.ApplyTime;
                model.RejectReason = item.Reason;
                model.FileUrl = item.FileUrl;
                model.State = item.State;

                data.hnrList.Add(model);
            }

            foreach (vw_AwdRecord_Rec item in awdRecordList)
            {
                returnAwdRecord model = new returnAwdRecord();
                model.AwdRecordID = item.AwdRecID;
                model.AwdeeName = item.TeamAwdeeName;
                model.AwdeeOrgID = item.TeamAwdeeOrgID;
                model.AwdeeOrgName = item.TeamAwdeeOrgName;
                model.AwdeeBranch = item.TeamAwdeeBranch;
                model.AwdID = item.AwdID;
                model.AwdName = item.AwdName;
                model.AwdOrgID = item.OrgID;
                model.AwdOrgName = item.OrgName;
                model.AwdProName = item.ProName;
                model.Grade = item.AwdGrade;
                model.GradeName = item.AwdGradeName;
                model.AwdYear = item.Year;
                model.AwdTerm = item.Term;
                model.AwdTime = item.Time;
                model.IsTeam = item.IsTeam;
                model.Teacher = item.Teacher;
                model.TeamMembersName = item.TeamMembers;
                model.TeamMembersOrgName = item.TeamMembersOrgName;
                model.ApplyAccountName = item.ApplyAccountName;
                model.ApplyAccountOrg = item.ApplyAccountOrgName;
                model.ApplyAccountRole = item.ApplyAccountRoleName;
                model.ApplyTime = item.ApplyTime;
                model.RejectReason = item.Reason;
                model.FileUrl = item.FileUrl;
                model.State = item.State;

                data.awdList.Add(model);
            }
            if (conditionModel.type == "1")
            {
                data.awdList = null;
                data.awdListNum = 0;
            }
            else if (conditionModel.type == "2")
            {
                data.hnrList = null;
                data.hnrListNum = 0;
            }
            else
            {
                //conditionModle.type == 0  不需要任何操作
            }
            #endregion

            return Success("获取数据成功", data);
        }
        #endregion
    }
}