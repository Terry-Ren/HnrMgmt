using HnrMgmtAPI.Common;
using HnrMgmtAPI.Models;
using HnrMgmtAPI.Models.API;
using HnrMgmtAPI.Models.API.Sys;
using System.Linq;
using System.Web.Http;

namespace HnrMgmtAPI.Controllers.API.Sys
{

    [RoutePrefix("api/role")]
    public class RoleController : BaseApiController
    {
        #region API接口管理（用户可以自行手动添加）
        /// <summary>
        /// 获取功能列表  返回全部功能列表
        /// </summary>
        /// <param name="access_token"></param>
        /// <returns></returns>
        [HttpGet, Route("getmenu")]
        public ApiResult GetMenu(string access_token)
        {
            result = AccessToken.Check(access_token, "api/role/getmenu");
            if (result == null)
            {
                #region 逻辑操作
                var menuList = from T_Right in db.T_Right orderby T_Right.Priority select T_Right;
                if (menuList.Any())
                {
                    Return_GetList<T_Right> right_list = new Return_GetList<T_Right>();
                    right_list.list = menuList.ToList();
                    right_list.count = menuList.Count();
                    return Success("获取功能列表成功", right_list);
                }
                else
                {
                    return Error("获取失败，功能列表为空");
                }
                #endregion
            }
            return result;
        }

        /// <summary>
        /// 添加功能列表 
        /// API接口功能列表中 0代表系统管理级  1代表普通操作级
        /// 此处允许用户添加的是  普通操作级  [Priority]值均为1
        /// </summary>
        /// <param name="access_token"></param>
        /// <param name="MenuName"></param>
        /// <param name="MenuUrl"></param>
        /// <returns></returns>
        [HttpPost, Route("addmenu")]
        public ApiResult AddMenu([FromBody]MenuAdd model)
        {
            result = AccessToken.Check(model.access_token, "api/role/addmenu");

            if (result == null)
            {
                #region 参数验证
                result = ParameterCheck.CheckParameters(model);
                if (result != null)
                {
                    return result;
                }
                #endregion

                #region 逻辑操作
                var menulist = from T_Right in db.T_Right where (T_Right.Url == model.MenuUrl) select T_Right;
                if (menulist.Any())
                {
                    //已存在
                    return Error("此API功能接口已存在");
                }
                else
                {
                    T_Right rightModel = new T_Right();
                    rightModel.ID = System.Guid.NewGuid().ToString();
                    rightModel.Name = model.MenuName;
                    rightModel.Url = model.MenuUrl;
                    rightModel.Priority = 1;
                    try
                    {
                        db.T_Right.Add(rightModel);

                        return Success("添加成功");
                    }
                    catch
                    {
                        return Error("添加失败");
                    }
                }
                #endregion
            }

            return result;
        }

        [HttpPost, Route("modmenu")]
        public ApiResult ModifyMenu([FromBody]MenuModify model)
        {
            result = AccessToken.Check(model.access_token, "api/role/addmenu");
            if (result == null)
            {
                T_Right rightModel = db.T_Right.Find(model.ID);
                if (rightModel != null)
                {
                    rightModel.Name = model.Name;
                    rightModel.Url = model.Url;

                    try
                    {
                        db.SaveChanges();

                        return Success("修改成功");
                    }
                    catch
                    {
                        return Error("修改失败");
                    }
                }
                else
                {
                    return Error("未找到此记录");
                }
            }
            return result;
        }
        #endregion

        #region 角色管理 （不对用户开放、数据库中设置用户层级、不允许用户修改）
        [HttpGet, Route("getrole")]
        public ApiResult GetRole(string access_token)
        {
            result = AccessToken.Check(access_token, "api/role/getrole");
            if (result == null)
            {
                #region 逻辑操作
                var roleList = from T_Role in db.T_Role orderby T_Role.RoleID select T_Role;
                if (roleList.Any())
                {
                    Return_GetList<T_Role> role_list = new Return_GetList<T_Role>();
                    role_list.list = roleList.ToList();
                    role_list.count = roleList.Count();
                    return Success("获取角色列表成功", role_list);
                }
                else
                {
                    return Error("获取失败，角色列表为空");
                }
                #endregion
            }
            return result;
        }
        #endregion

        #region 角色设定功能管理（用户可根据需要为某种角色设定可执行的操作）
        #endregion
    }
}
