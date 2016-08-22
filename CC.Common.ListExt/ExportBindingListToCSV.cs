using System;
using System.Collections.Generic;
using System.Reflection;
using System.IO;
using System.ComponentModel;

namespace CC.Common.ListExt
{
    public static class ExportBindingListToCsv
    {
        private static string _error = string.Empty;
        private static char _fieldSep = ',';
        private static char _fieldPrefix = '"';
        private static char _fieldPostfix = '"';
        private static bool _disabledPrePost;
        private static string _preData = string.Empty;
        private static string _postData = string.Empty;

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
        public static string ExportToCsv<T>(this BindingList<T> list, List<string> fields)
        {
            var ret = "";
            var dict = new Dictionary<string, PropertyInfo>();
            var type = typeof(T);
            var header = string.Empty;

            if (fields == null || fields.Count == 0)
            {
                fields = GetFields(list);
            }

            foreach (var field in fields)
            {
                string head;
                var pi = type.GetProperty(field.Trim());
                if (pi != null)
                {
                    dict.Add(field, pi);
                    head = PrePostFix(field) + _fieldSep.ToString();

                    var attributes = dict[field].GetCustomAttributes(typeof(CsvExport), false);
                    foreach (var attribute in attributes)
                    {
                        try
                        {
                            var csv = (CsvExport)attribute;
                            head = PrePostFix(csv.HeaderName) + _fieldSep.ToString();
                        }
                        catch
                        {
                            // ignored
                        }
                    }
                }
                else
                    throw new Exception(string.Format("{0} is not a valid property of type: \"{1}\"", field, type.Name));

                header += head;
            }

            ret += header.TrimEnd(_fieldSep);
            ret += Environment.NewLine;

            foreach (var item in list)
            {
                var row = string.Empty;
                //ret += item.ToString() + Environment.NewLine;
                foreach (var field in fields)
                {
                    //string msg = prop.GetValue(this, null).ToString();
                    //PropertyInfo pi = dict[field];
                    //object obj = pi.GetValue(item, null);

                    // If the column is null, then enter a blank space
                    var obj = dict[field].GetValue(item, null);
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

        public static bool ExportToCsv<T>(this BindingList<T> list, List<string> fields, string fileName)
        {
            var ret = true;
            var data = ExportToCsv(list, fields);
            try
            {
                using (var outfile = new StreamWriter(fileName))
                {
                    if (_preData != string.Empty)
                        outfile.Write(_preData);

                    outfile.Write(data);

                    if (_postData != string.Empty)
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

        public static bool ExportToCsv<T>(this BindingList<T> list, string fileName)
        {
            // If you just give me a filename, I'll figure out the fields...
            var fields = GetFields(list);

            return ExportToCsv(list, fields, fileName);
        }

        public static List<string> GetFields<T>(this BindingList<T> list)
        {
            var fields = new List<string>();
            var type = typeof(T);
            var props = type.GetProperties();
            foreach (var info in props)
            {
                fields.Add(info.Name);
            }
            return fields;
        }

        public static string ExportError<T>(this BindingList<T> list)
        {
            return _error;
        }

        public static void CsvFieldSep<T>(this BindingList<T> list, char fieldSep)
        {
            _fieldSep = fieldSep;
        }

        public static void CsvFieldPrefix<T>(this BindingList<T> list, char fieldPrefix)
        {
            _fieldPrefix = fieldPrefix;
        }

        public static void CsvFieldPostfix<T>(this BindingList<T> list, char fieldPostfix)
        {
            _fieldPostfix = fieldPostfix;
        }

        public static void CsvFieldPrefixPostfix<T>(this BindingList<T> list, char fieldPrefixPostfix)
        {
            _fieldPostfix = fieldPrefixPostfix;
            _fieldPrefix = fieldPrefixPostfix;
        }

        public static void DisablePrefixPostFix<T>(this BindingList<T> list)
        {
            _disabledPrePost = true;
        }

        public static void CsvPreData<T>(this BindingList<T> list, string data)
        {
            _preData = data;
        }

        public static void CsvPostData<T>(this BindingList<T> list, string data)
        {
            _postData = data;
        }

    }
}
