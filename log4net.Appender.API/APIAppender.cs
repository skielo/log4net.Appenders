using System;
using log4net.Core;
using log4net.Layout;
using System.Configuration;
using log4net.Appender.API.Language;
using System.Threading.Tasks;
using System.Net.Http;
using System.Text;
using Newtonsoft.Json;
using System.Net.Http.Headers;
using System.Net;
using log4net.Util;
using System.Linq;
using System.Collections.Generic;

namespace log4net.Appender.API
{
    /// <summary>
    /// This appender pushs your logs to a specific endpoint
    /// </summary>
    public class APIAppender : BufferingAppenderSkeleton
    {
        private string _baseUrl;
        private string _requestUrl;
        private string _basicUser;
        private string _basicPass;
        private bool _singleProcess;
        /// <summary>
        /// URL Key
        /// </summary>
        public string UrlKey { get; set; }
        /// <summary>
        /// Determine wether or send the list of events to the API or process one by one.
        /// </summary>
        public bool SingleProcess
        {
            get
            {
                return _singleProcess;
            }
            set
            {
                _singleProcess = value;
            }
        }
        /// <summary>
        /// Request URL to push your logs
        /// </summary>
        public string RequestUrl
        {
            get
            {
                return string.IsNullOrEmpty(_requestUrl) ? string.Empty : _requestUrl;
            }
            set
            {
                _requestUrl = value;
            }
        }
        /// <summary>
        /// Base URL for your endpoint
        /// </summary>
        public string BaseUrl
        {
            get
            {
                if (!string.IsNullOrWhiteSpace(UrlKey))
                {
                    return ConfigurationManager.AppSettings[UrlKey];
                }
                if (string.IsNullOrEmpty(_baseUrl))
                    throw new ApplicationException(Resources.UrlAPINotSpecified);
                return _baseUrl;
            }
            set
            {
                _baseUrl = value;
            }
        }
        /// <summary>
        /// User for basic authentication
        /// </summary>
        public string BasicUser {
            get { return _basicUser; }
            set { _basicUser = value; }
        }
        /// <summary>
        /// Password for basic authentication
        /// </summary>
        public string BasicPass
        {
            get { return _basicPass; }
            set { _basicPass = value; }
        }
        /// <summary>
        /// Default constructor
        /// </summary>
        public APIAppender()
        {
            PatternLayout layout = new PatternLayout();
            layout.ConversionPattern = PatternLayout.DetailConversionPattern;
            layout.ActivateOptions();
            Layout = layout;
        }

        /// <summary>
        /// This method pushs logs events to the specified endopoint
        /// </summary>
        /// <param name="events">Events to been pushed</param>
        protected override void SendBuffer(LoggingEvent[] events)
        {
            var clientHttp = new HttpClient();
            if (!string.IsNullOrEmpty(BasicUser) && !string.IsNullOrEmpty(BasicPass))
            {
                var header = GetAuthenticationHeader();
                clientHttp.DefaultRequestHeaders.TryAddWithoutValidation("Authorization", header);
            }
            if(this.SingleProcess)
            {
                Parallel.ForEach(events, log => ProcessEvent(log, clientHttp));
            }
            else
            {
                ProcessEvents(events, clientHttp);
            }
        }

        /// <summary>
        /// Constructs the Authentication header
        /// </summary>
        /// <returns>String representation of the basic authentication header</returns>
        private string GetAuthenticationHeader()
        {
            var plainTextBytes = Encoding.UTF8.GetBytes(string.Format("{0}:{1}", BasicUser, BasicPass));
            var header = string.Format("Basic {0}", Convert.ToBase64String(plainTextBytes));
            
            return header;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="loggingEvents"></param>
        /// <param name="clientHttp"></param>
        private void ProcessEvents(LoggingEvent[] loggingEvents, HttpClient clientHttp)
        {
            var eventContent = JsonConvert.SerializeObject(ParseEvents(loggingEvents).ToList());

            var content = new StringContent(eventContent, Encoding.UTF8, "application/json");
            
            clientHttp.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            InvokeAPI(clientHttp, content);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="loggingEvent"></param>
        /// <param name="clientHttp"></param>
        private void ProcessEvent(LoggingEvent loggingEvent, HttpClient clientHttp)
        {
            var eventContent = JsonConvert.SerializeObject(new APILoggingEventEntity(loggingEvent));

            var content = new StringContent(eventContent, Encoding.UTF8, "application/json");
            clientHttp.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            InvokeAPI(clientHttp, content);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="clientHttp"></param>
        /// <param name="byteContent"></param>
        private void InvokeAPI(HttpClient clientHttp, HttpContent byteContent)
        {
            try
            {
                var uri = new Uri(new Uri(BaseUrl), RequestUrl);
                var response = clientHttp.PostAsync(uri, byteContent).Result;
                if (response.StatusCode == HttpStatusCode.OK || 
                    response.StatusCode == HttpStatusCode.Accepted || 
                    response.StatusCode == HttpStatusCode.Created ||
                    response.StatusCode == HttpStatusCode.NoContent)
                {
                    LogLog.Debug(typeof(APIAppender), $"Event log has been processed. Status code> {response.StatusCode}");
                }
                else
                {
                    LogLog.Error(typeof(APIAppender), $"Something went wrong. Status response: {response.StatusCode}");
                }
            }
            catch (Exception ex)
            {
                LogLog.Error(typeof(APIAppender), $"There was an exception trying to Refresh Token: {ex.Message}, {ex.StackTrace}");
                throw ex;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="loggingEvents"></param>
        /// <returns></returns>
        private IEnumerable<APILoggingEventEntity> ParseEvents(LoggingEvent[] loggingEvents)
        {
            foreach (var @event in loggingEvents)
            {
                yield return new APILoggingEventEntity(@event);
            }
        }
    }
}
