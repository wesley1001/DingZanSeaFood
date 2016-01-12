using System.Collections.Generic;
using Yun.User;

namespace BreezeShop.Web.Areas.Admin.Models
{
    public class GetRoleModel
    {
        //店铺ID
        public IList<Roles> Roles { get; set; }
    }
}