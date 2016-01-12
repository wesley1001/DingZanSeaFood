
namespace BreezeShop.Web.Areas.Admin.Models
{
    public class ExportTradeFormat
    {
        /// <summary>
        /// 订单ID
        /// </summary>
        public int OrderId { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        public string CreateTime { get; set; }

        /// <summary>
        /// 支付时间
        /// </summary>
        public string PayTime { get; set; }

        /// <summary>
        /// 商品标题
        /// </summary>
        public string ItemTitle { get; set; }

        /// <summary>
        /// 商品规格
        /// </summary>
        public string Sku { get; set; }

        /// <summary>
        /// 商品价格
        /// </summary>
        public double Price { get; set; }

        /// <summary>
        /// 购买数量
        /// </summary>
        public int Quantity { get; set; }

        /// <summary>
        /// 支付的金额
        /// </summary>
        public double Money { get; set; }

        /// <summary>
        /// 总金额
        /// </summary>
        public double TotalMoney { get; set; }

        /// <summary>
        /// 收货人姓名
        /// </summary>
        public string RealName { get; set; }

        /// <summary>
        /// 收货人地址
        /// </summary>
        public string Address { get; set; }

        /// <summary>
        /// 手机号
        /// </summary>
        public string Mobile { get; set; }

        /// <summary>
        /// 买家留言
        /// </summary>
        public string Remark { get; set; }

        /// <summary>
        /// 交易状态
        /// </summary>
        public string TradeStatus { get; set; }

        /// <summary>
        /// 卖家留言
        /// </summary>
        public string SellerMemo { get; set; }
    }
}