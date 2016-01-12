using System;
using System.Linq;
using System.Web;
using System.Web.Configuration;
using Yun;
using Yun.Interface;

namespace BreezeShop.Core
{
    public static class YunClient
    {

        private static IYunClient _yunClient;
        private static object initLock = new object();
        private static readonly string _serverUrl = WebConfigurationManager.AppSettings["ServerUrl"];
        private static readonly string _serverKey = WebConfigurationManager.AppSettings["ServerKey"];
        private static readonly string _serverSecret = WebConfigurationManager.AppSettings["ServerSecret"];
        private static readonly string _webUrl = WebConfigurationManager.AppSettings["WebUrl"];


        public static string WebUrl
        {
            get { return _webUrl; }
        }

        public static string AppSecret
        {
            get { return _serverSecret; }
        }

        private static void Init()
        {
            if (_yunClient == null)
            {
                lock (initLock)
                {
                    if (_yunClient != null ) return;

                    if (new[] { _serverUrl, _serverKey, _serverUrl }.Any(string.IsNullOrWhiteSpace))
                    {
                        throw new Exception("服务端必要数据未初始化");
                    }

                    _yunClient = new DefaultYunClient(_serverUrl, _serverKey, _serverSecret);
                }
            }
        }


        public static IYunClient Instance
        {
            get
            {
                Init();

                return _yunClient;
            }
        }

    }
}
