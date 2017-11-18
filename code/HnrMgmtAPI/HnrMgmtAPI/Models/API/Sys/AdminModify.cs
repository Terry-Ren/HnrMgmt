using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HnrMgmtAPI.Models.API.Sys
{
    public class AdminModify
    {
        public string access_token { get; set; }

        public string AccountID { get; set; }

        public string AccountName { get; set; }

        public string OrgID { get; set; }

        public string Tel { get; set; }
    }
}