using System.Collections.Generic;

namespace HnrMgmtAPI.Models.API.Record
{
    public class ModifyAwdRecord
    {
        //令牌
        public string access_token { get; set; }

        //记录ID
        public string AwdRecordID { get; set; }

        //荣誉类别ID
        public string AwardID { get; set; }

        //获奖年份 格式：2016
        public string AwdYear { get; set; }

        //获奖时间 格式：2017-07
        public string AwdTime { get; set; }

        //获奖届数 （可空）
        public string AwdTerm { get; set; }

        //获奖项目名称
        public string AwdProName { get; set; }

        //项目所属部门 （可空）
        public string OrgID { get; set; }

        //项目指导老师 可为多人 （可空）
        public List<string> Teacher { get; set; }

        //是否为团队 （值为 0 或 1）
        public string IsTeam { get; set; }

        //团队成员 (若为团队获奖，则成员人数应大于1，若非团队获奖，则成员人数应为1)  团队成员列表的先后顺序即为在项目中的先后顺序
        public List<Awardee> Members { get; set; }

        //证明材料 文件名 (若不上传文件，请赋值为 -1)
        public string FileUrl { get; set; }
    }
}