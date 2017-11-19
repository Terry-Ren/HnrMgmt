namespace HnrMgmtAPI.Models.API.Record
{
    public class ModifyHnrRecord
    {
        //令牌
        public string access_token { get; set; }

        //记录ID
        public string HnrRecordID { get; set; }

        //荣誉类别ID
        public string HonorID { get; set; }

        //获奖年份 格式：2016-2017
        public string HnrAnnual { get; set; }

        //获奖时间 格式：2017-07
        public string HnrTime { get; set; }

        //获奖人ID
        public string AwdeeID { get; set; }

        //获奖人名字
        public string AwdeeName { get; set; }

        //获奖人 所属部门ID
        public string OrgID { get; set; }

        //获奖人 所属团支部
        public string Branch { get; set; }

        //证明材料 文件名 (若不上传文件，请赋值为 -1)
        public string FileUrl { get; set; }
    }
}