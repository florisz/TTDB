using System;

namespace TimeTraveller.General.Logging
{
    public class NullLogger : ILogger
    {
        #region Constructors
        private NullLogger()
        {
        }
        #endregion

        #region Singleton implementation
        private static readonly ILogger _instance = new NullLogger();

        public static ILogger Instance
        {
            get { return _instance; }
        }
        #endregion

        #region ILogger Members
        public void Debug(string message)
        {
        }

        public void Debug(string message, Exception exception)
        {
        }

        public void DebugFormat(string message, params object[] arguments)
        {
        }

        public void Error(string message)
        {
        }

        public void Error(string message, Exception exception)
        {
        }

        public void ErrorFormat(string message, params object[] arguments)
        {
        }

        public void Info(string message)
        {
        }

        public void Info(string message, Exception exception)
        {
        }

        public void InfoFormat(string message, params object[] arguments)
        {
        }

        public bool IsDebugEnabled
        {
            get { return false; }
        }

        public bool IsErrorEnabled
        {
            get { return false; }
        }

        public bool IsInfoEnabled
        {
            get { return false; }
        }

        public bool IsWarnEnabled
        {
            get { return false; }
        }

        public void Warn(string message)
        {
        }

        public void Warn(string message, Exception exception)
        {
        }

        public void WarnFormat(string message, params object[] arguments)
        {
        }
        #endregion
    }
}
