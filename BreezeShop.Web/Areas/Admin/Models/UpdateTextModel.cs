using System.ComponentModel.DataAnnotations;

namespace BreezeShop.Web.Areas.Admin.Models
{
    public class UpdateTextModel
    {
        [Required(ErrorMessage = "请输入标题")]
        [Display(Name = "标题")]
        public string Title { get; set; }

        public int ParentId { get; set; }

        [Display(Name = "是否显示")]
        public bool IsDisplay { get; set; }

        [Display(Name = "排序")]
        [Required(ErrorMessage = "请输入排序")]
        public double SortOrder { get; set; }


        [Display(Name = "超链接")]
        [Required(ErrorMessage = "请输入超链接")]
        public string HyperLink { get; set; }
    }
}