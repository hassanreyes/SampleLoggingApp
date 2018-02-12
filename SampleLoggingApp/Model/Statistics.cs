using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace SampleLoggingApp.Model
{
    public class Statistics
    {
        public class StageStatistics
        {
            private Stopwatch _watch = new Stopwatch();
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
            Console.WriteLine("{0: -20}|{1: 6N2}", "Elasped Time", "Private Memory");

            foreach(StageStatistics stage in Stages)
            {
                Console.WriteLine(new string('-', Console.WindowWidth));
                Console.WriteLine($"{stage.Name, -20}|{stage.Description}");
                Console.WriteLine($"{stage.ElapsedTime, -20}|{stage.Memory}");
            }
        }
    }
}
