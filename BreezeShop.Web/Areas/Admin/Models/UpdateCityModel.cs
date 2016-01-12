using System.ComponentModel.DataAnnotations;

namespace BreezeShop.Web.Areas.Admin.Models
{
    public class UpdateCityModel
    {
        [Required(ErrorMessage = "请输入名字")]
        public string Name { get; set; }


        public int ParentId { get; set; }

        [Required(ErrorMessage = "请输入排序")]
        public int Sort { get; set; }

        /// <summary>
        /// 状态
        /// </summary>
        [Required(ErrorMessage = "请输入状态码")]
        public int State { get; set; }


        public string Ext { get; set; }
    }
}