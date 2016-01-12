using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Mvc.Html;
using System.Web.Routing;

namespace BreezeShop.Web.Helper
{
    /// <summary>
    /// 分页控件属性
    /// </summary>
    public class Pager
    {
        /// <summary>
        /// 分页加载的模板名称
        /// </summary>
        public string PagerTempName { get; set; }

        public string PagerId { get; set; }

        public bool PagerShow { get; set; }

        /// <summary>
        /// 每页显示的记录数
        /// </summary>
        public long PageSize { get; set; }

        /// <summary>
        /// 当前页
        /// </summary>
        public long CurPage { get; set; }

        /// <summary>
        /// 显示页码的数目
        /// </summary>
        public long PageNum { get; set; }

        /// <summary>
        /// 总页数
        /// </summary>
        public long TotalPage { get; set; }

        /// <summary>
        /// 总记录数
        /// </summary>
        public long TotalSize { get; set; }

        public string ControllerName { get; set; }

        public string ActionName { get; set; }

        /// <summary>
        /// 页码属性
        /// </summary>
        public List<PageModel> PageList { get; set; }

        /// <summary>
        /// 开始页码
        /// </summary>
        public long StartPage
        {
            get
            {
                if (PageList != null && PageList.Count > 1)
                {
                    return PageList[0].PageIndex;
                }
                return 1;
            }
        }

        public long EndPage
        {
            get
            {
                if (PageList != null && PageList.Count > 1)
                {
                    return PageList[PageList.Count - 1].PageIndex;
                }
                return 1;
            }
        }

        public RouteValueDictionary CreateUrl(long paged)
        {
            var rtm = new RouteValueDictionary {{"p", paged}};
            var httpCurrent = HttpContext.Current;
            foreach (
                var q in
                    httpCurrent.Request.QueryString.AllKeys.Where(e => !string.IsNullOrEmpty(e) && e != "p")
                               .Where(q => !string.IsNullOrEmpty(httpCurrent.Request.QueryString[q])))
            {
                rtm.Add(q, httpCurrent.Request.QueryString[q]);
            }
            return rtm;
        }
    }

    /// <summary>
    /// 页码属性
    /// </summary>
    public class PageModel
    {

        public PageModel() { }

        public PageModel(int pageIndex, string pageText)
        {
            PageIndex = pageIndex;
            PageText = pageText;
        }
        /// <summary>
        /// 页码
        /// </summary>
        public int PageIndex { get; set; }

        /// <summary>
        /// 文本
        /// </summary>
        public string PageText { get; set; }
    }

    /// <summary>
    /// 分页帮助类
    /// </summary>
    public static class PageHelper
    {
        public static MvcHtmlString Pager(this HtmlHelper helper,
            string pagerId,//分页控件Id
            long curPage,//当前页码
            long totalSize,//总记录数
            string pagerTemp = "_TopPagerTemplate",//分页控件模板
            long pageSize = 10,//每页显示10条
            long pageNum = 5,//显示的页码数目
            string actionName=null,
            string controllerName = null
        )
        {
            var routeData = helper.ViewContext.RouteData;
            controllerName = string.IsNullOrEmpty(controllerName) ? routeData.GetRequiredString("controller") : controllerName;//当前的Controller
            actionName = string.IsNullOrEmpty(actionName) ? routeData.GetRequiredString("action") : actionName;//当前的action
            return Pager(helper, pagerId, curPage, pageSize, totalSize, pageNum, controllerName, actionName, pagerTemp);
        }

        public static MvcHtmlString Pager(this HtmlHelper helper,
            string pagerId, //分页控件Id
            long curPage, //当前页
            long pageSize, //每页显示的记录数目
            long totalSize, //总记录
            long pageNum, //显示的页码数目
            string controllerName, //控制器名称
            string actionName, //动作名称
            string pagerTemp = "_TopPagerTemplate"
        )
        {
            var pager = new Pager
            {
                PagerTempName = pagerTemp,
                PagerId = pagerId,
                PageSize = pageSize,
                TotalSize = totalSize,
                CurPage = curPage,
                TotalPage = (totalSize % pageSize == 0) ? (totalSize / pageSize) : (totalSize / pageSize) + 1,
                PageNum = pageNum,
                ControllerName = controllerName,
                ActionName = actionName
            };
            if (pager.TotalPage > 1 && pager.TotalPage >= curPage)
            {
                pager.PagerShow = true;//显示分页
                var pageList = new List<PageModel>();

                var leftPageNum = (curPage - pageNum < 1) ? 1 : (curPage - pageNum);//左边界
                var rightPageNum = (curPage + pageNum > pager.TotalPage) ? pager.TotalPage : (curPage + pageNum);//右边界
                var pageCount = rightPageNum - leftPageNum + 1;
                var sourceList = Enumerable.Range((int)leftPageNum, (int)pageCount);
                pageList.AddRange(sourceList.Select(p => new PageModel
                {
                    PageIndex = p,
                    PageText = p.ToString()
                }));
                pager.PageList = pageList;
            }
            else
            {
                pager.PagerShow = false;//页数少于一页，则不显示分页
            }
            return helper.Partial(pager.PagerTempName, pager);
        }
    }
}