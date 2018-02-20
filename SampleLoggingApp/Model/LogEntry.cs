using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace SampleLoggingApp.Model
{
    [DataContract(Name = "InnerData", Namespace = "SampleLoggingApp.Model")]
    public class InnerData
    {
        public string IpAddress { get; set; }
        public string Message { get; set; }
    }

    [DataContract(Name = "LogEntry", Namespace = "SampleLoggingApp.Model")]
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
        [DataMember(Name = "Tags")]
        public IEnumerable<string> Tags { get; set; }
        [DataMember(Name = "InnerData")]
        public InnerData InnerData { get; set; }

        public override string ToString()
        {
            return string.Format("[LogEntry: Timestamp={0}, Priority={1}, Source={2}, Message={3}, Tags={4}]", Timestamp, Priority, Source, Message, Tags);
        }
    }
}
