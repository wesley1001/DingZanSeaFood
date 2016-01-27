using BreezeShop.Core;
using System;
using System.Linq;
using System.Web.Mvc;
using System.Web.WebPages;
using Utilities.DataTypes.ExtensionMethods;
using Yun.Distribution.Request;
using Yun.User;
using Yun.User.Request;

namespace BreezeShop.Web.Areas.Admin.Controllers
{
    public class DistributionController : AdminAuthController
    {
        public ActionResult Index()
        {
            return View(YunClient.Instance.Execute(new GetDistributionLevelListRequest(), Token).DistributionLevels);
        }

        [HttpPost]
        public ActionResult AddTemplate(FormCollection collection, int id = 0, bool isDefault = false)
        {
            var level = collection["level"].Split(',');
            var levelId = collection["level-id"].Split(',');
            var isEdit = id > 0 || (isDefault && levelId.Any(e => !string.IsNullOrWhiteSpace(e)));

            var currentTemplate = isEdit
                ? YunClient.Instance.Execute(new GetDistributionLevelListRequest {TemplateId = isDefault ? 0 : id},
                    Token)
                    .DistributionLevels
                : null;

            var templateName = collection["templateName"];
            if (!isDefault && templateName.IsEmpty())
            {
                TempData["error"] = "编辑失败，模板名字不能为空!";
                //模板名字不能为空
                return View(currentTemplate);
            }

            //如果是编辑
            if (isEdit)
            {
                //只有非默认模板，才需要修改模板名字
                if (id > 0)
                {
                    if (!YunClient.Instance.Execute(
                        new UpdateDistributionTemplateRequest {Id = id, Name = templateName}, Token).Result)
                    {
                        TempData["error"] = "编辑失败，修改模板名字失败!";
                        return View(currentTemplate);
                    }
                }
            }
            else
            {
                //添加模板名字
                id =
                    (int)
                        YunClient.Instance.Execute(new AddDistributionTemplateRequest {Name = templateName}, Token)
                            .Result;
            }

            //更新具体的内容
            for (var i = 0; i < levelId.Length; i++)
            {
                //如果为空，则需要删除该条记录
                if (string.IsNullOrWhiteSpace(level[i]) && !string.IsNullOrWhiteSpace(levelId[i]))
                {
                    var r = YunClient.Instance.Execute(
                        new DeleteDistributionLevelRequest {Id = level[i].TryTo(0)},
                        Token);
                    if (r.Result)
                    {
                        continue;
                    }

                    return Content(r.ErrMsg);
                }

                if (string.IsNullOrWhiteSpace(levelId[i]) && !string.IsNullOrWhiteSpace(level[i]))
                {
                    var r = YunClient.Instance.Execute(
                        new AddDistributionLevelRequest
                        {
                            TemplateId = id,
                            Proportion = level[i].TryTo(0.0)
                        }, Token);
                    //ID不存在，内容存在，则需要添加
                    if (r.Result > 0)
                    {
                        continue;
                    }

                    return Content(r.ErrMsg);
                }

                if (!string.IsNullOrWhiteSpace(levelId[i]) && !string.IsNullOrWhiteSpace(level[i]))
                {
                    var r = YunClient.Instance.Execute(
                        new UpdateDistributionLevelRequest
                        {
                            Id = levelId[i].TryTo(0),
                            Proportion = level[i].TryTo(0.0)
                        }, Token);

                    if (r.Result)
                    {
                        continue;
                    }

                    return Content(r.ErrMsg);
                }

                if (string.IsNullOrWhiteSpace(levelId[i]) && string.IsNullOrWhiteSpace(level[i]))
                {
                    continue;
                }

                TempData["error"] = "编辑失败，删除记录失败!" + levelId[i] + "," + level[i];
                return View(currentTemplate);
            }

            TempData["success"] = "修改成功";
            return RedirectToAction("Index");
        }

        public ActionResult AddTemplate(int id = 0, bool isDefault = false)
        {
            if (isDefault || id > 0)
            {
                var d =
                    YunClient.Instance.Execute(new GetDistributionLevelListRequest {TemplateId = isDefault ? 0 : id},
                        Token)
                        .DistributionLevels;
                if (d != null)
                {
                    return View(d);
                }
            }

            return View();
        }

        public ActionResult DeleteTemplate(int id)
        {
            var r = YunClient.Instance.Execute(new DeleteDistributionTemplateRequest {Id = id}, Token).Result;

            return Json(r);
        }

        public ActionResult Users(int p = 1)
        {
            var req = YunClient.Instance.Execute(new GetCooperationRequest {PageNum = p, PageSize = 20}, Token);

            var page = new PageModel<Yun.Distribution.DistributionUser>
            {
                Items = req.Items,
                CurrentPage = p,
                TotalItems = req.TotalItem,
                ItemsPerPage = 20
            };

            if (req.Items != null && req.Items.Any())
            {
                ViewData["ParentUserList"] = YunClient.Instance.Execute(new FindUsersRequest
                {
                    UserIds = string.Join(",", req.Items.Select(e => e.ParentId).Distinct())
                }).Users;
            }

            return View(page);
        }

        /// <summary>
        /// 可以成为分销商的用户搜索
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
        /// <param name="realname"></param>
        /// <param name="p"></param>
        /// <returns></returns>
        public ActionResult UserSearch(string email, string mobile, string nick, DateTime? minregtime, DateTime? maxregtime,
            double? minmoney,
            double? maxmoney, long? minscore, long? maxscore, double minprepaid = 0, double maxprepaid = 0, string realname = "" ,int p = 1)
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
                MaxPrepaidCard = maxprepaid,
                RealName = realname
            });

            page.Items = req.Users;
            page.CurrentPage = p;
            page.TotalItems = req.TotalItem;

            return View(page);
        }


        public ActionResult CommissionDetails(int p = 1, int tradeId = 0, DateTime? minDateTime = null, DateTime? maxDateTime = null)
        {
            var req =
                YunClient.Instance.Execute(new GetDistributorsHistoryRequest
                {
                    PageNum = p,
                    PageSize = 20,
                    TradeId = tradeId,
                    StartDateTime = minDateTime,
                    EndDateTime = maxDateTime,
                    TradeStatus = "TRADE_FINISHED"
                });

            var page = new PageModel<Yun.Distribution.DistributionHistory>
            {
                Items = req.DistributionHistory,
                CurrentPage = p,
                TotalItems = req.TotalItem,
                ItemsPerPage = 20
            };

            return View(page);
        }

        /// <summary>
        /// 成为分销商
        /// </summary>
        /// <param name="nick"></param>
        /// <param name="parentId"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult BecomeDistributor(string nick ,int parentId = 0)
        {
            var req =
                YunClient.Instance.Execute(new AuditCooperationRequest
                {
                    SuperiorDistributorId = parentId,
                    UserName = nick,
                    Ip = Request.UserHostAddress
                });

            return Json(req.Result);
        }
    }
}
