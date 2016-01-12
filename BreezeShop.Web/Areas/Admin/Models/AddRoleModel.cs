using System.ComponentModel.DataAnnotations;

namespace BreezeShop.Web.Areas.Admin.Models
{
    public class AddRoleModel
    {
        [Required(ErrorMessage = "请输入角色名称")]
        public string Name { get; set; }

        [Required(ErrorMessage = "请输入角色排序")]
        public double Sort { get; set; }

        public string Description { get; set; }
    }
}