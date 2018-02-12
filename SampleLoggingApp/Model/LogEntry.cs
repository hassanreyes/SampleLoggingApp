using System;
using System.Runtime.Serialization;

namespace SampleLoggingApp.Model
{
    [DataContract(Name = "LogEntry", Namespace = "Logging")]
    public class LogEntry
    {
        [DataMember(Name = "Timestamp")]
        public DateTime Timestamp { get; set; }
        [DataMember(Name = "Priority")]
        public int Priority { get; set; }
        [DataMember(Name = "Source")]
        public string Source { get; set; }
        [DataMember(Name = "Message")]
        public string Message { get; set; }
        [DataMember(Name = "Tag")]
        public string Tag { get; set; }

    }
}
