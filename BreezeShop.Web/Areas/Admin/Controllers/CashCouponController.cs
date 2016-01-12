using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using BreezeShop.Core;
using BreezeShop.Web.Areas.Admin.Models;
using Yun.Item.Request;
using Yun.Marketing.Request;
using Yun.User;
using Yun.User.Request;

namespace BreezeShop.Web.Areas.Admin.Controllers
{
    public class CashCouponController : AdminAuthController
    {
        public ActionResult Index(int p = 1)
        {
            var page = new PageModel<Yun.Marketing.CashCouponDomain>();

            var req = YunClient.Instance.Execute(new FindCashCouponRequest {PageSize = 20, PageNum = p});

            page.Items = req.CashCoupons;
            page.CurrentPage = p;
            page.TotalItems = req.TotalItem;

            return View(page);
        }

        public ActionResult Add()
        {
            ViewData["Items"] = YunClient.Instance.Execute(new GetItemsRequest {Fields = "id,itemtitle", PageNum = 1, PageSize = 100}).Items;

            return View();
        }

        [HttpPost]
        public ActionResult Add(AddCashCouponModel model)
        {
            if (ModelState.IsValid)
            {
                var r = YunClient.Instance.Execute(new GenerateCashCouponRequest
                {
                    Num = model.Num,
                    Name = model.Name,
                    BeginTime = model.BeginTime,
                    EndTime = model.EndTime,
                    Credit = model.Credit,
                    CouponType = model.CashType,
                    MinPrice = model.MinCredit,
                    ItemdsId = model.ItemsId,
                    Range = string.IsNullOrEmpty(model.ItemsId)?0:1
                }, Token);

                if (!string.IsNullOrWhiteSpace(r.Result))
                {
                    TempData["success"] = "代金券生成成功";
                    return RedirectToAction("Index");
                }

                ViewData["Items"] = YunClient.Instance.Execute(new GetItemsRequest { Fields = "id,itemtitle", PageNum = 1, PageSize = 100 }).Items;
                TempData["error"] = "代金券生成失败，错误代码：" + r.ErrMsg;
            }

            return View(model);
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
    }
}
