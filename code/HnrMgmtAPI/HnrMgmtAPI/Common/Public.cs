using System.Collections.Generic;

namespace HnrMgmtAPI.Common
{
    public class Public
    {
        public static Dictionary<string, string> GetRolePermission()
        {
            Dictionary<string, string> rolePermission = new Dictionary<string, string>();

            rolePermission.Add("11", "22");
            rolePermission.Add("22", "11");

            //此处数据应从数据库读取  在此处把所有接口名称、路径、ID写入缓存


            return rolePermission;
        }
    }
}