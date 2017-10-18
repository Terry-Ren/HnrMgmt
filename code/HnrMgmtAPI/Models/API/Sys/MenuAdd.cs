using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HnrMgmtAPI.Models.API.Sys
{
    public class MenuAdd
    {
        public string access_token { get; set; }

        public string MenuName { get; set; }

        public string MenuUrl { get; set; }
    }
}