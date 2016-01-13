using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BreezeShop.Web.Controllers
{
    public class HomeController : MemberAuthController
    {
        public ActionResult Index()
        {
            return View();
        }

    }
}
