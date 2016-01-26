using BreezeShop.Core;
using BreezeShop.Core.FileFactory;
using System.Linq;
using System.Web.Mvc;
using BreezeShop.Core.DataProvider;
using Yun.Archive.Request;
using Yun.Trade.Request;

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
            Member.Token = "cX1efmVSVVlzMHZyczNrK2dNM0UzRU8zbVE2N0UxVnVGbjFxNElNaGV4UUVzRDU1SkU9";
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

        public ActionResult ArticleList()
        {
            return View(YunClient.Instance.Execute(new GetArchivesRequest
            {
                PageNum = 1,
                PageSize = 20,
                Fields = "id,title,createtime,thumb"
            }).Articles);
        }

        public ActionResult ArticleData(int p = 1)
        {
            return
                PartialView(
                    YunClient.Instance.Execute(new GetArchivesRequest
                    {
                        PageNum = p,
                        PageSize = 20,
                        Fields = "id,title,createtime,thumb"
                    }).Articles);
        }

    }
}
