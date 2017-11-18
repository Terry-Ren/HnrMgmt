using Aliyun.Acs.Core;
using Aliyun.Acs.Core.Exceptions;
using Aliyun.Acs.Core.Profile;
using Aliyun.Acs.Sms.Model.V20160927;
using System.Configuration;

namespace qingjia_MVC.Common
{
    public class ShortMessageClass
    {
        //阿里云 消息服务 所在地区
        private static string regionID = "";

        //阿里云 消息服务 AccessKey
        private static string accessKeyID = "";

        //阿里云 消息服务 AccessKeyCode
        private static string secret = "";

        /// <summary>
        /// 发送短信
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public static bool SendShortMessage(MessageModel model)
        {

            #region 短信服务测试代码
            if (ConfigurationManager.AppSettings["ShortMessageService"].ToString().Trim() != "1")
            {
                if (ConfigurationManager.AppSettings["ShortMessageService"].ToString().Trim() == "-1")
                {
                    //测试人员手机号
                    model.Tel = ConfigurationManager.AppSettings["Tel"].ToString().Trim();
                }
                else
                {
                    //非1 非-1 代表关闭服务
                    return false;
                }
            }
            #endregion

            //AccessKey 和 AccessKeyCode
            IClientProfile profile = DefaultProfile.GetProfile(regionID, accessKeyID, secret);
            IAcsClient client = new DefaultAcsClient(profile);
            SingleSendSmsRequest request = new SingleSendSmsRequest();
            try
            {
                //短信签名  【请假系统】
                //request.SignName = "请假系统";
                //if (model.MessageType == "go")
                //{
                //    //请假成功模板
                //    request.TemplateCode = "SMS_107115105";
                //}
                //else if (model.MessageType == "back")
                //{
                //    //销假成功模板
                //    request.TemplateCode = "SMS_27495348";
                //}
                //else if (model.MessageType == "failed")
                //{
                //    //驳回请假模板
                //    request.TemplateCode = "SMS_27620081";
                //}
                //else if (model.MessageType == "FindPsd")
                //{
                //    //短信验证找回密码
                //    request.TemplateCode = "SMS_60140885";
                //}
                //else
                //{
                //    return false;
                //}
                request.RecNum = model.Tel;
                //request.ParamString = "{\"name\":\"" + model.ST_Name + "\",\"lvnum\":\"" + model.LV_Num + "\"}";
                SingleSendSmsResponse httpResponse = client.GetAcsResponse(request);

                //短信发送记录
                SaveSendRecord();

                return true;
            }
            catch (ServerException e)
            {
                return false;
            }
            catch (ClientException e)
            {
                return false;
            }
        }

        /// <summary>
        /// 发送验证码
        /// </summary>
        /// <param name="Tel">号码</param>
        /// <param name="identifying_code">验证码</param>
        /// <returns></returns>
        public static bool SendShortMessage(string Tel, string identifying_code)
        {
            #region 短信服务测试代码
            if (ConfigurationManager.AppSettings["ShortMessageService"].ToString().Trim() != "1")
            {
                if (ConfigurationManager.AppSettings["ShortMessageService"].ToString().Trim() == "-1")
                {
                    //测试人员手机号
                    Tel = ConfigurationManager.AppSettings["Tel"].ToString().Trim();
                }
                else
                {
                    //非1 非-1 代表关闭服务
                    return false;
                }
            }
            #endregion

            return true;
        }

        /// <summary>
        /// 记录短信发送记录
        /// </summary>
        private static void SaveSendRecord()
        {

        }
    }

    /// <summary>
    /// 短信模型
    /// </summary>
    public class MessageModel
    {
        public string Tel { get; set; }
    }
}