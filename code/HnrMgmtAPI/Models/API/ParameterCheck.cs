using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web;

namespace HnrMgmtAPI.Models.API
{
    public class ParameterCheck
    {
        /// <summary>
        /// 检查参数是否为空
        /// </summary>
        /// <param name="modelObj"></param>
        /// <returns></returns>
        public static ApiResult CheckParameters(Object modelObj)
        {
            bool flag = false;

            ApiResult result = new ApiResult();

            PropertyInfo[] propertys = modelObj.GetType().GetProperties();

            Dictionary<string, string> errorFields = new Dictionary<string, string>();

            foreach (PropertyInfo p in propertys)
            {
                string name = p.Name;
                if (p.GetValue(modelObj, null) == null)
                {
                    errorFields.Add(name, "缺少参数");
                    flag = true;
                }
            }
            if (flag)
            {
                result.status = "error";
                result.messages = "参数格式错误";
                result.fieldErrors = errorFields;

                return result;
            }
            return null;
        }
    }
}