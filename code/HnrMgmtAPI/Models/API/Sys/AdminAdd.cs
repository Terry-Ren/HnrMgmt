using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HnrMgmtAPI.Models.API.Sys
{
    public class AdminAdd
    {
        public string access_token { get; set; }

        public string AccountID { get; set; }

        public string Name { get; set; }

        public string OrgID { get; set; }

        public string Tel { get; set; }

        //值 只能为2或者3
        public string RoleID { get; set; }
    }
}