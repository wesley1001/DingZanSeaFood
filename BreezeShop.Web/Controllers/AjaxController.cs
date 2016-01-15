using System.Web.Mvc;
using BreezeShop.Core;
using BreezeShop.Core.DataProvider;
using Yun.Trade.Request;

namespace BreezeShop.Web.Controllers
{
    public class AjaxController : MemberAuthController
    {
        public ActionResult DeleteCart(string cartId)
        {
            var r =
                YunClient.Instance.Execute(new RemoveCartRequest {CartIds = cartId},
                    Member.Token);

            return Json(new {error = r.ErrMsg, cartid = r.Result}, JsonRequestBehavior.AllowGet);
        }

        public ActionResult UpdateCard(int cartId, int quantity)
        {
            var r = YunClient.Instance.Execute(new UpdateCartRequest
            {
                CartId = cartId,
                Quantity = quantity
            }, Member.Token);

            return Json(new { error = r.ErrMsg, result = r.Result }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult AddCart(int itemId, int quantity, string delivery, int skuId = 0)
        {
            var r = YunClient.Instance.Execute(new AddToCartRequest
            {
                ItemId = itemId,
                Quantity = quantity,
                SkuId = skuId,
                Delivery = delivery
            }, Member.Token);

            return r.Result > 0
                ? Json(new {cartid = r.Result}, JsonRequestBehavior.AllowGet)
                : Json(new {error = r.ErrMsg, cartid = r.Result}, JsonRequestBehavior.AllowGet);
        }

    }
}
