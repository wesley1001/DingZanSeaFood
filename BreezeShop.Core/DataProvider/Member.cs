using System.Collections.Generic;
using BreezeShop.Core.Cache;
using BreezeShop.Core.Model.Enums;
using BreezeShop.Core.UserTrace;
using Utilities.Caching.Interfaces;
using Utilities.DataTypes.ExtensionMethods;
using Utilities.Encryption.ExtensionMethods;
using Utilities.Web.ExtensionMethods;
using Yun.User;
using Yun.User.Request;

namespace BreezeShop.Core.DataProvider
{
    public class Member
    {
        private static ICache<string> _userCache = new FileCache<string>("user");

        /// <summary>
        /// 获取当前登录人
        /// </summary>
        /// <returns></returns>
        public static UserDetail GetLoginMember(string token = null)
        {
            var t = string.IsNullOrEmpty(token)?Token: token;
            if (t.IsNullOrEmpty()) return null;

            return _userCache.Get(t.Hash(),
                () => YunClient.Instance.Execute(new GetUserRequest(), t).User);
        }

        /// <summary>
        /// 存在本地cookie中的授权代码
        /// </summary>
        public static string Token
        {
            get { return CookieHelper.GetCookie("usertoken"); }
            set { CookieHelper.WriteCookie("usertoken", value); }
        }

        public static string OpenId
        {
            get { return CookieHelper.GetCookie("wxopenid"); }
            set { CookieHelper.WriteCookie("wxopenid", value); }
        }

        public static string AdminToken
        {
            get { return CookieHelper.GetCookie("dtoken"); }
            set { CookieHelper.WriteCookie("dtoken", value); }
        }


        /// <summary>
        ///普通用户登出
        /// </summary>
        public static void Exit()
        {
            CookieHelper.DeleteCookie("usertoken");
        }

        public static void AdminExit()
        {
            CookieHelper.DeleteCookie("dtoken");
        }

        /// <summary>
        /// 一般登录
        /// </summary>
        /// <param name="userName"></param>
        /// <param name="password"></param>
        /// <param name="ip"></param>
        /// <returns></returns>
        public static KeyValuePair<bool, string> Login(string userName, string password, string ip, int loginType = 0)
        {
            var u =
                YunClient.Instance.Execute(new LoginRequest
                {
                    AppSecret = YunClient.AppSecret,
                    Ip = ip,
                    Password = password,
                    UserName = userName
                });

            if (u.IsError || u.UserId <= 0) return new KeyValuePair<bool, string>(false, u.ErrMsg);

            if (loginType <= 0)
            {
                Token = u.Token;
            }
            else
            {
                AdminToken = u.Token;
            }

            return new KeyValuePair<bool, string>(true, u.Token);
        }

        /// <summary>
        /// 动态码登录
        /// </summary>
        /// <param name="phone"></param>
        /// <param name="code"></param>
        /// <param name="ip"></param>
        /// <returns></returns>
        public static KeyValuePair<bool, string> PhoneLogin(string phone, string code, string ip)
        {
            var u =
                YunClient.Instance.Execute(new PhoneDynamicLoginRequest
                {
                    UserFlag = phone,
                    Phone = phone,
                    Code = code,
                    Ip = ip,
                    ShopId = GlobeInfo.InitiatedShopId,
                    CompanyId = GlobeInfo.InitiatedCompanyId
                });

            if (u.IsError || u.UserId <= 0) return new KeyValuePair<bool, string>(false, u.ErrMsg);

            Token = u.Token;
            return new KeyValuePair<bool, string>(true, null);
        }


        /// <summary>
        /// 发送登录动态码
        /// </summary>
        /// <param name="phone"></param>
        /// <returns></returns>
        public static KeyValuePair<bool, string> SendLoginCode(string phone)
        {
            var u =
                YunClient.Instance.Execute(new SendLoginCodePhoneRequest
                {
                    MobilePhone = phone,
                    CompanyId = GlobeInfo.InitiatedCompanyId
                });

            if (u.IsError || u.Result.IsNullOrEmpty()) return new KeyValuePair<bool, string>(false, u.ErrMsg);

            return new KeyValuePair<bool, string>(true, null);
        }

        /// <summary>
        /// 使用手机注册
        /// </summary>
        /// <param name="phone"></param>
        /// <param name="password"></param>
        /// <param name="code"></param>
        /// <param name="ip"></param>
        /// <returns></returns>
        public static KeyValuePair<bool, string> PhoneRegister(string phone, string password, string code, string ip)
        {
            var u =
                YunClient.Instance.Execute(new PhoneRegisterRequest
                {
                    Phone = phone,
                    Password = password,
                    Code = code,
                    ShopId = GlobeInfo.InitiatedShopId,
                    CompanyId = GlobeInfo.InitiatedCompanyId,
                    Ip = ip,
                    Secret = YunClient.AppSecret
                });

            if (u.IsError || u.UserId <= 0) return new KeyValuePair<bool, string>(false, u.ErrMsg);

            Token = u.Token;
            return new KeyValuePair<bool, string>(true, null);
        }

        /// <summary>
        /// 判断用户是否存在
        /// </summary>
        /// <param name="content"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public static bool ExistUser(string content, ExistUserEnum type)
        {
            var u =
                YunClient.Instance.Execute(new ExistUserRequest
                {
                    Content = content,
                    Type = (int)type
                });

            return !u.IsError && u.Result;
        }

        /// <summary>
        /// 修改当前登录人的密码
        /// </summary>
        /// <param name="password"></param>
        /// <param name="newpassword"></param>
        /// <returns></returns>
        public static bool ModifyLoginPassword(string password, string newpassword)
        {
            var u =
                YunClient.Instance.Execute(new ModifyPasswordRequest
                {
                    Password = password,
                    NewPassword = newpassword,
                    AppSecret = YunClient.AppSecret
                }, Token);

            return !u.IsError && u.Result;
        }

        /// <summary>
        /// 在未登录的情况下重置密码
        /// </summary>
        /// <param name="code"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        public static bool ResetPassword(string code, string password)
        {
            var u =
                YunClient.Instance.Execute(new ResetPasswordRequest
                {
                    Password = password,
                    Code = code,
                    UserFlag = MallBrowseTrace.GetUserGuid(),
                    AppSecret = YunClient.AppSecret
                });

            return !u.IsError && u.Result;
        }

        /// <summary>
        /// 解绑当前登录用户的邮箱
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        public static bool UnbindEmail(string code)
        {
            var token = Token;
            var r = YunClient.Instance.Execute(new UnbindEmailRequest
            {
                Code = code
            }, token);

            if (r.Result)
            {
                _userCache.Remove(token.Hash());
            }

            return r.Result;
        }

        /// <summary>
        /// 更换或绑定当前登录用户的邮箱
        /// </summary>
        /// <param name="code"></param>
        /// <param name="email"></param>
        /// <returns></returns>
        public static bool BindEmail(string code, string email)
        {
            var token = Token;

            var r =
                YunClient.Instance.Execute(new BindEmailRequest
                {
                    Code = code,
                    Email = email
                }, token);

            if (r.Result)
            {
                _userCache.Remove(token.Hash());
            }

            return r.Result;
        }

        /// <summary>
        /// 绑定手机
        /// </summary>
        /// <param name="code"></param>
        /// <param name="phonenumber"></param>
        /// <returns></returns>
        public static bool BindPhone(string code, string phonenumber)
        {
            var token = Token;

            var r =  YunClient.Instance.Execute(new BindPhoneRequest
                {
                    Code = code,
                    Phone = phonenumber
                }, token);

            if (r.Result)
            {
                _userCache.Remove(token.Hash());
            }

            return r.Result;
        }

        /// <summary>
        /// 用户注册
        /// </summary>
        /// <returns></returns>
        public static KeyValuePair<bool, string> RegisterRequest(string ip, string email, string username,
            string password, string mobile)
        {
            var r = YunClient.Instance.Execute(new RegisterRequest
            {
                Ip = ip,
                Email = email,
                UserName = username,
                Password = password,
                RegisterType = 0,
                AppSecret = YunClient.AppSecret,
                Mobile = mobile,
                CompanyId = GlobeInfo.InitiatedCompanyId,
                ShopId = GlobeInfo.InitiatedShopId
            });

            if (r.UserId > 0)
            {
                Token = r.Token;
            }

            return new KeyValuePair<bool, string>(r.UserId > 0, r.ErrMsg);
        }


        /// <summary>
        /// 手机注册
        /// </summary>
        /// <returns></returns>
        public static KeyValuePair<bool, string> PhoneRegisterRequest(string phone, string core, string password, string ip, string userFlag)
        {
            var r = YunClient.Instance.Execute(new PhoneRegisterRequest
            {
                Ip = ip,
                Phone = phone,
                Password = password,
                Code = core,
                 CompanyId = GlobeInfo.InitiatedCompanyId,
                ShopId = GlobeInfo.InitiatedShopId,
                UserFlag = MallBrowseTrace.GetUserGuid(),
                Secret = YunClient.AppSecret
            });

            if (r.UserId > 0)
            {
                Token = r.Token;
            }

            return new KeyValuePair<bool, string>(r.UserId > 0, r.ErrMsg);
        }

    }
}
