
namespace BreezeShop.Web.Areas.Admin.Models
{
    public class OnlinePaymentModel
    {

        public string AlipayPartner { get; set; }

        public string AlipayKey { get; set; }

        public string SellerEmail { get; set; }


        public string TenpayBargainorId { get; set; }

        public string TenpayKey { set; get; }


        public string ShengpayMsgSender { get; set; }

        public string ShengpayKey { set; get; }



        public string BankingDirectConnect { get; set; }


        public string PaymentCode { get; set; }
    }
}