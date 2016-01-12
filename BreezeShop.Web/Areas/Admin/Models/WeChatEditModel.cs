
using System.ComponentModel.DataAnnotations;

namespace BreezeShop.Web.Areas.Admin.Models
{
    public class WeChatEditModel
    {
        /// <summary>
        /// 公众号名称
        /// </summary>
        [Required(ErrorMessage = "请输入公众号名称")]
        public string Name { get; set; }


        /// <summary>
        /// 公众号账号
        /// </summary>
        [Required(ErrorMessage = "请输入公众号账号")]
        public string WxAccount { get; set; }

        /// <summary>
        /// 公众号原始ID
        /// </summary>
        public string OriginalId { get; set; }

        /// <summary>
        /// 级别
        /// </summary>
        public int Level { get; set; }


        /// <summary>
        /// APPID
        /// </summary>
        public string AppId { get; set; }


        public string AppSecret { get; set; }

        /// <summary>
        /// 接口地址
        /// </summary>
        public string ClientUrl { get; set; }

        /// <summary>
        /// Token
        /// </summary>
        [Required(ErrorMessage = "请填写Token")]
        public string Token { get; set; }


        public string EncodingAESKey { get; set; }

        /// <summary>
        /// 二维码
        /// </summary>
        public string QrCode { get; set; }


        /// <summary>
        /// 描述
        /// </summary>
        public string Description { get; set; }
    }
}