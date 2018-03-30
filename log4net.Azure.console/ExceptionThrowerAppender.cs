using log4net.Appender;
using log4net.Appender.API;
using log4net.Core;
using System;

namespace log4net.Azure.console
{
    public class ExceptionThrowerAppender : ConsoleAppender
    {
        public int ThrowExceptionForCount { get; set; } = 1;
        private int _count;

        public ExceptionThrowerAppender()
        {
            ErrorHandler = new PropogateErrorHandler();
        }

        protected override void Append(LoggingEvent loggingEvent)
        {
            if (0 == System.Threading.Interlocked.Increment(ref _count) % ThrowExceptionForCount)
            {
                throw new Exception($"Interval {ThrowExceptionForCount} is reached with counter {_count}");
            }

            base.Append(loggingEvent);
        }
    }
}
