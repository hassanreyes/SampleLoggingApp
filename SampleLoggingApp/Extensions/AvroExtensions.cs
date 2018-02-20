using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.Hadoop.Avro.Container;
using SampleLoggingApp.Model;

namespace SampleLoggingApp.Extensions
{
    public static class AvroExtensions
    {
        public static void UseAvroPushToS3Bucket(this SampleContext context)
        {
            context.PushToS3Bucket = (int numOfFiles, int numOfRecords) =>
            {
                //Get compression method
                var codec = Codec.Null;

                if (!String.IsNullOrEmpty(context.CompressionMethod))
                {
                    if (context.CompressionMethod.ToLower() == "deflate")
                        codec = Codec.Deflate;
                }

                using (var client = new SampleS3Client(context.Format, context.S3BucketName,
                                                       context.S3BucketPath, Amazon.RegionEndpoint.USEast1))
                {
                    for (int i = 0; i < numOfFiles; i++)
                    {
                        DateTime randDateTime = SampleData.RandDate();
                        var testData = new List<LogEntry>();

                        testData.AddRange(SampleData.GetBunchOfData(numOfRecords, randDateTime));

                        using (MemoryStream buffer = new MemoryStream())
                        {
                            using (var w = AvroContainer.CreateWriter<LogEntry>(buffer, codec))
                            {
                                using (var writer = new SequentialWriter<LogEntry>(w, 24))
                                {
                                    testData.ForEach(writer.Write);
                                }
                            }

                            //Objects are push sync. to keep the order.
                            client.PutObject(buffer, randDateTime);
                        }

                        SampleContext.ClearConsoleLine();
                        Console.Write($"\r{(i + 1f) / (float)numOfFiles,6:P2}");
                    }
                }
            };
        }
    }
}
