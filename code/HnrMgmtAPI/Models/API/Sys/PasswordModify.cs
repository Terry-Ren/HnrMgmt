using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HnrMgmtAPI.Models.API.Sys
{
    public class PasswordModify
    {
        //授权令牌
        public string access_token { get; set; }

        //修改用户ID
        public string ID { get; set; }

        //旧密码
        public string OldPwd { get; set; }

        //新密码 长度大于等于6位
        public string NewPwd { get; set; }
    }
}