using System.ComponentModel.DataAnnotations;

namespace BreezeShop.Web.Areas.Admin.Models
{
    public class AddMultiPeInfoModel
    {
        [Required(ErrorMessage = "请输入标题")]
        public string Title { get; set; }

        public int ParentId { get; set; }

        [Required(ErrorMessage = "请输入模块ID")]
        public int CurrentModuleId { get; set; }

        [Required(ErrorMessage = "请输入排序")]
        public double SortOrder { get; set; }


        [Required(ErrorMessage = "请输入超链接")]
        public string HyperLink { get; set; }

        public string AdditionalInfo { get; set; }

        public string SecondAdditionalInfo { get; set; }

        public string ThirdAdditionalInfo { get; set; }
    }
}