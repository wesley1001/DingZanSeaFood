using System.Web.Mvc;
using BreezeShop.Core;
using BreezeShop.Core.DataProvider;
using Yun.Distribution.Request;
using Yun.Marketing.Request;

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

    }
}
