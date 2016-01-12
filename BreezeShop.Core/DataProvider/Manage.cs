using System.Collections.Generic;
using BreezeShop.Core.Cache;
using BreezeShop.Core.FileFactory;
using Utilities.Caching.Interfaces;
using Utilities.DataTypes.ExtensionMethods;
using Yun.User;
using Yun.User.Request;

namespace BreezeShop.Core.DataProvider
{
    /// <summary>
    /// 系统级管理方法
    /// </summary>
    public class Manage
    {
        public static ICache<string> functionsCache = new FileCache<string>("functions");

        public static bool AddFunction(string name, string description, string url, bool display, string allowBlock, int parentId,int type, int sort)
        {
            var r = YunClient.Instance.Execute(new AddFunctionRequest
            {
                AllowBlock = allowBlock,
                Description = description,
                Display = display,
                Name = name,
                ParentId = parentId,
                Sort = sort,
                Url = url,
                Type = type,
                //Image = FileManage.GetFirstFile()
            }, Member.AdminToken);
            if (!r.IsError && r.Result > 0)
            {
                functionsCache.Clear();
            }
            
            return !r.IsError && r.Result > 0;
        }

        public static string UpdateFunction(int id, string name, int sort, string description, string url, bool display,
            string allowBlock, int parentId, int type, string image)
        {
            var r = YunClient.Instance.Execute(new UpdateFunctionRequest
            {
                Id = id,
                Name = name,
                Sort = sort,
                Description = description,
                Url = url,
                Display = display,
                AllowBlock = allowBlock,
                ParentId = parentId,
                Type = type,
                Image = image,
                //NewImage = FileManage.GetFirstFile()
            }, Member.AdminToken);
            if (r.Result)
            {
                functionsCache.Clear();
            }
            return r.IsError ? r.ErrMsg : "";
        }


        public static IList<Function> GetFunctions(string username, bool display, int? type)
        {
            var r =
                functionsCache.Get((username ?? "all") + "-" + display + (type == null ? "" : ("-" + type)) + "-functions",
                    () => YunClient.Instance.Execute(new GetFunctionsRequest
                    {
                        OnlyShowDisplay = true,
                        FunctionType = type
                    }, username.IsNullOrEmpty() ? null : Member.AdminToken).Functions);

            return r;
        }

        public static Function GetFunction(int id)
        {
            var r = YunClient.Instance.Execute(new GetFunctionRequest { Id = id }, Member.AdminToken);
            return r.Function;
        }

        public static bool DeleteFunction(int id)
        {
            var r = YunClient.Instance.Execute(new DeleteFunctionRequest { Id = id }, Member.AdminToken);
            if (r.Result)
            {
                functionsCache.Clear();
            }

            return r.Result;
        }

        public static IList<Roles> GetRoles(int type)
        {
            var r = YunClient.Instance.Execute(new GetRolesRequest{RoleType = type}, Member.AdminToken);
            return r.Roles;
        }

        public static IList<Organizations> GetOrganizationses()
        {
            var r = YunClient.Instance.Execute(new GetOrganizationsRequest(), Member.AdminToken);
            return r.Organizations;
        }

        public static IList<Function> GetRoleFunctions(int id)
        {
            var r = YunClient.Instance.Execute(new GetRoleFunctionsRequest
            { 
                RoleIds=id.ToString()
            }, Member.AdminToken);
            return r.Functions;
        }

        /// <summary>
        /// 新增角色
        /// </summary>
        /// <returns></returns>
        public static long AddPermissionRole(string name, double sort, string description, int type)
        {
            var r = YunClient.Instance.Execute(new AddRoleRequest
            {
                Name = name,
                Sort = sort,
                Description = description,
                RoleType = type
            }, Member.AdminToken);

            return r.Result;
        }

        /// <summary>
        /// 新增部门
        /// </summary>
        /// <returns></returns>
        public static long AddOrganizationDept(string name, double sort, string description, int parentid)
        {
            var r = YunClient.Instance.Execute(new AddOrganizationRequest
            {
                Name = name,
                Sort = sort,
                Description = description,
                ParentId = parentid
            },Member.AdminToken);

            return r.Result;
        }

        /// <summary>
        /// 获取多条数据
        /// </summary>
        /// <returns></returns>
        public static IList<Organizations> GetOrganizations()
        {
            var r = YunClient.Instance.Execute(new GetOrganizationsRequest(), Member.AdminToken);

            return r.IsError ? new List<Organizations>() : r.Organizations;
        }

        /// <summary>
        /// 获取全部上级部门
        /// </summary>
        /// <returns></returns>
        public static IList<Organizations> GetParentIds()
        {
            var r = YunClient.Instance.Execute(new GetOrganizationsRequest(), Member.AdminToken);

            return r.Organizations;
        }


        /// <summary>
        /// 新增员工
        /// </summary>
        /// <returns></returns>
        public static long AddUserOrganization(string username, string password, int organizationid, int higheruserid,
            string idcard, string roleids, string entrytime, string jobnum, string othername, string phone, string email,
            string plane, string workplace, int isfemale, string name, string ip, string remark)
        {
            var r = YunClient.Instance.Execute(new AddEmployeeRequest 
            {
                UserName = username,
                Password = password,
                OrganizationId = organizationid,
                HigherUserId = higheruserid,
                IdCard = idcard,
                RoleIds = roleids,
                EntryTime = entrytime,
                JobNum = jobnum,
                OtherName = othername,
                Phone = phone,
                Email = email,
                Plane = plane,
                WorkPlace = workplace,
                IsFemale = isfemale == 1,
                DisplayName = name,
                Ip = ip,
                AppSecret = YunClient.AppSecret,
                Description = remark
            }, Member.AdminToken);

            return r.Result;
        }

        /// <summary>
        /// 修改员工信息
        /// </summary>
        /// <returns></returns>
        public static long UpdateUserOrganization(int organizationid, int higheruserid,
            string idcard, string roleids, string entrytime, string jobnum, string othername, string phone, string email,
            string plane, string workplace, bool isfemale, string displayname, int userId)
        {
            var r = YunClient.Instance.Execute(new UpdateEmployeeRequest
            {
                OrganizationId = organizationid,
                HigherUserId = higheruserid,
                IdCard = idcard,
                RoleIds = roleids,
                EntryTime = entrytime,
                JobNum = jobnum,
                OtherName = othername,
                Phone = phone,
                Email = email,
                Plane = plane,
                WorkPlace = workplace,
                DisplayName = displayname,
                IsFemale = isfemale,
                UserId = userId
            }, Member.AdminToken);

            return r.Result;
        }

    }
}
