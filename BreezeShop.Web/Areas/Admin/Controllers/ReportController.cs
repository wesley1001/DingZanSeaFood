using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using BreezeShop.Core;
using Yun.Item.Request;

namespace BreezeShop.Web.Areas.Admin.Controllers
{
    public class ReportController : AdminAuthController
    {
        /// <summary>
        /// 日销售报表
        /// </summary>
        /// <param name="maxDateTime"></param>
        /// <param name="minDateTime"></param>
        /// <param name="p"></param>
        /// <returns></returns>
        public ActionResult SalesStatistics(string maxDateTime, string minDateTime, int p = 1)
        {
            var minTime = string.IsNullOrWhiteSpace(minDateTime)
                ? DateTime.Now.AddDays(-30)
                : Convert.ToDateTime(minDateTime);

            var maxTime = string.IsNullOrWhiteSpace(maxDateTime)
                ? DateTime.Now
                : Convert.ToDateTime(maxDateTime);

            var r = YunClient.Instance.Execute(new Yun.Trade.Request.GetTradeStatisticsRequest
            {
                MaxDateTime = maxTime,
                MinDateTime = minTime,
                PageNum = p,
                PageSize = 100
            });

            return View(new PageModel<Yun.Trade.TradeStatistics>
            {
                CurrentPage = p,
                Items = r.TradingStatistics,
                ItemsPerPage = 100,
                TotalItems = r.TotalItem,
            }
                );
        }

        /// <summary>
        /// 商品交易报表
        /// </summary>
        /// <param name="maxDateTime"></param>
        /// <param name="minDateTime"></param>
        /// <param name="p"></param>
        /// <returns></returns>
        public ActionResult ItemSalesReport(string maxDateTime, string minDateTime, int p = 1)
        {
            var minTime = string.IsNullOrWhiteSpace(minDateTime)
                ? DateTime.Now.AddDays(-1)
                : Convert.ToDateTime(minDateTime);

            var maxTime = string.IsNullOrWhiteSpace(maxDateTime)
                ? DateTime.Now
                : Convert.ToDateTime(maxDateTime);

            var r = YunClient.Instance.Execute(new Yun.Trade.Request.GetItemTradeStatisticsReportRequest
            {
                MaxDateTime = maxTime,
                MinDateTime = minTime,
                PageNum = p,
                PageSize = 20
            });

            ViewData["ItemCats"] =
                YunClient.Instance.Execute(new GetItemsRequest
                {
                    Ids = string.Join(",", r.ItemTradingStatistics.Select(e => e.ItemId)),
                    Fields = "id,item_cats,itemtitle"
                }).Items;

            return View(new PageModel<Yun.Trade.ItemTradeStatistics>
            {
                CurrentPage = p,
                Items = r.ItemTradingStatistics,
                ItemsPerPage = 20,
                TotalItems = r.TotalItem,
            }
                );
        }

        public ActionResult MonthSalesStatistics(int year = 2016)
        {
            return
                View(
                    YunClient.Instance.Execute(new Yun.Trade.Request.GetMonthTradeStatisticsRequest {Year = year})
                        .TradingStatistics);
        }

    }
}
