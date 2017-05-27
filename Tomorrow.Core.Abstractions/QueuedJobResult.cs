using System;

namespace Tomorrow.Core.Abstractions
{
    public class QueuedJobResult
    {
        public enum JobResult
        {
            /// <summary>
            /// Indicates that the job was completed as planned.
            /// </summary>
            Succeeded,

            /// <summary>
            /// Indicates that the job encountered an exception.
            /// </summary>
            Errored,

            /// <summary>
            /// Indicates that the job exceeded the timeout period.
            /// </summary>
            TimedOut,

            /// <summary>
            /// Indicates that the job was aborted by the runner due to
            /// circumstances such as resource exhaustion, or the runner being
            /// scheduled for termination.
            /// </summary>
            Aborted
        }

        public JobResult Result { get; set; }
        public Exception Exception { get; set; }
        public DateTimeOffset AsOf { get; set; } = DateTimeOffset.Now;

        public QueuedJobResult(Exception ex)
        {
            Result = JobResult.Errored;
            Exception = ex;
        }

        public QueuedJobResult()
        {
        }
    }
}