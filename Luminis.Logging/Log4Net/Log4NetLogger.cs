using System;

using log4net;

namespace Luminis.Logging.Log4Net
{
    public class Log4NetLogger : ILogger
    {
        #region Private properties
        private static readonly ILog _logger = LogManager.GetLogger(typeof(Log4NetLogger));
        #endregion

        #region Constructors
        public Log4NetLogger()
        {
        }
        #endregion

        #region ILogger Members

        public void Debug(string message)
        {
            _logger.Debug(message);
        }

        public void Debug(string message, Exception exception)
        {
            _logger.Debug(message, exception);
        }

        public void DebugFormat(string message, params object[] arguments)
        {
            _logger.DebugFormat(message, arguments);
        }

        public void Error(string message)
        {
            _logger.Error(message);
        }

        public void Error(string message, Exception exception)
        {
            _logger.Error(message, exception);
        }

        public void ErrorFormat(string message, params object[] arguments)
        {
            _logger.ErrorFormat(message, arguments);
        }

        public void Info(string message)
        {
            _logger.Info(message);
        }
        
        public void Info(string message, Exception exception)
        {
            _logger.Info(message, exception);
        }

        public void InfoFormat(string message, params object[] arguments)
        {
            _logger.InfoFormat(message, arguments);
        }

        public bool IsDebugEnabled
        {
            get { return _logger.IsDebugEnabled; }
        }

        public bool IsErrorEnabled
        {
            get { return _logger.IsErrorEnabled; }
        }

        public bool IsInfoEnabled
        {
            get { return _logger.IsInfoEnabled; }
        }

        public bool IsWarnEnabled
        {
            get { return _logger.IsWarnEnabled; }
        }

        public void Warn(string message)
        {
            _logger.Warn(message);
        }

        public void Warn(string message, Exception exception)
        {
            _logger.Warn(message, exception);
        }

        public void WarnFormat(string message, params object[] arguments)
        {
            _logger.WarnFormat(message, arguments);
        }

        #endregion
    }
}
