using log4net.Appender.Extensions;
using log4net.Core;
using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using log4net.Util;

namespace log4net.Appender
{
    /// <summary>
    /// This Failover appender has been created by: Jonathan Rupp https://github.com/jorupp
    /// 
    /// His code was part of the PullRequest #55 or the original source code.
    /// 
    /// Proposal: Async table appender w/ message splitting #55
    /// </summary>
    public class AsyncAzureTableAppender : AzureTableAppender
    {
        private readonly List<Task> _outstandingTasks = new List<Task>();

        private readonly Random _rnd = new Random();

        private Timer _autoFlushTimer;
        /// <summary>
        /// Number of the batch size
        /// </summary>
        public int BatchSize { get; set; } = 100;
        /// <summary>
        /// Number of retry
        /// </summary>
        public int RetryCount { get; set; } = 5;
        /// <summary>
        /// Interval to retry
        /// </summary>
        public TimeSpan RetryWait { get; set; } = new TimeSpan(0, 0, 5);
        /// <summary>
        /// Iterval to send logs
        /// </summary>
        public TimeSpan FlushInterval { get; set; } = new TimeSpan(0, 1, 0);
        /// <summary>
        /// build chunks of no more than 100 each of which share the same partition key
        /// </summary>
        /// <param name="events">Events to log</param>
        protected override void SendBuffer(LoggingEvent[] events)
        {
            var chunks = events.Select(GetLogEntity).GroupBy(e => e.PartitionKey).SelectMany(i => i.Batch(100)).ToList();
            var tasks = chunks.Select(chunk => Task.Run(async () => await Send(chunk))).ToList();

            lock (_outstandingTasks)
            {
                _outstandingTasks.AddRange(tasks);
            }

            tasks.ForEach(t => t.ContinueWith(_ =>
            {
                lock (_outstandingTasks)
                {
                    _outstandingTasks.Remove(t);
                }
            }));
        }

        /// <summary>
        /// wait for a bit longer each time, and add a bit of randomness to make sure we're not retrying in lockstep
        /// </summary>
        /// <param name="chunk">Portion of logs to send.</param>
        /// <returns></returns>
        private async Task Send(IEnumerable<ITableEntity> chunk)
        {
            var batchOperation = new TableBatchOperation();
            foreach (var azureLoggingEvent in chunk)
            {
                batchOperation.Insert(azureLoggingEvent);
            }

            var attempt = 0;
            while (true)
            {
                try
                {
                    var sw = System.Diagnostics.Stopwatch.StartNew();
                    await Table.ExecuteBatchAsync(batchOperation);
                    LogLog.Debug(typeof(AsyncAzureTableAppender), string.Format("Sent batch of {0} in {1}", batchOperation.Count, sw.Elapsed));
                    return;
                }
                catch (Exception ex)
                {
                    attempt++;
                    if (attempt >= RetryCount)
                    {
                        LogLog.Error(typeof(AsyncAzureTableAppender), string.Format("Exception sending batch, aborting: {0}", ex.Message));
                        return;
                    }

                    LogLog.Warn(typeof(AsyncAzureTableAppender), string.Format("Exception sending batch, retrying: {0}", ex.Message));

                    var wait = TimeSpan.FromSeconds(RetryWait.TotalSeconds * (attempt + GetExtraWaitModifier()));
                    await Task.Delay(wait);
                }
            }
        }

        private double GetExtraWaitModifier()
        {
            lock (_rnd)
            {
                return _rnd.NextDouble();
            }
        }

        /// <summary>
        /// Execute the active options for the appender
        /// </summary>
        public override void ActivateOptions()
        {
            base.ActivateOptions();

            _autoFlushTimer = new Timer(s =>
            {
                LogLog.Debug(typeof(AsyncAzureTableAppender), "Triggering flush");
                this.Flush(false);
            }, null, TimeSpan.FromSeconds(0), FlushInterval);
        }

        /// <summary>
        /// the close would have triggered a flush, which would have created messages in the queue.  Wait until they're all done.
        /// </summary>
        protected override void OnClose()
        {
            LogLog.Debug(typeof(AsyncAzureTableAppender), "Closing");
            if (null != _autoFlushTimer)
            {
                _autoFlushTimer.Dispose();
                _autoFlushTimer = null;
            }
            base.OnClose();

            
            Task[] tasks;
            lock (_outstandingTasks)
            {
                tasks = _outstandingTasks.ToArray();
            }
            LogLog.Debug(typeof(AsyncAzureTableAppender), $"Waiting on {tasks.Length} outstanding logging calls");
            Task.WaitAll(tasks);
            LogLog.Debug(typeof(AsyncAzureTableAppender), "Completing close");
        }
    }
} 