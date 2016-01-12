using System.Collections.Generic;
using Yun.WeiXin;

namespace BreezeShop.Web.Areas.Admin.Models
{
    public class WechatBaseRuleEditModel
    {
        public string RuleName { get; set; }

        public bool Disabled { get; set; }

        public int DisplayOrder { get; set; }

        public List<KeyValuePair<TriggerTypeEnum, string>> TriggerList { get; set; }

        public int ErrorCode { get; set; }
    }
}