using System.Web;
using Senparc.Weixin.MP.CommonAPIs;
using Senparc.Weixin.MP.Helpers;

namespace BreezeShop.Web.Helper
{
    public class WxJsSdk
    {
        public static System.Collections.Hashtable GetData(string appid, string secret)
        {
            if (string.IsNullOrEmpty(appid) || string.IsNullOrEmpty(secret)) return null;

            var ticket = AccessTokenContainer.TryGetJsApiTicket(appid, secret);
            //获取时间戳
            var timestamp = JSSDKHelper.GetTimestamp();
            //获取随机码
            var nonceStr = JSSDKHelper.GetNoncestr();

            return new System.Collections.Hashtable
            {
                {"appId", appid},
                {"nonceStr", nonceStr},
                {"timestamp", timestamp},
                {
                    "signature",
                    JSSDKHelper.GetSignature(ticket, nonceStr, timestamp,
                        HttpContext.Current.Request.Url.AbsoluteUri)
                }
            };
        }
    }
}