using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.IO;
using System.ComponentModel;

namespace CC.Common.ListExt
{
  public static class ExportBindingListToCSV
  {
    private static string _error = String.Empty;
    private static char _fieldSep = ',';
    private static char _fieldPrefix = '"';
    private static char _fieldPostfix = '"';
    private static bool _disabledPrePost = false;
    private static string _preData = String.Empty;
    private static string _postData = String.Empty;

    private static string PrePostFix(string item)
    {
      if (!_disabledPrePost)
      {
        return _fieldPrefix.ToString() + item + _fieldPostfix.ToString();
      }
      else
      {
        return item;
      }
    }

    // Takes a list of Objects and returns a csv
    public static string ExportToCSV<T>(this BindingList<T> list, List<string> fields)
    {
      string ret = "";
      Dictionary<string, PropertyInfo> dict = new Dictionary<string, PropertyInfo>();
      Type type = typeof(T);
      string header = String.Empty;
      string head = String.Empty;

      foreach (string field in fields)
      {
        PropertyInfo pi = type.GetProperty(field.Trim());
        if (pi != null)
        {
          dict.Add(field, pi);
          head = PrePostFix(field) + _fieldSep.ToString();

          object[] attributes;
          attributes = dict[field].GetCustomAttributes(typeof(CSVExport), false);
          foreach (Object attribute in attributes)
          {
            try
            {
              CSVExport csv = (CSVExport)attribute;
              head = PrePostFix(csv.HeaderName) + _fieldSep.ToString();
            }
            catch { }
          }
        }
        else
          throw new Exception(String.Format("{0} is not a valid property of type: \"{1}\"", field, type.Name));

        header += head;
      }

      ret += header.TrimEnd(_fieldSep);
      ret += Environment.NewLine;

      foreach (T item in list)
      {
        string row = String.Empty;
        //ret += item.ToString() + Environment.NewLine;
        foreach (string field in fields)
        {
          //string msg = prop.GetValue(this, null).ToString();
          //PropertyInfo pi = dict[field];
          //object obj = pi.GetValue(item, null);

          // If the column is null, then enter a blank space
          object obj = dict[field].GetValue(item, null);
          if (obj != null)
            row += PrePostFix(obj.ToString()) + _fieldSep.ToString();
          else
            row += _fieldSep;
        }

        ret += row.TrimEnd(_fieldSep);
        ret += Environment.NewLine;
      }
      return ret;
    }

    public static bool ExportToCSV<T>(this BindingList<T> list, List<string> fields, string fileName)
    {
      bool ret = true;
      string data = ExportToCSV(list, fields);
      try
      {
        using (StreamWriter outfile = new StreamWriter(fileName))
        {
          if (_preData != String.Empty)
            outfile.Write(_preData);

          outfile.Write(data);

          if (_postData != String.Empty)
            outfile.Write(_postData);
          outfile.Flush();
          outfile.Close();
        }
      }
      catch (Exception e)
      {
        ret = false;
        _error = e.Message;
      }
      return ret;
    }

    public static bool ExportToCSV<T>(this BindingList<T> list, string fileName)
    {
      // If you just give me a filename, I'll figure out the fields...
      List<string> fields = new List<string>();
      Type type = typeof(T);
      PropertyInfo[] props = type.GetProperties();
      foreach (PropertyInfo info in props)
      {
        fields.Add(info.Name);
      }

      return ExportToCSV(list, fields, fileName);
    }

    public static string ExportError<T>(this BindingList<T> list)
    {
      return _error;
    }

    public static void CSVFieldSep<T>(this BindingList<T> list, char fieldSep)
    {
      _fieldSep = fieldSep;
    }

    public static void CSVFieldPrefix<T>(this BindingList<T> list, char fieldPrefix)
    {
      _fieldPrefix = fieldPrefix;
    }

    public static void CSVFieldPostfix<T>(this BindingList<T> list, char fieldPostfix)
    {
      _fieldPostfix = fieldPostfix;
    }

    public static void CSVFieldPrefixPostfix<T>(this BindingList<T> list, char fieldPrefixPostfix)
    {
      _fieldPostfix = fieldPrefixPostfix;
      _fieldPrefix = fieldPrefixPostfix;
    }

    public static void DisablePrefixPostFix<T>(this BindingList<T> list)
    {
      _disabledPrePost = true;
    }

    public static void CSVPreData<T>(this BindingList<T> list, string data)
    {
      _preData = data;
    }

    public static void CSVPostData<T>(this BindingList<T> list, string data)
    {
      _postData = data;
    }

  }
}
