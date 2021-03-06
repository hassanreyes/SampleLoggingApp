﻿using System;
using System.Collections.Generic;
using System.IO;
using Parquet.Data;
using SampleLoggingApp.Model;

namespace SampleLoggingApp.Extensions
{
    public static class ParquetExtensions
    {
        public static void UseParquetPushToS3Bucket(this SampleContext context)
        {
            context.PushToS3Bucket = (int numOfFiles, int numOfRecords) =>
            {
                Schema schema = new Schema(
                    new DataField<DateTime>("Timestamp"),
                    new DataField<int>("Priority"),
                    new DataField<string>("Source"),
                    new DataField<string>("Message"),
                    new DataField<IEnumerable<string>>("Tags"),
                    new StructField("InnerData", 
                                    new DataField<string>("IpAddress"),
                                    new DataField<string>("Message")
                                   )
                );

                //Get compression method
                Parquet.CompressionMethod compressionMethod = Parquet.CompressionMethod.None;

                if (!String.IsNullOrEmpty(context.CompressionMethod))
                {
                    if (context.CompressionMethod.ToLower() == "snappy")
                        compressionMethod = Parquet.CompressionMethod.Snappy;
                    else if (context.CompressionMethod.ToLower() == "gzip")
                        compressionMethod = Parquet.CompressionMethod.Gzip;
                }

                using (var client = new SampleS3Client("parquet", context.S3BucketName, context.S3BucketPath,
                                                       Amazon.RegionEndpoint.USEast1))
                {
                    for (int i = 0; i < numOfFiles; i++)
                    {
                        DateTime randDateTime = SampleData.RandDate();
                        DataSet ds = new DataSet(schema);

                        foreach (LogEntry entry in SampleData.GetBunchOfData(numOfRecords, randDateTime))
                        {
                            ds.Add(new Row(entry.Timestamp, entry.Priority, entry.Source, entry.Message, entry.Tags, new Row(entry.InnerData.IpAddress, entry.InnerData.Message)) );
                        }

                        using (MemoryStream buffer = new MemoryStream())
                        {
                            using (var writer = new Parquet.ParquetWriter(buffer))
                            {
                                writer.Write(ds, compressionMethod);
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
