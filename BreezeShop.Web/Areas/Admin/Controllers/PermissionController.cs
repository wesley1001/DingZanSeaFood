using System;
using System.Linq;
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
            return View(YunClient.Instance.Execute(new GetRolesRequest(), Token).Roles);
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
            var req = YunClient.Instance.Execute(new DeleteRoleRequest { Id = id }, Token);
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

            var r = YunClient.Instance.Execute(new GetRoleRequest { Id = id }, Token);
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
                    var r = YunClient.Instance.Execute(new AddRoleRequest
                    {
                        Name = model.Name,
                        Sort = model.Sort,
                        Description = model.Description,
                        RoleType = 0
                    }, Token).Result;

                    return Json(r);
                }

                return Json(YunClient.Instance.Execute(new UpdateRoleRequest
                {
                    Description = model.Description,
                    Id = id,
                    Name = model.Name,
                    Sort = model.Sort,
                }, Token).Result);
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
                 new SaveRelationRoleFunctionRequest { RoleId = id, FunctionIds = collection["Functions"] }, Token).Result;
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
                    model.IsFemale == 1, model.DisplayName, id, model.Remark);

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
                }, Token).User;

            if (data != null)
            {
                r.DisplayName = data.DisplayName;
                r.Email = data.Email;
                r.Entry = data.EntryTime.Substring(0, data.EntryTime.IndexOf(' '));
                r.IdCard = data.IdCard;
                r.IsFemale = data.IsFemale ? 1 : 0;
                r.JobNum = data.JobNum;
                r.OrgId = (int)data.OrganizationId;
                r.OtherName = data.OtherName;
                r.Phone = data.Phone;
                r.Plane = data.Plane;
                r.Remark = data.Description;
                r.Roleids = data.Roles != null?data.Roles.Select(e=>(int)e.Key).ToList():null;
                r.UserName = data.UserName;
                r.WorkPlace = data.WorkPlace;
                return View(r);
            }

            TempData["error"] = "该职员不存在，请重新选择";
            return RedirectToAction("Employees");
        }

        public ActionResult DeleteEmployee(int id)
        {
            return Json(YunClient.Instance.Execute(new DeleteEmployeeRequest { Id = id }, Token).Result);
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
            }, Token);

            ViewBag.CurrentPage = p;
            ViewBag.TotalItems = users.TotalItem;

            return View(users.Users);
        }

        public ActionResult Organization()
        {
            return View();
        }

        public ActionResult OrganizationList()
        {
            return PartialView(YunClient.Instance.Execute(new GetOrganizationsRequest(), Token).Organizations);
        }

        public ActionResult AddOrganization(int id = 0, int parent = 0)
        {
            if (id <= 0)
            {
                return PartialView(new EditOrganizationModel
                {
                    Sort = 0,
                    ParentId = parent.ToString()
                });
            }

            var o = YunClient.Instance.Execute(new GetOrganizationRequest { Id = id }, Member.Token).Organization;
            return PartialView(new EditOrganizationModel
            {
                Description = o.Description,
                Name = o.Name,
                ParentId = o.ParentId.ToString(),
                Sort = o.Sort,
            });
        }

        [HttpPost]
        public ActionResult AddOrganization(EditOrganizationModel model, FormCollection collection, int id = 0)
        {
            if (ModelState.IsValid)
            {
                bool r;
                if (id > 0)
                {
                    r = YunClient.Instance.Execute(new UpdateOrganizationRequest
                    {
                        Description = model.Description,
                        Id = id,
                        Name = model.Name,
                        ParentId = collection["OrganizationId"].TryTo(0),
                        Sort = model.Sort
                    }, Token).Result;

                    return Json(r ? 1 : 0);
                }

                r = YunClient.Instance.Execute(new AddOrganizationRequest
                {
                    Description = model.Description,
                    Name = model.Name,
                    ParentId = collection["OrganizationId"].TryTo(0),
                    Sort = model.Sort
                }, Token).Result > 0;

                return Json(r ? 1 : 0);
            }

            return Json(-1);
        }

        public ActionResult DeleteOrganization(int id)
        {
            return Json(YunClient.Instance.Execute(new DeleteOrganizationRequest
            {
                Id = id
            }, Token).Result);
        }

        public ActionResult OrganizationDropDown(int id = 0)
        {
            ViewData["Id"] = id;
            return PartialView(YunClient.Instance.Execute(new GetOrganizationsRequest(), Token).Organizations);
        }

        public ActionResult SeeRoleFunctions(string id)
        {
            var r = YunClient.Instance.Execute(new GetRoleFunctionsRequest
            {
                RoleIds = id
            }, Token);

            return PartialView(r.Functions);
        }
    }
}
