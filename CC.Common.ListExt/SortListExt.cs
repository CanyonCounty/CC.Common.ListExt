using System;
using System.Collections.Generic;
using System.Reflection;

namespace CC.Common.ListExt
{
    public static class SortListExt
    {
        // http://www.codeproject.com/Articles/27834/Generic-List-Sort-Function
        /// <summary>
        /// Sorts the specified list.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list">The list to be sorted.</param>
        /// <param name="sortExpression">The sort expression; format:
        /// @param1 [sortdirection], @param2 [sortdirection], @param3 [sortdirection].
        /// Valid sortDirections are: asc, desc, ascending and descending.</param>
        public static void Sort<T>(this List<T> list, string sortExpression)
        {
            var sortExpressions = sortExpression.Split(new[] { "," }, StringSplitOptions.RemoveEmptyEntries);
            var comparers = new List<GenericComparer>();

            foreach (var sortExpress in sortExpressions)
            {
                var sortProperty = sortExpress.Trim().Split(' ')[0].Trim();
                var sortDirectionStr = sortExpress.Trim().Split(' ')[1].Trim();

                var type = typeof(T);
                var propertyInfo = type.GetProperty(sortProperty);
                if (propertyInfo == null)
                {
                    var props = type.GetProperties();
                    foreach (var info in props)
                    {
                        if (info.Name.ToLower() == sortProperty.ToLower())
                        {
                            propertyInfo = info;
                            break;
                        }
                    }
                    if (propertyInfo == null)
                    {
                        throw new Exception(string.Format("{0} is not a valid property of type: \"{1}\"", sortProperty, type.Name));
                    }
                }

                SortDirection sortDirection;
                if (sortDirectionStr.ToLower() == "asc" || sortDirectionStr.ToLower() == "ascending")
                {
                    sortDirection = SortDirection.Ascending;
                }
                else if (sortDirectionStr.ToLower() == "desc" ||
                        sortDirectionStr.ToLower() == "descending")
                {
                    sortDirection = SortDirection.Descending;
                }
                else
                {
                    throw new Exception("Valid SortDirections are: asc, ascending, desc and descending");
                }

                comparers.Add(new GenericComparer
                {
                    SortDirection = sortDirection,
                    PropertyInfo = propertyInfo,
                    Comparers = comparers
                });
            }
            list.Sort(comparers[0].Compare);
        }
    }

    public class GenericComparer
    {
        public List<GenericComparer> Comparers { get; set; }
        int _level;

        public SortDirection SortDirection { get; set; }
        public PropertyInfo PropertyInfo { get; set; }

        public int Compare<T>(T t1, T t2)
        {
            int ret;

            if (_level >= Comparers.Count)
                return 0;

            var t1Value = Comparers[_level].PropertyInfo.GetValue(t1, null);
            var t2Value = Comparers[_level].PropertyInfo.GetValue(t2, null);

            if (t1 == null || t1Value == null)
            {
                if (t2 == null || t2Value == null)
                {
                    ret = 0;
                }
                else
                {
                    ret = -1;
                }
            }
            else
            {
                if (t2 == null || t2Value == null)
                {
                    ret = 1;
                }
                else
                {
                    ret = ((IComparable)t1Value).CompareTo(((IComparable)t2Value));
                }
            }
            if (ret == 0)
            {
                _level += 1;
                ret = Compare(t1, t2);
                _level -= 1;
            }
            else
            {
                if (Comparers[_level].SortDirection == SortDirection.Descending)
                {
                    ret *= -1;
                }
            }
            return ret;
        }
    }

    public enum SortDirection
    {
        Ascending = 0,
        Descending = 1
    }
}
