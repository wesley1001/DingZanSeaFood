using System.Web.Mvc;
using BreezeShop.Core;
using BreezeShop.Core.DataProvider;
using BreezeShop.Web.Areas.Admin.Models;
using Yun.Site.Request;

namespace BreezeShop.Web.Areas.Admin.Controllers
{
    public class OfficeBuildingController : AdminAuthController
    {
        [HttpPost]
        public ActionResult Update(int id, UpdateCityModel model)
        {
            if (ModelState.IsValid)
            {
                var r = YunClient.Instance.Execute(new UpdateCityRequest
                {
                    Ext = model.Ext,
                    Id = id,
                    Name = model.Name,
                    Sort = model.Sort,
                    State = model.State
                }, Token);

                if (r.Result)
                {
                    TempData["success"] = "已成功修改";
                    return RedirectToAction("Index");
                }

                TempData["error"] = "添加失败，错误代码：" + r.ErrMsg;
            }

            return View(model);
        }

        public ActionResult Update(int id)
        {
            var r = YunClient.Instance.Execute(new GetCityRequest {Id = id}).City;

            return View(new UpdateCityModel
            {
                Name = r.Name,
                ParentId = r.ParentId,
                Sort = r.Sort,
                Ext = r.Ext,
                State = r.State
            });
        }

        public ActionResult Delete(int id)
        {
            var r =
                YunClient.Instance.Execute(new DeleteCityRequest {Id = id}, Member.AdminToken);

            return Json(r.Result ? new {result = 1, error = ""} : new {result = 0, error = r.ErrMsg});
        }

        public ActionResult Add(int id = 0)
        {
            return View(new UpdateCityModel {ParentId = id});
        }

        [HttpPost]
        public ActionResult Add(UpdateCityModel model, int id = 0)
        {
            if (ModelState.IsValid)
            {
                var r = YunClient.Instance.Execute(new AddCityRequest
                {
                    Ext = model.Ext,
                    Name = model.Name,
                    ParentId = model.ParentId,
                    Sort = model.Sort,
                    State = model.State
                }, Token);

                if (r.Result > 0)
                {
                    TempData["success"] = "已成功添加";
                    return RedirectToAction("Index");
                }

                TempData["error"] = "添加失败，错误代码："+r.ErrMsg;
            }

            return View(model);
        }

        public ActionResult Index(int p = 1)
        {
            var page = new PageModel<Yun.Site.CityDomain>();

            var req = YunClient.Instance.Execute(new GetCitiesRequest { GetCustomCity = true, PageSize = 20, PageNum = p });

            page.Items = req.Cities;
            page.CurrentPage = p;
            page.TotalItems = req.TotalItem;

            return View(page);   
        }

    }
}
