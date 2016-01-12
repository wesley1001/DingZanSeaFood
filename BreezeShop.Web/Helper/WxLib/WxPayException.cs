using System;

namespace BreezeShop.Web.Helper.WxLib
{
    public class WxPayException : Exception 
    {
        public WxPayException(string msg) : base(msg) 
        {

        }
     }
}