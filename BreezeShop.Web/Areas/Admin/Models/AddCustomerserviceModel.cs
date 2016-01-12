using System.ComponentModel.DataAnnotations;

namespace BreezeShop.Web.Areas.Admin.Models
{
    public class AddCustomerServiceModel
    {
        [Required(ErrorMessage = "请输入客服昵称")]
        [Display(Name = "客服昵称")]
        public string Nick { get; set; }

        [Display(Name="是否是女性")]
        public int Sex { get; set; }

        [Display(Name="联系电话")]
        public string Phone { get; set; }

        [Required(ErrorMessage = "请输入QQ")]
        [Display(Name = "QQ")]
        public string Qq { get; set; }

        [Display(Name = "旺旺")]
        public string Wangwang { get; set; }

        [Display(Name = "排序")]
        [Required(ErrorMessage = "请输入排序")]
        public double Sort { get; set; }
    }
}