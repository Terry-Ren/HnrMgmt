using System.Collections.Generic;

namespace HnrMgmtAPI.Models.API.Record
{
    public class SelectCondition
    {
        //授权令牌
        public string access_token { get; set; }

        //记录类型 0代表 同时查询荣誉信息记录和竞赛获奖记录 1代表查询荣誉记录 2代表查询竞赛获奖记录
        public string type { get; set; }

        //页码
        public int page { get; set; }

        //每页条数
        public int limit { get; set; }

        //排序方式 ASC 或 DESC
        public string sortDirection { get; set; }

        //排序字段
        public string sortField { get; set; }

        //查询条件集合
        public List<ConditionModel> conditions { get; set; }
    }

    //字段 查询条件
    public class ConditionModel
    {
        public string fieldName { get; set; }
        public List<FieldValue> fieldValues { get; set; }
    }

    //字段 查询条件值
    public class FieldValue
    {
        public string item { get; set; }
    }
}