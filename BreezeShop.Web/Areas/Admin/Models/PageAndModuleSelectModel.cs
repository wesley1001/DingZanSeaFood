using System.Collections.Generic;
using System.Web.Mvc;

namespace BreezeShop.Web.Areas.Admin.Models
{
    public class PageAndModuleSelectModel
    {
        public List<SelectListItem> PageList { get; set; }

        public List<SelectListItem> ModuleList { get; set; }


        private string _moduleId;

        public string ModuleId
        {
            get { return _moduleId; }
            set
            {
                _moduleId = value;
            }
        }



        private string _pageId;

        public string PageId
        {
            get { return _pageId; }
            set
            {
                _pageId = value;

                _moduleId = "";

            }
        }

    }
}