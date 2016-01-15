using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using BreezeShop.Core;
using BreezeShop.Core.DataProvider;
using Yun.Item.Request;
using Yun.Logistics.Request;

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

        public ActionResult Buy()
        {
            return View();
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

    }
}
