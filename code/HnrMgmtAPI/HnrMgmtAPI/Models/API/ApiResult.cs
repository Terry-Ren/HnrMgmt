using System.Collections.Generic;

namespace HnrMgmtAPI.Models.API
{
    public class ApiResult
    {
        /// <summary>
        /// 返回结果success或error 表示此次访问状态
        /// </summary>
        public string status { get; set; }

        /// <summary>
        /// 失败原因
        /// </summary>
        public string messages { get; set; }

        /// <summary>
        /// 字段格式错误信息
        /// </summary>
        public Dictionary<string, string> fieldErrors { get; set; }

        /// <summary>
        /// 返回数据
        /// </summary>
        public object data { get; set; }
    }
}