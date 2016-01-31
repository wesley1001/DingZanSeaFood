using System;
using System.Web.Mvc;
using BreezeShop.Core;
using BreezeShop.Web.Areas.Admin.Models;
using Yun.Item.Request;
using Yun.Marketing.Request;
using Yun.Response;
using Yun.User;
using Yun.User.Request;

namespace BreezeShop.Web.Areas.Admin.Controllers
{
    public class CashCouponController : AdminAuthController
    {
        private static DateTime? ConvertBeginTimeStatus(int timeStatus)
        {
            switch (timeStatus)
            {
                case 2: return DateTime.Now;
            }

            return null;
        }

        private static DateTime? ConvertEndTimeStatus(int timeStatus)
        {
            switch (timeStatus)
            {
                case 1: return DateTime.Now;
                case 3: return DateTime.Now;
            }

            return null;
        }

        public ActionResult Index(string title = "", int timeStatus = 0, int p = 1)
        {
            var page = new PageModel<Yun.Marketing.CashCouponCateogry>();

            var req =
                YunClient.Instance.Execute(new FindCashCouponCategoriesRequest
                {
                    PageSize = 20,
                    PageNum = p,
                    Name = title,
                    ValidityPeriod = timeStatus
                });

            page.Items = req.CashCouponCateogries;
            page.CurrentPage = p;
            page.TotalItems = req.TotalItem;

            return View(page);
        }

        public ActionResult Add(int id = 0)
        {
            ViewData["Items"] = YunClient.Instance.Execute(new GetItemsRequest {Fields = "id,itemtitle", PageNum = 1, PageSize = 100}).Items;

            var m = new AddCashCouponModel();
            if (id <= 0) return View(m);

            var r = YunClient.Instance.Execute(new GetCashCouponCategoryRequest { CategoryId = id }).CashCouponCateogry;
            m.Description = r.Description;
            m.ItemsId = r.RelateObjectId;
            m.Status = r.Status;
            m.UseCustom = r.UseCustom;
            m.BeginTime = DateTime.Parse(r.BeginTime);
            m.EndTime = DateTime.Parse(r.EndTime);
            m.CashType = r.CouponType;
            m.Credit = r.Credit;
            m.MinCredit = r.MinPrice;
            m.Name = r.Name;
            m.PerUserMaxQuantity = r.PerUserMaxQuantity;
            m.Num = r.Quantity;

            return View(m);
        }

        [HttpPost]
        public ActionResult Add(AddCashCouponModel model, int id = 0)
        {
            if (ModelState.IsValid)
            {
                IntResultResponse r;

                if (id <= 0)
                {
                    r = YunClient.Instance.Execute(new AddCashCouponCategoryRequest
                    {
                        Num = model.Num,
                        Name = model.Name,
                        BeginTime = model.BeginTime,
                        EndTime = model.EndTime,
                        Credit = model.Credit,
                        CouponType = model.CashType,
                        MinPrice = model.MinCredit,
                        Range = string.IsNullOrEmpty(model.ItemsId) ? 0 : 1,
                        Description = model.Description,
                        ItemsId = model.ItemsId,
                        UseCustom = model.UseCustom,
                        PerUserMaxQuantity = model.PerUserMaxQuantity,
                        Status = model.Status
                    }, Token);
                }
                else
                {
                    r = YunClient.Instance.Execute(new UpdateCashCouponCategoryRequest()
                    {
                        Num = model.Num,
                        Name = model.Name,
                        BeginTime = model.BeginTime,
                        EndTime = model.EndTime,
                        Credit = model.Credit,
                        CouponType = model.CashType,
                        MinPrice = model.MinCredit,
                        Range = string.IsNullOrEmpty(model.ItemsId) ? 0 : 1,
                        Description = model.Description,
                        ItemsId = model.ItemsId,
                        UseCustom = model.UseCustom,
                        PerUserMaxQuantity = model.PerUserMaxQuantity,
                        Status = model.Status,
                        CategoryId = id
                    }, Token);
                }

                if (r.Result > 0)
                {
                    TempData["success"] = "代金券数据提交成功";
                    return RedirectToAction("Index");
                }


                TempData["error"] = "代金券生成失败，错误代码：" + r.ErrMsg;
            }

            ViewData["Items"] =
                YunClient.Instance.Execute(new GetItemsRequest
                {
                    Fields = "id,itemtitle",
                    PageNum = 1,
                    PageSize = 100
                }).Items;
            return View(model);
        }

        public ActionResult DeleteCategory(int id)
        {
            var req = YunClient.Instance.Execute(new DeleteCashCouponCategoryRequest {CategoryId = id}, Token).Result;

            return Json(req);
        }

        public ActionResult Delete(int id)
        {
            var r = YunClient.Instance.Execute(new DeleteCashCouponRequest {Id = id}, Token).Result;

            return Json(r);
        }

        public ActionResult BindUser(int id, string email, string mobile, string nick, DateTime? minregtime, DateTime? maxregtime,
            double? minmoney,
            double? maxmoney, long? minscore, long? maxscore, double minprepaid = 0, double maxprepaid = 0, int p = 1)
        {
            var page = new PageModel<UserDetail>();
            var req = YunClient.Instance.Execute(new FindUsersRequest
            {
                Email = email,
                Mobile = mobile,
                Nick = nick,
                MinMoney = minmoney,
                MaxMoney = maxmoney,
                MinScore = minscore,
                MaxScore = maxscore,
                MinRegTime = minregtime,
                MaxRegTime = maxregtime,
                PageNum = p,
                PageSize = 20,
                MinPrepaidCard = minprepaid,
                MaxPrepaidCard = maxprepaid
            });

            page.Items = req.Users;
            page.CurrentPage = p;
            page.TotalItems = req.TotalItem;

            return View(page);
        }


        public ActionResult Bind(int id, int userid)
        {
            return Json(YunClient.Instance.Execute(new BindCashCouponRequest {Id = id, UserId = userid}).Result);
        }

        public ActionResult Detail(int id, int p = 1)
        {
            var cat = YunClient.Instance.Execute(new GetCashCouponCategoryRequest {CategoryId = id}).CashCouponCateogry;
            if (cat == null)
            {
                TempData["error500"] = "代金券不存在";
                return RedirectToAction("Error500", "Home");
            }

            ViewData["CatName"] = cat.Name;

            var page = new PageModel<Yun.Marketing.CashCoupon>();
            var req = YunClient.Instance.Execute(new FindCashCouponRequest { PageSize = 20, PageNum = p, CategoryId = id});
            page.Items = req.CashCoupons;
            page.CurrentPage = p;
            page.TotalItems = req.TotalItem;

            return View(page);
        }
    }
}
