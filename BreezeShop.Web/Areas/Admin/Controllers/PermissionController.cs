using System;
using System.Web.Mvc;
using System.Web.WebPages;
using BreezeShop.Core;
using BreezeShop.Core.DataProvider;
using BreezeShop.Web.Areas.Admin.Models;
using Utilities.DataTypes.ExtensionMethods;
using Yun.User.Request;

namespace BreezeShop.Web.Areas.Admin.Controllers
{
    public class PermissionController : AdminAuthController
    {
        /// <summary>
        /// 角色列表
        /// </summary>
        /// <returns></returns>
        public ActionResult Roles()
        {
            return View(Manage.GetRoles(0));
        }

        public ActionResult Functions(int type = 0)
        {
            return View(Manage.GetFunctions(null, false, type));
        }

        /// <summary>
        /// 新增全局功能
        /// </summary>
        /// <returns></returns>
        public ActionResult AddFunction(int id = 0, int type = 0)
        {
            if (type > 0)
            {
                return View(new AddFunctionModel { Type = type });
            }

            return View();
        }

        [HttpPost]
        public ActionResult AddFunction(AddFunctionModel model, int id = 0)
        {
            if (ModelState.IsValid)
            {
                var r = Manage.AddFunction(model.Name, model.Description, model.Url,
                    Request.Form["Display"].TryTo(0) == 1,
                    model.AllowBlock, id, model.Type, model.SortOrder);

                if (r)
                {
                    return RedirectToAction("Functions", new { type = model.Type });
                }

                TempData["error"] = "添加失败，该功能名已经存在";
            }

            return View(model);
        }

        /// <summary>
        /// 修改全局功能
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult EditFunction(int id)
        {
            var f = Manage.GetFunction(id);
            if (f != null)
            {
                return View(new UpdateFunctionModel
                {
                    AllowBlock = f.AllowBlock,
                    Description = f.Description,
                    Display = f.IsDisplay,
                    Image = f.Image,
                    Name = f.Name,
                    ParentId = (int)f.ParentId,
                    SortOrder = (int)f.Sort,
                    Type = (int)f.Type,
                    Url = f.Url
                });
            }

            TempData["error"] = "当前功能不存在";
            return RedirectToAction("Functions");
        }

        [HttpPost]
        public ActionResult EditFunction(UpdateFunctionModel model, int id)
        {
            if (ModelState.IsValid)
            {
                var r = Manage.UpdateFunction(id, model.Name, model.SortOrder, model.Description, model.Url,
                    Request.Form["IsDisplay"].TryTo(0) == 1,
                    model.AllowBlock, model.ParentId, model.Type, model.Image);

                if (r.IsEmpty())
                {
                    return RedirectToAction("Functions", new { type = model.Type });
                }

                TempData["error"] = r;
            }
            return View(model);
        }

        /// <summary>
        /// 删除权限
        /// 异步方法，返回json
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult DeleteFunction(int id)
        {
            return Json(Manage.DeleteFunction(id));
        }

        public ActionResult DeleteRole(int id)
        {
            var req = YunClient.Instance.Execute(new DeleteRoleRequest { Id = id }, Member.Token);
            return Json(req.Result);
        }

        /// <summary>
        /// 新增角色
        /// </summary>
        /// <returns></returns>
        public ActionResult AddRole(int id = 0)
        {
            if (id <= 0)
            {
                return PartialView(new AddRoleModel { Sort = 0 });
            }

            var r = YunClient.Instance.Execute(new GetRoleRequest { Id = id }, Member.Token);
            if (r != null)
            {
                return PartialView(new AddRoleModel
                {
                    Description = r.Role.Description,
                    Name = r.Role.Name,
                    Sort = r.Role.Sort
                });
            }

            return Content("该权限不存在，请重新选择");
        }

        [HttpPost]
        public ActionResult AddRole(AddRoleModel model, int id = 0)
        {
            if (ModelState.IsValid)
            {
                if (id <= 0)
                {
                    var r = Manage.AddPermissionRole(model.Name, model.Sort, model.Description, 0);
                    return Json(r);
                }

                return Json(YunClient.Instance.Execute(new UpdateRoleRequest
                {
                    Description = model.Description,
                    Id = id,
                    Name = model.Name,
                    Sort = model.Sort,
                }, Member.Token).Result);
            }

            return Json(-1);
        }

        /// <summary>
        /// 获取角色下的功能
        /// </summary>
        /// <returns></returns>
        public ActionResult RoleFunctions(int id)
        {
            ViewData["CurrentFunctions"] = Manage.GetRoleFunctions(id);
            return PartialView(Manage.GetFunctions(null, false, 0));
        }

        [HttpPost]
        public ActionResult RoleFunctions(int id, FormCollection collection)
        {
            var r = YunClient.Instance.Execute(
                 new SaveRelationRoleFunctionRequest { RoleId = id, FunctionIds = collection["Functions"] }, Member.Token).Result;
            return Json(r);
        }

        /// <summary>
        /// 新建员工
        /// </summary>
        /// <returns></returns>
        public ActionResult AddEmployee()
        {
            ViewData["Roles"] = Manage.GetRoles(0);
            return View();
        }

        [HttpPost]
        public ActionResult AddEmployee(AddOrganizationUserModel model, FormCollection collection)
        {
            if (ModelState.IsValid)
            {
                var r = Manage.AddUserOrganization(model.UserName, model.ConfirmPassword,
                    collection["OrganizationId"].TryTo(0),
                    0, model.IdCard, collection["Role"],
                    collection["EntryTime"].Is<DateTime>() ? collection["EntryTime"] : null,
                    model.JobNum, model.OtherName,
                    model.Phone, model.Email, model.Plane, model.WorkPlace, model.IsFemale, model.DisplayName,
                    Request.UserHostAddress, model.Remark);

                TempData["success"] = "已成功添加职员";
                return Json(r);
            }

            return Json(-3);
        }

        [HttpPost]
        public ActionResult UpdateEmployee(int id, UpdateOrganizationUserModel model, FormCollection collection)
        {
            if (ModelState.IsValid)
            {
                var r = Manage.UpdateUserOrganization(0, 0, model.IdCard,
                    collection["Role"],
                    collection["EntryTime"].Is<DateTime>() ? collection["EntryTime"] : null, model.JobNum,
                    model.OtherName, model.Phone, model.Email, model.Plane, model.WorkPlace,
                    model.IsFemale == 1, model.DisplayName, id);

                TempData["success"] = "已成功修改职员信息";
                return Json(r);
            }

            return Json(0);
        }

        public ActionResult UpdateEmployee(int id)
        {
            ViewData["Roles"] = Manage.GetRoles(0);

            var r = new UpdateOrganizationUserModel();
            var data =
                YunClient.Instance.Execute(new GerPermissionUserRequest
                {
                    Id = id,
                }, Member.Token).User;

            if (data != null)
            {
                r.DisplayName = data.DisplayName;
                r.Email = data.Email;
                r.Entry = data.EntryTime;
                r.IdCard = data.IdCard;
                r.IsFemale = data.IsFemale ? 1 : 0;
                r.JobNum = data.JobNum;
                r.OrgId = (int)data.OrganizationId;
                r.OtherName = data.OtherName;
                r.Phone = data.Phone;
                r.Plane = data.Plane;
                r.Remark = data.Description;
                r.Roleids = data.Roles;
                r.UserName = data.UserName;
                r.WorkPlace = data.WorkPlace;
                return View(r);
            }

            TempData["error"] = "该职员不存在，请重新选择";
            return RedirectToAction("Employees");
        }

        public ActionResult DeleteEmployee(int id)
        {
            return Json(YunClient.Instance.Execute(new DeleteEmployeeRequest { Id = id }, Member.Token).Result);
        }


        public ActionResult Employees(string organizationid = "0", string username = null, string realname = null, int p = 1)
        {
            var users = YunClient.Instance.Execute(new GetEmployeesRequest
            {
                UserName = username,
                OrganizationId = organizationid.TryTo(0),
                RealName = realname,
                PageNum = p,
                PageSize = 10
            }, Member.Token);

            ViewBag.CurrentPage = p;
            ViewBag.TotalItems = users.TotalItem;

            return View(users.Users);
        }


    }
}
