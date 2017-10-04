using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HnrMgmtAPI.Models.API.JC
{
    public class HonorModify
    {
        public string access_token { get; set; }

        public string honorID { get; set; }

        public string Name { get; set; }

        public string GradeName { get; set; }
    }
}