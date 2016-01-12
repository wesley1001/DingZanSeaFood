using System.ComponentModel.DataAnnotations;

namespace BreezeShop.Web.Areas.Admin.Models
{
    public class AddItemCategoryModel
    {
        [Required(ErrorMessage = "请输入分类名称")]
        [Display(Name = "分类名称")]
        public string Title { get; set; }


        public int Sort { get; set; }
    }
}