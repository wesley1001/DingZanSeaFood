using System.Collections.Generic;

namespace BreezeShop.Core
{
    public class PageModel<T>
    {
        public long CurrentPage { get; set; }
        public long TotalPages { get; set; }
        public long TotalItems { get; set; }
        public long ItemsPerPage { get; set; }
        public IList<T> Items { get; set; }
    }
}
