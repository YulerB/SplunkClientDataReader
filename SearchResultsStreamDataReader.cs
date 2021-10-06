using System;
using System.Data;
using System.Collections.Generic;
using System.Linq;
using Splunk.Client;

namespace SplunkClientDataReader
{
  public class SearchResultsStreamDataReader : IDataReader
  {
    private IEnumerator<SearchResult> m_Iterator;
    private SearchResult m_Current;
    private readonly IDictionary<int, string> m_IndexToNameMapping ;
    private readonly IReadOnlyCollection<string> fields;
    private readonly SearchResultStream results;

    public SearchResultsStreamDataReader(SearchResultStream results)
    {
      this.fields = results.FieldNames;
      this.m_IndexToNameMapping = new Dictionary<int, string>(this.fields.Count);
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

    public long GetBytes(int i, long fieldOffset, byte[] buffer, int bufferoffset, int length){
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

    public long GetChars(int i, long fieldOffset, char[] buffer, int bufferoffset, int length){
      var data = m_Current.GetValue(m_IndexToNameMapping[i]) as IEnumerable<char>;

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
      return m_Current.GetValue(m_IndexToNameMapping[i]).GetType().ToString();
    }

    public DateTime GetDateTime(int i){
      return (DateTime)m_Current.GetValue(m_IndexToNameMapping[i]);
    }

    public decimal GetDecimal(int i){
      return (decimal)m_Current.GetValue(m_IndexToNameMapping[i]);
    }

    public double GetDouble(int i){
      return (double)m_Current.GetValue(m_IndexToNameMapping[i]);
    }

    public Type GetFieldType(int i){
      return m_Current.GetValue(m_IndexToNameMapping[i]).GetType();
    }

    public float GetFloat(int i){
      return (float)m_Current.GetValue(m_IndexToNameMapping[i]);
    }

    public Guid GetGuid(int i){
      return (Guid)m_Current.GetValue(m_IndexToNameMapping[i]);
    }

    public short GetInt16(int i){
      return (short)m_Current.GetValue(m_IndexToNameMapping[i]);
    }

    public int GetInt32(int i){
      return (int)m_Current.GetValue(m_IndexToNameMapping[i]);
    }

    public long GetInt64(int i){
      return (long)m_Current.GetValue(m_IndexToNameMapping[i]);
    }

    public string GetName(int i){
      return m_IndexToNameMapping[i];
    }

    public int GetOrdinal(string name){
      int i = 0;

      foreach(var item in fields) {
        if(item == name) 
        {
          return i;
        }

        i++;
      }

      return -1;
    }

    public DataTable GetSchemaTable(){
      throw new NotImplementedException();
    }

    public string GetString(int i){
      return m_Current.GetValue(m_IndexToNameMapping[i]) as string;
    }

    public object GetValue(int i){
      return this[i];
    }

    public int GetValues(object[] values){
      int count = 0;
      for (int i = 0; i < this.FieldCount && i < values.Length; i++){
        values[i] = this[i];
        count++;
      }
      return count;
    }

    public bool IsDBNull(int i){
      object value = this[i];
      return value == null || value == DBNull.Value;
    }

    public bool NextResult(){
      throw new NotImplementedException();
    }

    public bool Read()
    {
      if(m_Iterator == null)
      {
        m_Iterator = results.GetEnumerator();
      }

      var ret = m_Iterator.MoveNext();
      if(ret)
      {
        m_Current = m_Iterator.Current;
      }

      return ret;    
    }
  }
}
