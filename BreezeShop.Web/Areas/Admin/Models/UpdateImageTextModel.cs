using System.ComponentModel.DataAnnotations;

namespace BreezeShop.Web.Areas.Admin.Models
{
    public class UpdateImageTextModel
    {
        [Required(ErrorMessage = "请输入标题")]
        [Display(Name = "标题")]
        public string Title { get; set; }

        public int ParentId { get; set; }

        public bool IsDisplay { get; set; }

        [Display(Name = "排序")]
        [Required(ErrorMessage = "请输入排序")]
        public double SortOrder { get; set; }


        [Display(Name = "超链接")]
        [Required(ErrorMessage = "请输入超链接")]
        public string HyperLink { get; set; }

        [Display(Name = "缩略图")]
        public string Image { get; set; }
    }
}