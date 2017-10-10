using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HnrMgmtAPI.Models.API.Auth
{
    public class LoginModel
    {
        public string id { get; set; }
        public string pwd { get; set; }
        public string roleID { get; set; }
    }
}