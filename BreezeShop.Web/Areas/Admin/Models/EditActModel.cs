using System;
using System.ComponentModel.DataAnnotations;

namespace BreezeShop.Web.Areas.Admin.Models
{
    public class EditActModel
    {

        [Required(ErrorMessage = "请选择开始时间")]
        public DateTime? StartTime { get; set; }

        [Required(ErrorMessage = "请选择结束时间")]
        public DateTime? EndTime { get; set; }

        [Required(ErrorMessage = "请输入标题")]
        public string Title { get; set; }

        public string ShareTitle { get; set; }

        public string ShareDescription { get; set; }

        public string Step { get; set; }

        public string Detail { get; set; }

        public string Image { get; set; }
    }
}