using System.ComponentModel.DataAnnotations;

namespace BreezeShop.Web.Areas.Admin.Models
{
    public class AddSitePageModel
    {
        [Required(ErrorMessage = "页面名字，不能重复")]
        [Display(Name = "页面名字")]
        public string Name { get; set; }
    }
}