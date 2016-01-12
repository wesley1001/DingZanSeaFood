
using System.Collections.Generic;
using System.Web;
using System.Web.Mvc;
using BreezeShop.Core.FileFactory;

namespace BreezeShop.Web.Helper
{
    public class MvcExcel
    {
        /// <summary>
        /// List&lt;T&gt;转化为Excel文件,并返回FileStreamResult
        /// </summary>
        /// <param name="list">需要转化的List&lt;T&gt;</param>
        /// <param name="headerList">Excel标题行的List列表</param>
        /// <param name="fileName">Excel的文件名</param>
        /// <returns></returns>
        public static FileStreamResult ExportListToExcel_MVCResult<T>(IList<T> list, IList<string> headerList, string fileName)
        {
            var fsr = new FileStreamResult(ExcelTool.ExportListToExcel(list, headerList, null), "application/ms-excel")
            {
                FileDownloadName = HttpUtility.UrlEncode(fileName + ".xls")
            };
            return fsr;
        }

        /// <summary>
        /// List&lt;T&gt;转化为Excel文件,并返回FileStreamResult
        /// </summary>
        /// <param name="list">需要转化的List&lt;T&gt;</param>
        /// <param name="headerList">Excel标题行的List列表</param>
        /// <param name="fileName">Excel的文件名</param>
        /// <param name="sortList">指定导出List&lt;T&gt中哪些属性,并按顺序排序</param>
        /// <returns></returns>
        public static FileStreamResult ExportListToExcel_MVCResult<T>(IList<T> list, IList<string> headerList, string fileName, IList<string> sortList)
        {
            var fsr = new FileStreamResult(ExcelTool.ExportListToExcel(list, headerList, sortList),
                "application/ms-excel") {FileDownloadName = HttpUtility.UrlEncode(fileName + ".xls")};
            return fsr;
        }
    }
}