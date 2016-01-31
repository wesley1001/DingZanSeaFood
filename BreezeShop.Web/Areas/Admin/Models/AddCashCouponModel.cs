using System;
using System.ComponentModel.DataAnnotations;

namespace BreezeShop.Web.Areas.Admin.Models
{
    public class AddCashCouponModel
    {
        /// <summary>
        /// 需要生成的数量
        /// </summary>
        [Required(ErrorMessage = "请输入需要生成的数量")]
        public int Num { get; set; }

        /// <summary>
        /// 面值
        /// </summary>
        [Required(ErrorMessage = "请输入面值")]
        public double Credit { get; set; }

        /// <summary>
        /// 开始时间
        /// </summary>
        public DateTime? BeginTime { get; set; }

        /// <summary>
        /// 结束时间
        /// </summary>
        public DateTime? EndTime { get; set; }

        /// <summary>
        /// 代金券名字
        /// </summary>
        [Required(ErrorMessage = "代金券名字")]
        public string Name { get; set; }


        public string CashType { get; set; }


        /// <summary>
        /// 最低满足金额
        /// </summary>
        public double MinCredit { get; set; }

        public string ItemsId { get; set; }

        /// <summary>
        /// 描述
        /// </summary>
        public string Description { get; set; }


        /// <summary>
        /// 是否使用预生成券号
        /// </summary>
        public bool UseCustom { get; set; }

        /// <summary>
        /// 每人限领
        /// </summary>
        public int PerUserMaxQuantity { get; set; }

        public int Status { get; set; }
    }
}