using System.ComponentModel.DataAnnotations;

namespace BreezeShop.Web.Areas.Admin.Models
{
    public class EditOrganizationModel
    {
        [Required(ErrorMessage = "请输入机构名字")]
        public string Name { get; set; }

        public string Description { get; set; }

        public string ParentId { get; set; }

        [Required(ErrorMessage = "请输入排序")]
        public double Sort { get; set; }
    }
}