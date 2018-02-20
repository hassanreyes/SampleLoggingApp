using System;
using System.Collections.Generic;
using Amazon.S3;
using SampleLoggingApp.Model;
using static SampleLoggingApp.Model.Statistics;

namespace SampleLoggingApp
{
    public class SampleContext
    {
        public const int MAX_QUERY_REPETITIONS = 10;
        public const int MAX_FETCHED_RECORDS = 10000;

        Statistics _stats = new Statistics();

        public Amazon.RegionEndpoint RegionEndpoint { get; set; }
        public string Format { get; set; }
        public string CompressionMethod { get; set; }
        public string S3BucketName { get; set; }
        public string S3BucketPath { get; set; }
        public string AthenaDataBase { get; set; }
        public string AthenaS3BucketOutputPath { get; set; }
        public ICollection<string> Queries { get; set; }
        public ContextOperation ContextOperation { get; set; }
        public Action<int,int> PushToS3Bucket { get; set; }
        public string ElasticsearchEndpoint { get; set; }

        public void Run()
        {
            SampleElasticsearchClient esClient;
            string report;
            int repetitions;

            switch (ContextOperation)
            {
                case ContextOperation.PushS3:
                    //If given, Run Action that push logs to S3
                    if (PushToS3Bucket != null)
                    {
                        if (CaptureS3Params(out int numOfFiles, out int numOfRecords))
                        {
                            Console.WriteLine($"Sample {Format}-log to S3://{S3BucketName}/{S3BucketPath}");

                            try
                            {
                                _stats.StartStage("File Upload", $"Uploading {numOfFiles} files with {numOfRecords * 10} records each file");

                                PushToS3Bucket(numOfFiles, numOfRecords);

                                _stats.StopCurrentStage();
                            }
                            catch (AmazonS3Exception amazonS3Exception)
                            {
                                if (amazonS3Exception.ErrorCode != null &&
                                    (amazonS3Exception.ErrorCode.Equals("InvalidAccessKeyId")
                                    ||
                                    amazonS3Exception.ErrorCode.Equals("InvalidSecurity")))
                                {
                                    Console.WriteLine("Check the provided AWS Credentials.");
                                    Console.WriteLine(
                                        "For service sign up go to http://aws.amazon.com/s3");
                                }
                                else
                                {
                                    Console.WriteLine(
                                        "Error occurred. Message:'{0}' when writing an object"
                                        , amazonS3Exception.Message);
                                }
                            }
                        }
                    }
                    break;
                case ContextOperation.QueryAthena:
                    //Run given queries.
                    if (Queries != null && Queries.Count > 0)
                    {
                        Console.WriteLine($"Sample {Format}-log querying from {AthenaDataBase} Athena's Database");

                        while (!CaptureQueryParams(out repetitions))
                        {
                            Console.WriteLine("Not valid number");
                        }

                        using (SampleAthenaClient athenaClient = new SampleAthenaClient("hassan", "s3://dj-rnc-feeds/hassan-log-sample/athena_output/", this.RegionEndpoint))
                        {
                            for (int n = 0; n < repetitions; n++)
                            {
                                foreach (string query in Queries)
                                {
                                    QueryStageStatistics queryStage = _stats.StartQueryStage(query);

                                    report = athenaClient.ExecuteQuery(query, ref queryStage);

                                    _stats.StopCurrentStage(report);
                                }
                            }
                        }
                    }
                    break;
                case ContextOperation.PushES:
                    esClient = new SampleElasticsearchClient(this.ElasticsearchEndpoint, this.RegionEndpoint);

                    Console.WriteLine($"Sample AWS Elasticsearch - Create Index to {this.ElasticsearchEndpoint}");

                    int numOfDocs;
                    while(!CaptureESParams(out numOfDocs))
                    {
                        Console.WriteLine("Not valid number");
                    }

                    _stats.StartStage("ES Indexing", $"Indexing {numOfDocs} documents");

                    esClient.PushLogs(numOfDocs);

                    _stats.StopCurrentStage();

                    break;
                case ContextOperation.QueryES:
                    esClient = new SampleElasticsearchClient(this.ElasticsearchEndpoint, this.RegionEndpoint);

                    Console.WriteLine($"Sample AWS Elasticsearch - Search on {this.ElasticsearchEndpoint}");

                    while (!CaptureQueryParams(out repetitions))
                    {
                        Console.WriteLine("Not valid number");
                    }

                    for (int n = 0; n < repetitions; n++)
                    {
                        var stage = _stats.StartQueryStage("MatchAll()");

                        report = esClient.QueryMatchAll(stage);

                        _stats.StopCurrentStage(report);
                    }

                    break;
                default:
                    break;
            }

            ClearConsoleLine();

            _stats.PrintStattistics();

            Console.WriteLine("Press any key to Exit");
            Console.ReadKey();
        }

        public static bool CaptureESParams(out int numOfRecords)
        {
            numOfRecords = 0;

            Console.WriteLine("Enter the number of records:");
            var numOfRecInput = Console.ReadLine();

            if (!int.TryParse(numOfRecInput, out numOfRecords))
            {
                return false;
            }

            return true;
        }

        public static bool CaptureS3Params(out int numOfFiles, out int numOfRecords)
        {
            numOfRecords = numOfFiles = -1;

            Console.WriteLine("Enter the number of files to be created:");
            var numOfFilesInput = Console.ReadLine();
            Console.WriteLine("Enter the number of records per file (x10):");
            var numOfRecInput = Console.ReadLine();

            if (!int.TryParse(numOfFilesInput, out numOfFiles))
            {
                return false;
            }

            if (!int.TryParse(numOfRecInput, out numOfRecords))
            {
                return false;
            }

            return true;
        }

        public static bool CaptureQueryParams(out int n)
        {
            n = MAX_QUERY_REPETITIONS;

            Console.WriteLine("Enter te number of times the Query Set will be executed:");
            var numOfFilesInput = Console.ReadLine();

            if (!int.TryParse(numOfFilesInput, out n))
            {
                Console.WriteLine("Not valid number");
                return false;
            }

            return true;
        }

        public static void ClearConsoleLine()
        {
            int currentLineCursor = Console.CursorTop;
            Console.SetCursorPosition(0, Console.CursorTop);
            Console.Write(new string(' ', Console.WindowWidth));
            Console.SetCursorPosition(0, currentLineCursor);
        }


    }
}
