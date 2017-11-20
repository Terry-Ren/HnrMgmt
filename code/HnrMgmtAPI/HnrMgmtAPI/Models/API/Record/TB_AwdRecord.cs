using System.Collections.Generic;

namespace HnrMgmtAPI.Models.API.Record
{
    public class TB_AwdRecord
    {
        //令牌
        public string access_token { get; set; }

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
        public List<TeacherInfo> Teacher { get; set; }

        //是否为团队 （值为 0 或 1）
        public string IsTeam { get; set; }

        //团队成员 (若为团队获奖，则成员人数应大于1，若非团队获奖，则成员人数应为1)  团队成员列表的先后顺序即为在项目中的先后顺序
        public List<Awardee> Members { get; set; }

        //证明材料 文件名 (若不上传文件，请赋值为 -1)
        public string FileUrl { get; set; }
    }

    public class Awardee
    {
        //获奖人ID
        public string AwdeeID { get; set; }

        //获奖人名字
        public string AwdeeName { get; set; }

        //获奖人 所属部门ID
        public string OrgID { get; set; }

        //获奖人 所属团支部
        public string Branch { get; set; }
    }

    public class TeacherInfo
    {
        //教师姓名
        public string TchName { get; set; }
    }
}