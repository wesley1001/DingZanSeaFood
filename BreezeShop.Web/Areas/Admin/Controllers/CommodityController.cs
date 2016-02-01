using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using System.Web.WebPages;
using BreezeShop.Core;
using BreezeShop.Core.DataProvider;
using BreezeShop.Core.FileFactory;
using BreezeShop.Web.Areas.Admin.Models;
using CgApi.Shop.Request;
using Utilities.DataTypes.ExtensionMethods;
using Utilities.IO.ExtensionMethods;
using Yun.Coupon.Request;
using Yun.Item;
using Yun.Item.Request;
using Yun.Logistics.Request;
using Yun.Shop.Request;

namespace BreezeShop.Web.Areas.Admin.Controllers
{
    public class CommodityController : AdminAuthController
    {
        public ActionResult CreateEditThumbs(FormCollection collection)
        {
            var thumb = (collection["Thumbs"] ?? "").Trim(',').Split(new[] { "," }, StringSplitOptions.RemoveEmptyEntries);
            if (thumb.Any())
            {
                var h = new Dictionary<string, string>();
                thumb.Where(e => e.IndexOf('^') > 0).ForEach(s => h.Add(s.Split('^')[0], s.Split('^')[1]));

                return PartialView(h);
            }

            return Content("");
        }

        public ActionResult CategoriesInShop(string ids = "")
        {
            var r = YunClient.Instance.Execute(new GetShopItemCategorysRequest
            {
                Display = 1,
                ParentId = null,
                ShopId = GlobeInfo.InitiatedShopId,
            }, Token).Categorys;

            if (r != null && r.Any())
            {
                return PartialView(r);
            }

            return Content("");
        }

        [HttpPost]
        public ActionResult ProcessSelectShop(FormCollection collection)
        {
            var name = collection["name"];
            var shopid = collection["shopid"].TryTo(0);
            var ischeck = collection["ischeck"].TryTo(false);
            var all = collection["all"];

            var r = new Dictionary<int, string>();
            if (!all.IsEmpty())
            {
                r = all.Deserialize<Dictionary<int, string>>();
            }

            if (ischeck && !r.ContainsKey(shopid))
            {
                r.Add(shopid, name);
            }

            if (!ischeck)
            {
                r.Remove(shopid);
            }
            return Content(r.Serialize());
        }

        public ActionResult FindPartner()
        {
            return PartialView();
        }

        [HttpPost]
        public ActionResult FindPartner(FormCollection collection, int p = 1)
        {
            var r = new PageModel<Yun.Shop.ShopDetail>();

            var req =
                YunClient.Instance.Execute(new SearchShopsRequest
                {
                    PageNum = p,
                    PageSize = 7,
                    ShopName = collection["q"],
                });

            r.CurrentPage = p;
            r.Items = req.Shops;
            r.TotalItems = req.TotalItem;

            if (r.Items != null && r.Items.Any())
            {
                return PartialView(r);
            }

            return Content("");
        }

        [HttpPost]
        public ActionResult RelateShopsResult(FormCollection collection)
        {
            if (!collection["json"].IsEmpty())
            {
                var r = collection["json"].Deserialize<Dictionary<int, string>>();
                return PartialView(r);
            }

            return Content("");
        }

        [HttpPost]
        public ActionResult DeleteItemImage(FormCollection collection)
        {
            var thumb = (collection["Thumbs"] ?? "").Trim(',').Split(new[] { "," }, StringSplitOptions.RemoveEmptyEntries);
            var delfile = collection["DelFile"];
            if (!delfile.IsEmpty() && thumb.Any())
            {
                var h = new Hashtable();
                var hasnotadd = new List<string>();
                thumb.Where(e => e.IndexOf('^') > 0).ForEach(s =>
                {
                    var key = s.Split('^')[0];
                    if (key != delfile.Trim() || hasnotadd.Contains(key))
                    {
                        h.Add(key, s.Split('^')[1]);
                    }
                    else
                    {
                        hasnotadd.Add(key);
                    }
                });

                if (h.Count > 0)
                {
                    var r = h.Keys.Cast<object>()
                        .Aggregate("", (current, k) => current + string.Format("{0}^{1},", k, h[k]));
                    return Content(r);
                }
            }

            return Content("");
        }

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult CreateSkuTable(FormCollection collection)
        {
            if (!collection["EditSkuData"].IsEmpty() && collection["EditSkuData"].Length > 10)
            {
                ViewData["SkuData"] = collection["EditSkuData"].Deserialize<IList<ItemSkuInDetail>>();
            }
            //6c2f4272,dbb9e417
            var tempskuid = (collection["CustomSkuId"] ?? "").Split(new[] { "," }, StringSplitOptions.RemoveEmptyEntries);
            //dbb9e417
            var skuId = tempskuid;
            if (tempskuid.Any())
            {
                //1
                var skuNames = new List<string>();
                tempskuid.ForEach(s =>
                {
                    var name = collection["CustomSkuN-" + s];
                    if (name.IsEmpty())
                    {
                        skuId = skuId.Where(e => e != s).ToArray();
                    }
                    else
                    {
                        skuNames.Add(name);
                    }
                });

                var r = new List<CustomSkuModel>();
                for (var i = 0; i < skuNames.Count; i++)
                {
                    //1
                    var name = skuNames[i];
                    var values = collection["CustomSkuV-" + skuId[i]].Split(new[] { "," }, StringSplitOptions.RemoveEmptyEntries);
                    var vids = collection["CustomSkuV-Id-" + skuId[i]].Split(new[] { "," }, StringSplitOptions.RemoveEmptyEntries);
                    if (values.Length > 0)
                    {
                        r.Add(new CustomSkuModel
                        {
                            Name = name.Trim(),
                            Values = values,
                            Id = skuId[i],
                            KeyIds = vids.Take(values.Length).ToArray()
                        });
                    }
                }

                if (r.Count > 0)
                {
                    var rows = 1;
                    r.ForEach(e =>
                    {
                        rows = e.Values.Count * rows;
                    });
                    if (rows <= 600)
                    {
                        ViewData["Rows"] = rows;
                        return PartialView(r);
                    }

                    return Content("false-sku超限");
                }

                return Content("false-skuid:" + skuId.ContactString("^") + "names:" + skuNames.ContactString("^"));
            }

            return Content("false-skuid:" + collection["CustomSkuId"]);
        }

        public ActionResult CustomPropTeplate()
        {
            if (!Request.Form["edit"].IsEmpty())
            {
                return PartialView(Request.Form["edit"].Deserialize<IList<ItemSpecInDetail>>());
            }
            return PartialView();
        }

        public ActionResult ExpressFare()
        {
            var t = YunClient.Instance.Execute(new GetExpressTemplatesRequest(), Token).FreightTemplates;
            return PartialView(t);
        }

        public ActionResult CouponFare()
        {
            var t = YunClient.Instance.Execute(new GetCouponTemplatesRequest(), Token).CouponTemplates;
            return PartialView(t);
        }

        public ActionResult LoadAddCommodityTemlate(int id, int itemId = 0)
        {
            var allprops = new List<int>();

            if (itemId > 0)
            {
                var detail = YunClient.Instance.Execute(new GetItemRequest { Id = itemId }).Item;
                ViewData["LoadedItem"] = detail;
                if (detail != null)
                {
                    if (detail.ItemProps != null && detail.ItemProps.Any())
                    {
                        detail.ItemProps.ForEach(e => e.Values.ForEach(c => allprops.Add((int) c.Id)));
                    }

                    var allThumbs = "";
                    (detail.Pictures ?? "").Split(new[] {","}, StringSplitOptions.RemoveEmptyEntries).ForEach(s =>
                    {
                        allThumbs += string.Format("{0}^{1},", s.Substring(s.LastIndexOf('/') + 1), s);
                    });
                    detail.Pictures = allThumbs;

                    if (detail.ItemShopCats == null)
                    {
                        detail.ItemShopCats = new List<ItemShopCat>();
                    }

                    if (detail.Partners != null && detail.Partners.Any())
                    {
                        var shopReq =
                            YunClient.Instance.Execute(new GetShopsRequest {Ids = string.Join(",", detail.Partners)})
                                .Shops;
                        if (shopReq != null && shopReq.Any())
                        {
                            ViewData["SuitShops"] = shopReq.ToDictionary(e => e.Id, e => e.Title).Serialize();
                        }
                    }

                    ViewData["SpecNames"] = detail.ItemSpecs != null ? detail.ItemSpecs.Serialize() : "";

                    if (detail.AdditionalInfo != null && detail.AdditionalInfo.IndexOf('-') > 0)
                    {
                        ViewData["MinFxPrice"] = detail.AdditionalInfo.Split('-')[0];
                        ViewData["MaxFxPrice"] = detail.AdditionalInfo.Split('-')[1];
                    }

                    ViewData["SkuValues"] = detail.ItemSkus != null ? detail.ItemSkus.Serialize() : "";
                }
            }

            ViewData["PropValueIds"] = allprops;
            return PartialView();
        }

        public ActionResult NotSold(int p = 1)
        {
            var page = new PageModel<GoodsDetail>();
            var req =
                YunClient.Instance.Execute(
                    new GetItemsRequest
                    {
                        ItemState = 2,
                        PageNum = p,
                        PageSize = 20,
                        Fields = "Id,SaleType,Picture,ItemTitle,Price,UpdateTime,Sales",
                        Sorts = "sortdesc",
                        IsDelete = 0
                    });

            page.CurrentPage = p;
            page.Items = req.Items;
            page.TotalItems = req.TotalItem;

            return View(page);
        }

        /// <summary>
        /// 出售中的商品
        /// </summary>
        /// <returns></returns>
        public ActionResult Index(string id, string title, string itemcode, int p = 1)
        {
            var page = new PageModel<GoodsDetail>();
            var req =
                YunClient.Instance.Execute(
                    new GetItemsRequest
                    {
                        Ids=id,
                        ItemTitle = title,
                        ItemState = 1,
                        PageNum = p,
                        PageSize = 20,
                        Fields = "Id,SaleType,Picture,ItemTitle,Price,UpdateTime,Sales",
                        Sorts = "sortdesc",
                        IsDelete = 0
                    });

            page.CurrentPage = p;
            page.Items = req.Items;
            page.TotalItems = req.TotalItem;

            return View(page);
        }

        public ActionResult Off(string id)
        {
            var request = YunClient.Instance.Execute(new OffsheIfItemsRequest {ItemIds = id}, Token);
            return Json(request.Result);
        }

        public ActionResult OnsheIf(string id)
        {
            var request = YunClient.Instance.Execute(new OnsheIfItemRequest { ItemId = id }, Token);
            return Json(request.Result);
        }

        /// <summary>
        /// 发布商品第一步，选择全局分类
        /// </summary>
        /// <returns></returns>
        public ActionResult Publish()
        {
            return View();
        }


        private string GetImages(string imgStr)
        {
            if (!imgStr.IsEmpty())
            {
                var thumb = imgStr.Trim(',').Split(new[] { "," }, StringSplitOptions.RemoveEmptyEntries);
                var hasnotadd = new List<string>();
                thumb.Where(e => e.IndexOf('^') > 0).ForEach(s => hasnotadd.Add(s.Split('^')[1]));
                return hasnotadd.ContactString(",");
            }

            return "";
        }

     
        public ActionResult Edit(int id)
        {
            var d = YunClient.Instance.Execute(new GetItemRequest {Id = id, Fields = "ItemCats"}).Item;
            if (d == null)
            {
                TempData["error"] = "您需要编辑的商品不存在，请返回后重新选择!";
            }

            return View(d);
        }

        public IDictionary<long, string> GetPrevCatsInShop(int catId)
        {
            return
                YunClient.Instance.Execute(new GetPrevsShopItemCategoryRequest {Id = catId}, Token).Categories
                    .ToDictionary(e => e.Key, e => e.Value);
        }

        private IList<long> FilterAllCategoriesInShop(string catids)
        {
            var r = new List<long>();
            if (catids.IsNotNullOrEmpty())
            {
                var ids = catids.Split(new[] { "," }, StringSplitOptions.RemoveEmptyEntries);
                foreach (var id in ids)
                {
                    r.AddRange(GetPrevCatsInShop(id.TryTo(0)).Keys);
                }
            }

            return r.Distinct().ToList();
        }

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult Edit(int id, FormCollection collection)
        {
            var cid = 0;
            var buylimit = collection["BuyLimit"];
            var title = collection["Product"];
            var subTitle = collection["Feature"];
            var msgTitle = collection["CouponMessage"];
            var saletype = collection["GoodsType"];
            var itemcode = collection["OuterId"];
            var price = collection["Price"];
            var volume = collection["Volume"];
            var weight = collection["Weight"];
            var marketPrice = collection["MarketPrice"];
            var stock = collection["MaxStore"].IsEmpty() ? "99999" : collection["MaxStore"];
            var imgs = GetImages(collection["Thumbs"]);
            var postType = collection["PostType"];
            var onTime = collection["PostTime"];
            var couponTemplateId = collection["CouponFareType"].IsEmpty() ? "0" : collection["CouponTemplate"];
            var expressTemplateId = collection["FareType"].IsEmpty() ? "0" : collection["FareTemplate"];
            var catsinshop = collection["ShopCategorys"];
            var propValues = collection["PropValue"];

            var detail = collection["Detail"];
            var seoTitle = collection["SeoTitle"];
            var seoKeywords = collection["SeoKeywords"];
            var seoDescription = collection["SeoDescription"];
            var expiredType = collection["ExpiredType"].TryTo(0);

            var suitShops =
                collection["SuitShop"].IsEmpty()
                    ? ""
                    : collection["SuitShop"].Deserialize<IDictionary<int, string>>()
                        .Select(e => e.Key)
                        .ToArray()
                        .ContactString(",");

            var expiredTime = 0;
            if (!collection["ExpiredTime"].IsEmpty())
            {
                expiredTime = collection["ExpiredTime"].TryTo<string, DateTime>().ToUnix();
            }

            if (expiredType == 1)
            {
                expiredTime = collection["ExpiredLastTime"].TryTo<string, DateTime>().ToUnix();
            }
            var extItemCatIds = collection["extItemCatIds"];
            var isrecomm = collection["IsRecomm"];
            var additionInfo = ProcessSkuFxPrice(collection["MinFxPrice"], collection["MaxFxPrice"]);
            List<string> customNames;
            var customSkus = GetCustomSkus(collection, out customNames);
            var r = YunClient.Instance.Execute(new UpdateItemRequest
            {
                ExtItemCatIds = extItemCatIds,
                BuyLimit = buylimit.TryTo(0),
                Volume = volume.TryTo(0.0),
                Weight = weight.TryTo(0.0),
                AdditionalInfo = additionInfo,
                CouponTemplateId = couponTemplateId.TryTo(0),
                ItemPropValues = propValues,
                CustomSpecNames = customNames.Serialize(),
                CustomSkus = customSkus.Serialize(),
                Description = detail,
                FreightTemplateId = expressTemplateId.TryTo(0),
                ItemCode = itemcode,
                IsRecommend = isrecomm.TryTo(0),
                ItemTitle = title,
                MsgTitle = msgTitle,
                OnShelfTime = onTime.IsEmpty() ? 0 : onTime.TryTo<string, DateTime>().ToUnix(),
                OnShelfType = postType.TryTo(0),
                SaleType = saletype == "goods" ? 1 : 3,
                SubTitle = subTitle,
                Price = price.TryTo(0.0),
                Pictures = imgs,
                Stock = stock.TryTo(0),
                ItemPartnerIds = suitShops,
                MarketPrice = marketPrice.TryTo(0.0),
                ShopCatIds = FilterAllCategoriesInShop(catsinshop).ContactString(","),
                ItemCatId = cid,
                ExpireTime = expiredTime,
                ItemId = id,
                SeoDescription = seoDescription,
                SeoKeyword = seoKeywords,
                SeoTitle = seoTitle,
                ExpireRule = expiredType,
                ExpireDays = collection["DuringDay"].TryTo(0),
                ExpireStart = collection["ExpiredStartTime"].TryTo<string, DateTime>().ToUnix(),
                SortOrder = collection["SortOrder"].TryTo(0)
            }, Token);

            return Content(r.IsError ? r.ErrMsg : r.Result ? 1.ToString() : "修改失败");
        }


        [HttpPost]
        [ValidateInput(false)]
        public ActionResult Publish(FormCollection collection)
        {

            var title = collection["Product"];
            var subTitle = collection["Feature"];
            var msgTitle = collection["CouponMessage"];
            var saletype = collection["GoodsType"];
            var itemcode = collection["OuterId"];
            var price = collection["Price"];
            var marketPrice = collection["MarketPrice"];
            var stock = collection["MaxStore"].IsEmpty() ? "99999" : collection["MaxStore"];
            var imgs = GetImages(collection["Thumbs"]);
            var postType = collection["PostType"];
            var onTime = collection["PostTime"];
            var couponTemplateId = collection["CouponFareType"].IsEmpty() ? "0" : collection["CouponTemplate"];
            var expressTemplateId = collection["FareType"].IsEmpty() ? "0" : collection["FareTemplate"];
            var catsinshop = collection["ShopCategorys"];
            var propValues = collection["PropValue"];
            var seoTitle = collection["SeoTitle"];
            var seoKeywords = collection["SeoKeywords"];
            var seoDescription = collection["SeoDescription"];
            var volume = collection["Volume"];
            var weight = collection["Weight"];
            var detail = collection["Detail"];
            var expiredTime = 0;
            if (!collection["ExpiredTime"].IsEmpty())
            {
                expiredTime = collection["ExpiredTime"].TryTo<string, DateTime>().ToUnix();
            }

            var expiredType = collection["ExpiredType"].TryTo(0);
            if (expiredType == 1)
            {
                expiredTime = collection["ExpiredLastTime"].TryTo<string, DateTime>().ToUnix();
            }

            var extItemCatIds = collection["extItemCatIds"];
            var suitShops =
                collection["SuitShop"].IsEmpty()
                    ? ""
                    : collection["SuitShop"].Deserialize<IDictionary<int, string>>()
                        .Select(e => e.Key)
                        .ToArray()
                        .ContactString(",");

            var isrecomm = collection["IsRecomm"];
            var additionInfo = ProcessSkuFxPrice(collection["MinFxPrice"], collection["MaxFxPrice"]);
            List<string> customNames;
            var customSkus = GetCustomSkus(collection, out customNames);
            var buylimit = collection["BuyLimit"];
            var r = YunClient.Instance.Execute(new AddItemRequest
            {
                ExtItemCatIds = extItemCatIds,
                BuyLimit = buylimit.TryTo(0),
                Volume = volume.TryTo(0.0),
                Weight = weight.TryTo(0.0),
                AdditionalInfo = additionInfo,
                CouponTemplateId = couponTemplateId.TryTo(0),
                ItemPropValues = propValues,
                CustomSpecNames = customNames.Serialize(),
                CustomSkus = customSkus.Serialize(),
                Description = detail,
                FreightTemplateId = expressTemplateId.TryTo(0),
                ItemCode = itemcode,
                IsRecommend = isrecomm.TryTo(0),
                ItemTitle = title,
                MsgTitle = msgTitle,
                OnShelfTime = onTime.IsEmpty() ? 0 : onTime.TryTo<string, DateTime>().ToUnix(),
                OnShelfType = postType.TryTo(0),
                SaleType = saletype == "goods" ? 1 : 3,
                SubTitle = subTitle,
                Price = price.TryTo(0.0),
                Pictures = imgs,
                Stock = stock.TryTo(0),
                ItemPartnerIds = suitShops,
                MarketPrice = marketPrice.TryTo(0.0),
                ShopCatIds = FilterAllCategoriesInShop(catsinshop).ContactString(","),
                ItemCatId = 0,
                ExpireTime = expiredTime,
                SeoDescription = seoDescription,
                SeoKeyword = seoKeywords,
                SeoTitle = seoTitle,
                ExpireRule = expiredType,
                ExpireDays = collection["DuringDay"].TryTo(0),
                ExpireStart = collection["ExpiredStartTime"].TryTo<string, DateTime>().ToUnix(),
                SortOrder = collection["SortOrder"].TryTo(0)
            }, Token);

            TempData["success"] = "“" + title + "”商品已成功添加";
            return Content(r.IsError ? r.ErrMsg : r.Result.ToString());
        }

        /// <summary>
        /// 删除商品
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult Delete(string id)
        {
            var r = YunClient.Instance.Execute(new DeleteItemRequest {ItemId = id}, Token);
            return Json(r.Result);
        }


        /// <summary>
        /// 店铺内商品分类列表
        /// </summary>
        /// <returns></returns>
        public ActionResult Category()
        {
            var r = YunClient.Instance.Execute(new GetShopItemCategorysRequest
            {
                Display = null,
                ParentId = null,
                ShopId = GlobeInfo.InitiatedShopId,
            }, Token).Categorys;

            return View(r);
        }

        /// <summary>
        /// 新增店铺内商品分类
        /// </summary>
        /// <returns></returns>
        public ActionResult AddCategory()
        {
            return View();
        }

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult AddCategory(AddItemCategoryModel model, int id = 0)
        {
            if (ModelState.IsValid)
            {
                var r = YunClient.Instance.Execute(new AddShopItemCategoryRequest
                {
                    Title = model.Title,
                    Display = Request.Form["Display"].TryTo(0) == 1,
                    Image = FileManage.GetFirstFile(),
                    ParentId = id,
                    Sort = model.Sort
                }, Token);

                if (r.Result > 0)
                {
                    TempData["success"] = "已成功添加“" + model.Title + "” 分类";
                    return RedirectToAction("Category");
                }
            }

            TempData["error"] = "新增商品分类失败，请刷新后重试";
            return View(model);
        }


        /// <summary>
        /// 编辑店铺内商品分类
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult EditCategory(int id)
        {
            var m = YunClient.Instance.Execute(new GetShopItemCategoryRequest
            {
                Id = id
            }, Token).Category;

            if (m != null)
            {
                return View(new UpdateItemCategoryModel
                {
                    Title = m.Title,
                    Display = m.Display,
                    Sort = (int) m.Sort,
                    Image = m.Image,
                    ParentId = (int) m.ParentId
                });
            }

            TempData["error"] = "该商品分类不存在";
            return View();
        }

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult EditCategory(UpdateItemCategoryModel model, int id)
        {
            if (ModelState.IsValid)
            {
                var r = YunClient.Instance.Execute(new UpdateShopItemCategoryRequest
                {
                    Id = id,
                    Title = model.Title,
                    Display = Request.Form["IsDisplay"].TryTo(0) == 1,
                    Image = model.Image,
                    NewImage = FileManage.GetFirstFile(),
                    Sort = model.Sort
                }, Token);

                if (r.Result)
                {
                   
                    TempData["success"] = "更新商品分类成功";
                    return RedirectToAction("Category");
                }
            }

            return View(model);
        }

        /// <summary>
        /// 删除商品分类
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult DeleteCategory(int id)
        {
            return Json(YunClient.Instance.Execute(new DeleteShopItemCategoryRequest {Id = id}, Token));
        }


        /// <summary>
        /// 删除内容
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult DeleteQuestion(int id)
        {
            return Json(true);
        }

        private static string ProcessSkuFxPrice(string min, string max)
        {
            if (min.IsNotNullOrEmpty() && max.IsNotNullOrEmpty())
            {
                return string.Format("{0}-{1}", min, max);
            }

            if (min.IsNotNullOrEmpty())
            {
                return string.Format("{0}-{1}", min, 0);
            }

            if (max.IsNotNullOrEmpty())
            {
                return string.Format("{0}-{1}", 0, max);
            }

            return "";
        }

        private IList<IList<string>> GetCustomSkus(FormCollection collection, out List<string> outSkuNames)
        {
            var rData = new List<IList<string>>();
            //6c2f4272,dbb9e417
            var tempskuid = (collection["CustomSkuId"] ?? "").Split(new[] { "," }, StringSplitOptions.RemoveEmptyEntries);
            //dbb9e417
            var skuId = tempskuid;
            //处理skuid的数量
            if (tempskuid.Any())
            {
                var skuNames = new List<string>();
                tempskuid.ForEach(s =>
                {
                    var name = collection["CustomSkuN-" + s];
                    if (name.IsEmpty())
                    {
                        skuId = skuId.Where(e => e != s).ToArray();
                    }
                    else
                    {
                        skuNames.Add(name);
                    }
                });

                outSkuNames = skuNames;

                //将列转换成行
                var skuValueModels = new List<CustomSkuValueModel>();
                for (var i = 0; i < skuNames.Count; i++)
                {
                    //1
                    var name = skuNames[i];
                    var values = collection["CustomSkuV-" + skuId[i]].Split(new[] { "," }, StringSplitOptions.RemoveEmptyEntries);
                    if (values.Length > 0)
                    {
                        skuValueModels.Add(new CustomSkuValueModel
                        {
                            Name = name.Trim(),
                            Values = values,
                        });
                    }
                }

                var counter = new int[skuValueModels.Count];
                var prevValue = new string[skuValueModels.Count];

                if (skuValueModels.Count > 0 && skuValueModels.Count<=4)
                {
                    if (skuValueModels.Count == 1)
                    {
                        for (var i = 0; i < skuValueModels[0].Values.Count; i++)
                        {
                            rData.Add(new List<string>
                            {
                                skuValueModels[0].Values[i],
                                collection["SkuPrice-" + i],
                                collection["SkuMaxStore-" + i],
                                collection["SkuItemCode-" + i],
                                ProcessSkuFxPrice(collection["SkuFxMin-" + i],collection["SkuFxMax-" + i]),
                                collection["SkuMarketPrice-" + i]
                            });
                        }
                    }

                    if (skuValueModels.Count == 2)
                    {
                        var rows = skuValueModels[0].Values.Count*skuValueModels[1].Values.Count;
                        for (var i = 0; i < rows; i++)
                        {
                            rData.Add(new List<string>
                            {
                                GetColValue(0,i,counter,skuValueModels,rows,prevValue),
                                GetColValue(1,i,counter,skuValueModels,rows,prevValue),
                                collection["SkuPrice-" + i],
                                collection["SkuMaxStore-" + i],
                                collection["SkuItemCode-" + i],
                                ProcessSkuFxPrice(collection["SkuFxMin-" + i],collection["SkuFxMax-" + i]),
                                collection["SkuMarketPrice-" + i]
                            });
                        }
                    }

                    if (skuValueModels.Count == 3)
                    {
                        var rows = skuValueModels[0].Values.Count * skuValueModels[1].Values.Count * skuValueModels[2].Values.Count;
                        for (var i = 0; i < rows; i++)
                        {
                            rData.Add(new List<string>
                            {
                                GetColValue(0,i,counter,skuValueModels,rows,prevValue),
                                GetColValue(1,i,counter,skuValueModels,rows,prevValue),
                                GetColValue(2,i,counter,skuValueModels,rows,prevValue),
                                collection["SkuPrice-" + i],
                                collection["SkuMaxStore-" + i],
                                collection["SkuItemCode-" + i],
                                ProcessSkuFxPrice(collection["SkuFxMin-" + i],collection["SkuFxMax-" + i]),
                                collection["SkuMarketPrice-" + i]
                            });
                        }
                    }

                    if (skuValueModels.Count == 4)
                    {
                        var rows = skuValueModels[0].Values.Count * skuValueModels[1].Values.Count * skuValueModels[2].Values.Count * skuValueModels[3].Values.Count;
                        for (var i = 0; i < rows; i++)
                        {
                            rData.Add(new List<string>
                            {
                                GetColValue(0,i,counter,skuValueModels,rows,prevValue),
                                GetColValue(1,i,counter,skuValueModels,rows,prevValue),
                                GetColValue(2,i,counter,skuValueModels,rows,prevValue),
                                GetColValue(3,i,counter,skuValueModels,rows,prevValue),
                                collection["SkuPrice-" + i],
                                collection["SkuMaxStore-" + i],
                                collection["SkuItemCode-" + i],
                                ProcessSkuFxPrice(collection["SkuFxMin-" + i],collection["SkuFxMax-" + i]),
                                collection["SkuMarketPrice-" + i]
                            });
                        }
                    }

                    return rData;
                }
            }

            outSkuNames = new List<string>();
            return null;
        }

        private static string GetColValue(int currentCol, int currentRow, int[] counter, List<CustomSkuValueModel> model,
            int rows, string[] prevValue)
        {
            var rowsspan = 1;
            for (var i = 0; i <= currentCol; i++)
            {
                rowsspan = rowsspan*model[i].Values.Count;
            }
            rowsspan = rows/rowsspan;

            if ((currentRow + rowsspan)%rowsspan == 0)
            {
                prevValue[currentCol] = model[currentCol].Values[counter[currentCol]];
                counter[currentCol]++;
                if (counter[currentCol] == model[currentCol].Values.Count)
                {
                    counter[currentCol] = 0;
                }
            }

            return prevValue[currentCol];
        }

        private static readonly int _brandSpecialType = 1;

        public ActionResult Brand(int p = 1)
        {
            var req = YunClient.Instance.Execute(new GetItemPropValuesRequest
            {
                PageNum = p,
                PageSize = 20,
                SpecialType = _brandSpecialType,
            }, Token);

            return View(new PageModel<ItemPropValue>
            {
                CurrentPage = p,
                Items = req.PropValues,
                ItemsPerPage = 20,
                TotalItems = req.TotalItem,
            });
        }

        /// <summary>
        /// 新增/编辑品牌
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult AddBrand(int id = 0)
        {
            return View();
        }

        /// <summary>
        /// 新增/编辑品牌
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult AddBrand(AddSpuModel model ,int id = 0)
        {
            if (ModelState.IsValid)
            {
                
            }

            return View(model);
        }

        public ActionResult DeleteItemPropValue(int id)
        {
            return Json(true);
        }

    }
}
