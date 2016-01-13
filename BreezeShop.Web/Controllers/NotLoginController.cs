using BreezeShop.Core;
using BreezeShop.Core.FileFactory;
using Senparc.Weixin.MP.AdvancedAPIs.TemplateMessage;
using System.Linq;
using System.Web.Mvc;
using BreezeShop.Core.DataProvider;
using Utilities.DataTypes.ExtensionMethods;
using Yun.Archive.Request;
using Yun.Marketing.Request;
using Yun.Trade;
using Yun.Trade.Request;
using Yun.User.Request;

namespace BreezeShop.Web.Controllers
{
    public class NotLoginController : WxAuthController
    {
        private static object _hasDoTrade = new object();

        public ActionResult CiyiesData(int parentId)
        {
            return Json(new { data = SystemCity.GetCities(parentId) }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult SimulationLogin()
        {
            Member.Token = "NX0iIzRGc1k2dzdGUFIrRmFTbXhqZzVUUCtvbnJXc0hoWkV2V0w5R3VZZHBJT3NrNUU9";
            Member.OpenId = "onUkywrE3glAbBpzA2SekpX-b4cc";

            return Content("写入成功");
        }

        public ActionResult CompletePayDo(string tradeno, int id = 0)
        {
            //对已经处理的订单写入appdata中的文本，确保每个订单只运行一次
            //读取文件，每行一个订单号
            lock (_hasDoTrade)
            {
                var path = Server.MapPath("~/app_data/paycompletehistory.txt");
                var tradeIds = System.IO.File.ReadAllLines(path).ToList();
                if (tradeIds.Contains(tradeno))
                {
                    return Content("已经处理了");
                }

                tradeIds.Add(tradeno);

                //写入文件
                System.IO.File.WriteAllLines(path, tradeIds);
            }

            var trade = YunClient.Instance.Execute(new GetTradeRequest { Id = id, TradeNo = tradeno}).Trade;
            if (trade == null)
            {
                return Content("订单不存在");
            }

            if (trade.TradeStatus == "WAIT_SELLER_SEND_GOODS")
            {
                return Content("发送成功");
            }

            return Content("订单状态不正确");
        }


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

        public ActionResult Article(int id)
        {
            return View(YunClient.Instance.Execute(new GetArchiveRequest { Id = id }).Article);
        }
    }
}
