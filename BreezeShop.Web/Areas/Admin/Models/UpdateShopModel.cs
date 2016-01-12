using System.ComponentModel.DataAnnotations;

namespace BreezeShop.Web.Areas.Admin.Models
{
    public class UpdateShopModel
    {
        [Required(ErrorMessage = "请输入店铺名称")]
        [Display(Name = "店铺名称")]
        public string Title { get; set; }

        [Display(Name = "联系电话")]
        public string Phone { get; set; }

        [Display(Name = "营业时间")]
        public string Hours { get; set; }

        [Display(Name = "地址")]
        public string Address { get; set; }

        [Display(Name = "简介")]
        public string Summary { get; set; }

        [Display(Name = "公告")]
        public string Bulletin { get; set; }

        [Display(Name = "描述")]
        public string Description { get; set; }

        public string Location { get; set; }

        /// <summary>
        /// 坐标
        /// </summary>
        public string Longlat { get; set; }

        public string Image { get; set; }
    }
}