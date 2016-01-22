using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using BreezeShop.Core;
using BreezeShop.Core.DataProvider;
using Utilities.DataTypes.ExtensionMethods;
using Yun.Distribution.Request;
using Yun.Marketing.Request;
using Yun.Pay.Request;

namespace BreezeShop.Web.Controllers
{
    public class CenterController : MemberAuthController
    {
        public ActionResult Index()
        {
            var token = Member.Token;

            //获取用户信息
            var user = Member.GetLoginMember(token);

            //分销业绩
            ViewData["DistributorPerformance"] =
                YunClient.Instance.Execute(new GetUserDistributionPerformanceRequest(), token).DistributorPerformance;

            //上下级分销关系
            var sibling = YunClient.Instance.Execute(new GetMySiblingDistributorsRequest {UserId = (int) user.UserId});

            //下级
            ViewData["LowerDistributors"] = sibling.LowerDistributors;

            //上级
            ViewData["SuperiorDistributor"] = sibling.SuperiorDistributor;
            
            return View(user);
        }


        public ActionResult QrCode()
        {
            return View(Member.GetLoginMember());
        }

        public ActionResult MyCashCoupons()
        {
            var req =
                YunClient.Instance.Execute(new GetMyCashCouponsRequest {HasExpired = false, HasUsed = false},
                    Member.Token).CashCoupons;

            return View(req);
        }

        [HttpPost]
        public ActionResult MyAccount(FormCollection collection)
        {
            var realname = collection["realname"];
            var bank = collection["bank"];
            var subBranch = collection["SubBranch"];
            var accountName = collection["AccountName"];
            var bankAccount = collection["BankAccount"];
            var province = collection["province"];
            var city = collection["city"];
            var area = collection["area"];

            var banks = YunClient.Instance.Execute(new GetBandBanksRequest(), Member.Token).Banks;
            if (banks != null && banks.Any())
            {
                var r = YunClient.Instance.Execute(new ModifyUserBankCardRequest
                {
                    AccountName = accountName,
                    BankName = bank,
                    BankNum = bankAccount,
                    Location = string.Format("{0},{1},{2}", province, city, area),
                    Id = banks[0].Id,
                    RealName = realname,
                    SubBranch = subBranch
                }, Member.Token);

                return Json(r.Result);
            }

            var req = YunClient.Instance.Execute(new AddUserBankCardRequest
            {
                AccountName = accountName,
                BankName = bank,
                BankNum = bankAccount,
                Location = string.Format("{0},{1},{2}", province, city, area),
                RealName = realname,
                SubBranch = subBranch
            }, Member.Token);

            return Json(req.Result);
        }

        public ActionResult MyAccount()
        {
            var banks = YunClient.Instance.Execute(new GetBandBanksRequest(), Member.Token).Banks;


            var provinces = SystemCity.GetCities(0);

            var dataProvinces = new List<SelectListItem>();
            var dataCities = new List<SelectListItem>();
            var dataAreas = new List<SelectListItem>();

            if (banks != null && banks.Any())
            {
                var location = banks[0].Location.Split(',');

                dataProvinces.AddRange(provinces.Select(e => new SelectListItem
                {
                    Text = e.Name,
                    Value = e.Name + "-" + e.Id,
                    Selected = e.Name.IndexOf(location[0], StringComparison.CurrentCultureIgnoreCase) >= 0
                }));


                var cities = SystemCity.GetCities(
                    provinces.Single(
                        e => e.Name.IndexOf(location[0], StringComparison.CurrentCultureIgnoreCase) >= 0).Id);
                dataCities.AddRange(cities.Select(e => new SelectListItem
                {
                    Text = e.Name,
                    Value = e.Name + "-" + e.Id,
                    Selected = e.Name.IndexOf(location[1], StringComparison.CurrentCultureIgnoreCase) >= 0
                }));

                dataAreas.AddRange(SystemCity.GetCities(
                    cities.Single(e => e.Name.IndexOf(location[1], StringComparison.CurrentCultureIgnoreCase) >= 0).Id)
                    .Select(e => new SelectListItem
                    {
                        Text = e.Name,
                        Value = e.Name + "-" + e.Id,
                        Selected = e.Name.IndexOf(location[2], StringComparison.CurrentCultureIgnoreCase) >= 0
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

            return View(banks != null && banks.Any()?banks[0]:null);
        }

        public ActionResult Withdrawals()
        {
            var banks = YunClient.Instance.Execute(new GetBandBanksRequest(), Member.Token).Banks;
            if (banks != null && banks.Any())
            {
                ViewData["Money"] = Member.GetLoginMember().Money;
                return View(banks[0]);
            }

            return RedirectToAction("MyAccount");
        }

        [HttpPost]
        public ActionResult Withdrawals(FormCollection collection)
        {
            var money = collection["money"].TryTo(0.0);
            var bankId = collection["BankId"].TryTo(0);

            if (money <= 0 || bankId<=0)
            {
                return Json(-1);
            }

            var req = YunClient.Instance.Execute(new ApplyWithdrawalsRequest {BankId = bankId, Money = money},
                Member.Token);

            return Json(req.Result > 0 ? 1 : 0);
        }
    }
}
