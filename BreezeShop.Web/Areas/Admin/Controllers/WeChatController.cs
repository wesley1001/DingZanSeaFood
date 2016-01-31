using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Web.Mvc;
using BreezeShop.Core;
using BreezeShop.Core.FileFactory;
using BreezeShop.Web.Areas.Admin.Models;
using Senparc.Weixin.Entities;
using Senparc.Weixin.MP;
using Senparc.Weixin.MP.CommonAPIs;
using Senparc.Weixin.MP.Entities;
using Senparc.Weixin.MP.Entities.Menu;
using Utilities.DataTypes.ExtensionMethods;
using Yun.WeiXin;
using Yun.WeiXin.Request;

namespace BreezeShop.Web.Areas.Admin.Controllers
{
    public class WeChatController : AdminAuthController
    {
        private Yun.WeiXin.AccountDomain _wxAccount = null;

        //初始化account，只允许一个一个微信账号
        public WeChatController()
        {
            var accounts = YunClient.Instance.Execute(new GetWxAccountsRequest {PageNum = 1, PageSize = 1}).Accounts;
            if (accounts != null && accounts.Any())
            {
                _wxAccount = accounts[0];
            }
        }

        /// <summary>
        /// 如果未设置微信账号，则跳转到设置界面
        /// </summary>
        /// <param name="filterContext"></param>
        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            var actionName = filterContext.ActionDescriptor.ActionName;

            if (actionName.ToLower() != "index" && _wxAccount == null)
            {
                TempData["error"] = "微信账号还未设置，请先设置微信账号的基本信息";
                filterContext.Result = new RedirectResult(Url.Action("Index", "WeChat"));
                return;
            }

            base.OnActionExecuting(filterContext);
        }

        /// <summary>
        /// 微信公众账号基本信息编辑
        /// </summary>
        /// <returns></returns>
        public ActionResult Index()
        {
            WeChatEditModel weChat = null;

            if (_wxAccount!=null)
            {
                weChat = new WeChatEditModel
                {
                    AppSecret = _wxAccount.Secret,
                    AppId = _wxAccount.AppId,
                    ClientUrl = _wxAccount.CallbackUrl,
                    EncodingAESKey = _wxAccount.Encodingaeskey,
                    Level = _wxAccount.Level,
                    Name = _wxAccount.Name,
                    OriginalId = _wxAccount.Original,
                    QrCode = _wxAccount.QrCode,
                    Token = _wxAccount.AccessToken,
                    WxAccount = _wxAccount.Account,
                    Description = _wxAccount.Description
                };
            }

            return View(weChat);
        }

        [HttpPost]
        public ActionResult Index(WeChatEditModel model)
        {
            if (ModelState.IsValid)
            {
                if (_wxAccount != null)
                {
                    var img = FileManage.UploadOneFile();

                    //有公众账号的情况下，则更新
                    var r = YunClient.Instance.Execute(new UpdateWxAccountRequest
                    {
                        AppSecret = model.AppSecret,
                        AppId = model.AppId,
                        EncodingAESKey = model.EncodingAESKey,
                        Level = model.Level,
                        Name = model.Name,
                        Original = model.OriginalId,
                        Qrcode = string.IsNullOrEmpty(img) ? model.QrCode : img,
                        Token = model.Token,
                        Account = model.WxAccount,
                        Description = model.Description,
                        Id = _wxAccount.Id
                    }, Token);

                    if (r.Result)
                    {
                        TempData["success"] = "公众号信息已成功更新";
                    }
                    else
                    {
                        TempData["error"] = "公众号信息更新失败，错误信息：" + r.ErrMsg;
                        return PartialView(model);
                    }
                }
                else
                {
                    //否则创建
                    var r = YunClient.Instance.Execute(new AddWxAccountRequest
                    {
                        AppSecret = model.AppSecret,
                        AppId = model.AppId,
                        EncodingAESKey = model.EncodingAESKey,
                        Level = model.Level,
                        Name = model.Name,
                        Original = model.OriginalId,
                        Qrcode = FileManage.UploadOneFile(),
                        Token = model.Token,
                        Account = model.WxAccount,
                        Description = model.Description
                    }, Token);

                    if (r.Result > 0)
                    {
                        TempData["success"] = "公众号信息已成功更新";
                    }
                    else
                    {
                        TempData["error"] = "公众号信息更新失败，错误信息：" + r.ErrMsg;
                        return PartialView(model);
                    }
                }

                return RedirectToAction("Index");
            }

            TempData["error"] = "请输入必填项";
            return PartialView(model);
        }

        public ActionResult DeleteTextReply(int id)
        {
            return Json(YunClient.Instance.Execute(new DeleteWxTextRequest{RuleId = id}, Token).Result);
        }

        public ActionResult DeleteNewsReply(int id)
        {
            return Json(YunClient.Instance.Execute(new DeleteWxNewsRequest { RuleId = id }, Token).Result);
        }

        public ActionResult AddNewsReply(int id = 0)
        {
            if (id > 0)
            {
                var req = YunClient.Instance.Execute(new GetNewsReplyRequest { RuleId = id });
                ViewData["ruleDetail"] = req.RuleDetail;
                ViewData["ruleReplies"] = req.Replies;
            }

            return View();
        }

        [HttpPost]
        public ActionResult AddNewsReply(FormCollection collection, int id = 0)
        {
            var newsJson = new List<NewsJsonModel>();

            var title = collection["title"].Split(',');
            var creator = collection["creator"].Split(',');
            var sortOrder = collection["sort_order"].Split(',');
            var thumbs = collection["thumbs"].Split(',');
            var description = collection["description"].Split(',');
            var source = collection["source"].Split(',');

            //检测数据
            for (var i = 0; i < title.Length; i++)
            {
                if (string.IsNullOrEmpty(title[i]))
                {
                    continue;
                }

                newsJson.Add(new NewsJsonModel
                {
                    creator = creator[i],
                    description = description[i],
                    sort = sortOrder[i].TryTo(0),
                    thumb = thumbs[i],
                    title = title[i],
                    url = source[i]
                });
            }

            var b = AddBaseRule(collection);
            if (b.ErrorCode <= 0)
            {
                return Json(new { error = b.ErrorCode });
            }


            if (id <= 0)
            {
                var req = YunClient.Instance.Execute(new AddWxNewsReplyRequest
                {
                    AccountId = _wxAccount.Id,
                    Disabled = b.Disabled ? 1 : 0,
                    Keywords = b.TriggerList,
                    Name = b.RuleName,
                    Sort = b.DisplayOrder,
                    NewsJson = newsJson
                }, Token);

                return Json(req.Result > 0 ? new { error = "" } : new { error = req.ErrMsg });
            }

            var reqUpdate = YunClient.Instance.Execute(new UpdateWxNewsRequest
            {
                Disabled = b.Disabled ? 1 : 0,
                Keywords = b.TriggerList,
                Name = b.RuleName,
                Sort = b.DisplayOrder,
                RuleId = id,
                NewsJson = newsJson
            }, Token);

            return Json(reqUpdate.Result ? new { error = "" } : new { error = reqUpdate.ErrMsg });
        }

        public ActionResult AddTextReply(int id = 0)
        {
            if (id > 0)
            {
                var req = YunClient.Instance.Execute(new GetBasicTextReplyRequest {RuleId = id});
                ViewData["ruleDetail"] = req.RuleDetail;
                ViewData["ruleReplies"] = req.Replies;
            }

            return View();
        }

        private static WechatBaseRuleEditModel AddBaseRule(NameValueCollection collection)
        {
            var r = new WechatBaseRuleEditModel();

            #region 规则保存通用
            var ruleName = collection["name"];
            if (string.IsNullOrEmpty(ruleName))
            {
                r.ErrorCode = -1;
                return r;
            }

            var status = collection["status"];
            var istop = collection["istop"].TryTo(0);
            var displayOrder = istop == 1 ? 255 : collection["displayorder_rule"].TryTo(0);
            var keywordinput = collection["keywordinput"];

            var triggerTypeList = new List<KeyValuePair<TriggerTypeEnum, string>>();
            if (!string.IsNullOrEmpty(keywordinput))
            {
                triggerTypeList.Add(new KeyValuePair<TriggerTypeEnum, string>(TriggerTypeEnum.Equal, keywordinput));
            }

            if (collection["adv-trigger"] == "1")
            {
                foreach (var triggerType in new[] { "container", "regular", "direct" })
                {
                    TriggerTypeEnum triggerEnum;
                    if (Enum.TryParse(triggerType, true, out triggerEnum))
                    {
                        triggerTypeList.AddRange(
                            (collection[triggerType + "-keywords"] ?? "").Split(new[] { "," },
                                StringSplitOptions.RemoveEmptyEntries)
                                .Select(k => new KeyValuePair<TriggerTypeEnum, string>(triggerEnum, k)));
                    }
                    else
                    {
                        r.ErrorCode = -3;
                        return r;
                    }
                }
            }

            if (!triggerTypeList.Any())
            {
                r.ErrorCode = -2;
                return r;
            }

            r.RuleName = ruleName;
            r.ErrorCode = 1;
            r.DisplayOrder = displayOrder;
            r.Disabled = status == "0";
            r.TriggerList = triggerTypeList;
            return r;
            #endregion
        }

        [HttpPost]
        public ActionResult AddTextReply(FormCollection collection, int id = 0)
        {
            var b = AddBaseRule(collection);
            if (b.ErrorCode <= 0)
            {
                return Json(new {error = b.ErrorCode});
            }

            if (id <= 0)
            {
                var req = YunClient.Instance.Execute(new AddWxTextRequest
                {
                    AccountId = _wxAccount.Id,
                    Content =
                        (collection["replycontent"] ?? "").Split(new[] {","}, StringSplitOptions.RemoveEmptyEntries)
                            .ToList(),
                    Disabled = b.Disabled ? 1 : 0,
                    Keywords = b.TriggerList,
                    Name = b.RuleName,
                    Sort = b.DisplayOrder
                }, Token);

                return Json(req.Result > 0 ? new {error = ""} : new {error = req.ErrMsg});
            }

            var reqUpdate = YunClient.Instance.Execute(new UpdateWxTextRequest
            {
                Content =
                    (collection["replycontent"] ?? "").Split(new[] {","}, StringSplitOptions.RemoveEmptyEntries)
                        .ToList(),
                Disabled = b.Disabled ? 1 : 0,
                Keywords = b.TriggerList,
                Name = b.RuleName,
                Sort = b.DisplayOrder,
                RuleId = id
            }, Token);

            return Json(reqUpdate.Result ? new {error = ""} : new {error = reqUpdate.ErrMsg});
        }

        public ActionResult NewsReply(string title, int p = 1)
        {
            var page = new PageModel<Yun.WeiXin.RuleDomain>();

            var req =
                YunClient.Instance.Execute(new GetNewsRulesRequest
                {
                    AccountId = _wxAccount.Id,
                    PageNum = p,
                    PageSize = 20,
                    Title = title
                }, Token);

            page.Items = req.Rules;
            page.CurrentPage = p;
            page.TotalItems = req.TotalItem;

            return View(page);   
        }

        /// <summary>
        /// 纯文字回复
        /// </summary>
        /// <returns></returns>
        public ActionResult TextReply(string title ,int p = 1)
        {
            var page = new PageModel<Yun.WeiXin.RuleDomain>();

            var req =
                YunClient.Instance.Execute(new GetBasicTextRulesRequest
                {
                    AccountId = _wxAccount.Id,
                    PageNum = p,
                    PageSize = 20,
                    Title = title
                }, Token);

            page.Items = req.Rules;
            page.CurrentPage = p;
            page.TotalItems = req.TotalItem;

            return View(page);   
        }

        [HttpPost]
        public ActionResult AddUserApiReply(FormCollection collection, int id = 0)
        {
            var b = AddBaseRule(collection);
            if (b.ErrorCode <= 0)
            {
                return Json(new { error = b.ErrorCode });
            }

            if (id <= 0)
            {
                var req = YunClient.Instance.Execute(new AddWxCustomApiRequest
                {
                    AccountId = _wxAccount.Id,
                    Disabled = b.Disabled ? 1 : 0,
                    Keywords = b.TriggerList,
                    Name = b.RuleName,
                    Sort = b.DisplayOrder,
                    AccessToken = collection["wetoken"],
                    DefaultText = collection["default-text"],
                    RemoteUrl = collection["apiurl"]
                }, Token);

                return Json(req.Result > 0 ? new { error = "" } : new { error = req.ErrMsg });
            }

            var reqUpdate = YunClient.Instance.Execute(new UpdateWxCustomApiRequest
            {
                Disabled = b.Disabled ? 1 : 0,
                Keywords = b.TriggerList,
                Name = b.RuleName,
                Sort = b.DisplayOrder,
                RuleId = id,
                AccessToken = collection["wetoken"],
                DefaultText = collection["default-text"],
                RemoteUrl = collection["apiurl"]
            }, Token);

            return Json(reqUpdate.Result ? new { error = "" } : new { error = reqUpdate.ErrMsg });
        }


        public ActionResult AddUserApiReply(int id = 0)
        {
            if (id > 0)
            {
                var req = YunClient.Instance.Execute(new GetUserApiRepliesRequest { RuleId = id });
                ViewData["ruleDetail"] = req.RuleDetail;
                ViewData["ruleReplies"] = req.Reply;
            }

            return View();
        }

        public ActionResult DeleteUserApiReply(int id)
        {
            return Json(YunClient.Instance.Execute(new DeleteWxCustomApiRequest { RuleId = id }, Token).Result);
        }


        public ActionResult UserApiReply(string title, int p = 1)
        {
            var page = new PageModel<Yun.WeiXin.RuleDomain>();

            var req =
                YunClient.Instance.Execute(new GetUserApiRulesRequest
                {
                    AccountId = _wxAccount.Id,
                    PageNum = p,
                    PageSize = 20,
                    Title = title
                }, Token);

            page.Items = req.Rules;
            page.CurrentPage = p;
            page.TotalItems = req.TotalItem;

            return View(page);   
        }

        public ActionResult CustomMenus()
        {
            if (!string.IsNullOrEmpty(_wxAccount.AppId) && !string.IsNullOrEmpty(_wxAccount.Secret))
            {
                var accessToken = AccessTokenContainer.TryGetAccessToken(_wxAccount.AppId, _wxAccount.Secret);
                var result = CommonApi.GetMenu(accessToken);

                if (result != null)
                {
                    return View(result);
                }
            }

            return View();
        }

        public ActionResult GetMenu(string token)
        {
            if (string.IsNullOrEmpty(_wxAccount.AppId) || string.IsNullOrEmpty(_wxAccount.Secret))
            {
                return Json(new {error = "AppId或AppSecret为空"}, JsonRequestBehavior.AllowGet);
            }

            var accessToken = AccessTokenContainer.TryGetAccessToken(_wxAccount.AppId, _wxAccount.Secret);
            var result = CommonApi.GetMenu(accessToken);
            if (result == null)
            {
                return Json(new {error = "菜单不存在或验证失败！"}, JsonRequestBehavior.AllowGet);
            }

            return Json(result, JsonRequestBehavior.AllowGet);
        }


        [HttpPost]
        public ActionResult CreateMenu(GetMenuResultFull resultFull, MenuMatchRule menuMatchRule)
        {
            if (string.IsNullOrEmpty(_wxAccount.AppId) || string.IsNullOrEmpty(_wxAccount.Secret))
            {
                return Json(new { error = "AppId或AppSecret为空" }, JsonRequestBehavior.AllowGet);
            }

            var token = AccessTokenContainer.TryGetAccessToken(_wxAccount.AppId, _wxAccount.Secret);

            var useAddCondidionalApi = menuMatchRule != null && !menuMatchRule.CheckAllNull();
            var apiName = string.Format("使用接口：{0}。", (useAddCondidionalApi ? "个性化菜单接口" : "普通自定义菜单接口"));
            try
            {
                //重新整理按钮信息
                WxJsonResult result = null;
                IButtonGroupBase buttonGroup = null;
                if (useAddCondidionalApi)
                {
                    //个性化接口
                    buttonGroup = CommonApi.GetMenuFromJsonResult(resultFull, new ConditionalButtonGroup()).menu;

                    var addConditionalButtonGroup = buttonGroup as ConditionalButtonGroup;
                    addConditionalButtonGroup.matchrule = menuMatchRule;
                    result = CommonApi.CreateMenuConditional(token, addConditionalButtonGroup);
                    apiName += string.Format("menuid：{0}。", (result as CreateMenuConditionalResult).menuid);
                }
                else
                {
                    //普通接口
                    buttonGroup = CommonApi.GetMenuFromJsonResult(resultFull, new ButtonGroup()).menu;
                    result = CommonApi.CreateMenu(token, buttonGroup);
                }

                return Json(new {success = result.errmsg == "ok", message = "菜单更新成功。" + apiName });
            }
            catch (Exception ex)
            {
                return Json(new {success = false, message = string.Format("更新失败：{0}。{1}", ex.Message, apiName)});
            }
        }

        public ActionResult EditCustomMenu()
        {
            return View();
        }

        public ActionResult DeleteCustomMenu(string menuId)
        {
            try
            {
                if (string.IsNullOrEmpty(_wxAccount.AppId) || string.IsNullOrEmpty(_wxAccount.Secret))
                {
                    return Json(new { error = true, message = "AppId或AppSecret为空" }, JsonRequestBehavior.AllowGet);
                }

                var token = AccessTokenContainer.TryGetAccessToken(_wxAccount.AppId, _wxAccount.Secret);

                var result = string.IsNullOrEmpty(menuId) ? CommonApi.DeleteMenu(token) : CommonApi.DeleteMenuConditional(token,menuId);
                var json = new
                {
                    error = result.errmsg != "ok",
                    message = result.errmsg
                };

                return Json(json, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                var json = new { error = true, message = ex.Message };
                return Json(json, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult NewReplyAside(int index = 0, List<Yun.WeiXin.ImageTextReplyModel> model = null)
        {
            ViewData["Index"] = index;
            return PartialView(model);
        }

        private readonly IDictionary<string, string> _specialReply = new Dictionary<string, string>
            {
                {"imageReply", "image"},
                {"voiceReply", "voice"},
                {"videoReply", "video"},
                {"locationReply", "location"},
                {"locationselectReply", "event_location"},
                {"linkReply", "link"},
            };

        private readonly IDictionary<string, string> _systemReply = new Dictionary<string, string>
            {
                {"default_reply", "default"},
                {"welcome_reply", "event_subscribe"}
            };

        [HttpPost]
        public ActionResult SystemReply(FormCollection collection)
        {
            foreach (var i in _systemReply)
            {
                YunClient.Instance.Execute(
                    new SetMsgReplyRuleRequest
                    {
                        AccountId = _wxAccount.Id,
                        MsgType = i.Value,
                        RuleId = collection[i.Key].TryTo(0) > 0 ? collection[i.Key] : ""
                    }, Token);
            }

            TempData["success"] = "提交成功，已成功保存数据";
            return RedirectToAction("SystemReply");
        }


        public ActionResult SystemReply()
        {
            foreach (var i in _systemReply)
            {
                ViewData[i.Key] =
                    YunClient.Instance.Execute(new GetMsgReplyRuleRequest
                    {
                        AccountId = _wxAccount.Id,
                        MsgType = i.Value
                    })
                        .Rules.FirstOrDefault();
            }

            return View(YunClient.Instance.Execute(new FindRulesRequest {AccountId = _wxAccount.Id}).Rules);
        }

        public ActionResult SpecialReply()
        {
            foreach (var i in _specialReply)
            {
                ViewData[i.Key] =
                    YunClient.Instance.Execute(new GetMsgReplyRuleRequest
                    {
                        AccountId = _wxAccount.Id,
                        MsgType = i.Value
                    })
                        .Rules.FirstOrDefault();
            }

            return View(YunClient.Instance.Execute(new FindRulesRequest {AccountId = _wxAccount.Id}).Rules);
        }
            
        [HttpPost]
        public ActionResult SpecialReply(FormCollection collection)
        {
            foreach (var i in _specialReply)
            {
                YunClient.Instance.Execute(
                    new SetMsgReplyRuleRequest
                    {
                        AccountId = _wxAccount.Id,
                        MsgType = i.Value,
                        RuleId = collection[i.Key].TryTo(0) > 0 ? collection[i.Key] : ""
                    }, Token);
            }

            TempData["success"] = "提交成功，已成功保存数据";
            return RedirectToAction("SpecialReply");
        }

    }
}
