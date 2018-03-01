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
  
  *CompressionMethod*: The compression method supported by the chosen formatter. For now use "Snappy" for parquet files and "Deflate" for avro files.
  
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
4. Create the following path inside your S3 bucket: <BUCKET_NAME>/<BUCKET_PATH>/<FORMAT> were the format must be parquet or avro.
5. Athena database with the following table structure:

  ```
  CREATE EXTERNAL TABLE `parquet`(
  `timestamp` timestamp, 
  `priority` int, 
  `source` string, 
  `message` string, 
  `tags` array<string>, 
  `innerdata` struct<ip_address:string,message:string>)
PARTITIONED BY ( 
  `year` int, 
  `month` int, 
  `day` int)
ROW FORMAT SERDE 
  'org.apache.hadoop.hive.ql.io.parquet.serde.ParquetHiveSerDe' 
STORED AS INPUTFORMAT 
  'org.apache.hadoop.hive.ql.io.parquet.MapredParquetInputFormat' 
OUTPUTFORMAT 
  'org.apache.hadoop.hive.ql.io.parquet.MapredParquetOutputFormat'
  ```
  > The table name must match with the Format value in the appsettings.json.
   
6. Elasticsearch isntance running with a valid endpoint.
7. No schema need to be defined in Elasticsearch. The application creates the required index with the name: logs

> **Check for permissions in order to be able to connect from internet and allow Athena to read/write from/to S3.**

### Usage 

The application will ask you for the task to perform, you must choose one of the following options:

```
Select Operation to be performed:
[0] Push to S3 Only
[1] Run Athena Queries Only
[2] Push to AWS Elasticsearch
[3] Query AWS Elasticsearch
```

#### \[0] Push to S3

The application will ask you for the number of files to generate and the number of records into each file (with a factor of x10).

The application will partition the data by: p.e /parquet/year=2018/month=01/day=01/

#### \[1] Run Athena Queries

The application will ask you for the number of times the Query Set will be executed:

Execute at least one query: SELECT * FROM <FORMAT> 

The application retrieves only the first 10,000 records in order to get an statistic meature.

#### \[2] Push to Amazon Elasticsearch

#### \[3] Query Amazon Elasticsearch

#### Application Report

## License

This project is licensed under the MIT License - see the [LICENSE.md](LICENSE.md) file for details
