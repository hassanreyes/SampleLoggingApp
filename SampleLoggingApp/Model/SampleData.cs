using System;
using System.Collections.Generic;

namespace SampleLoggingApp.Model
{

    public static class SampleData
    {

        public static ICollection<LogEntry> GetData(DateTime dt) => new List<LogEntry>()
            {
            new LogEntry { Timestamp = dt, Priority = (int)LogPriority.Info, Source = "Hassan.Sample.Logging.A", Message = "This is an info message", Tags = new List<string> { "Catfish", "DJ", "Hassan Reyes" }, InnerData = GetRandInnerData() },
            new LogEntry { Timestamp = dt, Priority = (int)LogPriority.Warning, Source = "Hassan.Sample.Logging.B", Message = "Something happends!", Tags = new List<string> {"Catfish", "DJ" }, InnerData = GetRandInnerData() },
            new LogEntry { Timestamp = dt, Priority = (int)LogPriority.Error, Source = "Hassan.Sample.Logging.B", Message = "Error message!", Tags = new List<string> {"Catfish", "DJ" }, InnerData = GetRandInnerData() },
            new LogEntry { Timestamp = dt, Priority = (int)LogPriority.Critical, Source = "Hassan.Sample.Logging.B", Message = "Critical Event", Tags = new List<string> {"Catfish", "DJ", "Hassan Reyes" }, InnerData = GetRandInnerData() },
            new LogEntry { Timestamp = dt, Priority = (int)LogPriority.Debug, Source = "Hassan.Sample.Logging.A", Message = "Keep tracking", Tags = new List<string> {"Catfish", "DJ" }, InnerData = GetRandInnerData() },
            new LogEntry { Timestamp = dt, Priority = (int)LogPriority.Info, Source = "Hassan.Sample.Logging.A", Message = "Other info message", Tags = new List<string> {"Catfish", "DJ", "Hassan" }, InnerData = GetRandInnerData() },
            new LogEntry { Timestamp = dt, Priority = (int)LogPriority.Trace, Source = "Hassan.Sample.Logging.A", Message = "Trace...", Tags = new List<string> {"Catfish", "DJ" }, InnerData = GetRandInnerData() },
            new LogEntry { Timestamp = dt, Priority = (int)LogPriority.Trace, Source = "Hassan.Sample.Logging.B", Message = "Trace.......", Tags = new List<string> {"Catfish", "DJ", "Reyes" }, InnerData = GetRandInnerData() },
            new LogEntry { Timestamp = dt, Priority = (int)LogPriority.Info, Source = "Hassan.Sample.Logging.B", Message = "Info message", Tags = new List<string> {"Catfish", "DJ", "Kangaroo" }, InnerData = GetRandInnerData() },
            new LogEntry { Timestamp = dt, Priority = (int)LogPriority.Critical, Source = "Hassan.Sample.Logging.C", Message = "Critical!!!", Tags = new List<string> {"Catfish", "DJ", "RnC" }, InnerData = GetRandInnerData() }
            };

        public static ICollection<LogEntry> GetBunchOfData(int factor, DateTime dt)
        {
            List<LogEntry> data = new List<LogEntry>(10 * factor);

            for (int i = 0; i < factor; i++)
            {
                data.AddRange(GetData(dt));
            }

            return data;
        }

        public static InnerData GetRandInnerData()
        {
            return new InnerData()
            {
                IpAddress = GetRandomIpAddress(),
                Message = "A detailed message related to message"
            };
        }

        private static Random random = new Random();
        public static string GetRandomIpAddress()
        {
            return $"{random.Next(1, 255)}.{random.Next(0, 255)}.{random.Next(0, 255)}.{random.Next(0, 255)}";
        }

        private static Random gen = new Random();
        private static DateTime start = new DateTime(2017, 1, 1);
        public static DateTime RandDate()
        {
            int range = (DateTime.Today - start).Days;
            return start.AddDays(gen.Next(range));
        }

    }
}
