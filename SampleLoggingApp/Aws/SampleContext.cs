using System;
using System.Collections.Generic;
using Amazon.S3;
using SampleLoggingApp.Model;

namespace SampleLoggingApp
{
    public class SampleContext
    {
        Statistics _stats = new Statistics();

        public Amazon.RegionEndpoint RegionEndpoint { get; set; }
        public string Format { get; set; }
        public string CompressionMethod { get; set; }
        public string S3BucketName { get; set; }
        public string S3BucketPath { get; set; }
        public string AthenaDataBase { get; set; }
        public string AthenaS3BucketOutputPath { get; set; }
        public ICollection<string> Queries { get; set; }
        public bool IsPushToS3Bucket { get; set; }
        public bool IsQueryAthena { get; set; }
        public Action<int,int> PushToS3Bucket { get; set; }

        public void Run()
        {
            //If given, Run Action that push logs to S3
            if(IsPushToS3Bucket && PushToS3Bucket != null)
            {
                if (CaptureParams(out int numOfFiles, out int numOfRecords))
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

            //Run given queries.
            if(IsQueryAthena && Queries != null && Queries.Count > 0)
            {
                Console.WriteLine($"Sample {Format}-log querying from {AthenaDataBase} Athena's Database");

                using (SampleAthenaClient athenaClient = new SampleAthenaClient("hassan", "s3://dj-rnc-feeds/hassan-log-sample/athena_output/", Amazon.RegionEndpoint.USEast1))
                {
                    foreach(string query in Queries)
                    {
                        _stats.StartStage("Querying", query);

                        var report = athenaClient.ExecuteQuery(query);

                        _stats.StopCurrentStage(report);
                    }
                }
            }

            _stats.PrintStattistics();

            Console.WriteLine("Press any key to Exit");
            Console.ReadKey();
        }

        public static bool CaptureParams(out int numOfFiles, out int numOfRecords)
        {
            numOfRecords = numOfFiles = -1;

            Console.WriteLine("Enter the number of files to be created:");
            var numOfFilesInput = Console.ReadLine();
            Console.WriteLine("Enter the number of records per file (x10):");
            var numOfRecInput = Console.ReadLine();

            if (!int.TryParse(numOfFilesInput, out numOfFiles))
            {
                Console.WriteLine("Not valid number");
                return false;
            }

            if (!int.TryParse(numOfRecInput, out numOfRecords))
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
