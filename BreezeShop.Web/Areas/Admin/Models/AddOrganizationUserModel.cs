using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace BreezeShop.Web.Areas.Admin.Models
{
    public class AddOrganizationUserModel
    {
        [Required(ErrorMessage = "请输入用户名")]
        public string UserName { get; set; }

        [Required(ErrorMessage = "请输入密码")]
        [DataType(DataType.Password, ErrorMessage = "请输入正确的密码")]
        [Display(Name = "密码")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Required(ErrorMessage = "请输入确认密码")]
        [Display(Name = "确认密码")]
        [Compare("Password", ErrorMessage = "密码和确认密码不匹配。")]
        public string ConfirmPassword { get; set; }

        public string IdCard { get; set; }

        public string JobNum { get; set; }

        public string OtherName { get; set; }

        public string Phone { get; set; }

        public string Email { get; set; }

        public string Plane { get; set; }

        public string WorkPlace { get; set; }

        public string Roleids { get; set; }

        public int IsFemale { get; set; }

        public string DisplayName { get; set; }

        public string Remark { get; set; }

    }
}