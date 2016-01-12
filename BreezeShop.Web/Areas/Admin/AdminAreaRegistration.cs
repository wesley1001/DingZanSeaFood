using System.Web.Mvc;
using LowercaseRoutesMVC;

namespace BreezeShop.Web.Areas.Admin
{
    public class AdminAreaRegistration : AreaRegistration
    {
        public override string AreaName
        {
            get
            {
                return "Admin";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context)
        {
            context.MapRouteLowercase(
                "Admin_default",
                "Admin/{controller}/{action}/{id}",
                new { action = "Index", controller="Login", id = UrlParameter.Optional }
                , new[] { "BreezeShop.Web.Areas.Admin.Controllers" }
            );
        }
    }
}
