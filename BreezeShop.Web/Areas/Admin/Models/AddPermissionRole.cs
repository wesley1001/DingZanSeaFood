using System.ComponentModel.DataAnnotations;

namespace BreezeShop.Web.Areas.Admin.Models
{
    public class AddPermissionRole
    {
        [Required(ErrorMessage = "请输入名字")]
        [Display(Name = "名字")]
        public string Name { get; set; }

        [Display(Name="排序")]
        public int Sort { get; set; }

        [Display(Name="描述")]
        public string Description { get; set; }
    }
}