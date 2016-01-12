using System.ComponentModel.DataAnnotations;

namespace BreezeShop.Web.Areas.Admin.Models
{
    public class UpdateCustomBoxModel
    {
        [Required(ErrorMessage = "请输入标题")]
        public string Title { get; set; }

        public int ParentId { get; set; }

        [Required(ErrorMessage = "请输入排序")]
        public double SortOrder { get; set; }

        [Display(Name = "自定义内容")]
        public string CustomText { get; set; }

        public bool IsDisplay { get; set; }
    }
}