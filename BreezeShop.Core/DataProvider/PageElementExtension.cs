using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BreezeShop.Core.Cache;
using Utilities.Caching.Interfaces;
using Yun.Site;
using Yun.Site.Request;

namespace BreezeShop.Core.DataProvider
{
    public class PageElementExtension
    {
        public static readonly ICache<string> _moduleContentCache = new FileCache<string>("modulecontents");

        public static IList<OnlyText> GetTextsElement(int moduleid, int num)
        {
            var r = _moduleContentCache.Get(moduleid.ToString(),
                () => YunClient.Instance.Execute(new GetTextsSiteElementRequest
                {
                    ModuleId = moduleid,
                    Num = num
                }).Texts);

            return r ?? new List<OnlyText>();
        }

        public static IList<OnlyText> GetTextsElement(string moduleFlag, int num)
        {
            var r = _moduleContentCache.Get(moduleFlag,
                () => YunClient.Instance.Execute(new GetTextsSiteElementRequest
                {
                    Num = num,
                    ModuleFlag = moduleFlag
                }).Texts);

            return r ?? new List<OnlyText>();
        }

        public static IList<CustomBox> GetCustomsSiteElement(int moduleid, int num)
        {
            var r = _moduleContentCache.Get(moduleid.ToString(),
                () => YunClient.Instance.Execute(new GetCustomsSiteElementRequest
                {
                    ModuleId = moduleid,
                    Num = num
                }).CustomBoxes);

            return r ?? new List<CustomBox>();
        }

        public static IList<ImageText> GetImageTextsSiteElement(int moduleid, int num)
        {
            var r = _moduleContentCache.Get(moduleid.ToString(),
                () => YunClient.Instance.Execute(new GetImageTextsSiteElementRequest
                {
                    ModuleId = moduleid,
                    Num = num
                }).ImageTexts);

            return r ?? new List<ImageText>();
        }

        public static IList<ImageText> GetImageTextsSiteElement(string moduleFlag, int num)
        {
            var r = _moduleContentCache.Get(moduleFlag,
                () => YunClient.Instance.Execute(new GetImageTextsSiteElementRequest
                {
                    Num = num,
                    ModuleFlag = moduleFlag
                }).ImageTexts);
            return r ?? new List<ImageText>();
        }

        public static IList<MultipleInfo> GetMultipleInfosSiteElement(int moduleid, int num)
        {
            var r = _moduleContentCache.Get(moduleid.ToString(),
                () => YunClient.Instance.Execute(new GetmultipleinfosSiteElementRequest
                {
                    ModuleId = moduleid,
                    Num = num
                }).MultipleInfoes);

            return r ?? new List<MultipleInfo>();
        }
        public static IList<MultipleInfo> GetMultipleInfosSiteElement(string moduleFlag, int num)
        {
            var r = _moduleContentCache.Get(moduleFlag,
                () => YunClient.Instance.Execute(new GetmultipleinfosSiteElementRequest
                {
                    Num = num,
                    ModuleFlag = moduleFlag
                }).MultipleInfoes);
            return r ?? new List<MultipleInfo>();
        }
    }
}
