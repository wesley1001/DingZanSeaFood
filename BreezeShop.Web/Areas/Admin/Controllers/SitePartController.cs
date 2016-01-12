using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.WebPages;
using BreezeShop.Core;
using BreezeShop.Core.Cache;
using BreezeShop.Core.DataProvider;
using BreezeShop.Core.FileFactory;
using BreezeShop.Web.Areas.Admin.Models;
using Utilities.Caching.Interfaces;
using Utilities.DataTypes.ExtensionMethods;
using Yun.Site;
using Yun.Site.Request;

namespace BreezeShop.Web.Areas.Admin.Controllers
{
    public class SitePartController : AdminAuthController
    {


        /// <summary>
        /// 获取所有模块类型
        /// </summary>
        /// <returns></returns>
        public static IDictionary<string, string> GetSiteModulesType()
        {
            var r = YunClient.Instance.Execute(new GetSiteModulesTypeRequest(), Member.AdminToken);
            return r.Modules.ToDictionary(e => e.Key, e => e.Value);
        }

        private static void ClearCache(int moduleId)
        {
            var module = YunClient.Instance.Execute(new GetPageModuleRequest { ModuleId = moduleId }).PageModule;
            if (module != null)
            {
                PageElementExtension._moduleContentCache.Remove(moduleId.ToString());
                PageElementExtension._moduleContentCache.Remove(module.Name);
            }
        }

        public ActionResult Index()
        {
            return View(YunClient.Instance.Execute(new GetSitePagesRequest(), Token).Pages);
        }

        public ActionResult Delete(int id, int moduleId)
        {
            var r = YunClient.Instance.Execute(new DeleteSiteElementRequest {Id = id}, Token).Result;
            if (r)
            {
                ClearCache(moduleId);
            }

            return Json(r);
        }


        public ActionResult DeleteModule(int id)
        {
            return Json(YunClient.Instance.Execute(new DeleteSitePageModuleRequest
            {
                Id = id
            }, Token).Result);
        }

        public ActionResult DeletePage(int id)
        {
            return Json(YunClient.Instance.Execute(new DeleteSitePageRequest {Id = id}, Token).Result);
        }

        public ActionResult AddModule(int value, string text)
        {
            return PartialView(new AddModuleModel
            {
                PageName = text,
                PageId = value
            });
        }

        public ActionResult UpdateModule(int id, string name)
        {
            return PartialView(YunClient.Instance.Execute(new GetPageModuleRequest { ModuleId = id }).PageModule);
        }

        [HttpPost]
        public ActionResult UpdatePage(int id, FormCollection collection)
        {
            if (!collection["ModuleName"].IsEmpty())
            {
                var lognImg = FileManage.UploadOneFile();
                return Json(YunClient.Instance.Execute(new UpdateSitePageRequest
                {
                    Id = id,
                    Name = collection["ModuleName"].Trim(),
                    Thumb = lognImg != "" ? lognImg : collection["thumb"]
                }, Token).Result);
            }

            return Json(false);
        }

        [HttpPost]
        public ActionResult UpdateModule(int id, FormCollection collection)
        {
            if (!collection["ModuleName"].IsEmpty())
            {
                var lognImg = FileManage.UploadOneFile();

                var r =
                    YunClient.Instance.Execute(
                        new UpdateSitePageModuleRequest
                        {
                            Id = id,
                            Name = collection["ModuleName"].Trim(),
                            Remark = collection["remark"],
                            ModuleThumb = lognImg != "" ? lognImg : collection["thumb"],
                            ModuleFlag = collection["moduleflag"]
                        }, Token);

                if (r.Result)
                {
                    ClearCache(id);
                }

                return Json(r.Result);
            }

            return Json(false);
        }

        [HttpPost]
        public ActionResult AddModule(int pageid, string modulename, string moduletype, string remark, string moduleFlag)
        {
            var r = YunClient.Instance.Execute(new AddSitePageModuleRequest
            {
                PageId = pageid,
                Name = modulename,
                ModuleType = moduletype,
                ModuleFlag = moduleFlag,
                ModuleThumb = FileManage.UploadOneFile(),
                Remark = remark,
            }, Token);

            return Json(r.Result);
        }

        /// <summary>
        /// 吧文本框的值传到下拉框（页面模块）
        /// </summary>
        /// <param name="name"></param>
        /// <param name="remark"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult AddPage(string name, string remark)
        {
            var r = YunClient.Instance.Execute(new AddSitePageRequest
            {
                Name = name,
                Thumb = FileManage.UploadOneFile(),
                Remark = remark,
            }, Token);

            return Json(r.Result);
        }

        public ActionResult AddPage()
        {
            return PartialView();
        }

        public ActionResult ModuleSelect(ModuleSelectModel model)
        {
            var r = YunClient.Instance.Execute(new GetSitePageModulesRequest
            {
                PageId = model.PageId.TryTo(0),
            }, Token).Modules;

            var moduleList =
                r.Select(
                    e =>
                        new SelectListItem
                        {
                            Value =
                                e.Id.ToString() + "|" + e.ModuleType + "|" + e.Thumb + "|" + e.Remark + "|" +
                                e.ModuleType,
                            Text = e.Title
                        })
                    .ToList();

            model.ModuleList = moduleList;

            return PartialView(model);
        }

        public ActionResult SiteData(int moduleId)
        {
            var r = YunClient.Instance.Execute(new GetSiteElementsRequest
            {
                ModuleId = moduleId,
                PageNum = 1,
                PageSize = 200
            });

            return PartialView(r.Elements);
        }

        /// <summary>
        /// 新增自定义文本框
        /// </summary>
        /// <returns></returns>
        public ActionResult AddCustomBox(int moduleId, int p = 0)
        {
            return PartialView(new AddCustomBoxModel { CurrentModuleId = moduleId, ParentId = p });
        }

        [HttpPost]
        public ActionResult AddCustomBox(AddCustomBoxModel model)
        {
            if (!ModelState.IsValid) return Json(false);

            var r = YunClient.Instance.Execute(new AddSiteElementCustomBoxRequest
            {
                Title = model.Title,
                Display = Request.Form["Display"].TryTo(0) == 1,
                ModuleId = model.CurrentModuleId,
                CustomText = model.CustomText,
                SortOrder = model.SortOrder,
                ParentId = model.ParentId,
            }, Token);

            return Json(r.Result);
        }

        /// <summary>
        /// 更新自定义文本框
        /// </summary>
        /// <returns></returns>
        public ActionResult UpdateCustomBox(int id, int moduleId)
        {
            var d = YunClient.Instance.Execute(new GetCustomSiteElementRequest
            {
                Id = id
            }, Token).CustomBox;


            if (d != null)
            {
                return PartialView(new UpdateCustomBoxModel
                {
                    CustomText = d.CustomText,
                    IsDisplay = d.Display,
                    SortOrder = double.Parse(d.Sort),
                    Title = d.Title,
                    ParentId = d.ParentId
                });
            }

            return PartialView();
        }

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult UpdateCustomBox(UpdateCustomBoxModel model, int id, int moduleId, string moduleFlag)
        {
            if (!ModelState.IsValid) return Json(false);

            var r = YunClient.Instance.Execute(new UpdateSiteElementCustomboxRequest
            {
                Id = id,
                Title = model.Title,
                SortOrder = model.SortOrder,
                CustomText = model.CustomText,
                Display = Request.Form["Display"].TryTo(0) == 1,
                ParentId = model.ParentId
            }, Token);

            if (r.Result)
            {
                ClearCache(moduleId);
            }

            return Json(r.Result);
        }

        /// <summary>
        /// 新增纯文字文本框
        /// </summary>
        /// <returns></returns>
        public ActionResult AddText(int moduleId, int p = 0)
        {
            return PartialView(new AddTextModel { CurrentModuleId = moduleId, ParentId = p });
        }

        [HttpPost]
        public ActionResult AddText(AddTextModel model, string moduleFlag)
        {
            if (ModelState.IsValid)
            {
                var r = YunClient.Instance.Execute(new AddSiteElementTextRequest
                {
                    Title = model.Title,
                    Display = Request.Form["Display"].TryTo(0) == 1,
                    ModuleId = model.CurrentModuleId,
                    HyperLink = model.HyperLink,
                    SortOrder = model.SortOrder,
                    ParentId = model.ParentId
                }, Token);

                if (r.Result > 0)
                {
                    ClearCache(model.CurrentModuleId);
                }

                return Json(r.Result);
            }

            return Json(false);
        }

        /// <summary>
        /// 更新纯文字文本框
        /// </summary>
        /// <returns></returns>
        public ActionResult UpdateText(int id, int moduleId)
        {
            var d = YunClient.Instance.Execute(new GetTextSiteElementRequest
            {
                Id = id
            }, Token).Text;

            if (d != null)
            {
                return PartialView(new UpdateTextModel
                {
                    HyperLink = d.HyperLink,
                    IsDisplay = d.Display,
                    SortOrder = double.Parse(d.Sort),
                    Title = d.Title,
                    ParentId = d.ParentId
                });
            }

            return PartialView();
        }

        [HttpPost]
        public ActionResult UpdateText(UpdateTextModel model, int id, int moduleId, string moduleFlag)
        {
            if (ModelState.IsValid && moduleId > 0)
            {
                var r = YunClient.Instance.Execute(new UpdateSiteElementTextRequest
                {
                    Id = id,
                    Title = model.Title,
                    Display = Request.Form["Display"].TryTo(0) == 1,
                    HyperLink = model.HyperLink,
                    SortOrder = model.SortOrder,
                    ParentId = model.ParentId
                }, Token);

                if (r.Result)
                {
                    ClearCache(moduleId);
                }

                return Json(r.Result);
            }

            return Json(false);
        }

        /// <summary>
        /// 新增图文文本
        /// </summary>
        /// <returns></returns>
        public ActionResult AddImageText(int moduleId, int p = 0)
        {
            return PartialView(new AddImageTextModel { CurrentModuleId = moduleId, ParentId = p });
        }

        [HttpPost]
        public ActionResult AddImageText(AddImageTextModel model)
        {
            if (!ModelState.IsValid) return Json(false);

            var r = YunClient.Instance.Execute(new AddSiteElementImageTextRequest
            {
                Title = model.Title,
                Display = Request.Form["Display"].TryTo(0) == 1,
                ModuleId = model.CurrentModuleId,
                HyperLink = model.HyperLink,
                SortOrder = model.SortOrder,
                Thumb = FileManage.UploadOneFile(),
                ParentId = model.ParentId
            }, Token);

            if (r.Result > 0)
            {
                ClearCache(model.CurrentModuleId);
            }

            return Json(r.Result);
        }

        /// <summary>
        /// 更新图文文本
        /// </summary>
        /// <returns></returns>
        public ActionResult UpdateImageText(int id, int moduleId)
        {
            var d = YunClient.Instance.Execute(new GetImageTextSiteElementRequest
            {
                Id = id
            }, Token).ImageText;

            if (d != null)
            {
                return PartialView(new UpdateImageTextModel
                {
                    Image = d.Image,
                    IsDisplay = d.Display,
                    SortOrder = double.Parse(d.Sort),
                    Title = d.Title,
                    HyperLink = d.HyperLink,
                    ParentId = d.ParentId
                });
            }

            return PartialView();
        }

        [HttpPost]
        public ActionResult UpdateImageText(UpdateImageTextModel model, int id, int moduleId)
        {
            if (ModelState.IsValid)
            {
                var img = FileManage.UploadOneFile();
                var r = YunClient.Instance.Execute(new UpdateSiteElementImageTextRequest
                {
                    Id = id,
                    Title = model.Title,
                    Display = Request.Form["Display"].TryTo(0) == 1,
                    HyperLink = model.HyperLink,
                    Image = string.IsNullOrEmpty(img) ? model.Image : img,
                    SortOrder = model.SortOrder,
                    ParentId = model.ParentId
                }, Token);

                if (r.Result)
                {
                    ClearCache(moduleId);
                }

                return Json(r.Result);
            }

            return Json(false);
        }

        /// <summary>
        /// 新增多图文文本
        /// </summary>
        /// <returns></returns>
        public ActionResult AddMultipleInfo(int moduleId, int p = 0)
        {
            return PartialView(new AddMultiPeInfoModel { CurrentModuleId = moduleId, ParentId = p });
        }

        [HttpPost]
        public ActionResult AddMultipleInfo(AddMultiPeInfoModel model, string moduleFlag)
        {
            if (ModelState.IsValid)
            {
                var r = YunClient.Instance.Execute(new AddSiteElementMultipeinfoRequest
                {
                    Title = model.Title,
                    Display = Request.Form["Display"].TryTo(0) == 1,
                    ModuleId = model.CurrentModuleId,
                    HyperLink = model.HyperLink,
                    AdditionalInfo = model.AdditionalInfo,
                    SecondAdditionalInfo = model.SecondAdditionalInfo,
                    ThirdAdditionalInfo = model.ThirdAdditionalInfo,
                    SortOrder = model.SortOrder,
                    ParentId = model.ParentId,
                    Thumb = FileManage.UploadOneFile()
                }, Token);

                if (r.Result > 0)
                {
                    ClearCache(model.CurrentModuleId);
                }

                if (r.Result > 0)
                {
                    return Json(r.Result);
                }
            }

            return Json(false);
        }

        /// <summary>
        /// 更新多图文文本
        /// </summary>
        /// <returns></returns>
        public ActionResult UpdateMultipleInfo(int id, int moduleId)
        {
            var d = YunClient.Instance.Execute(new GetMultipleInfoSiteElementRequest
            {
                Id = id
            }, Token).MultipleInfo;

            if (d != null)
            {
                return PartialView(new UpdateMultipeInfoModel
                {
                    Image = d.Image,
                    IsDisplay = d.Display,
                    SortOrder = double.Parse(d.Sort),
                    Title = d.Title,
                    HyperLink = d.HyperLink,
                    AdditionalInfo = d.AdditionalInfo,
                    SecondAdditionalInfo = d.SecondAdditionalInfo,
                    ThirdAdditionalInfo = d.ThirdAdditionalInfo,
                    ParentId = d.ParentId
                });
            }

            return PartialView();
        }

        [HttpPost]
        public ActionResult UpdateMultipleInfo(UpdateMultipeInfoModel model, int id, int moduleId, string moduleFlag)
        {
            if (ModelState.IsValid)
            {
                var img = FileManage.UploadOneFile();

                var r = YunClient.Instance.Execute(new UpdateSiteElementMultipeinfoRequest
                {
                    Id = id,
                    Title = model.Title,
                    Display = Request.Form["Display"].TryTo(0) == 1,
                    Image = string.IsNullOrEmpty(img) ? model.Image : img,
                    HyperLink = model.HyperLink,
                    AdditionalInfo = model.AdditionalInfo,
                    SecondAdditionalInfo = model.SecondAdditionalInfo,
                    ThirdAdditionalInfo = model.ThirdAdditionalInfo,
                    SortOrder = model.SortOrder,
                    ParentId = model.ParentId
                }, Token);

                if (r.Result)
                {
                    ClearCache(moduleId);
                }
                return Json(r.Result);
            }

            return Json(false);
        }
    }
}
