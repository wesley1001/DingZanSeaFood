using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using BreezeShop.Core;
using BreezeShop.Core.DataProvider;
using BreezeShop.Core.FileFactory;
using BreezeShop.Web.Areas.Admin.Models;
using Yun.Pay.Request;
using Yun.User;
using Yun.User.Request;

namespace BreezeShop.Web.Areas.Admin.Controllers
{
    public class MemberController : AdminAuthController
    {
        /// <summary>
        /// 普通用户列表
        /// </summary>
        /// <param name="email"></param>
        /// <param name="mobile"></param>
        /// <param name="nick"></param>
        /// <param name="minregtime"></param>
        /// <param name="maxregtime"></param>
        /// <param name="minmoney"></param>
        /// <param name="maxmoney"></param>
        /// <param name="minscore"></param>
        /// <param name="maxscore"></param>
        /// <param name="minprepaid"></param>
        /// <param name="maxprepaid"></param>
        /// <param name="p"></param>
        /// <returns></returns>
        public ActionResult Index(string email, string mobile, string nick, DateTime? minregtime, DateTime? maxregtime,
            double? minmoney,
            double? maxmoney, long? minscore, long? maxscore, double minprepaid = 0, double maxprepaid = 0, int p = 1)
        {
            var page = new PageModel<UserDetail>();
            var req = YunClient.Instance.Execute(new FindUsersRequest
            {
                Email = email,
                Mobile = mobile,
                Nick = nick,
                MinMoney = minmoney,
                MaxMoney = maxmoney,
                MinScore = minscore,
                MaxScore = maxscore,
                MinRegTime = minregtime,
                MaxRegTime = maxregtime,
                PageNum = p,
                PageSize = 20,
                MinPrepaidCard = minprepaid,
                MaxPrepaidCard = maxprepaid
            });

            page.Items = req.Users;
            page.CurrentPage = p;
            page.TotalItems = req.TotalItem;

            return View(page);
        }

        private void SetUserLocationData(string province, string city, string area)
        {
            //获取省的数据
            var provinces = SystemCity.GetCities(0);
            ViewData["Provinces"] = provinces.Select(e => new SelectListItem
            {
                Text = e.Name,
                Value = e.Name + "-" + e.Id,
                Selected = !string.IsNullOrEmpty(province) && e.Name.IndexOf(province, StringComparison.Ordinal) >= 0
            });

            //获取市的数据
            if (string.IsNullOrEmpty(province))
            {
                var t = ((IEnumerable<SelectListItem>)ViewData["Provinces"]).ToList();
                t.Insert(0, new SelectListItem
                {
                    Text = "-请选择省-",
                    Value = ""
                });
                ViewData["Provinces"] = t;


                ViewData["Cities"] = new List<SelectListItem>
                {
                    new SelectListItem
                    {
                        Text = "-请选择城市-",
                        Value = ""
                    }
                };
                ViewData["Areas"] = new List<SelectListItem>
                {
                    new SelectListItem
                    {
                        Text = "-请选择地区-",
                        Value = ""
                    }
                };
            }
            else
            {
                var cities =
                    SystemCity.GetCities(
                        provinces.Single(e => e.Name.IndexOf(province, StringComparison.Ordinal) >= 0).Id);
                ViewData["Cities"] = cities.Select(e => new SelectListItem
                {
                    Text = e.Name,
                    Value = e.Name + "-" + e.Id,
                    Selected =
                        !string.IsNullOrEmpty(city) && e.Name.IndexOf(city, StringComparison.Ordinal) >= 0
                });

                if (string.IsNullOrEmpty(city))
                {
                    ViewData["Areas"] = new List<SelectListItem>
                    {
                        new SelectListItem
                        {
                            Text = "-请选择地区-",
                            Value = ""
                        }
                    };
                }
                else
                {
                    //获取区的数据
                    var areas =
                        SystemCity.GetCities(
                            cities.Single(e => e.Name.IndexOf(city, StringComparison.Ordinal) >= 0).Id);
                    ViewData["Areas"] = areas.Select(e => new SelectListItem
                    {
                        Text = e.Name,
                        Value = e.Name + "-" + e.Id,
                        Selected =
                            !string.IsNullOrEmpty(area) && e.Name.IndexOf(area, StringComparison.Ordinal) >= 0
                    });
                }
            }
        }

        public ActionResult Edit(int id = 0)
        {
            var req = YunClient.Instance.Execute(new GetUserRequest {UserId = id}, id > 0 ? null : Token).User;

            if (req == null)
            {
                Error404Message = "当前用户不存在";
                return RedirectToAction("Error404", "Home");
            }

            SetUserLocationData(req.Province, req.City, req.Area);

            var rtm = new UpdateMemberModel
            {
                Avatar = req.Avatar,
                UserId = req.UserId,
                Nick = req.Nick,
                Sex = req.Sex,
                CreateTime = req.CreateTime,
                LastVisit = req.LastVisit,
                Email = req.Email,
                Mobile = req.Mobile,
                Score = req.Score,
                Money = req.Money,
                RealName = req.RealName,
                PrepaidCard = req.PrepaidCard,
                Province = req.Province,
                Address = req.Address,
                Area = req.Area,
                City = req.City,
                Remark = req.Remark
            };

            if (!string.IsNullOrEmpty(req.Birthday))
            {
                rtm.Birthday = Convert.ToDateTime(req.Birthday);
            }

            return View(rtm);
        }

        public ActionResult WithdrawCashList(int p = 1,DateTime? minDateTime = null, DateTime? maxDateTime = null, int? status = null)
        {
            var req =  YunClient.Instance.Execute(new GetWithdrawalsDetailListRequest
            {
                PageSize = 20,
                PageNum = p,
                MinCreateTime = minDateTime,
                MaxCreateTime = maxDateTime,
                Status = status
            });

            var page = new PageModel<Yun.Pay.WithdrawalsDetail>
            {
                Items = req.WithdrawalsDetails,
                CurrentPage = p,
                TotalItems = req.TotalItem,
                ItemsPerPage = 20
            };

            return View(page);
        }

        public ActionResult ModifyPassword()
        {
            return PartialView();
        }

        [HttpPost]
        public ActionResult ModifyPassword(int id, ResetPasswordModel model)
        {
            if (ModelState.IsValid)
            {
                var req = YunClient.Instance.Execute(new ModifyPasswordUserRequest
                {
                    Id = id,
                    Password = model.Password,
                    AppSecret = YunClient.AppSecret
                }, Token);

                return Json(!req.IsError);
            }
            return Json(false);
        }

        public ActionResult ModifyMoney()
        {
            return PartialView();
        }

        [HttpPost]
        public ActionResult ModifyMoney(int id, ModifyMoneyModel model)
        {
            //如果是减少款项，需要去判断用户余额，防止出现负数
            if (model.Money < 0)
            {
                var u = YunClient.Instance.Execute(new GetUserRequest {UserId = id});
                if (double.Parse(u.User.Money) - model.Money < 0)
                {
                    return Json(false);
                }
            }

            var req = YunClient.Instance.Execute(new ModifyUserMoneyRequest
            {
                UserId = id,
                Money = model.Money,
                Remark = model.Remark,
                Ip = Request.UserHostAddress,
                AllowNegative = true
            }, Token).Result;

            return Json(req);
        }


        [HttpPost]
        public ActionResult Edit(UpdateMemberModel model, string redirectUrl = "")
        {
            var province = string.IsNullOrEmpty(model.Province) ? "" : model.Province.Split('-')[0];
            var city = string.IsNullOrEmpty(model.City) ? "" : model.City.Split('-')[0];
            var area = string.IsNullOrEmpty(model.Area) ? "" : model.Area.Split('-')[0];

            var img = FileManage.UploadOneFile();

            var req = YunClient.Instance.Execute(new ModifyUserInfoRequest
            {
                Avatar = string.IsNullOrEmpty(img) ? model.Avatar : img,
                Email = model.Email,
                IsMale = model.Sex == "男" ? 1 : 0,
                Mobile = model.Mobile,
                Nick = model.Nick,
                RealName = model.RealName,
                Address = model.Address,
                Province = province,
                City = city,
                Area = area,
                Birthday = model.Birthday,
                Remark = model.Remark,
                Phone = model.Mobile
            }, Token);

            if (req.Result > 0)
            {
                TempData["success"] = "保存成功";
                if (!string.IsNullOrEmpty(redirectUrl))
                {
                    return Redirect(redirectUrl);
                }
            }

            SetUserLocationData(province, city, area);

            TempData["error"] = "编辑失败，错误原因：" + req.ErrMsg;
            return View(model);
        }
    }
}
