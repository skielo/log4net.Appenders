using log4net.Core;
using System;

namespace log4net.Appender.API
{
    public class FailOverErrorHandler : IErrorHandler
    {
        public FailoverAppender FailOverAppender { get; set; }

        public FailOverErrorHandler(FailoverAppender failOverAppender)
        {
            FailOverAppender = failOverAppender;
        }

        public void Error(string message, Exception e, ErrorCode errorCode)
            => FailOverAppender.ActivateFailOverMode();

        public void Error(string message, Exception e)
            => FailOverAppender.ActivateFailOverMode();

        public void Error(string message)
            => FailOverAppender.ActivateFailOverMode();
    }
}
