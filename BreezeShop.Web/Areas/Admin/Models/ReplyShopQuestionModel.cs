using System.ComponentModel.DataAnnotations;

namespace BreezeShop.Web.Areas.Admin.Models
{
    public class ReplyShopQuestionModel
    {
        public int Id { get; set; }

        public string Liuyan { get; set; }

        [Required (ErrorMessage = "请回复内容")]
        [Display(Name = "回复内容")]
        public string Content { get; set; }
    }
}