using System;
using static SampleLoggingApp.Model.Statistics;
using Nest;
using SampleLoggingApp.Model;
using System.Collections.Generic;
using Elasticsearch.Net;
using System.Diagnostics;
using Amazon.Runtime;
using Elasticsearch.Net.Aws;

namespace SampleLoggingApp
{
    public class SampleElasticsearchClient
    {
        const int MAX_DOCS_PER_REQUEST = 50000;

        ElasticClient _esClient;

        public SampleElasticsearchClient(string endPoint, Amazon.RegionEndpoint regionEndPoint)
        {
            AWSCredentials awsCredentials = FallbackCredentialsFactory.GetCredentials();
            AwsHttpConnection conn = new AwsHttpConnection(regionEndPoint.SystemName, 
                                                           new StaticCredentialsProvider( new AwsCredentials()
            {
                AccessKey = awsCredentials.GetCredentials().AccessKey,
                SecretKey = awsCredentials.GetCredentials().SecretKey,
                Token = awsCredentials.GetCredentials().Token
            }));

            var pool = new SingleNodeConnectionPool(new Uri(endPoint));
            ConnectionSettings settings = new ConnectionSettings(pool, conn)
                .DisableDirectStreaming()
                .InferMappingFor<LogEntry>(m => m.IndexName("logs"));
                //.DefaultMappingFor<LogEntry>(m => m.IndexName("logs"));

            _esClient = new ElasticClient(settings);

            IndexName logIndex = IndexName.From<LogEntry>();

            var req = new IndexExistsRequest(logIndex);
            var res = _esClient.IndexExists(req);
            if (!res.Exists)
            {
                _esClient.CreateIndex("logs", c => c
                                      .Mappings(md => md.Map<LogEntry>(m => m.AutoMap())));
            }
        }

        public void PushLogs(int totalDocuments)
        {
            if (totalDocuments > MAX_DOCS_PER_REQUEST)
            {
                int n = totalDocuments / MAX_DOCS_PER_REQUEST;
                int remain = totalDocuments % MAX_DOCS_PER_REQUEST;
                for (int i = 0; i < n; i++)
                {
                    SecurePushLogs(MAX_DOCS_PER_REQUEST);
                }
                if (remain > 0)
                    SecurePushLogs(remain);
            }
            else
            {
                SecurePushLogs(totalDocuments);
            }
        }

        private void SecurePushLogs(int n)
        {
            if (n > MAX_DOCS_PER_REQUEST)
                throw new Exception("The number of documents to index is too large.");

            var bulkRequest = new BulkRequest("logs")
            {
                Operations = GetBunchOfDataOperations(n, SampleData.RandDate())
            };

            var response = _esClient.Bulk(bulkRequest);


        }

        public string QueryMatchAll(QueryStageStatistics queryStats)
        {
            var matchAll = new QueryContainerDescriptor<LogEntry>().MatchAll();

            Stopwatch watch = new Stopwatch();
            watch.Start();

            var searchResponse = _esClient.Search<LogEntry>(s => s
                                                            .Query(q => matchAll));

            long total = searchResponse.Total;

            queryStats.QueryExecutionTime = watch.Elapsed;
            queryStats.EngineExecutionTimeInMillis = searchResponse.Took;

            Console.WriteLine();
            int count = 0;

            var printOut = new Action(() =>
            {
                foreach (LogEntry log in searchResponse.Documents)
                {
                    Console.Write("\r" + log.ToString());
                    count++;
                }
            });

            printOut();

            watch.Restart();

            if(SampleContext.MAX_FETCHED_RECORDS > count)
            {
                searchResponse = _esClient.Search<LogEntry>(s => s
                                                            .Query(q => matchAll)
                                                            .From(count)
                                                            .Size(SampleContext.MAX_FETCHED_RECORDS - count));

                printOut();
            }

            queryStats.DataFetchingTime = watch.Elapsed;

            return $"Query Execution took {TimeSpan.FromMilliseconds(searchResponse.Took)} executing query and {queryStats.DataFetchingTime} fetching first {SampleContext.MAX_FETCHED_RECORDS} records.";
        }

        public static IList<IBulkOperation> GetBunchOfDataOperations(int n, DateTime dt)
        {
            var entries = new List<LogEntry>(SampleData.GetData(dt));
            var maxN = entries.Count;
            var operations = new List<IBulkOperation>(n);

            for (int i = 0, x = 0; i < n; i++, x++)
            {
                x = x >= maxN ? 0 : x;
                operations.Add(new BulkIndexOperation<LogEntry>(entries[x]));
            }

            return operations;
        }
    }
}
