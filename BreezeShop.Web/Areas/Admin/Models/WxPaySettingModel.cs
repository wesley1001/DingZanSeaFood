
using System.ComponentModel.DataAnnotations;

namespace BreezeShop.Web.Areas.Admin.Models
{
    public class WxPaySettingModel
    {
        [Required(ErrorMessage = "请输入APPID")]
        public string AppId { get; set; }

        [Required(ErrorMessage = "请输入商户ID")]
        public string MchId { get; set; }

        [Required(ErrorMessage = "请输入KEY")]
        public string Key { get; set; }

        public string ClientNotifyUrl { get; set; }
    }
}