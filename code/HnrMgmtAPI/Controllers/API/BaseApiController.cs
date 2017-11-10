using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using HnrMgmtAPI.Models;
using HnrMgmtAPI.Models.API;
using System.Linq.Expressions;

namespace HnrMgmtAPI.Controllers.API
{
    public class BaseApiController : ApiController
    {
        /// <summary>
        /// 构造方法
        /// </summary>
        public BaseApiController()
        {
            //测试状态 沉睡一秒
            if (System.Configuration.ConfigurationManager.AppSettings["IsTest"].ToString() == "1")
            {
                //线程 暂停一秒
                System.Threading.Thread.Sleep(500);
            }
        }

        /// <summary>
        ///  API 返回结果
        /// </summary>
        public ApiResult result;

        /// <summary>
        ///  实例化数据库连接
        /// </summary>
        public Entities db = new Entities();

        /// <summary>
        ///  发生错误 返回错误信息
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        public static ApiResult Error(string message)
        {
            ApiResult result = new ApiResult();
            result.status = "error";
            result.messages = message;
            return result;
        }

        /// <summary>
        ///  发生错误 返回错误信息及错误字段
        /// </summary>
        /// <param name="message"></param>
        /// <param name="fieldErrors"></param>
        /// <returns></returns>
        public static ApiResult Error(string message, Dictionary<string, string> fieldErrors)
        {
            ApiResult result = new ApiResult();
            result.status = "error";
            result.messages = message;
            result.fieldErrors = fieldErrors;
            return result;
        }

        /// <summary>
        ///  操作成功 返回信息
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        public static ApiResult Success(string message)
        {
            ApiResult result = new ApiResult();
            result.status = "success";
            result.messages = message;
            return result;
        }

        /// <summary>
        ///  操作成功 返回信息及数据
        /// </summary>
        /// <param name="message"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        public static ApiResult Success(string message, object data)
        {
            ApiResult result = new ApiResult();
            result.status = "success";
            result.messages = message;
            result.data = data;
            return result;
        }

        /// <summary>
        /// 排序 分页
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="returnList"></param>
        /// <param name="page"></param>
        /// <param name="limit"></param>
        /// <param name="sortDirection"></param>
        /// <param name="sortField"></param>
        /// <returns></returns>
        public static List<T> GetList<T>(IQueryable<T> returnList, int page, int limit, string sortDirection, string sortField)
        {
            if (sortDirection != null && sortField != null && (sortDirection == "ASC" || sortDirection == "DESC"))
            {
                if (typeof(T).GetProperty(sortField) != null)
                {
                    #region 动态排序

                    var property = typeof(T).GetProperty(sortField);
                    var parameter = Expression.Parameter(typeof(T), "p");
                    var propertyAccess = Expression.MakeMemberAccess(parameter, property);
                    var orderByExp = Expression.Lambda(propertyAccess, parameter);

                    if (sortDirection == "ASC")//正序
                    {
                        MethodCallExpression resultExp = Expression.Call(typeof(Queryable), "OrderBy", new Type[] { typeof(T), property.PropertyType }, returnList.Expression, Expression.Quote(orderByExp));
                        returnList = (IOrderedQueryable<T>)returnList.Provider.CreateQuery<T>(resultExp);
                    }
                    if (sortDirection == "DESC")//倒序
                    {
                        MethodCallExpression resultExp = Expression.Call(typeof(Queryable), "OrderByDescending", new Type[] { typeof(T), property.PropertyType }, returnList.Expression, Expression.Quote(orderByExp));
                        returnList = (IOrderedQueryable<T>)returnList.Provider.CreateQuery<T>(resultExp);
                    }

                    #endregion
                }
            }

            if (page > 0 && limit > 0)
            {
                return returnList.Skip((page - 1) * limit).Take(limit).ToList();
            }
            else
            {
                return returnList.ToList();
            }
        }

        public static string Change(string type,string parm)
        {
            string value = "Error";

            if(type == "HnrGradeName" || type == "AwdOrgName")
            {
                if(parm == "0")
                {
                    value = "院级";
                }
                if(parm == "1")
                {
                    value = "校级";
                }
                if(parm == "2")
                {
                    value = "省级";
                }
                if(parm == "3")
                {
                    value = "国家级";
                }
            }
            if(type == "AwdGrade")
            {
                if(parm == "0")
                {
                    value = "三等奖（铜奖）";
                }
                if(parm == "1")
                {
                    value = "二等奖（银奖）";
                }
                if(parm == "2")
                {
                    value = "一等奖（金奖）";
                }
                if(parm == "3")
                {
                    value = "特等奖";
                }
            }

            if(type == "State")
            {
                if(parm =="0")
                {
                    value = "待审核";
                }
                if(parm =="0")
                {

                }
                if(parm =="0")
                {

                }
            }

            return value;
        }
    }
}