# Splunk Client Data Reader

[![.NET](https://github.com/YulerB/SplunkClientDataReader/actions/workflows/dotnet.yml/badge.svg)](https://github.com/YulerB/SplunkClientDataReader/actions/workflows/dotnet.yml)
![NuGet Badge](https://buildstats.info/nuget/SplunkClientDataReader)
[![Quality Gate Status](https://sonarcloud.io/api/project_badges/measure?project=YulerB_SplunkClientDataReader&metric=alert_status)](https://sonarcloud.io/dashboard?id=YulerB_SplunkClientDataReader)

This package provides SearchResultStreamDataReader, the missing link between Splunk.Client and SqlBulkCopy (SqlBulkCopy takes an IDataReader).

Usage:
```csharp
using Splunk.Client;
using SplunkClientDataReader;
using System.Data.SqlClient;

...

public async Task StreamSplunkToSqlServer()
{
  
  using(SqlBulkCopy sqlBulkCopy = new SqlBulkCopy(connectionString, SqlBulkCopyOptions.Default)){
    sqlBulkCopy.BatchSize = 5000;
    sqlBulkCopy.EnableStreaming = true;
    sqlBulkCopy.BulkCopyTimeout = 360;
    sqlBulkCopy.DestinationTableName = "dbo.SplunkData";

    using (var context = new Context(Scheme.Https, endPoint.Host, endPoint.Port))
    {
      using (var client = new Service(context))
      {
        await client.LogOnAsync(credential.UserName, credential.Password).ConfigureAwait(false);

        using(var results = await client.ExportSearchResultsAsync(search, searchExportArgs).ConfigureAwait(false))
        {
          using(var reader = new SearchResultStreamDataReader(results))
          {
            await sqlBlockCopy.WriteToServerAsync(reader).ConfigureAwait(false);
          }
        }
      }
    }
  }
}
```
