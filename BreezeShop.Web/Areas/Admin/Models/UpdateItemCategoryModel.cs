using System.ComponentModel.DataAnnotations;

namespace BreezeShop.Web.Areas.Admin.Models
{
    public class UpdateItemCategoryModel
    {
        [Required(ErrorMessage = "请输入分类名称")]
        [Display(Name = "分类名称")]
        public string Title { get; set; }

        public string Image { get; set; }

        public int ParentId { get; set; }

        public bool Display { get; set; }

        public int Sort { get; set; }

    }
}