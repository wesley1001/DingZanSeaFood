using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using BreezeShop.Core;
using BreezeShop.Core.FileFactory;
using BreezeShop.Web.Areas.Admin.Models;
using Utilities.DataTypes.ExtensionMethods;
using Yun.Archive.Request;
using Yun.Item.Request;
using Yun.Util;

namespace BreezeShop.Web.Areas.Admin.Controllers
{
    public class ArticleController : AdminAuthController
    {

        /// <summary>
        /// 文章列表
        /// </summary>
        /// <returns></returns>
        public ActionResult Index(string title, int p=1)
        {
            var page = new PageModel<Yun.Archive.ArticleDetail>();

            var req =
                YunClient.Instance.Execute(new GetArchivesRequest
                {
                    Fields = "",
                    PageNum = p,
                    PageSize = 10,
                    Title = title
                });

            page.Items = req.Articles;
            page.CurrentPage = p;
            page.TotalItems = req.TotalItem;

            return View(page);   
        }

        public ActionResult CategorySelect(string ids)
        {
            ViewData["ids"] = ids;
            return PartialView(YunClient.Instance.Execute(new GetCategoriesRequest()).Categorys);
        }

        public ActionResult BatchUpdateCategorySort(string ids)
        {
            var id = ids.Split(',');
            var y = 0;
            var sort = new Dictionary<int, int>();
            for (var i = id.Length - 1; i >= 0; i--)
            {
                sort.Add(int.Parse(id[i]), y);
                y += 1;
            }


            return Json(YunClient.Instance.Execute(new BatchUpdateArchiveSortRequest { Sort = sort }), Token);
        }

        public ActionResult DeleteTag(int id)
        {
            return Json(YunClient.Instance.Execute(new DeleteTagRequest { Id = id }), Token);
        }

        public ActionResult TagList(int p = 1)
        {
            var page = new PageModel<Yun.Archive.Tag>();
            var req = YunClient.Instance.Execute(new GetTagsRequest { PageNum = p, PageSize = 40 }, Token);

            page.Items = req.Tags;
            page.CurrentPage = p;
            page.TotalItems = req.TotalItem;

            return View(page);
        }

        /// <summary>
        /// 新增文章
        /// </summary>
        /// <returns></returns>
        public ActionResult Add()
        {
            return View(new AddArticleModel());
        }

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult Add(AddArticleModel model, string status, int categoryId = 0)
        {
            if (ModelState.IsValid)
            {
                var r = YunClient.Instance.Execute(new AddArchiveRequest
                {
                    Title = model.Title,
                    CategoryId = categoryId,
                    Detail = model.Detail,
                    Sort = model.Sort,
                    Tags = model.Tags,
                    Status = status,
                    PostMeta = string.IsNullOrEmpty(model.Summary) ? null : string.Format("summary:{0}", model.Summary),
                    Thumb = FileManage.UploadOneFile()
                }, Token);

                if (r.Result > 0)
                {
                    TempData["success"] = "新增文章成功";
                    return RedirectToAction("Index");
                }
            }

            TempData["error"] = "新增文章失败，请刷新后重试";
            return View(model);
        }

        /// <summary>
        /// 编辑文章
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult Edit(int id)
        {
            var data = YunClient.Instance.Execute(new GetArchiveRequest {Id = id}).Article;
            if (data != null)
            {
                return View(new UpdateArticleModel
                {
                    Categoryid = ((data.Categorys != null && data.Categorys.Any()) ? data.Categorys.Last().Key : 0).ToString(),
                    Detail = data.Detail,
                    Image = data.Thumb,
                    Summary = data.PostMeta.GetValue("summary"),
                    Sort = Convert.ToInt32(data.Sort),
                    Tags = data.Tags.Select(e=>e.Value).ContactString(","),
                    Title = data.Title,
                    Status = data.Status,
                });
            }

            TempData["error"] = "文章不存在";
            return RedirectToAction("Index");
        }

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult Edit(UpdateArticleModel model, int id, string status, int categoryId)
        {
            if (ModelState.IsValid)
            {
                var img = FileManage.UploadOneFile();

                var r = YunClient.Instance.Execute(new UpdateArchiveRequest
                {
                    Id = id,
                    Title = model.Title,
                    Detail = model.Detail,
                    Sort = model.Sort,
                    CategoryId = categoryId,
                    Tags = model.Tags,
                    PostMeta = string.IsNullOrEmpty(model.Summary) ? null : string.Format("summary:{0}", model.Summary),
                    Status = status,
                    Image = string.IsNullOrEmpty(img) ? model.Image : img
                }, Token);

                if (r.Result)
                {
                    TempData["success"] = "文章保存成功";
                    return RedirectToAction("Index");
                }
            }

            TempData["error"] = "修改文章失败";
            return View(model);
        }

        /// <summary>
        /// 删除文章
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult Delete(int id)
        {
            return Json(YunClient.Instance.Execute(new DeleteArchiveRequest { Id = id }, Token));
        }


        /// <summary>
        /// 文章的分类列表
        /// </summary>
        /// <returns></returns>
        public ActionResult Category()
        {
            return View(YunClient.Instance.Execute(new GetCategoriesRequest()).Categorys);
        }

        /// <summary>
        /// 新增文章分类
        /// </summary>
        /// <returns></returns>
        public ActionResult AddCategory()
        {

            return View();
        }

        [HttpPost]
        public ActionResult AddCategory(AddArticleCategoryModel model, int id = 0)
        {
            if (ModelState.IsValid)
            {
                var r = YunClient.Instance.Execute(new AddCategoryRequest
                {
                    Name = model.Title,
                    Display = Request.Form["Display"].TryTo(0) == 1,
                    Image = FileManage.GetFirstFile(),
                    Description = model.Description,
                    ParentId = id
                }, Token);

                if (r.Result > 0)
                {
                    TempData["success"] = "已成功添加“"+ model.Title+"” 分类";
                    return RedirectToAction("Category");
                }

                TempData["error"] = "添加失败，该分类已经存在";
            }


            return View(model);
        }

        /// <summary>
        /// 编辑文章分类
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult EditCategory(int id)
        {
            var category = YunClient.Instance.Execute(new GetCategoryRequest {Id = id}).Category;

            var model = new UpdateArticleCategoryModel
            {
                Image = category.Image,
                Title = category.Name,
                Display = category.Display,
                ParentId = (int) category.ParentId,
                Description = category.Description,
                Sort = (int) category.Sort
            };

            return View(model);
        }

        [HttpPost]
        public ActionResult EditCategory(UpdateArticleCategoryModel model, int id)
        {
            if (ModelState.IsValid)
            {
                var r = YunClient.Instance.Execute(new UpdateCategoryRequest
                {
                    Id = id,
                    Name = model.Title,
                    Display = Request.Form["IsDisplay"].TryTo(0) == 1,
                    Image = model.Image,
                    NewImage = FileManage.GetFirstFile(),
                    Sort = model.Sort,
                    ParentId = model.ParentId,
                    Description = model.Description
                }, Token);

                if (r.Result)
                {
                    TempData["success"] = "已成功修改文章分类";
                    return RedirectToAction("Category");
                }
            }

            return View(model);
        }

        /// <summary>
        /// 删除文章分类
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult DeleteCategory(int id)
        {
            return Json(YunClient.Instance.Execute(new DeleteCategoryRequest { Id = id }, Token).Result);
        }

    }
}
