using System;

namespace TimeTraveller.General.Logging
{
    public interface ILogger
    {
        void Debug(string message);
        void Debug(string message, Exception exception);
        void DebugFormat(string message, params object[] arguments);
        void Error(string message);
        void Error(string message, Exception exception);
        void ErrorFormat(string message, params object[] arguments);
        void Info(string message);
        void Info(string message, Exception exception);
        void InfoFormat(string message, params object[] arguments);
        bool IsDebugEnabled { get; }
        bool IsErrorEnabled { get; }
        bool IsInfoEnabled { get; }
        bool IsWarnEnabled { get; }
        void Warn(string message);
        void Warn(string message, Exception exception);
        void WarnFormat(string message, params object[] arguments);
    }
}
