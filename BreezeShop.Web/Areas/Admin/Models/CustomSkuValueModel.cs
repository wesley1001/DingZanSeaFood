using System.Collections.Generic;

namespace BreezeShop.Web.Areas.Admin.Models
{
    public class CustomSkuValueModel
    {
        public string Name { get; set; }

        public IList<string> Values { get; set; }
    }
}