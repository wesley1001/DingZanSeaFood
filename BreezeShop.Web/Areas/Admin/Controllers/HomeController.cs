using System.Web.Mvc;
using BreezeShop.Core.DataProvider;

namespace BreezeShop.Web.Areas.Admin.Controllers
{
    public class HomeController : AdminAuthController
    {
        /// <summary>
        /// 左导航条
        /// </summary>
        /// <returns></returns>
        public ActionResult LeftNav()
        {
            return PartialView(Manage.GetFunctions(LoginUserDetail.Nick, true, 0));
        }

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Error500()
        {
            return View();
        }

        public ActionResult Error404()
        {
            return View();
        }
    }
}
