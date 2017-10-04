using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HnrMgmtAPI.Models.API.JC
{
    public class Return_GetList<T>
    {
        public List<T> list { get; set; }

        public int count { get; set; }
    }
}