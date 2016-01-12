using System.Collections.Generic;
using Yun.Logistics;

namespace BreezeShop.Web.Areas.Admin.Models
{
    public class DeliveryTemplateFreight
    {
        public List<FareFreeCondition> FareFreeConditions { get; set; }

        public List<PostfareDetail> kuaidi { get; set; }
        public string kuaidi1 { get; set; }
        public string kuaidi2 { get; set; }
        public string kuaidi3 { get; set; }
        public string kuaidi4 { get; set; }
        public List<PostfareDetail> ziti { get; set; }
        public string ziti1 { get; set; }
        public string ziti2 { get; set; }
        public string ziti3 { get; set; }
        public string ziti4 { get; set; }
        public List<PostfareDetail> shangjia { get; set; }
        public string shangjia1 { get; set; }
        public string shangjia2 { get; set; }
        public string shangjia3 { get; set; }
        public string shangjia4 { get; set; }
    }
}