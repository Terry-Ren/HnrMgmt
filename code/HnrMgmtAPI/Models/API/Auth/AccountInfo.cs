using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HnrMgmtAPI.Models.API.Auth
{
    public class AccountInfo
    {
        public string access_token { get; set; }
        public string id { get; set; }
        public string name { get; set; }
        public string orgName { get; set; }
        public string roleID { get; set; }
        public string roleName { get; set; }
        public string tel { get; set; }
        public string state { get; set; }
    }
}