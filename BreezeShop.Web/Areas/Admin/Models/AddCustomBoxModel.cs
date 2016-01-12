using System.ComponentModel.DataAnnotations;

namespace BreezeShop.Web.Areas.Admin.Models
{
    public class AddCustomBoxModel
    {
        [Required(ErrorMessage = "请输入标题")]
        public string Title { get; set; }

        [Required(ErrorMessage = "模块ID")]
        public int CurrentModuleId { get; set; }

        [Required(ErrorMessage = "请输入自定义内容")]
        public string CustomText { get; set; }

        [Required(ErrorMessage = "请输入排序")]
        public double SortOrder { get; set; }

        public int ParentId { get; set; }
    }
}