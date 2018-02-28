# Sample Logging Application (POC)

Console tool wich interact with Amazon S3, Amazon Athena and Elasticsearch in order to get metrics about running time, sacanned memory and 
query execution.

## Getting Started

Be sure to have a proper appsettings.json file configured with the following fields:

```json
{
  "Format": "parquet",
  "_Format": "avro",
  "AthenaDataBase": "<DB_NAME>",
  "AthenaS3BucketOutputPath": "s3://<BUCKET_NAME>/<PATH>/athena_output/",
  "BucketName": "<BUCKET_NAME>",
  "BucketPath": "<BUCKET_PATH>/",
  "CompressionMethod": "Snappy",
  "_CompressionMethod": "Deflate",
  "ElasticsearchEndpoint": "<ELASTICSEARCH_ENDPOINT>"
}
```
**Amazon S3 and Athena Configuration**

  *Format*: The format used to encode sample logs that going to be pushed to S3. Possible values are: parquet | avro.
  
  *CompressionMethod*: The compression method supported by the choosed formatter. For now use "Snappy" for parquet files and "Deflate" for avro files.
  
  *BucketName*: The name of the bucket in S3 to push and pull out the sample logs.
  
  *BuketPath*: The path within the bucket. You don't need to repeat the buket name.
  
  *AthenaDataBase*: Athena DataBase name.
  
  *AthenaS3BucketOutputPath*: The bucket where Athena will going to put the query results.
  
**Amazon Elasticsearch Configuration**

  *ElsticsearchEndpoint*: The endpoint address of the Elasticsearch instance in AWS.
  
### Prerequisites

1. AWS account.
2. AWS ~/.aws/credentials file since the application uses the default role.
3. S3 Bucket with read, write rights from internet assigned to the user/role.
4. Athena database with the following table structure:


5. Elasticsearch isntance running with a valid endpoint.

> **Check for permissions in order to be able to connect from internet and allow Athena to read/write from/to S3.**

### Usage 

## License

This project is licensed under the MIT License - see the [LICENSE.md](LICENSE.md) file for details
