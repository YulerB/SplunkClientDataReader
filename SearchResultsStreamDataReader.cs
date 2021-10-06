using System.Data;
using Spunk.Client;

public class SearchResultsStreamDataReader : IDataReader
{
  private IEnumerator<SearchResult> m_Iterator;
  private SearchResult m_Current;
  private readonly IDictionary<int, string> m_IndexToNameMapping = new Dictionary<int, string>();
  private readonly IReadOnlyCollection<string> fields;
  private readonly SearchResultStream results;
  
  public SearchResultsStreamDataReader(SearchResultsStream results)
  {
    this.fields = results.FieldNames;
    this.results = results;
    int i = 0;
    
    foreach(var item in fields)
    {
      m_IndexToNameMapping.Add(i, item);
      i++;      
    }
  }
  
  public object this[string name]
  {
    get{
      return m_Current.GetValue(name);
    }
  }
  
  
  public object this[int i]
  {
    get{
      return m_Current.GetValue(m_IndexToNameMapping[i]);
    }
  }
  
  
  public int Depth
  {
    get{
      return 1;
    }
  }
  
  public int FieldCount
  {
    get{
      return fields.Count;
    }
  }


  public bool IsClosed
  {
    get
    {
      return m_Iterator == null;
    }
  }
  
  public int RecordsAffected
  {
    get{
      return Convert.ToInt32(results.ReadCount);
    }
  }
  
  
  public void Close()
  {
    if (m_Iterator != null){
      m_Current = null;
      m_Iterator.Dispose();
      m_Iterator = null;
    }
  }
  
  public void Dispose(){
    Close();
  }

}
