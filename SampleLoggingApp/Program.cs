using System;
using System.IO;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using SampleLoggingApp.Aws.Extensions;
using SampleLoggingApp.Extensions;
namespace SampleLoggingApp
{

    class Program
    {
        static void Main(string[] args)
        {
            //Load configuration
            var configBuilder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json");

            var config = configBuilder.Build();

            Console.WriteLine("Select Operation to be performed:");
            Console.WriteLine("[0] Push to S3 Only");
            Console.WriteLine("[1] Run Athena Queries Only");
            Console.WriteLine("[2] Push and Query");
            var strOption = Console.ReadLine();

            var format = config["Format"].ToLower();

            //AWS Application context
            SampleContext context = new SampleContext()
            {
                RegionEndpoint = Amazon.RegionEndpoint.USEast1,
                IsPushToS3Bucket = !String.IsNullOrWhiteSpace(strOption) && (strOption.Equals("0") || strOption.Equals("2")),
                IsQueryAthena = !String.IsNullOrWhiteSpace(strOption) && (strOption.Equals("1") || strOption.Equals("2")),
                //Athena queries
                Queries = new List<string>()
                {
                    "SELECT * FROM " + config["Format"]
                }
            }.UseConfiguration(config);

            if (format == "parquet")
                context.UseParquetPushToS3Bucket();
            else if (format == "avro")
                context.UseAvroPushToS3Bucket();
            else
            {
                Console.WriteLine("Not Supported Format!");
                return;
            }

            context.Run();
        }
    }
}
