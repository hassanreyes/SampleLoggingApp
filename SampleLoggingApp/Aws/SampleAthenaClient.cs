using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Amazon.Athena.Model;

namespace SampleLoggingApp
{
    public class SampleAthenaClient : IDisposable
    {
        Amazon.Athena.AmazonAthenaClient _athenaClient;

        public string DataBase { get; protected set; }
        public string OutputLocation { get; set; }

        public SampleAthenaClient(string dataBase, string outputLocation, Amazon.RegionEndpoint endpoint)
        {
            _athenaClient = new Amazon.Athena.AmazonAthenaClient(endpoint);

            this.DataBase = dataBase;
            this.OutputLocation = outputLocation;
        }

        public string ExecuteQuery(string query)
        {
            TimeSpan queryExecTime, recordsReturnTime;

            QueryExecutionContext queryExecContext = new QueryExecutionContext()
            {
                Database = this.DataBase
            };

            ResultConfiguration resultConfig = new ResultConfiguration() 
            { 
                OutputLocation = this.OutputLocation 
            };

            StartQueryExecutionRequest startQueryExecRequest = new StartQueryExecutionRequest()
            {
                QueryExecutionContext = queryExecContext,
                QueryString = query,
                ResultConfiguration = resultConfig
            };

            var watch = new Stopwatch();

            #region Query Execution
            //Start Query execution and wait till start command has completed
            var startQueryExecResult = _athenaClient.StartQueryExecutionAsync(startQueryExecRequest);

            //Start measurement
            watch.Start();

            while(!startQueryExecResult.IsCompleted)
            {
                Thread.Sleep(100);
                Console.Write(":");
            }

            if (startQueryExecResult.Exception != null)
            {
                throw new Exception("Query Execution", startQueryExecResult.Exception);
            }

            if (startQueryExecResult.IsCanceled)
            {
                return "- Cancelled -";
            }

            //Query if query execution has finished
            GetQueryExecutionRequest getQueryExecRequest = new GetQueryExecutionRequest() 
            { 
                QueryExecutionId = startQueryExecResult.Result.QueryExecutionId 
            };
            Task<Amazon.Athena.Model.GetQueryExecutionResponse> getQueryExecResult = null;

            bool isQueryRunning = true;
            while(isQueryRunning)
            {
                getQueryExecResult = _athenaClient.GetQueryExecutionAsync(getQueryExecRequest);
                var state = getQueryExecResult.Result.QueryExecution.Status.State;

                if (state == Amazon.Athena.QueryExecutionState.FAILED)
                {
                    throw new Exception("Query Execution", getQueryExecResult.Exception);
                }
                else if (state == Amazon.Athena.QueryExecutionState.CANCELLED)
                {
                    return "- Canceled -";
                }
                else if(state == Amazon.Athena.QueryExecutionState.SUCCEEDED)
                {
                    isQueryRunning = false;
                    queryExecTime = watch.Elapsed;
                }
                else
                {
                    Thread.Sleep(100);
                    Console.Write(":");
                }
            }

            #endregion

            watch.Restart();

            #region Get Query Result

            GetQueryResultsRequest getQueryResultRequest = new GetQueryResultsRequest()
            {
                QueryExecutionId = getQueryExecResult.Result.QueryExecution.QueryExecutionId
            };

            var getQueryResultsResult =_athenaClient.GetQueryResultsAsync(getQueryResultRequest);

            //No data process is taken account. Only take meassurements.
            long contentSize = 0, totalRows = 0;

            while(true)
            {
                //while (!getQueryResultsResult.IsCompleted)
                //{
                //    Thread.Sleep(100);
                //    Console.Write(".");
                //}

                if (getQueryResultsResult.Exception != null)
                {
                    throw new Exception("Retrieving Query Rows", getQueryResultsResult.Exception);
                }

                if (getQueryResultsResult.IsCanceled)
                {
                    return "- Canceled -";
                }

                while(getQueryResultsResult.Status == System.Threading.Tasks.TaskStatus.Running)
                {
                    Thread.Sleep(100);
                    Console.Write(".");
                }

                totalRows += getQueryResultsResult.Result.ResultSet.Rows.Count;

                if(getQueryResultsResult.Result.NextToken == null)
                {
                    recordsReturnTime = watch.Elapsed;
                    contentSize += getQueryResultsResult.Result.ContentLength;
                    break;
                }

                getQueryResultsResult = _athenaClient.GetQueryResultsAsync(new GetQueryResultsRequest() 
                    { 
                        QueryExecutionId = getQueryResultRequest.QueryExecutionId, 
                        NextToken = getQueryResultRequest.NextToken 
                    }
                );
            }

            #endregion

            return $"{totalRows} Rows with a total of {contentSize/1024f} KB " + 
                "took {queryExecTime} executing query and {recordsReturnTime} returning records.";
        }

        #region IDisposable Support
        private bool disposedValue = false; // To detect redundant calls

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    if (_athenaClient != null)
                        _athenaClient.Dispose();
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
