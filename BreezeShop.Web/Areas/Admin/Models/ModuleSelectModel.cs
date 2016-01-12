using System.Collections.Generic;
using System.Web.Mvc;

namespace BreezeShop.Web.Areas.Admin.Models
{
    public class ModuleSelectModel
    {
        public List<SelectListItem> ModuleList { get; set; }

        public string ModuleId { get; set; }

        public string PageId { get; set; }

        public string CssClass { get; set; }

    }
}
