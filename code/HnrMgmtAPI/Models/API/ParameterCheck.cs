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
            string typeName = modelObj.GetType().ToString();

            Dictionary<string, string> errorFields = new Dictionary<string, string>();

            foreach (PropertyInfo p in propertys)
            {
                string name = p.Name;
                if (p.GetValue(modelObj, null) == null)
                {
                    errorFields.Add(name, "缺少参数");
                    flag = true;
                }
                else
                {
                    if (CheckFormat(typeName, name, p.GetValue(modelObj, null).ToString()))
                    {
                        errorFields.Add(name, "参数格式错误");
                        flag = true;
                    }
                }
            }
            if (flag)
            {
                result.status = "error";
                result.messages = "参数错误";
                result.fieldErrors = errorFields;

                return result;
            }
            return null;
        }

        /// <summary>
        /// 检查属性参数是否符合格式要求
        /// </summary>
        /// <param name="typeName">模型名称</param>
        /// <param name="paramName">属性名称</param>
        /// <param name="value">属性值</param>
        /// <returns></returns>
        private static bool CheckFormat(string typeName, string paramName, string value)
        {
            bool flag = false;

            switch (typeName)
            {
                case "AwardAdd":

                case "AwardModify":

                    if (paramName == "GrandeName" && value != "0" && value != "1" && value != "2" && value != "3")
                    {
                        flag = true;
                    }
                    if (paramName == "Grade" && value != "0" && value != "1" && value != "2" && value != "3")
                    {
                        flag = true;
                    }
                    break;

                case "HonorAdd":

                case "HonorModify":

                    if (paramName == "GrandeName" && value != "0" && value != "1" && value != "2" && value != "3")
                    {
                        flag = true;
                    }
                    break;

            }

            return flag;//返回true代表存在错误   不存在错误则返回false
        }
    }
}