using System.Collections.Generic;

namespace BreezeShop.Web.Areas.Admin.Models
{
    public class UpdateOrganizationUserModel
    {
        public string UserName { get; set; }

        public string IdCard { get; set; }

        public string JobNum { get; set; }

        public string OtherName { get; set; }

        public string Phone { get; set; }

        public int OrgId { get; set; }

        public string Entry { get; set; }

        public string Email { get; set; }

        public string Plane { get; set; }

        public string WorkPlace { get; set; }

        public IList<int> Roleids { get; set; }

        public int IsFemale { get; set; }

        public string DisplayName { get; set; }

        public string Remark { get; set; }
    }
}