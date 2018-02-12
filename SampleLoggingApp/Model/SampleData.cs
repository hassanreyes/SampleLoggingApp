using System;
using System.Collections.Generic;

namespace SampleLoggingApp.Model
{

    public static class SampleData
    {

        public static ICollection<LogEntry> GetData() => new List<LogEntry>()
            {
            new LogEntry { Timestamp = DateTime.UtcNow, Priority = (int)LogPriority.Info, Source = "Hassan.Sample.Logging.A", Message = "This is an info message", Tag = "Catfish, DJ, Hassan Reyes" },
            new LogEntry { Timestamp = DateTime.UtcNow, Priority = (int)LogPriority.Warning, Source = "Hassan.Sample.Logging.B", Message = "Something happends!", Tag = "Catfish, DJ" },
            new LogEntry { Timestamp = DateTime.UtcNow, Priority = (int)LogPriority.Error, Source = "Hassan.Sample.Logging.B", Message = "Error message!", Tag = "Catfish, DJ" },
            new LogEntry { Timestamp = DateTime.UtcNow, Priority = (int)LogPriority.Critical, Source = "Hassan.Sample.Logging.B", Message = "Critical Event", Tag = "Catfish, DJ, Hassan Reyes" },
            new LogEntry { Timestamp = DateTime.UtcNow, Priority = (int)LogPriority.Debug, Source = "Hassan.Sample.Logging.A", Message = "Keep tracking", Tag = "Catfish, DJ" },
            new LogEntry { Timestamp = DateTime.UtcNow, Priority = (int)LogPriority.Info, Source = "Hassan.Sample.Logging.A", Message = "Other info message", Tag = "Catfish, DJ, Hassan" },
            new LogEntry { Timestamp = DateTime.UtcNow, Priority = (int)LogPriority.Trace, Source = "Hassan.Sample.Logging.A", Message = "Trace...", Tag = "Catfish, DJ" },
            new LogEntry { Timestamp = DateTime.UtcNow, Priority = (int)LogPriority.Trace, Source = "Hassan.Sample.Logging.B", Message = "Trace.......", Tag = "Catfish, DJ, Reyes" },
            new LogEntry { Timestamp = DateTime.UtcNow, Priority = (int)LogPriority.Info, Source = "Hassan.Sample.Logging.B", Message = "Info message", Tag = "Catfish, DJ" },
            new LogEntry { Timestamp = DateTime.UtcNow, Priority = (int)LogPriority.Critical, Source = "Hassan.Sample.Logging.C", Message = "Critical!!!", Tag = "Catfish, DJ, RnC" },
            };

        public static ICollection<LogEntry> GetBunchOfData(int factor)
        {
            List<LogEntry> data = new List<LogEntry>(10 * factor);

            for (int i = 0; i < factor; i++)
            {
                data.AddRange(GetData());
            }

            return data;
        }

    }
}
