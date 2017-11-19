using System;
using System.Collections.Generic;

namespace HnrMgmtAPI.Models.API.Record
{
    public class RecordList
    {
        //荣誉记录总数
        public int hnrListNum { get; set; }

        //获奖记录总数
        public int awdListNum { get; set; }

        //荣誉记录列表
        public List<returnHnrRecord> hnrList { get; set; }

        //奖项记录列表
        public List<returnAwdRecord> awdList { get; set; }
    }

    public class returnHnrRecord
    {
        public string HnrRecordID { get; set; }
        public string AwdeeName { get; set; }
        public string AwdeeOrgName { get; set; }
        public string AwdeeBranch { get; set; }
        public string HnrName { get; set; }
        public string GradeName { get; set; }
        public string HnrAnnual { get; set; }
        public string HnrTime { get; set; }
        public string ApplyAccountName { get; set; }
        public string ApplyAccountOrg { get; set; }
        public string ApplyAccountRole { get; set; }
        public System.DateTime ApplyTime { get; set; }
        public string RejectReason { get; set; }
        public string FileUrl { get; set; }
        public string State { get; set; }
    }

    public class returnAwdRecord
    {
        public string AwdRecordID { get; set; }
        public string AwdeeName { get; set; }
        public string AwdeeOrgName { get; set; }
        public string AwdeeBranch { get; set; }
        public string AwdName { get; set; }
        public string AwdOrgName { get; set; }
        public string AwdProName { get; set; }
        public string Grade { get; set; }
        public string GradeName { get; set; }
        public string AwdYear { get; set; }
        public string AwdTerm { get; set; }
        public string AwdTime { get; set; }
        public string IsTeam { get; set; }
        public string Teacher { get; set; }
        public string ApplyAccountName { get; set; }
        public string ApplyAccountOrg { get; set; }
        public string AppltAccountRole { get; set; }
        public Nullable<System.DateTime> ApplyTime { get; set; }
        public string RejectReason { get; set; }
        public string FileUrl { get; set; }
        public string State { get; set; }
    }
}