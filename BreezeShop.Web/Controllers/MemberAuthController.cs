using System.Web;
using System.Web.Mvc;
using System.Web.WebPages;
using BreezeShop.Core;
using BreezeShop.Core.DataProvider;
using Senparc.Weixin.MP;
using Senparc.Weixin.MP.AdvancedAPIs;

namespace BreezeShop.Web.Controllers
{
    public class MemberAuthController : WxAuthController
    {
        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            base.OnActionExecuting(filterContext);

            //判断用户是否已经登录
            if (Member.Token.IsEmpty() || Member.OpenId.IsEmpty())
            {
                //Member.Token = "TUR1MGk0MnpIQjIwQUN2MC81Z1QyUHdrOEhyWjhhU3FPNG1udEo2MFpVeWZmR1l0RHc9";
                //Member.OpenId = "odsEZxOLSFbcFXVBi5BKbI-s1U9A";
                //return;

                //进入微信授权，快速注册
                var oauth2Url = OAuthApi.GetAuthorizeUrl(appId, YunClient.WebUrl+"oauth2/userinfocallback",
                    HttpUtility.UrlEncode(Request.Url.ToString()), OAuthScope.snsapi_userinfo);
                filterContext.Result = new RedirectResult(oauth2Url);
            }
        }

    }
}
