using System;
using Utilities.IO.Logging;
using Utilities.IO.Logging.Enums;

namespace BreezeShop.Core
{
    public class ExceptionLog
    {
        public ExceptionLog(Type type = null)
        {
            if (type != null)
            {
                DeclaringType = type;
            }
        }

        private static string _root = AppDomain.CurrentDomain.BaseDirectory + "app_data\\logs\\message\\";

        private FileLog _log;

        public Type DeclaringType { get; set; }

        private void InitFileLog()
        {
            var now = DateTime.Now;
            var path = _root + now.ToString("yyyyMMdd") + "\\" + now.ToString("HH") + ".log";

            _log = new FileLog(path);
        }

        protected virtual string GetErrorMessage(Exception ex, MessageType type)
        {
            return GetErrorMessage(ex.ToString(), type);
        }

        protected virtual string GetErrorMessage(string message, MessageType type)
        {
            return string.Format("[{0}] {1} {2} {3}", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff"), type,
                DeclaringType != null ? DeclaringType.FullName : "", message);
        }

        #region 传入参数为Exception的方法组
        protected virtual void Log(Exception ex, MessageType type)
        {
            InitFileLog();

            _log.LogMessage(GetErrorMessage(ex, type), type);
        }

        public void General(Exception ex)
        {
            Log(ex,MessageType.General);
        }

        public void Debug(Exception ex)
        {
            Log(ex, MessageType.Debug);
        }


        public void Trace(Exception ex)
        {
            Log(ex, MessageType.Trace);
        }

        
        public void Info(Exception ex)
        {
            Log(ex, MessageType.Info);
        }
        
        public void Warn(Exception ex)
        {
            Log(ex, MessageType.Warn);
        }

        public void Error(Exception ex)
        {
            Log(ex, MessageType.Error);
        }


        #endregion

        #region 传入参数为自定义文字
        protected virtual void Log(string message, MessageType type)
        {
            InitFileLog();
            _log.LogMessage(GetErrorMessage(message, type), type);
        }

        public void General(string message)
        {
            Log(message, MessageType.General);
        }



        public void Debug(string message)
        {
            Log(message, MessageType.Debug);
        }

        public void Trace(string message)
        {
            Log(message, MessageType.Trace);
        }

        public void Info(string message)
        {
            Log(message, MessageType.Info);
        }


        public void Warn(string message)
        {
            Log(message, MessageType.Warn);
        }

        public void Error(string message)
        {
            Log(message, MessageType.Error);
        }

        #endregion

        public void Dispose()
        {
            _log.Dispose();
        }

    }
}
