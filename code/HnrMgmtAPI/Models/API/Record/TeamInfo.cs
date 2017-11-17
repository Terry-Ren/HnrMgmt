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
        public string MemberID { get; set; }
        public string MemberName { get; set; }
        public string MemberOrgName { get; set; }
        public string MemberBranch { get; set; }
        public string Rank { get; set; }
    }
}