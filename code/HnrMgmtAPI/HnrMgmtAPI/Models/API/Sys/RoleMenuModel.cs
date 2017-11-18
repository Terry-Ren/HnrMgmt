using System.Collections.Generic;

namespace HnrMgmtAPI.Models.API.Sys
{
    public class MenuModel
    {
        public string ApiID { get; set; }

        public string ApiName { get; set; }

        public string ApiUrl { get; set; }
    }

    public class RoleMenuModel
    {
        public string RoleName { get; set; }

        public string RoleID { get; set; }

        public int count { get; set; }

        public List<MenuModel> MenuList { get; set; }

        public RoleMenuModel()
        {
            MenuList = new List<MenuModel>();
        }
    }
}