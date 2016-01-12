using System.ComponentModel.DataAnnotations;

namespace BreezeShop.Web.Areas.Admin.Models
{
    public class UpdateMultipeInfoModel
    {
        [Required(ErrorMessage = "请输入标题")]
        [Display(Name = "标题")]
        public string Title { get; set; }

        public int ParentId { get; set; }

        [Display(Name = "是否显示")]
        public bool IsDisplay { get; set; }

        [Display(Name = "缩略图")]
        public string Image { get; set; }

        [Display(Name = "排序")]
        public double SortOrder { get; set; }

        [Display(Name = "超链接")]
        [Required(ErrorMessage = "请输入超链接")]
        public string HyperLink { get; set; }

        [Display(Name = "附加文本1")]
        public string AdditionalInfo { get; set; }

        [Display(Name = "附加文本2")]
        public string SecondAdditionalInfo { get; set; }

        [Display(Name = "附加文本3")]
        public string ThirdAdditionalInfo { get; set; }
    }
}