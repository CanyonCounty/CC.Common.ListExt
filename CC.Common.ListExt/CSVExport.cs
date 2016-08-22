using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CC.Common.ListExt
{
  [AttributeUsage(AttributeTargets.Property)]
  public class CSVExport : System.Attribute
  {
    private string _headerName;
    public CSVExport(string headerName)
    {
      _headerName = headerName;
    }

    public String HeaderName
    {
      get { return _headerName; }
    }
  }
}
