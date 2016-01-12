using System.ComponentModel.DataAnnotations;

namespace BreezeShop.Web.Areas.Admin.Models
{
    public class AddTextModel
    {
        [Required(ErrorMessage = "请输入标题")]
        public string Title { get; set; }

        public int ParentId { get; set; }

        [Required(ErrorMessage = "模块ID")]
        public int CurrentModuleId { get; set; }

        [Required(ErrorMessage = "请输入排序")]
        public double SortOrder { get; set; }

        [Required(ErrorMessage = "请输入超链接")]
        public string HyperLink { get; set; }
    }
}