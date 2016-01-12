using Utilities.DataTypes.ExtensionMethods;
using Utilities.Web.ExtensionMethods;

namespace BreezeShop.Core.UserTrace
{
    public class MallBrowseTrace : BaseBrowseTrace
    {

        protected override string CreateGuid()
        {
            var guid = CookieHelper.GetCookie("anonymousid");
            if (guid.IsNullOrEmpty())
            {
                guid = System.Guid.NewGuid().ToString();
                CookieHelper.WriteCookie("anonymousid", guid, 525600);
            }

            return guid;
        }

        public static string GetUserGuid()
        {
            return CookieHelper.GetCookie("anonymousid");
        }

        public static string GetSourceOfPurchase()
        {
            return CookieHelper.GetCookie("referer");
        }

        public static bool IsMobile
        {
            get
            {
                var isMobile =
                    System.Web.HttpContext.Current.Request.IsMobile();
                var enforce = CookieHelper.GetCookie("notmobile").TryTo(0);

                return isMobile && enforce == 0;
            }
        }

        public static void SetPcBrowse()
        {
            CookieHelper.WriteCookie("notmobile", "1");
        }

        /// <summary>
        /// 记录用户的购买来源
        /// </summary>
        public void SourceOfPurchase()
        {
            var url = System.Web.HttpContext.Current.Request.Url.ToString();

            if (string.IsNullOrEmpty(CookieHelper.GetCookie("referer")))
            {
                CookieHelper.WriteCookie("referer",
                    System.Web.HttpContext.Current.Request.UrlReferrer == null
                        ? url
                        : System.Web.HttpContext.Current.Request.UrlReferrer.ToString());
            }
        }

    }
}
