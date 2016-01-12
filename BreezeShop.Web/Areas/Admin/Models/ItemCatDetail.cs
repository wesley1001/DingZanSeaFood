using System.Collections.Generic;
using Yun.Item;

namespace BreezeShop.Web.Areas.Admin.Models
{
    public class ItemCatDetail
    {
        /// <summary>
        /// 分类详情
        /// </summary>
        public ItemCat ItemCat { get; set; }

        /// <summary>
        /// 是否快递等
        /// </summary>
        public string Type { get; set; }


        /// <summary>
        /// 商品属性名称
        /// </summary>
        public IList<ItemPropInCat> ItemProps { get; set; }

        /// <summary>
        /// 商品销售属性名称
        /// </summary>
        public IList<ItemSpecInCat> ItemSpecs { get; set; }
    }
}