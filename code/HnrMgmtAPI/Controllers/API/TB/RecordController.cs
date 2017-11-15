using HnrMgmtAPI.Common;
using HnrMgmtAPI.Models;
using HnrMgmtAPI.Models.API;
using HnrMgmtAPI.Models.API.TB;
using System;
using System.Collections.Generic;
using System.Data.Entity.Validation;
using System.Linq;
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

                #region 信息检查 主要面向学院账号和学生账号
                if (userInfo.userRoleID == "3")
                {
                    if (model.OrgID != userInfo.userOrgID)
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
                awdRecord.Year = model.Year.ToString().Trim();
                awdRecord.Time = model.AwdTime.ToString().Trim();
                awdRecord.Term = (model.Term == null) ? null : model.Term.ToString().Trim();
                awdRecord.IsTeam = model.IsTeam;
                awdRecord.ProName = (model.ProjectName == null) ? null : model.ProjectName;
                awdRecord.FileUrl = "";
                if (model.IsTeam == "0")
                {
                    //非团队获奖
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

                            model.MemberID = item.AwdeeID;
                            model.MemberName = item.AwdeeName;
                            model.MemberOrgName = item.AwdeeOrgName;
                            model.MemberBranch = item.AwdeeBranch;
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
        #endregion

        #region 数据库搜索操作
        private ApiResult GetData(string access_token, string type, int page, int limit, string sortDirection, string sortField)
        {
            #region
            UserInfo userInfo = AccessToken.GetUserInfo(access_token);

            List<vw_HnrRecord> hnrRecordList = new List<vw_HnrRecord>();
            List<vw_AwdRecord_Rec> awdRecordList = new List<vw_AwdRecord_Rec>();

            var hnrRecord = from vw_HnrRecord in db.vw_HnrRecord where (vw_HnrRecord.State != "-1" && vw_HnrRecord.State != "-2") orderby vw_HnrRecord.ApplyTime descending select vw_HnrRecord;
            var awdRecord = from vw_AwdRecord_Rec in db.vw_AwdRecord_Rec where (vw_AwdRecord_Rec.State != "-1" && vw_AwdRecord_Rec.State != "-2") orderby vw_AwdRecord_Rec.ApplyTime descending select vw_AwdRecord_Rec;
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
                hnrRecord = from vw_HnrRecord in db.vw_HnrRecord where (vw_HnrRecord.AwardeeOrgID == userInfo.userOrgID && vw_HnrRecord.State != "-1" && vw_HnrRecord.State != "-2") orderby vw_HnrRecord.ApplyTime descending select vw_HnrRecord;
                awdRecord = from vw_AwdRecord_Rec in db.vw_AwdRecord_Rec where (vw_AwdRecord_Rec.OrgID == userInfo.userOrgID && vw_AwdRecord_Rec.State != "-1" && vw_AwdRecord_Rec.State != "-2") orderby vw_AwdRecord_Rec.ApplyTime descending select vw_AwdRecord_Rec;
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
                model.AwardeeName = item.AwardeeName;
                model.AwardeeOrgName = item.AwardeeOrgName;
                model.AwardeeBranch = item.AwardeeBranch;
                model.HnrName = item.HnrName;
                model.GradeName = item.HnrGradeName;
                model.HnrAnnual = item.HnrAnnual;
                model.HnrTime = item.HnrTime;
                model.ApplyAccountName = item.ApplyAccountName;
                model.ApplyAccountOrg = item.ApplyAccountOrgName;
                model.ApplyAccountRole = item.ApplyAccountRoleName;
                model.ApplyTime = item.ApplyTime;
                model.FileUrl = item.FileUrl;
                model.State = item.State;

                data.hnrList.Add(model);
            }

            foreach (vw_AwdRecord_Rec item in awdRecordList)
            {
                returnAwdRecord model = new returnAwdRecord();
                model.AwdRecordID = item.AwdRecID;
                model.AwardeeName = item.TeamAwdeeName;
                model.AwardeeOrgName = item.TeamAwdeeOrgName;
                model.AwardeeBranch = item.TeamAwdeeBranch;
                model.AwdName = item.AwdName;
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
                model.AppltAccountRole = item.ApplyAccountRoleName;
                model.ApplyTime = item.ApplyTime;
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
                return Error("type字段赋值错误");
            }
            #endregion

            return Success("获取数据成功", data);
        }
        #endregion

        #region 编辑功能

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
                                    return Error("出现未知错误，请联系管理员");
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
                                    return Error("出现未知错误，请联系管理员");
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
                                return Error("出现未知错误，请联系管理员");
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
                        return Error("令牌已过期，请重新登录");
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
                                    return Error("出现未知错误，请联系管理员");
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
                                    return Error("出现未知错误，请联系管理员");
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
                                return Error("出现未知错误，请联系管理员");
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
                        return Error("令牌已过期，请重新登录");
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
                        return Error("出现未知错误，请联系管理员");
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
                        return Error("出现未知错误，请联系管理员");
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
    }
}