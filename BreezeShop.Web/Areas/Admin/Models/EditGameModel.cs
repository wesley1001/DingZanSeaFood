using System;
using System.ComponentModel.DataAnnotations;

namespace BreezeShop.Web.Areas.Admin.Models
{
    public class EditGameModel
    {
        [Required(ErrorMessage = "请输入游戏标题")]
        public string Title { get; set; }

        [Required(ErrorMessage = "请输入游戏开始时间")]
        public DateTime StartTime { get; set; }

        [Required(ErrorMessage = "请输入游戏结束时间")]
        public DateTime EndTime { get; set; }

        [Required(ErrorMessage = "最大游戏次数")]
        public int MaxTimes { get; set; }

        [Required(ErrorMessage = "游戏类型")]
        public string GameType { get; set; }
        

        public string ShareTitle { get; set; }
        

        public string ShareDescription { get; set; }


        public string Image { get; set; }


        public string Detail { get; set; }

        [Required(ErrorMessage = "请选择商品")]
        public int GoodsId { get; set; }


        /// <summary>
        /// 描述
        /// </summary>
        public string Description { get; set; }
    }
}