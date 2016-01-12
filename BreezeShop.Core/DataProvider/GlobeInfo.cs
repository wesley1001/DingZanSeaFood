using System;
using System.IO;
using System.Text;
using System.Web;
using System.Xml.Serialization;
using BreezeShop.Core.Model;
using Yun.Shop;
using Yun.Shop.Request;

namespace BreezeShop.Core.DataProvider
{
    public class GlobeInfo
    {
        private static object _LockObj = new object();

        //当前加载的店铺信息
        private static ShopDetail _singleShopDetail;


        private static void Init()
        {
            if (_singleShopDetail != null) return;

            lock (_LockObj)
            {
                if (_singleShopDetail != null) return;

                var r = YunClient.Instance.Execute(new SearchShopsRequest {PageNum = 1, PageSize = 1}).Shops;
                if (r.Count <= 0) throw new Exception("店铺数据不存在，无法进行网站初始化");

                _singleShopDetail = r[0];
            }
        }


        public static ShopDetail GetInitiatedShop
        {
            get
            {
                Init();

                return _singleShopDetail;
            }
        }

        /// <summary>
        /// 获取已加载的店铺ID
        /// </summary>
        public static int InitiatedShopId
        {
            get { return (int)GetInitiatedShop.Id; }
        }



        /// <summary>
        /// 获取已加载的公司id
        /// </summary>
        public static int InitiatedCompanyId
        {
            get { return (int)GetInitiatedShop.CompanyId; }
        }

        public static WebSetting WebSetting
        {
            get
            {
                var path = HttpContext.Current.Server.MapPath("~/app_data/setting.xml");
                if (System.IO.File.Exists(path))
                {
                    var t = System.IO.File.ReadAllText(path);
                    if (!string.IsNullOrWhiteSpace(t))
                    {
                        using (var sr = new StringReader(t))
                        {
                            var xmldes = new XmlSerializer(typeof(WebSetting));
                            return (WebSetting)xmldes.Deserialize(sr);
                        }
                    }
                }

                return new WebSetting();
            }
            set
            {
                //如果文件夹不存在，则先创建
                if (!System.IO.File.Exists(HttpContext.Current.Server.MapPath("~/app_data")))
                {
                    System.IO.Directory.CreateDirectory(HttpContext.Current.Server.MapPath("~/app_data"));
                }

                if (value != null)
                {
                    using (var ms = new MemoryStream())
                    {
                        var t = value.GetType();
                        var xml = new XmlSerializer(t);
                        xml.Serialize(ms, value);
                        var arr = ms.ToArray();
                        var xmlString = Encoding.UTF8.GetString(arr, 0, arr.Length);
                        ms.Close();

                        System.IO.File.WriteAllText(HttpContext.Current.Server.MapPath("~/app_data/setting.xml"), xmlString, Encoding.UTF8);
                    }
                }
            }
        }
    }

}
