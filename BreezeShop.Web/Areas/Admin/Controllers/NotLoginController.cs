using System.Web.Mvc;
using BreezeShop.Core.FileFactory;

namespace BreezeShop.Web.Areas.Admin.Controllers
{
    public class NotLoginController : Controller
    {
        /// <summary>
        /// kindeditor图片上传
        /// </summary>
        /// <returns></returns>
        public ActionResult Upload()
        {
            return Json(new KindeditorMode().Upload(), JsonRequestBehavior.AllowGet);
        }

        public ActionResult UpdateImages()
        {
            return Json(string.Join(",", FileManage.Upload()));
        }

    }
}
