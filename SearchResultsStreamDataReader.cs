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

  public bool GetBoolean(int i){
    return (bool)m_Current.GetValue(m_IndexToNameMapping[i]);
  }

  public byte GetByte(int i){
    return (byte) m_Current.GetValue(m_IndexToNameMapping[i]);
  }
   
  public byte GetBytes(int i, long fieldOffset, byte[] buffer, int bufferOffset, int length){
    var data = m_Current.GetValue(m_IndexToNameMapping[i]) as IEnumerable<byte>;
    
    long count = 0;
    foreach(var x in data.Skip((int)fieldOffset).Take(length).Select((b, idx) => new { b, idx }))
    {
      buffer[bufferoffset + x.idx] = x.b;
      count++;
    }
    return count;
  }

  public char GetChar(int i){
    return (char) m_Current.GetValue(m_IndexToNameMapping[i]);
  }

  public char GetChars(int i, long fieldOffset, char[] buffer, int bufferOffset, int length){
    long count = 0;
    foreach(var x in data.Skip((int)fieldOffset).Take(length).Select((b, idx) => new { b, idx }))
    {
      buffer[bufferoffset + x.idx] = x.b;
      count++;
    }
    return count;
  }
  
  public IDataReader GetData(int i){
    throw new NotImplementedException();
  }
  
  
  public string GetDataTypeName(int i){
    return (string) m_Current.GetValue(m_IndexToNameMapping[i]);
  }
}
