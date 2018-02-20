using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace SampleLoggingApp.Model
{
    public class Statistics
    {
        public class QueryStageStatistics : StageStatistics
        {
            public TimeSpan QueryExecutionTime { get; set; }
            public TimeSpan DataFetchingTime { get; set; }
            public long DataScannedInBytes { get; set; }
            public long EngineExecutionTimeInMillis { get; set; }


        }

        public class StageStatistics
        {
            protected Stopwatch _watch = new Stopwatch();

            public string Name { get; set; }
            public string Description { get; set; }
            public string Report { get; set; }
            public TimeSpan ElapsedTime { get => _watch.Elapsed; }
            public long Memory { get; set; }

            public StageStatistics Start() { _watch.Start(); return this; }
            public void Stop() => _watch.Stop();
        }

        public List<StageStatistics> Stages { get; protected set; }
        public StageStatistics CurrentStage { get; protected set; }

        public Statistics()
        {
            this.Stages = new List<StageStatistics>();
        }

        public StageStatistics StartStage(string name, string description)
        {
            CurrentStage = new StageStatistics() { Name = name, Description = description };
            this.Stages.Add(CurrentStage);

            return CurrentStage.Start();
        }

        public QueryStageStatistics StartQueryStage(string query)
        {
            CurrentStage = new QueryStageStatistics() { Name = "Quering", Description = query };
            this.Stages.Add(CurrentStage);

            return (QueryStageStatistics) CurrentStage.Start();
        }

        public void StopCurrentStage(string report = "")
        {
            CurrentStage.Memory = Process.GetCurrentProcess().PrivateMemorySize64;
            CurrentStage.Report = report;
            CurrentStage.Stop();
        }

        public void PrintStattistics()
        {
            Console.WriteLine(new string('-', Console.WindowWidth));
            Console.WriteLine("{0: -20}|{1}", "Stage Name", "Description");
            Console.WriteLine("{0: -20}|{1: 6N2}", "Elasped Time", "Report");

            List<TimeSpan> qExecTime = new List<TimeSpan>();
            List<TimeSpan> qFetchTime = new List<TimeSpan>();
            List<long> qExecTimeMs = new List<long>();
            List<long> qDataScanned = new List<long>();
            List<TimeSpan> times = new List<TimeSpan>();

            foreach(StageStatistics stage in Stages)
            {
                Console.WriteLine(new string('-', Console.WindowWidth));
                Console.WriteLine($"{stage.Name}\t\t|{stage.Description}");
                Console.WriteLine($"{stage.ElapsedTime}\t\t|{stage.Report}");
                times.Add(stage.ElapsedTime);

                if(stage is QueryStageStatistics)
                {
                    qExecTime.Add(((QueryStageStatistics)stage).QueryExecutionTime);
                    qFetchTime.Add(((QueryStageStatistics)stage).DataFetchingTime);
                    qExecTimeMs.Add(((QueryStageStatistics)stage).EngineExecutionTimeInMillis);
                    qDataScanned.Add(((QueryStageStatistics)stage).DataScannedInBytes);
                }
            }
            Console.WriteLine(new string('=', Console.WindowWidth));
            double avgTicks = times.Average(ts => ts.Ticks);
            Console.ForegroundColor = ConsoleColor.DarkRed;
            Console.WriteLine($"Stage Avg Time: {new TimeSpan(Convert.ToInt64(avgTicks))}");
            if(qExecTime.Count > 0)
            {
                avgTicks = qExecTime.Average(ts => ts.Ticks);
                Console.WriteLine($"Query Exec. Avg Time: {new TimeSpan(Convert.ToInt64(avgTicks))}");
                avgTicks = qFetchTime.Average(ts => ts.Ticks);
                Console.WriteLine($"Data Feching Avg. Time: {new TimeSpan(Convert.ToInt64(avgTicks))}");
                Console.WriteLine($"Engine Execution Avg. Time (ms): {TimeSpan.FromMilliseconds(Convert.ToDouble(qExecTimeMs.Average()))}");
                Console.WriteLine($"Data Scanned Avg. (bytes): {qDataScanned.Average()}");
            }

        }
    }
}
