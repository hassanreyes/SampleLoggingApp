using System;
using Microsoft.Extensions.Configuration;

namespace SampleLoggingApp.Aws.Extensions
{
    public static class SampleContextExtensions
    {
        public static SampleContext UseConfiguration(this SampleContext context, IConfigurationRoot config)
        {
            context.Format = config["Format"];
            context.CompressionMethod = config["CompressionMethod"];
            context.S3BucketName = config["BucketName"];
            context.S3BucketPath = config["BucketPath"];
            context.AthenaDataBase = config["AthenaDataBase"];
            context.AthenaS3BucketOutputPath = config["AthenaS3BucketOutputPath"];
            context.ElasticsearchEndpoint = config["ElasticsearchEndpoint"];

            return context;
        }
    }
}
