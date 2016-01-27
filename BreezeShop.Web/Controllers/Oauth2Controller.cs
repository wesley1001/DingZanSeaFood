using System;
using System.Web;
using System.Web.Mvc;
using BreezeShop.Core;
using BreezeShop.Core.DataProvider;
using Senparc.Weixin;
using Senparc.Weixin.Exceptions;
using Senparc.Weixin.MP.AdvancedAPIs;
using Senparc.Weixin.MP.AdvancedAPIs.OAuth;
using Yun.User.Request;

namespace BreezeShop.Web.Controllers
{
    public class Oauth2Controller : WxAuthController
    {
        /// <summary>
        /// OAuthScope.snsapi_userinfo方式回调
        /// </summary>
        /// <param name="code"></param>
        /// <param name="state"></param>
        /// <returns></returns>
        public ActionResult UserInfoCallback(string code, string state)
        {
            if (string.IsNullOrEmpty(code))
            {
                return Content("您拒绝了授权！");
            }

            //if (state != "JeffreySu")
            //{
            //    //这里的state其实是会暴露给客户端的，验证能力很弱，这里只是演示一下
            //    //实际上可以存任何想传递的数据，比如用户ID，并且需要结合例如下面的Session["OAuthAccessToken"]进行验证
            //    return Content("验证失败！请从正规途径进入！");
            //}

            OAuthAccessTokenResult result = null;

            //通过，用code换取access_token
            try
            {
                result = OAuthApi.GetAccessToken(appId, secret, code);
            }
            catch (Exception ex)
            {
                return Content(ex.Message);
            }
            if (result.errcode != ReturnCode.请求成功)
            {
                return Content("错误：" + result.errmsg);
            }
            //下面2个数据也可以自己封装成一个类，储存在数据库中（建议结合缓存）
            //如果可以确保安全，可以将access_token存入用户的cookie中，每一个人的access_token是不一样的
            //Session["OAuthAccessTokenStartTime"] = DateTime.Now;
            //Session["OAuthAccessToken"] = result;

            //因为第一步选择的是OAuthScope.snsapi_userinfo，这里可以进一步获取用户详细信息
            try
            {
                var userInfo = OAuthApi.GetUserInfo(result.access_token, result.openid);
                //快捷登陆
                var r = YunClient.Instance.Execute(new LoginOauth2Request
                {
                    Avatar = userInfo.headimgurl,
                    OAuth2Type = "weixin",
                    OAuthId = userInfo.openid,
                    RealName = userInfo.nickname,
                    ShopId = GlobeInfo.InitiatedShopId
                });

                if (r.UserId > 0)
                {
                    Member.Token = r.Token;
                    Member.OpenId = userInfo.openid;
                }

                //return Content(userInfo.Serialize());

                return Redirect(HttpUtility.UrlDecode(state));
            }
            catch (ErrorJsonResultException ex)
            {
                return Content(ex.Message);
            }
        }

    }
}
