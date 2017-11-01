using System;
using System.Collections.Generic;
using System.Reflection;

namespace HnrMgmtAPI.Models.API
{
    public class ParameterCheck
    {
        /// <summary>
        /// 检查参数是否为空
        /// </summary>
        /// <param name="modelObj"></param>
        /// <returns></returns>
        public static ApiResult CheckParameters(object modelObj)
        {
            bool flag = false;

            ApiResult result = new ApiResult();

            PropertyInfo[] propertys = modelObj.GetType().GetProperties();
            string _typeName = modelObj.GetType().ToString();

            //获取类名
            string typeName = _typeName.Substring((_typeName.LastIndexOf(".") + 1), (_typeName.Length - 1 - _typeName.LastIndexOf(".")));
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
        /// 检查参数是否为空（部分参数可以为空）
        /// </summary>
        /// <param name="modelObj"></param>
        /// <param name="nullPropertyList">可为空的参数名数组</param>
        /// <returns></returns>
        public static ApiResult CheckIsNullParameters(object modelObj, List<string> nullPropertyList)
        {
            bool flag = false;

            ApiResult result = new ApiResult();

            PropertyInfo[] propertys = modelObj.GetType().GetProperties();
            string _typeName = modelObj.GetType().ToString();

            //获取类名
            string typeName = _typeName.Substring((_typeName.LastIndexOf(".") + 1), (_typeName.Length - 1 - _typeName.LastIndexOf(".")));
            Dictionary<string, string> errorFields = new Dictionary<string, string>();

            foreach (PropertyInfo p in propertys)
            {
                string name = p.Name;
                if (nullPropertyList.Exists(item => item.ToString() == name))
                {
                    //可为空的数组中 存在此参数名 说明此参数可为空  如果为空则跳过 如果不为空 则验证参数值是否符合要求
                    if (p.GetValue(modelObj, null) == null)
                    {
                        continue;
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
                else
                {
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
        /// <param name="typeName">类名称</param>
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

                case "PasswordModify"://长度大于六位
                    if (paramName == "NewPwd" && value.ToString().Length < 6)
                    {
                        flag = true;
                    }
                    break;

                case "TB_HnrRecord":
                    if (paramName == "HnrMonth" && (Convert.ToInt32(value) < 1 || Convert.ToInt32(value) > 12))
                    {
                        flag = true;
                    }
                    if (paramName == "Annual")
                    {
                        if (value.Length != 9 || value.Substring(4, 1) != "-")
                        {
                            flag = true;
                        }
                    }
                    break;
                case "TB_AwdRecord":
                    if (paramName == "HnrMonth" && (Convert.ToInt32(value) < 1 || Convert.ToInt32(value) > 12))
                    {
                        flag = true;
                    }
                    if (paramName == "IsTeam" && value.ToString() != "0" && value.ToString() != "1")
                    {
                        flag = true;
                    }
                    break;
            }

            return flag;//返回true代表存在错误   不存在错误则返回false
        }
    }
}