using System;

namespace CC.Common.ListExt
{
    [AttributeUsage(AttributeTargets.Property)]
    public class CsvExport : Attribute
    {
        private readonly string _headerName;
        public CsvExport(string headerName)
        {
            _headerName = headerName;
        }

        public string HeaderName
        {
            get { return _headerName; }
        }
    }
}
