using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using HnrMgmtAPI.Controllers.API;
using HnrMgmtAPI.Models.API;
using HnrMgmtAPI.Models.API.TB;
using HnrMgmtAPI.Common;

namespace HnrMgmtAPI.Controllers.API.TB
{
    [RoutePrefix("")]
    public class RecordController : BaseApiController
    {
        #region 荣誉信息填报
        [HttpPost, Route("")]
        public ApiResult HnrRecord([FromBody]TB_HnrRecord hnrRecord)
        {
            result = AccessToken.Check(hnrRecord, "api/account/teacher");
            return null;
        }
        #endregion

        #region 竞赛获奖填报
        [HttpPost, Route("")]
        public ApiResult AwdRecord()
        {
            return null;
        }
        #endregion
    }
}
