using System;
using System.IO;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using SampleLoggingApp.Aws.Extensions;
using SampleLoggingApp.Extensions;
using SampleLoggingApp.Model;

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
            Console.WriteLine("[2] Push to AWS Elasticsearch");
            Console.WriteLine("[3] Query AWS Elasticsearch");
            var strOption = Console.ReadLine();

            int opt;
            if(int.TryParse(strOption, out opt))
            {
                var format = config["Format"].ToLower();

                //AWS Application context
                SampleContext context = new SampleContext()
                {
                    RegionEndpoint = Amazon.RegionEndpoint.USEast1,
                    ContextOperation = (ContextOperation)opt,

                    //Athena queries
                    Queries = new List<string>()
                    {
                        "SELECT * FROM " + config["Format"]/*,

                        "SELECT * FROM " + config["Format"] + " WHERE Priority = 3",

                        "SELECT * FROM " + config["Format"] + " WHERE regexp_like(tag, 'Reyes|RnC')",

                        "SELECT * FROM " + config["Format"] + " WHERE Timestamp BETWEEN Timestamp '2018-02-12' AND Timestamp '2018-02-12'"*/
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
}
