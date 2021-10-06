# Splunk Client Data Reader

Usage:
```csharp
using Splunk.Client;
using SplunkClientDataReader;
using System.Data.SqlClient;

...

public async Task StreamSplunkToSqlServer()
{
  
  SqlBulkCopy sqlBulkCopy = new SqlBulkCopy(connectionString, SqlBulkCopyOptions.Default);
  sqlBulkCopy.BatchSize = 5000;
  sqlBulkCopy.EnableStreaming = true;
  sqlBulkCopy.BulkCopyTimeout = 360;
  sqlBulkCopy.DestinationTableName = "dbo.SplunkData";

  using (var context = new Context(Scheme.Https, endPoint.Host, endPoint.Port))
  {
    using (var client = new Service(context))
    {
      await client.LogOnAsync(credential.UserName, credential.Password).ConfigureAwait(false);

      using(SearchResultsStream results = await client.ExportSearchResultsAsync(search, searchExportArgs).ConfigureAwait(false))
      {
        using(SearchResultsStreamDataReader reader = new SearchResultStreamDataReader(results))
        {
          await sqlBlockCopy.WriteToServerAsync(reader).ConfigureAwait(false);
        }
      }
    }
  }
}
```
