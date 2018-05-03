using log4net.Core;

namespace log4net.Appender.API
{
    /// <summary>
    /// This Failover appender has been created by: Moaid Hathot
    /// 
    /// You can find the original post here:
    /// https://blog.oz-code.com/disaster-strikes-complete-guide-failover-appenders-log4net/
    /// 
    /// The original source code for this post is in this repo: https://github.com/MoaidHathot/FailOverAppender
    /// 
    /// Nevertheless I've made a copule of modifications to the original code.
    /// </summary>
    public class FailoverAppender : AppenderSkeleton
    {
        private AppenderSkeleton _primaryAppender;
        private AppenderSkeleton _failOverAppender;
        /// <summary>
        /// Configured primary appender
        /// </summary>
        public AppenderSkeleton PrimaryAppender
        {
            get { return _primaryAppender; }
            set
            {
                _primaryAppender = value;
                SetAppenderErrorHandler(value);
            }
        }
        /// <summary>
        /// Configured failover appender
        /// </summary>
        public AppenderSkeleton FailOverAppender
        {
            get { return _failOverAppender; }
            set
            {
                _failOverAppender = value;
                SetAppenderErrorHandler(value);
            }
        }
        /// <summary>
        /// Defines the default error handler
        /// </summary>
        public IErrorHandler DefaultErrorHandler { get; set; }
        /// <summary>
        /// Determine whether or not log to the failover appender
        /// </summary>
        public bool LogToFailOverAppender { get; private set; }
        /// <summary>
        /// Appender configured to be used as failover
        /// </summary>
        public FailoverAppender()
        {
            DefaultErrorHandler = ErrorHandler;
            ErrorHandler = new FailOverErrorHandler(this);
        }
        /// <summary>
        /// Append the event log to the configured appender.
        /// </summary>
        /// <param name="loggingEvent">Event to log</param>
        protected override void Append(LoggingEvent loggingEvent)
        {
            if (LogToFailOverAppender)
            {
                _failOverAppender?.DoAppend(loggingEvent);
            }
            else
            {
                try
                {
                    _primaryAppender?.DoAppend(loggingEvent);
                }
                catch
                {
                    ActivateFailOverMode();
                    Append(loggingEvent);
                    DeActivateFailOverMode();
                }
            }
        }

        private void SetAppenderErrorHandler(AppenderSkeleton appender)
            => appender.ErrorHandler = new PropogateErrorHandler();

        internal void ActivateFailOverMode()
        {
            ErrorHandler = DefaultErrorHandler;
            LogToFailOverAppender = true;
        }

        internal void DeActivateFailOverMode()
        {
            ErrorHandler = DefaultErrorHandler;
            LogToFailOverAppender = false;
        }
    }
}
