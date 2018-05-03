using log4net.Core;
using System;
using System.Collections;
using System.Text;

namespace log4net.Appender.API
{
    /// <summary>
    /// This entity is used to push to an API endpoint
    /// </summary>
    public class APILoggingEventEntity
    {
        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="e"></param>
        public APILoggingEventEntity(LoggingEvent e)
        {
            Domain = e.Domain;
            Identity = e.Identity;
            Level = e.Level.ToString();
            var sb = new StringBuilder(e.Properties.Count);
            foreach (DictionaryEntry entry in e.Properties)
            {
                sb.AppendFormat("{0}:{1}", entry.Key, entry.Value);
                sb.AppendLine();
            }
            Properties = sb.ToString();
            Message = e.RenderedMessage + Environment.NewLine + e.GetExceptionString();
            ThreadName = e.ThreadName;
            EventTimeStamp = e.TimeStamp;
            UserName = e.UserName;
            Location = e.LocationInformation.FullInfo;
            ClassName = e.LocationInformation.ClassName;
            FileName = e.LocationInformation.FileName;
            LineNumber = e.LocationInformation.LineNumber;
            MethodName = e.LocationInformation.MethodName;
            StackFrames = e.LocationInformation.StackFrames;

            if (e.ExceptionObject != null)
            {
                Exception = e.ExceptionObject.ToString();
            }
        }
        /// <summary>
        /// User name
        /// </summary>
        public string UserName { get; set; }
        /// <summary>
        /// Timestamp of the event
        /// </summary>
        public DateTime EventTimeStamp { get; set; }
        /// <summary>
        /// Thread name
        /// </summary>
        public string ThreadName { get; set; }
        /// <summary>
        /// Message of the event
        /// </summary>
        public string Message { get; set; }
        /// <summary>
        /// Properties of the event
        /// </summary>
        public string Properties { get; set; }
        /// <summary>
        /// Event level
        /// </summary>
        public string Level { get; set; }
        /// <summary>
        /// Identity of the event
        /// </summary>
        public string Identity { get; set; }
        /// <summary>
        /// Domain of the event
        /// </summary>
        public string Domain { get; set; }
        /// <summary>
        /// Location
        /// </summary>
        public string Location { get; set; }
        /// <summary>
        /// Exception message
        /// </summary>
        public string Exception { get; set; }
        /// <summary>
        /// Class name of the call
        /// </summary>
        public string ClassName { get; set; }
        /// <summary>
        /// File name of the call
        /// </summary>
        public string FileName { get; set; }
        /// <summary>
        /// Line number
        /// </summary>
        public string LineNumber { get; set; }
        /// <summary>
        /// Method name which executes the log
        /// </summary>
        public string MethodName { get; set; }
        /// <summary>
        /// Stack frames
        /// </summary>
        public StackFrameItem[] StackFrames { get; set; }
    }
}
