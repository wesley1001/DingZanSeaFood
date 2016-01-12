using System.ComponentModel.DataAnnotations;
using Yun.Util;

namespace BreezeShop.Web.Areas.Admin.Models
{
    public class AddFunctionModel
    {
        [Required(ErrorMessage = "请输入功能名称")]
        public string Name { get; set; }

        public string Description { get; set; }

        [Required(ErrorMessage = "请输入的目标网址")]
        public string Url { get; set; }

        public string AllowBlock { get; set; }


        public int Type { get; set; }

       
        public FileItem Image { get; set; }

        /// <summary>
        /// 排序
        /// </summary>
        public int SortOrder { get; set; }
    }
}