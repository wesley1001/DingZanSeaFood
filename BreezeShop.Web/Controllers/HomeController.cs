using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web.Mvc;
using BreezeShop.Core;
using BreezeShop.Core.DataProvider;
using Utilities.DataTypes.ExtensionMethods;
using Yun.Item.Request;
using Yun.Logistics;
using Yun.Logistics.Request;
using Yun.Marketing;
using Yun.Marketing.Request;
using Yun.Pay.Request;
using Yun.Site.Request;
using Yun.Trade;
using Yun.Trade.Request;

namespace BreezeShop.Web.Controllers
{
    public class HomeController : MemberAuthController
    {
        public ActionResult Index()
        {
            var req = YunClient.Instance.Execute(new GetItemsRequest
            {
                Fields = "id,pictures,picture,itemtitle",
                PageNum = 1,
                PageSize = 10,
                ItemState = 1,
                Sorts = "sortdesc",
                IsDelete = 0
            });

            ViewData["itemTotal"] = YunClient.Instance.Execute(new GetItemsRequest
            {
                Fields = "id",
                PageNum = 1,
                PageSize = 1,
                ItemState = 1,
                IsDelete = 0
            }).TotalItem;

            return View(req.Items);
        }

        public ActionResult List()
        {
            return View(YunClient.Instance.Execute(new GetShopItemCategorysRequest { ParentId = 0,ShopId = GlobeInfo.InitiatedShopId, Display = 1}).Categorys);
        }

        public ActionResult ItemData(int id = 0, int p = 1, string q = "")
        {
            var req = YunClient.Instance.Execute(new GetItemsRequest
            {
                Fields = "id,pictures,picture,itemtitle,marketprice,price",
                PageNum = p,
                PageSize = 200,
                ItemState = 1,
                Sorts = "sortdesc",
                IsDelete = 0,
                ItemTitle = q,
                ShopCatIds = id>0?id.ToString():""
            });

            return PartialView(req.Items);
        }


        public ActionResult GoodsDetail(int id = 0)
        {
            if (id <= 0)
            {
                return Content("商品不存在");
            }

            var item = YunClient.Instance.Execute(new GetItemRequest {Id = id}).Item;
            if (item != null && !item.IsDelete && item.ItemState == 1)
            {
                if (item.ItemSkus != null && item.ItemSkus.Any())
                {
                    //生成SKU的JSON数据
                    var skuData = item.ItemSkus.Where(e => e.Stock > 0)
                        .Aggregate("{",
                            (current, sku) =>
                                current +
                                string.Format("\"{0}\": {{ price:{1},  count:{2}, skuid:{3} }},",
                                    string.Join(";", sku.SpecIds), sku.Price, sku.Stock, sku.Id));
                    skuData = skuData.Trim(',');
                    skuData += "}";
                    ViewData["SkuData"] = skuData;
                }

                ViewData["ExpressTemplate"] =
                    YunClient.Instance.Execute(new GetExpressTemplateRequest {Id = (int) item.FreightTemplateId}).Result;

                return View(item);
            }

            return Content("商品不存在");
        }

        /// <summary>
        /// 购买页面
        /// </summary>
        /// <returns></returns>
        public ActionResult Buy(int addressId = 0)
        {
            var token = Member.Token;

            var carts = YunClient.Instance.Execute(new GetShoppingCartsRequest(), token).Items;

            if (carts != null && carts.Any())
            {
                //代金券信息
                ViewData["cashCoupons"] =
                    YunClient.Instance.Execute(
                        new GetMyCashCouponsRequest
                        {
                            HasExpired = false,
                            HasUsed = false,
                            TradePrice = carts.Sum(e => e.Quantity * e.ItemInfo.Price.TryTo(0.0)),
                            ItemPriceJson =
                                carts.Select(
                                    e =>
                                        new GetMyCashCouponItemPriceJson
                                        {
                                            item_id = e.ItemInfo.ItemId,
                                            item_total_price = e.Quantity * e.ItemInfo.Price.TryTo(0.0)
                                        }).ToList()
                        }, token)
                        .CashCoupons;
            }


            //默认收货地址

            var address = addressId > 0
                ? YunClient.Instance.Execute(new GetAddressRequest { Id = addressId }, token).Result
                : YunClient.Instance.Execute(new GetDefaultAddressRequest(), token).Result;

            ViewData["defaultAddress"] = address;
            ViewData["ExpressPrice"] = "0.00";

            if (carts != null && carts.Any())
            {
                var template = YunClient.Instance.Execute(new GetTheMostExpensiveTemplateRequest
                {
                    ItemIds = string.Join(",", carts.Select(e => e.ItemInfo.ItemId))
                }).Result;

                if (address != null)
                {
                    ViewData["ExpressPrice"] = SystemCity.GetExpressPrice(4, (int)carts.Sum(e => e.Quantity),
                        string.Format("{0}{1}{2}{3}", address.Province, address.City, address.Area, address.Street),
                        carts.Sum(e => e.Quantity * e.ItemInfo.Price.TryTo(0.0)),
                        carts.Sum(e => e.Quantity * e.ItemInfo.Weight),
                        carts.Sum(e => e.Quantity * e.ItemInfo.Volume), template).ToString("f2");
                }
            }

            //购物车内商品信息
            return View(carts);
        }

        public ActionResult GetExpressPrice(int addressId)
        {
            var token = Member.Token;

            var carts = YunClient.Instance.Execute(new GetShoppingCartsRequest(), token).Items;

            //如果购物车内没有商品，则直接返回0
            if (carts == null || !carts.Any())
            {
                return Content("0");
            }

            var template = YunClient.Instance.Execute(new GetTheMostExpensiveTemplateRequest
            {
                ItemIds = string.Join(",", carts.Select(e => e.ItemInfo.ItemId))
            }).Result;

            if (template == null)
            {
                return Content("0");
            }

            //默认收货地址
            var address = addressId > 0
                ? YunClient.Instance.Execute(new GetAddressRequest { Id = addressId }, token).Result
                : YunClient.Instance.Execute(new GetDefaultAddressRequest(), token).Result;

            if (address != null)
            {
                var price = SystemCity.GetExpressPrice(4, (int)carts.Sum(e => e.Quantity),
                    address.Province + address.City + address.Area,
                    carts.Sum(e => e.Quantity * e.ItemInfo.Price.TryTo(0.0)), carts.Sum(e => e.Quantity * e.ItemInfo.Weight),
                    carts.Sum(e => e.Quantity * e.ItemInfo.Volume), template);

                return Content(price.ToString("f2"));
            }

            return Content("0.00");
        }

        public ActionResult TradeAddress()
        {
            return View(YunClient.Instance.Execute(new GetAddressesRequest(), Member.Token).Result);
        }

        public ActionResult ManageAddress()
        {
            return View(YunClient.Instance.Execute(new GetAddressesRequest(), Member.Token).Result);
        }


        public ActionResult AdderssEdit(int id = 0)
        {
            var provinces = SystemCity.GetCities(0);

            var dataProvinces = new List<SelectListItem>();
            var dataCities = new List<SelectListItem>();
            var dataAreas = new List<SelectListItem>();

            UserAddress address = null;

            if (id > 0)
            {
                address = YunClient.Instance.Execute(new GetAddressRequest { Id = id }, Member.Token).Result;

                dataProvinces.AddRange(provinces.Select(e => new SelectListItem
                {
                    Text = e.Name,
                    Value = e.Name + "-" + e.Id,
                    Selected = e.Name.IndexOf(address.Province, StringComparison.CurrentCultureIgnoreCase) >= 0
                }));


                var cities = SystemCity.GetCities(
                    provinces.Single(
                        e => e.Name.IndexOf(address.Province, StringComparison.CurrentCultureIgnoreCase) >= 0).Id);
                dataCities.AddRange(cities.Select(e => new SelectListItem
                {
                    Text = e.Name,
                    Value = e.Name + "-" + e.Id,
                    Selected = e.Name.IndexOf(address.City, StringComparison.CurrentCultureIgnoreCase) >= 0
                }));

                dataAreas.AddRange(SystemCity.GetCities(
                    cities.Single(e => e.Name.IndexOf(address.City, StringComparison.CurrentCultureIgnoreCase) >= 0).Id)
                    .Select(e => new SelectListItem
                    {
                        Text = e.Name,
                        Value = e.Name + "-" + e.Id,
                        Selected = e.Name.IndexOf(address.Area, StringComparison.CurrentCultureIgnoreCase) >= 0
                    }));
            }
            else
            {
                dataProvinces.Add(new SelectListItem { Text = "-请选择省-" });
                dataCities.Add(new SelectListItem { Text = "-请选择城市-" });
                dataAreas.Add(new SelectListItem { Text = "-请选择地区-" });

                dataProvinces.AddRange(provinces.Select(e => new SelectListItem
                {
                    Text = e.Name,
                    Value = e.Name + "-" + e.Id,
                }));
            }

            ViewData["ProvincesList"] = dataProvinces;
            ViewData["CitiesList"] = dataCities;
            ViewData["AreasList"] = dataAreas;

            return View(address);
        }

        public ActionResult DeleteAddress(int id)
        {
            if (id <= 0)
            {
                return Json(false);
            }

            var r = YunClient.Instance.Execute(new DeleteAddressRequest { Id = id }, Member.Token).Result;
            return Json(r);
        }

        [HttpPost]
        public ActionResult AdderssEdit(FormCollection collection, int id = 0)
        {
            var province = collection["province"];
            var city = collection["city"];
            var area = collection["area"];
            var street = collection["street"];
            //var detail = collection["detail"];
            var realname = collection["realname"];
            var tel = collection["tel"];
            var isdefault = collection["isdefault"].TryTo(0);

            var r = YunClient.Instance.Execute(new SaveAddressRequest
            {
                Id = id,
                City = city.Split('-')[0],
                IsDefault = isdefault,
                Name = realname,
                Mobile = tel,
                Street = street,
                Area = area.Split('-')[0],
                Province = province.Split('-')[0]
            }, Member.Token).Result;

            return Json(r);
        }

        private static IList<string> hasCreatedPayId = new List<string>();

        public ActionResult ToPay(int id, int flowId, string payId)
        {
            var trade = YunClient.Instance.Execute(new GetTradeRequest { Id = id }).Trade;
            if (trade == null)
            {
                return Content("订单不存在");
            }

            if (trade.Orders.Any(e => Math.Abs(e.AdjustPrice) > 0) && hasCreatedPayId.All(e => e != payId))
            {
                lock (hasCreatedPayId)
                {
                    if (trade.Orders.Any(e => Math.Abs(e.AdjustPrice) > 0) && hasCreatedPayId.All(e => e != payId))
                    {
                        YunClient.Instance.Execute(new GeneratePayTradeRequest
                        {
                            OnlineMoney = trade.Money,
                            ClientType = "MOBILE",
                            Id = trade.Id,
                            Ip = Request.UserHostAddress,
                            PaymentIns = "Weixin"
                        }, Member.Token);

                        hasCreatedPayId.Add(payId);
                    }
                }
            }

            if (flowId > 0 || !string.IsNullOrEmpty(payId))
            {
                var unifiedorder = YunClient.Instance.Execute(new WeixinUnifiedorderRequest
                {
                    Id = flowId.TryTo(0),
                    Openid = Member.OpenId,
                    SpbillCreateIp = Request.UserHostAddress,
                    TradeType = "JSAPI",
                    TradeNum = payId
                });

                if (!unifiedorder.IsError && unifiedorder.UnifiedorderResult != null)
                {
                    var html = YunClient.Instance.Execute(new GenerateJsApiPayParmRequest
                    {
                        PrepayId = unifiedorder.UnifiedorderResult.PrepayId
                    }).JsApiPayResult;

                    ViewData["Trade"] = trade;

                    return View(html);
                }

                return Content(unifiedorder.ErrMsg);
            }

            return Content("支付订单生成失败");
        }


        [HttpPost]
        public ActionResult Buy(FormCollection collection)
        {
            var token = Member.Token;

            var addressId = collection["addressid"].TryTo(0);
            if (addressId <= 0)
            {
                return Json(new { error = -100 });
            }

            var address = YunClient.Instance.Execute(new GetAddressRequest { Id = addressId }, token).Result;
            if (address == null)
            {
                return Json(new { error = -300 });
            }

            var items = YunClient.Instance.Execute(new GetShoppingCartsRequest(), token).Items;
            if (items == null || !items.Any())
            {
                return Json(new { error = -200 });
            }

            var cashcoupon = collection["use-cashcoupon"].TryTo(0);

            var addTrade = YunClient.Instance.Execute(new AddmultiExpressTradeRequest
            {
                Address =
                    string.Format("{0} {1} {2} {3} {4}", address.Province, address.City, address.Area,
                        address.Street, address.Detail).Trim(),
                CashCouponId = cashcoupon /*cashcoupon*/,
                Mobile = address.Mobile,
                RealName = address.Name,
                Items = new List<BuyItemBatch>
                {
                    new BuyItemBatch
                    {
                        DeliveryType = "EXPRESS",
                        Remark = collection["note"],
                        Items = items.Select(e => new BuyItem
                        {
                            ItemId = e.ItemInfo.ItemId,
                            Quantity = e.Quantity,
                            SkuId = e.ItemInfo.SkuId
                        }).ToList()
                    }
                },
                TradeNum = DateTime.Now.ToString("yyyyMMddHHmmssfffffff", DateTimeFormatInfo.InvariantInfo),
                Ext = Member.OpenId
            }, token);

            if (addTrade.TradeId != null && addTrade.TradeId.Any())
            {
                YunClient.Instance.Execute(
                    new RemoveCartRequest { CartIds = string.Join(",", items.Select(e => e.CardId)) }, token);

                var trade = YunClient.Instance.Execute(new GetTradeRequest { Id = addTrade.TradeId[0].TryTo(0) }).Trade;

                if (trade.Money <= 0)
                {
                    var r = YunClient.Instance.Execute(new CompleteNoPaidTradeRequest { TradeId = trade.Id });

                    if (r.Result)
                    {
                        //交易完成后购买流程
                        Utilities.Web.ExtensionMethods.HTTPRequestExtensions.GetHtmlCode(YunClient.WebUrl +
                                                                                         "NotLogin/CompletePayDo/" +
                                                                                         trade.Id + "?tradeno=" +
                                                                                         trade.PayId);

                        return Json(new {tradeid = addTrade.TradeId[0], flowid = 1});
                    }

                    return Json(new { error = r.ErrMsg });
                }
                else
                {
                    var r = YunClient.Instance.Execute(new GeneratePayTradeRequest
                    {
                        OnlineMoney = trade.Money,
                        ClientType = "MOBILE",
                        Id = trade.Id,
                        Ip = Request.UserHostAddress,
                        PaymentIns = "Weixin"
                    }, token).Result;

                    return Json(new { tradeid = addTrade.TradeId[0], flowid = r });
                }
            }

            return Json(new { error = addTrade.ErrMsg });
        }

        public ActionResult StreetComplete(string q)
        {
            var req =
                YunClient.Instance.Execute(new GetCitiesRequest
                {
                    CityName = q,
                    GetCustomCity = true,
                    PageNum = 1,
                    PageSize = 100,
                    State = 1
                }).Cities;

            return PartialView(req);
        }


    }
}
