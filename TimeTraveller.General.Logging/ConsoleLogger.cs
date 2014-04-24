using System;

namespace TimeTraveller.General.Logging
{
    public class ConsoleLogger : ILogger
    {
        #region Constructors
        private ConsoleLogger()
        {
        }
        #endregion

        #region Singleton implementation
        private static readonly ILogger _instance = new ConsoleLogger();

        public static ILogger Instance
        {
            get { return _instance; }
        }
        #endregion

        #region ILogger Members

        public void Debug(string message)
        {
            Log("DEBUG", message, null);
        }

        public void Debug(string message, Exception exception)
        {
            Log("DEBUG", message, exception);
        }

        public void DebugFormat(string message, params object[] arguments)
        {
            Log("DEBUG", message, null, arguments);
        }

        public void Error(string message)
        {
            Log("ERROR", message, null);
        }

        public void Error(string message, Exception exception)
        {
            Log("ERROR", message, exception);
        }

        public void ErrorFormat(string message, params object[] arguments)
        {
            Log("ERROR", message, null, arguments);
        }

        public void Info(string message)
        {
            Log("INFO", message, null);
        }

        public void Info(string message, Exception exception)
        {
            Log("INFO", message, exception);
        }

        public void InfoFormat(string message, params object[] arguments)
        {
            Log("INFO", message, null, arguments);
        }

        public bool IsDebugEnabled
        {
            get { return true; }
        }

        public bool IsErrorEnabled
        {
            get { return true; }
        }

        public bool IsInfoEnabled
        {
            get { return true; }
        }

        public bool IsWarnEnabled
        {
            get { return true; }
        }

        public void Warn(string message)
        {
            Log("WARN", message, null);
        }

        public void Warn(string message, Exception exception)
        {
            Log("WARN", message, exception);
        }

        public void WarnFormat(string message, params object[] arguments)
        {
            Log("WARN", message, null, arguments);
        }

        #endregion

        #region Private Methods
        private void Log(string severity, string message, Exception exception, params object[] arguments)
        {
            Console.WriteLine("{0}: {1}", severity, string.Format(message, arguments));
            if (exception != null)
            {
                Log(exception);
            }
        }

        private void Log(Exception exception)
        {
            Console.WriteLine("{0}: {1}\r\n", exception.GetType().FullName, exception.Message, exception.StackTrace);
            if (exception.InnerException != null)
            {
                Console.WriteLine("--- Inner exception ---");
                Log(exception.InnerException);
            }
        }
        #endregion
    }
}
