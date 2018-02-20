using System;
using System.IO;
using System.Threading;
using Amazon.S3;
using Amazon.S3.Model;

namespace SampleLoggingApp
{
    public class SampleS3Client: IDisposable
    {
        private AmazonS3Client _s3Client;

        public string Format { get; private set; }
        public string BucketName { get; private set; }
        public string BucketPath { get; private set; }

        public SampleS3Client(string format, string bucketName, string bucketPath, Amazon.RegionEndpoint endpoint)
        {
            this.Format = format;
            this.BucketName = bucketName;
            this.BucketPath = bucketPath;
            _s3Client = new AmazonS3Client(endpoint);
        }

        public void PutObject(Stream buffer, DateTime dt)
        {
            var partitionPath = String.Format("year={0:yyyy}/month={0:MM}/day={0:dd}", dt);
            var putRequest = new PutObjectRequest()
            {
                BucketName = this.BucketName,
                /*ContentType = "application/octet-stream"*/
                InputStream = buffer,
                Key = $"{this.BucketPath}{Format}/{partitionPath}/{Guid.NewGuid().ToString()}.{Format}",
            };

            var putResponse = _s3Client.PutObjectAsync(putRequest);

            //Wait to complete before continue
            while (!putResponse.IsCompleted)
            {
                if (putResponse.Exception != null)
                {
                    Console.WriteLine(putResponse.Exception.Message);
                    break;
                }
                Thread.Sleep(100);
                Console.Write(".");
            }
        }

        #region IDisposable Support
        private bool disposedValue = false; // To detect redundant calls

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    if (_s3Client != null)
                        _s3Client.Dispose();
                }

                disposedValue = true;
            }
        }

        // This code added to correctly implement the disposable pattern.
        public void Dispose()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            Dispose(true);
        }
        #endregion

    }
}
