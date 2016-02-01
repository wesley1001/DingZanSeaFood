using System.ComponentModel.DataAnnotations;

namespace BreezeShop.Web.Areas.Admin.Models
{
    public class AddSpuModel
    {
        [Required(ErrorMessage = "请输入名字")]
        [Display(Name = "名字")]
        [RegularExpression(@"^[-_a-zA-Z0-9\u4e00-\u9fa5]{1,24}$", ErrorMessage = "只能包含数字、下划线、字母、中文，1-24个字符")]
        public string Name { get; set; }

        [Required(ErrorMessage = "请输入排序")]
        [DataType(@"^[0-9]*$", ErrorMessage = "请输入正确的排序")]
        [Display(Name = "排序")]
        public string Sort { get; set; }

        public bool Display { get; set; }

        public string Img { get; set; }
    }
}