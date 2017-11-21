using System.Collections.Generic;

namespace HnrMgmtAPI.Models.API.Record
{
    public class TeamInfo
    {
        public int Num { get; set; }

        public List<Member> Members { get; set; }
    }

    public class Member
    {
        public string AwdeeID { get; set; }
        public string AwdeeName { get; set; }
        public string OrgID { get; set; }
        public string OrgName { get; set; }
        public string Branch { get; set; }
        public string Rank { get; set; }
    }
}