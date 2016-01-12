using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace BreezeShop.Web.Areas.Admin.Models
{
    public class LogInModel
    {
        [Required(ErrorMessage="请输入用户名或Email")]
        [Display(Name = "登录账号")]
        public string UserName { get; set; }

        [Required(ErrorMessage="请输入密码")]
        [DataType(DataType.Password)]
        [Display(Name = "密码")]
        public string Password { get; set; }
    }


    public class ResetPasswordModel
    {
        [Required(ErrorMessage = "请输入密码")]
        [DataType(DataType.Password, ErrorMessage = "请输入正确的密码")]
        [Display(Name = "密码")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "确认密码")]
        [Compare("Password", ErrorMessage = "密码和确认密码不匹配。")]
        [Required(ErrorMessage = "请输入确认密码")]
        public string ConfirmPassword { get; set; }
    }

    public class ModifyMoneyModel
    {
        [Required(ErrorMessage = "请输入余额")]
        [Display(Name = "余额")]
        public double Money { get; set; }

        [Required(ErrorMessage = "请输入原因")]
        [Display(Name = "原因")]
        [RegularExpression(@"^[-_a-zA-Z0-9\u4e00-\u9fa5]{1,24}$", ErrorMessage = "只能包含数字、下划线、字母、中文，1-24个字符")]
        public string Remark { get; set; }
    }
}
