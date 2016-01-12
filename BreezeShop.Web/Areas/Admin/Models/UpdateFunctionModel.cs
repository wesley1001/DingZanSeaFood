
namespace BreezeShop.Web.Areas.Admin.Models
{
    public class UpdateFunctionModel
    {
        public string Name { get; set; }

        public string Description { get; set; }

        public string Url { get; set; }

        public bool Display { get; set; }

        public string AllowBlock { get; set; }

        /// <summary>
        /// 排序
        /// </summary>
        public int SortOrder { get; set; }

        public int ParentId { get; set; }

        public int Type { get; set; }

        public string Image { get; set; }
    }
}