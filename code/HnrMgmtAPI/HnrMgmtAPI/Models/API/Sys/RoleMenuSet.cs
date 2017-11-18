using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HnrMgmtAPI.Models.API.Sys
{
    public class RoleMenuSet
    {
        public string access_token { get; set; }

        public string RoleID { get; set; }

        public List<string> MenuList { get; set; }
    }
}