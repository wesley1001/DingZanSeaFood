using System;

namespace BreezeShop.Web.Areas.Admin.Models
{
    public class UpdateMemberModel
    {
        /// <summary>
        /// 用户ID
        /// </summary>
        public long UserId { get; set; }

        /// <summary>
        /// 用户名
        /// </summary>
        public string Nick { get; set; }

        /// <summary>
        /// 性别
        /// </summary>
        public string Sex { get; set; }

        /// <summary>
        /// 用户创建时间
        /// </summary>
        public string CreateTime { get; set; }

        /// <summary>
        /// 最后访问时间
        /// </summary>
        public string LastVisit { get; set; }

        /// <summary>
        /// 头像
        /// </summary>
        public string Avatar { get; set; }

        /// <summary>
        /// 会员卡
        /// </summary>
        public string IdCard { get; set; }

        /// <summary>
        /// 邮箱
        /// </summary>
        public string Email { get; set; }


        public string Mobile { get; set; }

        /// <summary>
        /// 积分
        /// </summary>
        public long Score { get; set; }


        /// <summary>
        /// 用户余额
        /// </summary>
        public string Money { get; set; }

        /// <summary>
        /// 用户充值卡余额
        /// </summary>
        public double PrepaidCard { get; set; }

        /// <summary>
        /// 真实姓名
        /// </summary>
        public string RealName { get; set; }

        /// <summary>
        /// 生日
        /// </summary>
        public DateTime? Birthday { get; set; }

        /// <summary>
        /// 省
        /// </summary>
        public string Province { get; set; }

        /// <summary>
        /// 市
        /// </summary>
        public string City { get; set; }

        /// <summary>
        /// 区
        /// </summary>
        public string Area { get; set; }

        /// <summary>
        /// 详细地址
        /// </summary>
        public string Address { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        public string Remark { get; set; }
        
    }
}