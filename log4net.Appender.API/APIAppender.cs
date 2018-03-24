using System;
using log4net.Core;
using log4net.Layout;
using System.Configuration;
using log4net.Appender.API.Language;
using System.Threading.Tasks;

namespace log4net.Appender.API
{
    public class APIAppender : BufferingAppenderSkeleton
    {
        public string UrlKey { get; set; }
        private string _url;
        public string Url
        {
            get
            {
                if (!string.IsNullOrWhiteSpace(UrlKey))
                {
                    return ConfigurationManager.AppSettings[UrlKey];
                }
                if (string.IsNullOrEmpty(_url))
                    throw new ApplicationException(Resources.UrlAPINotSpecified);
                return _url;
            }
            set
            {
                _url = value;
            }
        }
        public APIAppender()
        {
            PatternLayout layout = new PatternLayout();
            layout.ConversionPattern = PatternLayout.DetailConversionPattern;
            layout.ActivateOptions();
            Layout = layout;
        }

        protected override void SendBuffer(LoggingEvent[] events)
        {
            Parallel.ForEach(events, ProcessEvent);
        }

        private void ProcessEvent(LoggingEvent loggingEvent)
        {
            // Create a message and add it to the queue.
            //CloudQueueMessage message = new CloudQueueMessage(RenderLoggingEvent(loggingEvent));
            //_queue.AddMessage(message);
        }
    }
}
