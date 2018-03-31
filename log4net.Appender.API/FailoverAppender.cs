using log4net.Core;

namespace log4net.Appender.API
{
    /// <summary>
    /// This appender supports failover mechanisim. In case the primary appender fails uses a secondary
    /// appender to log.
    /// 
    /// This solution is an implementation of https://blog.oz-code.com/disaster-strikes-complete-guide-failover-appenders-log4net/
    /// </summary>
    public class FailoverAppender : AppenderSkeleton
    {
        private AppenderSkeleton _primaryAppender;
        private AppenderSkeleton _failOverAppender;
        
        /// <summary>
        /// Primary appender to push logs
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
        /// Failover appender in case the primary fails
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
        /// Default error handler
        /// </summary>
        public IErrorHandler DefaultErrorHandler { get; set; }
        /// <summary>
        /// Determine wheter or not the failover appender is enable
        /// </summary>
        public bool LogToFailOverAppender { get; private set; }
        /// <summary>
        /// Default constructor
        /// </summary>
        public FailoverAppender()
        {
            DefaultErrorHandler = ErrorHandler;
            ErrorHandler = new FailOverErrorHandler(this);
        }
        /// <summary>
        /// Push logs entities to the primary appender.
        /// </summary>
        /// <param name="loggingEvent"></param>
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
