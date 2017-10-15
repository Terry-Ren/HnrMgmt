namespace HnrMgmtAPI.Models.API.Auth
{
    public class AccountInfo
    {
        public string access_token { get; set; }
        public string ID { get; set; }
        public string Name { get; set; }
        public string OrgName { get; set; }
        public string RoleID { get; set; }
        public string RoleName { get; set; }
        public string Tel { get; set; }

        //账号状态 0代表冻结状态  1代表使用状态
        public string State { get; set; }
    }
}