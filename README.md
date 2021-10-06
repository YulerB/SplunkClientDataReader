# SplunkClientDataReader

Usage:
```
using Splunk.Client;
using SplunkClientDataReader;
...
using (var context = new Context(Scheme.Https, endPoint.Host, endPoint.Port)){
  using (var client = new Service(context)){
    await client.LogOnAsync(credential.UserName, credential.Password).ConfigureAwait(false);
    
    using(SearchResultsStream results = await client.ExportSearchResultsAsync(search, searchExportArgs).ConfigureAwait(false)){
      using(SearchResultsStreamDataReader reader = new SearchResultStreamDataReader(results))
      {
        await sqlBlockCopy.WriteToServerAsync(reader).ConfigureAwait(false);
      }
    }
  }
}
```
