using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using System.Web.WebPages;
using BreezeShop.Core;
using BreezeShop.Core.DataProvider;
using BreezeShop.Web.Areas.Admin.Models;
using BreezeShop.Web.Helper;
using BreezeShop.Web.Helper.WxLib;
using Utilities.DataTypes.ExtensionMethods;
using Yun.Coupon.Request;
using Yun.Logistics.Request;
using Yun.Pay;
using Yun.Pay.Request;
using Yun.Trade;
using Yun.Trade.Request;

namespace BreezeShop.Web.Areas.Admin.Controllers
{
    public class TradeController : AdminAuthController
    {
        #region 已售出商品列表 View

        /// <summary>
        /// 已售出商品列表 View
        /// </summary>
        /// <param name="itemtitle">商品名称</param>
        /// <param name="nick">买家</param>
        /// <param name="commentstatus">评价状态</param>
        /// <param name="orderdatebegin">订单开始时间</param>
        /// <param name="orderdateend">订单结束时间</param>
        /// <param name="tradestatus">订单状态</param>
        /// <param name="logisticsservice"></param>
        /// <param name="mobile">手机</param>
        /// <param name="p">当前页</param>
        /// <param name="orderid">订单id</param>
        /// <returns></returns>
        public ActionResult Index(string itemtitle, string nick, string commentstatus, string orderdatebegin,
            string orderdateend,
            string tradestatus, string logisticsservice, string mobile, int p = 1, long orderid = 0)
        {
            var page = new PageModel<SnapshotTrade> {CurrentPage = p};
            var req =
                YunClient.Instance.Execute(
                    new GetTradesSoldRequest
                    {
                        ItemTitle = itemtitle,
                        Nick = nick,
                        MinCreateTime = orderdatebegin.IsEmpty() ? 0 : Convert.ToDateTime(orderdatebegin).ToUnix(),
                        MaxCreateTime = orderdateend.IsEmpty() ? 0 : Convert.ToDateTime(orderdateend).ToUnix(),
                        TradeStatus = tradestatus,
                        OrderId = orderid,
                        LogisticsService = logisticsservice,
                        CommentStatus = commentstatus,
                        Mobile = mobile,
                        PageNum = p,
                        PageSize = 20
                    }, Token);

            page.Items = req.Trades;
            page.TotalItems = req.TotalItem;

            return View(page);
        }

        #endregion

        #region 订单详情 View
        /// <summary>
        /// 订单详情 View
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult Detail(int id)
        {
            var model = YunClient.Instance.Execute(new GetTradeRequest { Id = id }).Trade;
            if (model.IsCoupon)
            {
                ViewData["Coupons"] = YunClient.Instance.Execute(new GetBuyerCouponsRequest
                {
                    PageNum = 1,
                    PageSize = 100,
                    TradeId = id,
                }, Token).Coupons;
            }

            return View(model);
        } 
        #endregion

        #region 发货 View

        /// <summary>
        /// 发货 View
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult Delivery(int id = 0)
        {
            if (id <= 0)
            {
                return Content("订单不存在");
            }

            ViewData["LogisticsCompanyNames"] =
                YunClient.Instance.Execute(new GetLogisticsCompanyNameRequest(), Token).LogisticsCompanyNames;

            return View(YunClient.Instance.Execute(new GetTradeRequest {Id = id}).Trade);
        }

        public ActionResult ModifyDelivery(int id = 0)
        {
            if (id <= 0)
            {
                return Content("订单不存在");
            }

            ViewData["LogisticsCompanyNames"] =
                YunClient.Instance.Execute(new GetLogisticsCompanyNameRequest(), Token).LogisticsCompanyNames;

            return View(YunClient.Instance.Execute(new GetTradeRequest {Id = id}).Trade);
        }

        [HttpPost]
        public ActionResult ModifyDelivery(FormCollection collection, int id = 0)
        {
            if (id > 0)
            {
                var r = YunClient.Instance.Execute(new ModifyDeliveryInfoRequest
                {
                    TradeId = id,
                    ExpressName = collection["name"],
                    ExpressEnName = collection["enname"],
                    TrackingNumber = collection["number"]
                }, Token);

                return Json(new { result = r.Result, error = r.ErrMsg });
            }

            return Json(new { result = false, error = "订单不存在" });
        }

        #endregion

        #region 修改地址 View

        /// <summary>
        /// 修改地址 View
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult ModifyAddress(int id)
        {
            var trade = YunClient.Instance.Execute(new GetTradeRequest {Id = id}).Trade;
            if (trade != null)
            {
                var splitAddress = trade.Address.Split(' ');

                //获取省的数据
                var provinces = SystemCity.GetCities(0);
                ViewData["Provinces"] = provinces.Select(e => new SelectListItem
                {
                    Text = e.Name,
                    Value = e.Name + "-" + e.Id,
                    Selected = e.Name.IndexOf(splitAddress[0], StringComparison.Ordinal) >= 0
                });

                //获取市的数据
                var cities = SystemCity.GetCities(provinces.Single(e => e.Name.IndexOf(splitAddress[0], StringComparison.Ordinal) >= 0).Id);
                ViewData["Cities"] = cities.Select(e => new SelectListItem
                {
                    Text = e.Name,
                    Value = e.Name + "-" + e.Id,
                    Selected = e.Name.IndexOf(splitAddress[1], StringComparison.Ordinal) >= 0
                });

                //获取区的数据
                var areas = SystemCity.GetCities(cities.Single(e => e.Name.IndexOf(splitAddress[1], StringComparison.Ordinal) >= 0).Id);
                ViewData["Areas"] = areas.Select(e => new SelectListItem
                {
                    Text = e.Name,
                    Value = e.Name + "-" + e.Id,
                    Selected = e.Name.IndexOf(splitAddress[2], StringComparison.Ordinal) >= 0
                });

                return View(trade);
            }

            return Content("订单不存在");
        }

        #endregion

        #region 修改价格 View
        /// <summary>
        /// 修改价格 View
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult ModifyTradePrice(int id = 0)
        {
            SnapshotTrade model = YunClient.Instance.Execute(
                new GetTradeRequest { Id = id }).Trade;
            return View(model);
        } 
        #endregion

        #region 延长收货时间 View
        /// <summary>
        /// 延长收货时间 View
        /// </summary>
        /// <returns></returns>
        public ActionResult Delay()
        {
            return View();
        } 
        #endregion

        #region 批量发货 View
        /// <summary>
        /// 批量发货 View
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        public ActionResult BulkDelivery(string ids)
        {
            var list = new List<SnapshotTrade>();
            if (!string.IsNullOrEmpty(ids))
            {
                foreach (var id in ids.Split(',').ToList())
                {
                    int tradeid = 0;
                    if (int.TryParse(id, out tradeid))
                    {
                        var model = YunClient.Instance.Execute(
                            new GetTradeRequest
                            {
                                Id = tradeid
                            }).Trade;
                        if (model != null && model.TradeStatus == "WAIT_SELLER_SEND_GOODS")
                        {
                            list.Add(model);
                        }
                    }
                }
            }
            return View(list);
        } 
        #endregion

        #region 标记订单 View
        /// <summary>
        /// 标记订单 View
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult Mark(string id)
        {

            var model = new MemoModel();
            int tradeid = 0;
            if (int.TryParse(id, out tradeid))
            {
                model = YunClient.Instance.Execute(
                    new GetTradeRequest
                    {
                        Id = tradeid
                    }).Trade.SellerMemo;
            }
            return View(model);
        } 
        #endregion

        #region 修改手机 View
        /// <summary>
        /// 修改手机 View
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult ModifyMoblie(int id = 0)
        {
            var model = YunClient.Instance.Execute(
                new GetTradeRequest
                {
                    Id = id
                }).Trade;
            return View(model);
        } 
        #endregion

        #region 交易流水 View
        /// <summary>
        /// 交易流水 View
        /// </summary>
        /// <param name="nick"></param>
        /// <param name="p"></param>
        /// <returns></returns>
        public ActionResult TradeBlotter(string nick, int p = 1)
        {
            var page = new PageModel<FlowRecord>();
            var req = YunClient.Instance.Execute(new GetAccountReportRequest
            {
                Nick = nick,
                PageSize = 20,
                PageNum = p
            },Token);
            page.Items = req.Records;
            page.CurrentPage = p;
            page.TotalItems = req.TotalItem;

            return View(page);
        } 
        #endregion

        #region 关闭订单 View
        /// <summary>
        /// 关闭订单View
        /// </summary>
        /// <returns></returns>
        public ActionResult CloseTrade()
        {
            return View();
        } 
        #endregion

        #region 发货 
        /// <summary>
        /// 发货
        /// </summary>
        /// <param name="collection">物流信息</param>
        /// <param name="id">订单id</param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult Delivery(FormCollection collection, int id = 0)
        {
            if (id > 0)
            {
                var r = YunClient.Instance.Execute(new DeliveryTradeRequest
                {
                    OrderId = id,
                    ExpressName = collection["name"],
                    ExpressEnName = collection["enname"],
                    TrackingNumber = collection["number"]
                }, Token);
                return Json(new { result = r.Result, error = r.ErrMsg });
            }

            return Json(new { result = false, error = "订单不存在" });
        } 
        #endregion

        #region 修改订单价格      public ActionResult ModifyTradePrice(FormCollection collection, int id = 0)
        /// <summary>
        /// 修改订单价格
        /// </summary>
        /// <param name="collection">价格信息</param>
        /// <param name="id">订单id</param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult ModifyTradePrice(FormCollection collection, int id = 0)
        {
            if (id > 0)
            {
                var req = new ModifyPriceRequest
                {
                    Price = collection["price"]
                };
                if (double.Parse(collection["postfee"]) > 0)
                {
                    req.Postfee = double.Parse(collection["postfee"]);
                }
                var r = YunClient.Instance.Execute(req, Token);
                return Json(new { result = r.Result, error = r.ErrMsg });
            }
            return Json(new { result = false, error = "订单不存在" });
        } 
        #endregion

        #region 修改收货地址      public ActionResult ModifyAddress(FormCollection collection, int id = 0)

        /// <summary>
        /// 修改收货地址
        /// </summary>
        /// <param name="collection">收货信息</param>
        /// <param name="id">订单id</param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult ModifyAddress(FormCollection collection, int id = 0)
        {
            if (id > 0)
            {
                var r = YunClient.Instance.Execute(new UpdateShippingAddressRequest
                {
                    RealName = collection["realname"],
                    Address =
                        string.Format("{0} {1} {2} {3}", collection["province"].Split('-')[0],
                            collection["city"].Split('-')[0], collection["area"].Split('-')[0],
                            collection["address"]),
                    Mobile = collection["mobile"],
                    OrderId = id
                }, Token);

                return Json(new {result = r.Result, error = r.ErrMsg});
            }

            return Json(new {result = false, error = "订单不存在"});
        }

        #endregion

        #region 批量标记        public ActionResult BulkMark(FormCollection collection, string id)
        /// <summary>
        /// 批量标记
        /// </summary>
        /// <param name="collection">批量标记</param>
        /// <param name="id">订单id</param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult BulkMark(FormCollection collection, string id)
        {
            int flag = 0;
            int.TryParse(collection["flag"], out flag);
            if (!string.IsNullOrEmpty(id))
            {
                var req = new BatchSaveMemoTradeRequest
                {
                    Ids = id,
                    Flag = flag,
                    Memo = collection["remark"]
                };
                var r = YunClient.Instance.Execute(req, Token);
                return Json(new { result = r.Result, error = r.ErrMsg });
            }
            return Json(new { result = false, error = "订单不存在" });
        } 
        #endregion

        #region 延长收货时间      public ActionResult Delay(int id = 0, int days = 0)
        /// <summary>
        /// 延长收货时间
        /// </summary>
        /// <param name="id">订单id</param>
        /// <param name="days">天数</param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult Delay(int id, int days)
        {
            if (id > 0)
            {
                var order = YunClient.Instance.Execute(new GetTradeRequest {Id = id}).Trade;
                if (order != null)
                {
                    if (order.IsCoupon)
                    {
                        var r = YunClient.Instance.Execute(new DelayCouponRequest
                        {
                            TradeId = id,
                            Day = days,
                        }, Token);
                        return Json(new { result = r.Result, error = r.ErrMsg });
                    }
                    else
                    {
                        var r = YunClient.Instance.Execute(new DelayReceiveTimeRequest
                        {
                            OrderId = id,
                            Days = days
                        }, Token);
                        return Json(new { result = r.Result, error = r.ErrMsg });
                    }
                }

                return Json(new { result = false, error = "订单不存在" });
            }
            return Json(new { result = false, error = "订单不存在" });
        } 
        #endregion

        #region 标记订单        public ActionResult Mark(FormCollection collection, string id)
        /// <summary>
        /// 标记订单
        /// </summary>
        /// <param name="collection">备注信息</param>
        /// <param name="id">订单id</param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult Mark(FormCollection collection, string id)
        {
            int tradeid = 0;
            int flag = 0;
            int.TryParse(collection["flag"], out flag);
            if (int.TryParse(id, out tradeid))
            {
                var req = new SaveMemoRequest
                {
                    Id = tradeid,
                    Flag = flag,
                    Memo = collection["remark"]
                };
                var r = YunClient.Instance.Execute(req, Token);
                return Json(new { result = r.Result, error = r.ErrMsg });
            }
            return Json(new { result = false, error = "订单不存在" });
        } 
        #endregion

        #region 修改手机号       public ActionResult ModifyMoblie(FormCollection collection, int id = 0)
        /// <summary>
        /// 修改手机号
        /// </summary>
        /// <param name="collection">手机</param>
        /// <param name="id">订单id</param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult ModifyMoblie(FormCollection collection, int id = 0)
        {
            //下面注释代码仅用于测试
            //var moblie = collection["moblie"];
            if (id > 0)
            {
                var req = new UpdateMobileRequest
                {
                    OrderId = id,
                    Mobile = collection["moblie"].Trim()
                };
                var r = YunClient.Instance.Execute(req, Token);
                return Json(new { result = r.Result, error = "手机号码输入错误" });
            }
            return Json(new { result = false, error = "订单不存在" });
        } 
        #endregion

        #region 重发电子券      public ActionResult ResendCoupon(int id = 0)

        /// <summary>
        /// 重发电子券
        /// </summary>
        /// <param name="id">订单id</param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult ResendCoupon(int id = 0)
        {
            if (id > 0)
            {
                var model = YunClient.Instance.Execute(new GetSoldCouponsRequest {TradeId = id}).Coupons;
                var c = model.Where(e => !e.IsConsume)
                    .Aggregate(string.Empty, (current, o) => current + ("," + o.CouponId));
                if (c.Length >= 1)
                {
                    c = c.Substring(1);
                    var req = new ResendCouponRequest
                    {
                        Coupons = c
                    };
                    var r = YunClient.Instance.Execute(req, Token);
                    return Json(new {result = r.Result, error = r.ErrMsg});
                }

                return Json(new {result = false, error = "所有电子券都已消费"});
            }

            return Json(new {result = false, error = "订单不存在"});
        }

        #endregion

        #region 关闭订单        public ActionResult CloseTrade(FormCollection collection, int id = 0)

        /// <summary>
        /// 关闭订单
        /// </summary>
        /// <param name="collection">关闭理由</param>
        /// <param name="id">订单id</param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult CloseTrade(FormCollection collection, int id = 0)
        {
            if (id > 0)
            {
                var r = YunClient.Instance.Execute(new CloseTradeRequest
                {
                    Id = id,
                    CloseReason = "卖家主动关闭订单"
                }, Token);

                return Json(new {result = r.Result?"1":"0", error = r.ErrMsg});
            }

            return Json(new {result = 0, error = "订单不存在"});
        }

        #endregion

        public ActionResult DoWxRefund(int orderId, int tradeId)
        {
            var detail =
                YunClient.Instance.Execute(new GetRefundDetailRequest { OrderId = orderId }, Token)
                    .Refund;
            if (detail != null)
            {
                var trade = YunClient.Instance.Execute(new GetTradeRequest { Id = tradeId }).Trade;

                if (trade.Money <= 0)
                {
                    var r= YunClient.Instance.Execute(
                        new ChangeRefundStatusRequest { OrderRefundId = detail.Id, Remark = "", Status = "SUCCESS" },
                        Token).Result;

                    //退款成功
                    return Content("0元退款成功,代码：" + r);
                }


                if (trade.PaymentInfo.OnlinePayResult == null)
                {
                    return Content("错误，在线付款信息不存在");
                }

                var setting =
                    YunClient.Instance.Execute(new GetOnlinePaymentRequest { PayMethod = "MOBILE", CompanyId = 0 }, Token)
                        .Setting.Weixinpay;


                //微信退款
                var exRefund = WxPayApi.Refund(detail.OrderInfo.Money, detail.Online, detail.RefundBatchNo,
                    trade.PaymentInfo.OnlinePayResult.TradeNo, setting.Key, setting.AppId, setting.MchId,
                    trade.PaymentInfo.Flowid.ToString());


                if (exRefund.GetValue("result_code").Equals("SUCCESS"))
                {
                    var r= YunClient.Instance.Execute(new CompleteRefundRequest { RefundId = detail.RefundId }, Token).Result;

                    //退款成功
                    return Content("退款成功,代码：" + r);
                }

                ViewData["RefundResult"] = exRefund.ToPrintStr();
                return View();
            }

            return Content("订单不存在");
        }

        public ActionResult RunRefund(int orderId, int tradeId)
        {
            var trade = YunClient.Instance.Execute(new GetTradeRequest {Id = tradeId}).Trade;
            if (trade != null && trade.Orders.Any(e => e.Id == orderId))
            {
                var order = trade.Orders.Single(e => e.Id == orderId);
                var result =
                    YunClient.Instance.Execute(
                        new CreateRefundRequest {OnlineMoney = order.Money, OrderId = orderId, Reason = "全额退款"}, Token)
                        .RefundData;

                if (result.Key <= 0)
                {
                    return Json(new {error = result.Key, result = false});
                }

                var detail =
                    YunClient.Instance.Execute(new GetRefundDetailRequest {OrderId = orderId}, Token)
                        .Refund;

                if (trade.Money <= 0)
                {
                    YunClient.Instance.Execute(
                        new ChangeRefundStatusRequest {OrderRefundId = detail.Id, Remark = "", Status = "SUCCESS"},
                        Token);

                    return Json(new { error = result.Key, result = true });
                }

                var setting =
                    YunClient.Instance.Execute(new GetOnlinePaymentRequest {PayMethod = "MOBILE", CompanyId = 0}, Token)
                        .Setting.Weixinpay;

                //进入微信退款流程
                //微信退款
                var wxRefundInit = WxPayApi.Refund(detail.OrderInfo.Money, detail.Online, detail.RefundBatchNo,
                    trade.PaymentInfo.OnlinePayResult.TradeNo, setting.Key, setting.AppId, setting.MchId,
                    trade.PaymentInfo.Flowid.ToString());

                if (wxRefundInit.GetValue("result_code").Equals("SUCCESS"))
                {
                    YunClient.Instance.Execute(new CompleteRefundRequest {RefundId = detail.RefundId}, Token);

                    //退款成功
                    return Json(new {error = "", result = true});
                }



                return Json(new {error = wxRefundInit.ToPrintStr(), result = false});
            }

            return Json(new {error = "-1", result = false});
        }

        private static string TranslationTradeState(string state)
        {
            switch (state)
            {
                case "WAIT_BUYER_CONFIRM_GOODS":
                    return "卖家已发货";
                case "WAIT_BUYER_PAY":
                    return "等待买家付款";
                case "交易成功":
                    return "交易关闭";
                case "WAIT_SELLER_SEND_GOODS":
                    return "等待卖家发货";
            }

            return state;
        }

        public ActionResult ExportTradesExcel(string itemtitle, string nick, string commentstatus, string orderdatebegin,
            string orderdateend,
            string tradestatus, string logisticsservice, string mobile, long orderid = 0)
        {
            var req =
                YunClient.Instance.Execute(
                    new GetTradesSoldRequest
                    {
                        ItemTitle = itemtitle,
                        Nick = nick,
                        MinCreateTime =
                            string.IsNullOrEmpty(orderdatebegin) ? 0 : Convert.ToDateTime(orderdatebegin).ToUnix(),
                        MaxCreateTime =
                            string.IsNullOrEmpty(orderdateend) ? 0 : Convert.ToDateTime(orderdateend).ToUnix(),
                        TradeStatus = tradestatus,
                        OrderId = orderid,
                        LogisticsService = logisticsservice,
                        CommentStatus = commentstatus,
                        Mobile = mobile,
                        PageNum = 1,
                        PageSize = 10000,
                        Sort = "paydesc"
                    }, Member.Token).Trades;

            
            if (req != null && req.Any())
            {
                var data = new List<ExportTradeFormat>();

                foreach (var m in req)
                {
                    if (m.Orders.Count == 1)
                    {
                        data.Add(new ExportTradeFormat
                        {
                            Address = m.Address,
                            Mobile = m.Mobile,
                            RealName = m.RealName,
                            CreateTime = m.CreateTime,
                            OrderId = m.Id,
                            PayTime = m.PayTime,
                            Money = m.Money,
                            TotalMoney = m.TotalFunds,
                            Remark = m.Remark,
                            TradeStatus = TranslationTradeState(m.TradeStatus),
                            ItemTitle = m.Orders[0].ItemTitle,
                            Price = m.Orders[0].Price,
                            Quantity = m.Orders[0].Quantity,
                            Sku =
                                (m.Orders[0].SkuNames == null || !m.Orders[0].SkuNames.Any())
                                    ? ""
                                    : m.Orders[0].SkuNames.Aggregate("",
                                        (current, s) => current + string.Format("{0}:{1},", s.Key, s.Value)).Trim(','),
                           SellerMemo = m.SellerMemo==null?"": m.SellerMemo.Remark
                        });

                        continue;
                    }


                    data.AddRange(m.Orders.Select(order => new ExportTradeFormat
                    {
                        Address = m.Address,
                        Mobile = m.Mobile,
                        RealName = m.RealName,
                        CreateTime = m.CreateTime,
                        OrderId = m.Id,
                        PayTime = m.PayTime,
                        Money = m.Money,
                        TotalMoney = m.TotalFunds,
                        Remark = m.Remark,
                        TradeStatus = TranslationTradeState(m.TradeStatus),
                        ItemTitle = order.ItemTitle,
                        Price = order.Price,
                        Quantity = order.Quantity,
                        Sku =
                            (order.SkuNames == null || !order.SkuNames.Any())
                                ? ""
                                : order.SkuNames.Aggregate("",
                                    (current, s) => current + string.Format("{0}:{1},", s.Key, s.Value)).Trim(','),
                        SellerMemo = m.SellerMemo == null ? "" : m.SellerMemo.Remark
                    }));
                }

                return MvcExcel.ExportListToExcel_MVCResult(data.Select(e => new
                {
                    e.OrderId,
                    e.CreateTime,
                    e.PayTime,
                    e.ItemTitle,
                    e.Sku,
                    e.Price,
                    e.Quantity,
                    e.Money,
                    e.TotalMoney,
                    e.RealName,
                    e.Address,
                    e.Mobile,
                    e.Remark,
                    e.TradeStatus,
                    e.SellerMemo
                }).ToList(),
                    new[]
                    {
                        "订单编号", "创建时间", "付款时间", "商品名称", "规格型号", "商品价格", "购买数量", "支付金额", "总金额", "收货人姓名", "收货地址", "收货人电话",
                        "买家留言", "状态", "卖家留言"
                    },
                    "卖家订单-" + DateTime.Now.ToString("yyyyMMddHHmmss"));
            }

            return Content("无匹配订单数据");
        }

    }

}
