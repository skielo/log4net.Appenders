using log4net.Core;

namespace log4net.Appender.API
{
    public class FailoverAppender : AppenderSkeleton
    {
        private AppenderSkeleton _primaryAppender;
        private AppenderSkeleton _failOverAppender;
        
        public AppenderSkeleton PrimaryAppender
        {
            get { return _primaryAppender; }
            set
            {
                _primaryAppender = value;
                SetAppenderErrorHandler(value);
            }
        }

        public AppenderSkeleton FailOverAppender
        {
            get { return _failOverAppender; }
            set
            {
                _failOverAppender = value;
                SetAppenderErrorHandler(value);
            }
        }

        public IErrorHandler DefaultErrorHandler { get; set; }

        public bool LogToFailOverAppender { get; private set; }

        public FailoverAppender()
        {
            DefaultErrorHandler = ErrorHandler;
            ErrorHandler = new FailOverErrorHandler(this);
        }

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
