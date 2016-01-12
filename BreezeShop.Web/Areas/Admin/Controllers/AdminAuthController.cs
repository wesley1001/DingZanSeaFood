using System.Web;
using System.Web.Mvc;
using System.Web.WebPages;
using BreezeShop.Core.DataProvider;
using Yun.User;

namespace BreezeShop.Web.Areas.Admin.Controllers
{
    public class AdminAuthController : Controller
    {
        protected UserDetail LoginUserDetail;
        protected string Token;

        protected string Error404Message
        {
            get { return TempData["error404"] == null ? "" : TempData["error404"].ToString(); }
            set { TempData["error404"] = value; }
        }

        protected string Error500Message
        {
            get { return TempData["error500"] == null ? "" : TempData["error500"].ToString(); }
            set { TempData["error500"] = value; }
        }

        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            Token = Member.AdminToken;
            if (Token.IsEmpty())
            {
                filterContext.Result =
                    new RedirectResult(Url.Action("Index", "Login",
                        new {returnurl = HttpUtility.UrlEncode(Request.Url.ToString())}));

                return;
            }

            LoginUserDetail = Member.GetLoginMember(Token);
            ViewData["UserName"] = LoginUserDetail.Nick;
            base.OnActionExecuting(filterContext);
        }

        protected override void OnException(ExceptionContext filterContext)
        {
            TempData["error500"] = filterContext.Exception.ToString();
            filterContext.Result = new RedirectResult(Url.Action("Error500", "Home", new {area = "Admin"}));

            base.OnException(filterContext);
        }
    }
}
