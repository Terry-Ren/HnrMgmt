using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HnrMgmtAPI.Models.API.JC
{
    public class Return_GetHonor
    {
        public List<T_Honor> list { get; set; }

        public int count { get; set; }
    }
}