using System;
using System.Collections.Generic;
using System.Linq;
using Yun.Logistics;
using Yun.Site;
using Yun.Site.Request;

namespace BreezeShop.Core.DataProvider
{
    public class SystemCity
    {
        private static List<Yun.Site.CityDomain> _cities = new List<CityDomain>();

        public static List<Yun.Site.CityDomain> GetCities(int parentId)
        {
            LoadCity();

            return _cities.Where(e => e.ParentId == parentId).ToList();
        }

        private static void LoadCity()
        {
            if (_cities.Any()) return;

            lock (_cities)
            {
                if (!_cities.Any())
                {
                    _cities =
                        YunClient.Instance.Execute(new GetCitiesRequest {PageNum = 1, PageSize = 100000}).Cities;
                }
            }
        }


        /// <summary>
        /// 获取城市并对数据进行分组
        /// </summary>
        public static List<Yun.Site.CityDomain> GetAdminClientCities()
        {
            LoadCity();

            var citys = new List<Yun.Site.CityDomain>
            {
                new Yun.Site.CityDomain()
                {
                    Id = -11,
                    Sort = 0,
                    Name = "华东",
                    ParentId = -100
                },
                new Yun.Site.CityDomain()
                {
                    Id = -21,
                    Sort = 0,
                    Name = "华北",
                    ParentId = -100
                },
                new Yun.Site.CityDomain()
                {
                    Id = -3,
                    Sort = 0,
                    Name = "华中",
                    ParentId = -100
                },
                new Yun.Site.CityDomain()
                {
                    Id = -4,
                    Sort = 0,
                    Name = "华南",
                    ParentId = -100
                },
                new Yun.Site.CityDomain()
                {
                    Id = -5,
                    Sort = 0,
                    Name = "东北",
                    ParentId = -100
                },
                new Yun.Site.CityDomain()
                {
                    Id = -6,
                    Sort = 0,
                    Name = "西北",
                    ParentId = -100
                },
                new Yun.Site.CityDomain()
                {
                    Id = -7,
                    Sort = 0,
                    Name = "西南",
                    ParentId = -100
                },
                new Yun.Site.CityDomain()
                {
                    Id = -8,
                    Sort = 0,
                    Name = "港澳台",
                    ParentId = -100
                }
            };

            foreach (var a in _cities)
            {
                var tempParentId = a.ParentId;

                switch (a.Name)
                {
                    case "上海":
                    case "江苏省":
                    case "浙江省":
                    case "安徽省":
                    case "江西省":
                        tempParentId = -11;
                        break;
                    case "北京":
                    case "天津":
                    case "山西省":
                    case "山东省":
                    case "河北省":
                    case "内蒙古自治区":
                        tempParentId = -21;
                        break;
                    case "湖南省":
                    case "湖北省":
                    case "河南省":
                        tempParentId = -3;
                        break;
                    case "广东省":
                    case "广西省":
                    case "福建省":
                    case "海南省":
                        tempParentId = -4;
                        break;
                    case "辽宁省":
                    case "吉林省":
                    case "黑龙江省":
                        tempParentId = -5;
                        break;
                    case "陕西省":
                    case "新疆省":
                    case "勒泰省":
                    case "甘肃省":
                    case "宁夏":
                    case "青海省":
                        tempParentId = -6;
                        break;
                    case "重庆":
                    case "云南省":
                    case "贵州省":
                    case "西藏自治区":
                    case "四川省":
                        tempParentId = -7;
                        break;
                    case "香港特别行政区":
                    case "澳门特别行政区":
                    case "台湾省":
                        tempParentId = -8;
                        break;
                }

                citys.Add(new Yun.Site.CityDomain()
                {
                    Id = a.Id,
                    Sort = a.Sort,
                    Name = a.Name,
                    ParentId = tempParentId
                });
            }

            return citys;

        }

        /// <summary>
        /// 计算快递价格
        /// </summary>
        /// <param name="expressId"></param>
        /// <param name="quantity"></param>
        /// <param name="address"></param>
        /// <returns></returns>
        public static double GetExpressPrice(int expressId, int quantity, string address,
            double totalPrice, double totalWeight, double totalVolume, DeliveryTemplate fareDetail)
        {
            if (fareDetail == null || expressId <= 0) return 0;

            //_log.Trace("物流模板已匹配");

            //直接包邮
            if (fareDetail.FareFree < 0)
            {
                return 0;
            }

            //如果有包邮政策，首先计算包邮条件
            if (fareDetail.FareFreeConditions != null && fareDetail.FareFreeConditions.Any())
            {
                //筛选出匹配的递送方式
                var suitFareFree = fareDetail.FareFreeConditions.Where(e => e.DeliveryId == expressId).ToList();

                //如果没有，则不需要进行包邮处理
                if (suitFareFree.Any())
                {
                    //筛选出合适的城市
                    FareFreeCondition suit = null;

                    foreach (var condition in suitFareFree)
                    {
                        if (suit != null)
                        {
                            break;
                        }

                        if (condition.Cities == null)
                        {
                            continue;
                        }

                        foreach (var c in condition.Cities)
                        {
                            //直辖市，直辖区的变更，特殊处理

                            if (
                                address.IndexOf(GetSuitCityName((int) c.Key, c.Value), System.StringComparison.Ordinal) >=
                                0)
                            {
                                suit = condition;
                                break;
                            }
                        }
                    }

                    //如果有的话，则进行价格和数量的核实
                    if (suit != null)
                    {
                        //满多少件包邮
                        if (suit.FreeType == 0)
                        {
                            //按重量或按体积,按件计费
                            if ((fareDetail.PriceType == 1 && suit.Preferential >= totalWeight) ||
                                (fareDetail.PriceType == 2 && suit.Preferential >= totalVolume) || (
                                    fareDetail.PriceType == 0 && suit.Preferential <= quantity))
                            {
                                return 0;
                            }
                        }

                        //满多少元包邮
                        if (suit.FreeType == 1 && suit.Price <= totalPrice)
                        {
                            return 0;
                        }

                        //满多少件并且多少元才包邮
                        if (suit.FreeType == 2 && suit.Price <= totalPrice &&
                            ((fareDetail.PriceType == 1 && suit.Preferential >= totalWeight) ||
                             (fareDetail.PriceType == 2 && suit.Preferential >= totalVolume) || (
                                 fareDetail.PriceType == 0 && suit.Preferential <= quantity)))
                        {
                            return 0;
                        }
                    }
                }
            }

            PostfareDetail suitExpress = null;

            //判断适配哪个城市
            foreach (
                var postfareDetail in
                    fareDetail.Freight.Where(e => e.DeliveryId == expressId && e.Cities != null && e.Cities.Any()))
            {
                if (suitExpress != null)
                {
                    break;
                }

                if (postfareDetail.Cities == null)
                {
                    continue;
                }

                foreach (var c in postfareDetail.Cities)
                {
                    if (address.IndexOf(GetSuitCityName((int) c.Key, c.Value), System.StringComparison.Ordinal) >= 0)
                    {
                        suitExpress = postfareDetail;
                        break;
                    }
                }
            }

            //如果没有匹配到合适的价格，则取默认价格
            if (suitExpress == null)
            {
                suitExpress = fareDetail.Freight.FirstOrDefault(e => e.Cities == null || !e.Cities.Any());
                if (suitExpress == null)
                {
                    return 0;
                }
            }


            //选择计价单位
            var lastQuantity = 0.0;
            switch (fareDetail.PriceType)
            {
                case 0:
                    lastQuantity = quantity;
                    break;
                case 1:
                    lastQuantity = totalWeight;
                    break;
                case 2:
                    lastQuantity = totalVolume;
                    break;
            }

            //根据数量和运费条件直接计算价格
            if (lastQuantity <= suitExpress.BaseQuantity)
            {
                return suitExpress.BasePrice;
            }

            //超过初始数量
            var canDivisible = Math.Abs((lastQuantity - suitExpress.BaseQuantity)%suitExpress.AddQuantity) < 0.01;

            return ((int) ((lastQuantity - suitExpress.BaseQuantity)/suitExpress.AddQuantity) +
                    (canDivisible ? 0 : 1))*
                   suitExpress.AddPrice +
                   suitExpress.BasePrice;
        }

        /// <summary>
        /// 获取正确的可以比较的城市名字
        /// </summary>
        /// <param name="cityId"></param>
        /// <param name="cityName"></param>
        /// <returns></returns>
        private static string GetSuitCityName(int cityId, string cityName)
        {
            switch (cityId)
            {
                case 26041:
                    return "上海";
                case 26525:
                    return "北京";
                case 26543:
                    return "天津";
                case 29030:
                    return "重庆";
                default:
                    return cityName;
            }
        }
    }
}
