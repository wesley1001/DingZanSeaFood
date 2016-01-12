using System.ComponentModel.DataAnnotations;

namespace BreezeShop.Web.Areas.Admin.Models
{
    public class UpdateCustomerServiceModel
    {
        [Required(ErrorMessage = "请输入客服昵称")]
        [Display(Name = "客服昵称")]
        public string Nick { get; set; }

        public int Sex { get; set; }

        [Display(Name = "联系电话")]
        public string Phone { get; set; }

        [Display(Name = "QQ")]
        public string Qq { get; set; }

        [Display(Name = "旺旺")]
        public string Wangwang { get; set; }

        [Display(Name = "是否显示")]
        public bool Display { get; set; }

        [Display(Name = "排序")]
        public double Sort { get; set; }
    }
}