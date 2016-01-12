using System.ComponentModel.DataAnnotations;

namespace BreezeShop.Web.Areas.Admin.Models
{
    public class AddModuleModel
    {
        public string PageName { get; set; }

        [Display(Name = "页面ID")]
        public int PageId { get; set; }

        [Display(Name = "模块名称")]
        public string ModuleName { get; set; }

        [Display(Name = "模块类型")]
        public string ModuleType { get; set; }

        [Display(Name = "模块标识")]
        public string ModuleFlag { get; set; }
    }
}