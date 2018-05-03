using log4net.Core;
using System;

namespace log4net.Appender.API
{
    /// <summary>
    /// This is an implementation of the fail over error handler.
    /// </summary>
    public class FailOverErrorHandler : IErrorHandler
    {
        /// <summary>
        /// Failover appender
        /// </summary>
        public FailoverAppender FailOverAppender { get; set; }
        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="failOverAppender"></param>
        public FailOverErrorHandler(FailoverAppender failOverAppender)
        {
            FailOverAppender = failOverAppender;
        }
        /// <summary>
        /// Error method
        /// </summary>
        /// <param name="message"></param>
        /// <param name="e"></param>
        /// <param name="errorCode"></param>
        public void Error(string message, Exception e, ErrorCode errorCode)
            => FailOverAppender.ActivateFailOverMode();
        /// <summary>
        /// Error method
        /// </summary>
        /// <param name="message"></param>
        /// <param name="e"></param>
        public void Error(string message, Exception e)
            => FailOverAppender.ActivateFailOverMode();
        /// <summary>
        /// Error method
        /// </summary>
        /// <param name="message"></param>
        public void Error(string message)
            => FailOverAppender.ActivateFailOverMode();
    }
}
