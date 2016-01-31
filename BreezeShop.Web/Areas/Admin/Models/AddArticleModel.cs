using System;
using System.ComponentModel.DataAnnotations;

namespace BreezeShop.Web.Areas.Admin.Models
{
    public class AddArticleModel
    {
        [Required(ErrorMessage = "请输入文章名称")]
        public string Title{get;set;}

        [Required(ErrorMessage = "请输入文章详情")]
        public string Detail { get; set; }

        [Display(Name = "排序")]
        public int Sort { get; set; }

        [Display(Name = "文章简介")]
        public string Summary { get; set; }

        [Display(Name = "文章分类id")]
        public string Categoryid { get; set; }

        [Display(Name = "文章的标签")]
        public string Tags { get; set; }

        public string Score { get; set; }

        /// <summary>
        /// 发布时间
        /// </summary>
        public DateTime PostTime { get; set; }

        public string Status { get; set; }

    }
}