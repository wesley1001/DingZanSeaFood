using System.Web;
using System.Web.Mvc;
using System.Web.WebPages;
using BreezeShop.Core.DataProvider;
using BreezeShop.Web.Areas.Admin.Models;

namespace BreezeShop.Web.Areas.Admin.Controllers
{
    public class LoginController : Controller
    {
        public ActionResult Exit()
        {
            Member.AdminExit();
            return RedirectToAction("Index", "Login", new {area = "Admin"});
        }

        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Index(LogInModel model, string returnurl)
        {
            if (ModelState.IsValid)
            {
                var l = Member.Login(model.UserName, model.Password, Request.UserHostAddress, 1);
                if (l.Key)
                {
                    var u = Member.GetLoginMember(l.Value);
                    if (u.IsStaff || u.IsLegal)
                    {
                        if (returnurl.IsEmpty())
                        {
                            return RedirectToAction("Index", "Home");
                        }

                        return Redirect(HttpUtility.UrlDecode(returnurl));
                    }

                    return View(model);
                }
            }

            return View(model);
        }
    }
}
