using System.Linq;
using System.Net.NetworkInformation;
using System.Web.Mvc;
using BreezeShop.Core;
using Senparc.Weixin.MP.CommonAPIs;
using Yun.WeiXin.Request;

namespace BreezeShop.Web.Controllers
{
    public class WxAuthController : Controller
    {
        private static ExceptionLog _log = new ExceptionLog(typeof(WxAuthController));

        //下面换成账号对应的信息，也可以放入web.config等地方方便配置和更换
        protected static string appId ;
        protected static string secret;
        protected static object initLock = new object();
        protected static string wxtoken;
        protected static string encodingAESKey;
        protected static string wxkey;

        private void InitAppKey()
        {
            if (new[] {appId, secret}.Any(string.IsNullOrEmpty))
            {
                lock (initLock)
                {
                    if (new[] {appId, secret}.Any(string.IsNullOrEmpty))
                    {
                        var account = YunClient.Instance.Execute(new GetWxAccountsRequest { PageNum = 1, PageSize = 1 }).Accounts[0];
                        appId = account.AppId;
                        secret = account.Secret;
                        wxtoken = account.AccessToken;
                        encodingAESKey = account.Encodingaeskey;

                        AccessTokenContainer.Register(appId, secret);
                    }
                }
            }
        }

        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            InitAppKey();
            base.OnActionExecuting(filterContext);
        }

        protected override void OnException(ExceptionContext filterContext)
        {
            _log.Error(filterContext.Exception.ToString());
            base.OnException(filterContext);
        }
    }
}