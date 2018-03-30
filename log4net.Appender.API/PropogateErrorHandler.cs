using log4net.Core;
using System;

namespace log4net.Appender.API
{
    public class PropogateErrorHandler : IErrorHandler
    {
        public void Error(string message, Exception e, ErrorCode errorCode)
        {
            throw new AggregateException(message, e);
        }

        public void Error(string message, Exception e)
        {
            throw new AggregateException(message, e);
        }

        public void Error(string message)
        {
            throw new LogException($"Error logging an event: {message}");
        }
    }
}
