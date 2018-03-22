using System;
using log4net.Appender;
using log4net.Core;
using NUnit.Framework;

namespace log4net.Azure.Tests
{
    [TestFixture]
    public class UnitTestAzureAppendBlobAppender
    {
        private AzureAppendBlobAppender _appender;

        [OneTimeSetUp]
        public void Initialize()
        {
            _appender = new AzureAppendBlobAppender()
                {
                    ConnectionString = "UseDevelopmentStorage=true",
                    ContainerName = "testLoggingBlob",
                    DirectoryName = "testLogging"
                };
            _appender.ActivateOptions();
        }

        [Test]
        public void Test_Blob_Appender()
        {
            var @event = MakeEvent();

            _appender.DoAppend(@event);
        }

        [Test]
        public void Test_Blob_Appender_Multiple_5()
        {
            _appender.DoAppend(MakeEvents(5));
        }

        [Test]
        public void Test_Blob_Appender_Multiple_10()
        {
            _appender.DoAppend(MakeEvents(10));
        }

        [Test]
        public void Test_Blob_Appender_Multiple_100()
        {
            _appender.DoAppend(MakeEvents(100));
        }

        private static LoggingEvent[] MakeEvents(int number)
        {
            var result = new LoggingEvent[number];
            for (int i = 0; i < number; i++)
            {
                result[i] = MakeEvent();
            }
            return result;
        }

        private static LoggingEvent MakeEvent()
        {
            return new LoggingEvent(
                new LoggingEventData
                    {
                        Domain = "testDomain",
                        Identity = "testIdentity",
                        Level = Level.Critical,
                        LoggerName = "testLoggerName",
                        Message = "testMessage",
                        ThreadName = "testThreadName",
                        TimeStampUtc = DateTime.UtcNow,
                        UserName = "testUsername",
                        LocationInfo = new LocationInfo("className", "methodName", "fileName", "lineNumber")
                    }
                );
        }
    }
}