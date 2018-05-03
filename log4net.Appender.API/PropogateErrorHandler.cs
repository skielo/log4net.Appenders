using log4net.Core;
using System;

namespace log4net.Appender.API
{
    /// <summary>
    /// 
    /// </summary>
    public class PropogateErrorHandler : IErrorHandler
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        /// <param name="e"></param>
        /// <param name="errorCode"></param>
        public void Error(string message, Exception e, ErrorCode errorCode)
        {
            throw new AggregateException(message, e);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        /// <param name="e"></param>
        public void Error(string message, Exception e)
        {
            throw new AggregateException(message, e);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        public void Error(string message)
        {
            throw new LogException($"Error logging an event: {message}");
        }
    }
}
