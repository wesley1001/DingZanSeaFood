using System.Collections.Generic;
using System.Web.Mvc;

namespace BreezeShop.Web.Areas.Admin.Models
{
    public class PageSelectModel
    {
        public List<SelectListItem> PageList { get; set; }

        public string PageId { get; set; }


        public string CssClass { get; set; }
    }
}