using BreezeShop.Core;
using BreezeShop.Core.DataProvider;
using BreezeShop.Core.Model;
using BreezeShop.Web.Areas.Admin.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Utilities.DataTypes.ExtensionMethods;
using Yun.Logistics;
using Yun.Logistics.Request;
using Yun.Pay.Request;

namespace BreezeShop.Web.Areas.Admin.Controllers
{
    public class SettingController : AdminAuthController
    {
        public ActionResult WebSetting()
        {
            return View(GlobeInfo.WebSetting);
        }

        [HttpPost]
        public ActionResult WebSetting(WebSetting model)
        {
            GlobeInfo.WebSetting = model;
            TempData["success"] = "保存成功";

            return RedirectToAction("WebSetting");
        }

        public ActionResult WxPay()
        {
            var setting =
                YunClient.Instance.Execute(new GetOnlinePaymentRequest {PayMethod = "MOBILE", CompanyId = 0}, Token).Setting.Weixinpay;
            return View(new WxPaySettingModel
            {
                AppId = setting.AppId,
                Key = setting.Key,
                MchId = setting.MchId,
                ClientNotifyUrl = setting.ClientNotifyUrl
            });
        }

        [HttpPost]
        public ActionResult WxPay(WxPaySettingModel model)
        {
            if (ModelState.IsValid)
            {
                var r = YunClient.Instance.Execute(
                    new SetOnlinePaymentRequest
                    {
                        WeixinPay =
                            string.Format("{0},{1},{2},{3}", model.AppId, model.MchId, model.Key,
                                model.ClientNotifyUrl),
                        PayMethod = "MOBILE",
                        UseSelf = true,
                        CompanyId = 0
                    }, Token);

                if (r.Result)
                {
                    TempData["success"] = "保存成功";
                }
                else
                {
                    TempData["error"] = "保存失败，错误代码：" + r.ErrMsg;
                }

                return RedirectToAction("WxPay");
            }

            return View(model);
        }


        public ActionResult Logistics()
        {
            var r = YunClient.Instance.Execute(new GetDeliveryTemplatesRequest(), Token);
            return View(new PageModel<DeliveryTemplate>
            {
                Items = r.FreightTemplates
            });
        }

        public ActionResult FreightTables()
        {
            return PartialView(SystemCity.GetAdminClientCities());
        }

        public ActionResult AddTemplate(string id)
        {
            ViewData["AllCitys"] = SystemCity.GetAdminClientCities();

            var ps = new DeliveryTemplateFreight
            {
                kuaidi = new List<PostfareDetail>(),
                ziti = new List<PostfareDetail>(),
                shangjia = new List<PostfareDetail>(),
                FareFreeConditions = new List<FareFreeCondition>()
            };

            int templateid;
            var model = new DeliveryTemplate();
            if (int.TryParse(id, out templateid))
            {
                var r =
                    YunClient.Instance.Execute(new GetExpressTemplateRequest
                    {
                        Id = templateid
                    }, Token);

                model = r.Result;

                if (model.Freight.Any(e => e.DeliveryId == 4 && (e.Cities == null || !e.Cities.Any())))
                {
                    var m = model.Freight.First(e => e.DeliveryId == 4 && (e.Cities == null || !e.Cities.Any()));
                    ps.kuaidi1 = m.BaseQuantity.ToString();
                    ps.kuaidi2 = m.BasePrice.ToString();
                    ps.kuaidi3 = m.AddQuantity.ToString();
                    ps.kuaidi4 = m.AddPrice.ToString();
                }

                foreach (var a in model.Freight.Where(e => e.DeliveryId == 4 && (e.Cities != null && e.Cities.Any())))
                {
                    ps.kuaidi.Add(a);
                }

                if (model.Freight.Any(e => e.DeliveryId == 419 && (e.Cities == null || !e.Cities.Any())))
                {
                    var m = model.Freight.First(e => e.DeliveryId == 419 && (e.Cities == null || !e.Cities.Any()));
                    ps.ziti1 = m.BaseQuantity.ToString();
                    ps.ziti2 = m.BasePrice.ToString();
                    ps.ziti3 = m.AddQuantity.ToString();
                    ps.ziti4 = m.AddPrice.ToString();
                }

                foreach (var a in model.Freight.Where(e => e.DeliveryId == 419 && (e.Cities != null && e.Cities.Any())))
                {
                    ps.ziti.Add(a);
                }

                if (model.Freight.Any(e => e.DeliveryId == 2 && (e.Cities == null || !e.Cities.Any())))
                {
                    var m = model.Freight.First(e => e.DeliveryId == 2 && (e.Cities == null || !e.Cities.Any()));
                    ps.shangjia1 = m.BaseQuantity.ToString();
                    ps.shangjia2 = m.BasePrice.ToString();
                    ps.shangjia3 = m.AddQuantity.ToString();
                    ps.shangjia4 = m.AddPrice.ToString();
                }
                foreach (var a in model.Freight.Where(e => e.DeliveryId == 2 && (e.Cities != null && e.Cities.Any())))
                {
                    ps.shangjia.Add(a);
                }
            }

            ViewBag.Prices = ps;

            return View(model);
        }

        [HttpPost]
        public ActionResult SaveTemplate(int templateId, string title, string farefree, string price, string logPrice, string farePrices, string priceType)
        {
            bool result = false;
            List<LogisticsPriceJson> logisticsPrice = new List<LogisticsPriceJson>();
            List<FareFreeJson> fareFreeStrategy = new List<FareFreeJson>();
            if (!string.IsNullOrWhiteSpace(price))
            {
                string[] defaultPrice = price.Split('#');
                for (int i = 0; i < defaultPrice.Length; i++)
                {
                    logisticsPrice.Add(new LogisticsPriceJson()
                    {
                        basequantity = int.Parse(defaultPrice[i].Split(',')[1]),
                        baseprice = int.Parse(defaultPrice[i].Split(',')[2]),
                        addquantity = int.Parse(defaultPrice[i].Split(',')[3]),
                        addprice = int.Parse(defaultPrice[i].Split(',')[4]),
                        cityid = new List<int>(),
                        deliveryid = int.Parse(defaultPrice[i].Split(',')[0]),

                    });
                }
            }
            if (!string.IsNullOrWhiteSpace(logPrice))
            {
                var pr = logPrice.Split('#');
                for (var i = 0; i < pr.Length; i++)
                {
                    if (pr[i] == "") continue;
                    if (pr[i].IndexOf("-", StringComparison.Ordinal) == 0) pr[i] = pr[i].Remove(0, pr[i].IndexOf(",", StringComparison.Ordinal) + 1);
                    var jsons = pr[i].Split('-');
                    for (var y = 0; y < jsons.Length - 1; y++)
                    {
                        var json = jsons[y].Split(',');
                        var ids = new List<int>();
                        for (var x = 0; x < json.Length - 4; x++)
                        {
                            if (int.Parse(json[x]) > 0)
                            {
                                ids.Add(int.Parse(json[x]));
                            }
                        }
                        logisticsPrice.Add(new LogisticsPriceJson()
                        {
                            basequantity = int.Parse(json[json.Length - 4]),
                            baseprice = int.Parse(json[json.Length - 3]),
                            addquantity = int.Parse(json[json.Length - 2]),
                            addprice = int.Parse(json[json.Length - 1]),
                            cityid = ids,
                            deliveryid = int.Parse(jsons.LastOrDefault()),
                        });
                    }
                }
            }

            if (!string.IsNullOrWhiteSpace(farePrices))
            {
                var pr = farePrices.Split('#');
                for (var i = 0; i < pr.Length; i++)
                {
                    if (pr[i] == "") continue;
                    if (pr[i].IndexOf("-", StringComparison.Ordinal) == 0) pr[i] = pr[i].Remove(0, pr[i].IndexOf(",", StringComparison.Ordinal) + 1);
                    var jsons = pr[i].Split('-');
                    for (var y = 0; y < jsons.Length - 1; y++)
                    {
                        var json = jsons[y].Split(',');
                        var ids = new List<int>();
                        for (var x = 0; x < json.Length - 3; x++)
                        {
                            if (int.Parse(json[x]) > 0)
                                ids.Add(int.Parse(json[x]));
                        }
                        fareFreeStrategy.Add(new FareFreeJson()
                        {
                            freetype = int.Parse(json[json.Length - 3]),
                            preferential = int.Parse(json[json.Length - 2]),
                            price = int.Parse(json[json.Length - 1]),
                            cityid = ids,
                            deliveryid = int.Parse(jsons.LastOrDefault()),
                        });
                    }
                }
            }

            if (templateId == 0)
            {

                var r = YunClient.Instance.Execute(new AddDeliveryTemplateRequest
                {
                    Title = title,
                    Farefree = int.Parse(farefree),
                    FareFreeStrategy = fareFreeStrategy,
                    LogisticsPrice = logisticsPrice,
                    PriceType = priceType.TryTo(0)
                }, Token);
                result = r.Result > 0;
            }
            else
            {
                var r = YunClient.Instance.Execute(new UpdateDeliveryTemplateRequest
                {
                    Id = templateId,
                    Title = title,
                    FareFreeStrategy = fareFreeStrategy,
                    LogisticsPrice = logisticsPrice,
                    Farefree = int.Parse(farefree),
                    PriceType = priceType.TryTo(0)
                }, Token);
                result = r.Result;
            }
            return Json(result);
        }

        [HttpPost]
        public ActionResult Delete(int id = 0)
        {
            if (id > 0)
            {
                var r = YunClient.Instance.Execute(new DeleteDeliveryTemplateRequest
                {
                    Id = id
                }, Token);
                return Json(new {result = r.Result, error = r.ErrMsg});
            }

            return Json(new {result = false, error = "物流模板不存在"});
        }

    }
}
