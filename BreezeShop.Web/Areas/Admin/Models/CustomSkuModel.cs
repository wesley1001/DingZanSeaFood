using System.Collections.Generic;

namespace BreezeShop.Web.Areas.Admin.Models
{
    public class CustomSkuModel
    {
        public string Name { get; set; }

        public string Id { get;set; }

        public IList<string> Values { get; set; }

        public IList<string> KeyIds { get; set; }
    }
}