using System;
using System.Runtime.Serialization;

namespace SampleLoggingApp.Model
{
    [DataContract]
    public enum LogPriority
    {
        [EnumMember(Value = "0")]
        Critical = 0,
        [EnumMember(Value = "1")]
        Fatal = 1,
        [EnumMember(Value = "2")]
        Error = 2,
        [EnumMember(Value = "3")]
        Warning = 3,
        [EnumMember(Value = "4")]
        Debug = 4,
        [EnumMember(Value = "5")]
        Info = 5,
        [EnumMember(Value = "6")]
        Trace = 6
    }

    public enum ContextOperation
    {
        PushS3,
        QueryAthena,
        PushES,
        QueryES
    }
}
