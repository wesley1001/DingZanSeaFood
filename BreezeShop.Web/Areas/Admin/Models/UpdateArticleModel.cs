using System;
using System.ComponentModel.DataAnnotations;

namespace BreezeShop.Web.Areas.Admin.Models
{
    public class UpdateArticleModel
    {
        [Required(ErrorMessage = "请输入文章名称")]
        public string Title { get; set; }

        [Required(ErrorMessage = "请输入文章详情")]
        public string Detail { get; set; }

        public double Sort { get; set; }

        [Display(Name = "文章简介")]
        public string Summary { get; set; }

        public string Categoryid { get; set; }

        [Display(Name = "文章的标签")]
        public string Tags { get; set; }

        [Display(Name = "文章的缩略图")]
        public string Image { get; set; }

        public string Score { get; set; }

        public string Status { get; set; }

        /// <summary>
        /// 发布时间
        /// </summary>
        public DateTime PostTime { get; set; }
    }
}