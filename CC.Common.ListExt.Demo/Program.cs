﻿using System;
using System.Collections.Generic;

namespace CC.Common.ListExt.Demo
{
    public static class Program
    {
        /// <summary>
        /// Class used to test the Export functionality
        /// </summary>
        private class ExportTest
        {
            private EmployeeList _list;
            private List<string> _fields;
            private Random _gen = new Random();

            private DateTime GetRandomDate(DateTime from, DateTime to)
            {
                var range = new TimeSpan(to.Ticks - from.Ticks);
                return from + new TimeSpan((long)(range.Ticks * _gen.NextDouble()));
            }

            private DateTime RandomDate()
            {
                return GetRandomDate(new DateTime(1930, 1, 1), DateTime.Now);
            }

            private string Test(string sort)
            {
                var ret = string.Empty;

                if (null == _list)
                {
                    _list = new EmployeeList
                    {
                        new Employee("Ken", "Wilcox", "Programmer", new DateTime(1974, 05, 25, 21, 35, 40)),
                        new Employee("Jane", "Doe", "Programmer", RandomDate()),
                        new Employee("John", "Doe", "Marketting", RandomDate()),
                        new Employee("Adelle", "Wilcox", "Programmer", RandomDate())
                    };
                }

                // this is to get resharper to not be so "helpful"
                var emp = new Employee("nothing", "nothing", "nothing", DateTime.Today);

                if (null == _fields)
                {
                    // What fields we want to export, and in what order
                    _fields = new List<string> {"Date", "FirstName", "LastName"};

                    // Or we can exclude one
                    _fields = _list.GetFields();
                    _fields.Remove("Date");

                    // if I get a null or empty list of fields I'll use all of them
                    // no point in using this to create an empty file
                    //_fields = new List<string>();

                    // if it's not in this if resharper wants to use the object initializer
                    // then it says the fields can be private, but private fields fail to export
                    emp.FirstName = "NotNull";
                    emp.LastName = "NotNull";
                    emp.Title = "NotNull";
                    emp.Date = DateTime.Today.AddDays(-1);
                    _list.Add(emp);
                }

                try
                {
                    _list.Sort(sort); // Is this cool or what!!!11!!!

                    //_list.CsvFieldSep('\t');
                    //_list.CsvFieldPrefixPostfix('\'');
                    //_list.CsvFieldPrefix('[');
                    //_list.CsvFieldPostfix(']');

                    //_list.DisablePrefixPostFix();

                    // These are only written when you export to file
                    _list.CsvPreData("This is some cool report header - it's just text, add what you want\n");
                    _list.CsvPostData("This is some cool report footer - it just does whatever you want too!\n\nSee more here\n");

                    _list.ExportToCsv(_fields, @"C:\Temp\data.csv");
                    var data = _list.ExportToCsv(_fields);
                    Console.Write(data);
                }
                catch (Exception ex)
                {
                    ret = ex.Message;
                }

                return ret;
            }

            public static void Main()
            {
                var t = new ExportTest();
                Console.WriteLine(t.Test("LastName desc, FirstName asc"));
            }
        }

        public class Employee
        {
            [CsvExport("First Name")]
            public string FirstName { get; set; }
            [CsvExport("Last Name")]
            public string LastName { get; set; }
            [CsvExport("Position")]
            public string Title { get; set; }
            [CsvExport("Hire Date")]
            public DateTime Date { get; set; }

            public Employee(string firstName, string lastName, string title, DateTime date)
            {
                FirstName = firstName;
                LastName = lastName;
                Title = title;
                Date = date;
            }

            public override string ToString()
            {
                return string.Format("Title: {0} - Name: {1}, {2} Date: {3}", Title, LastName, FirstName, Date);
            }
        }

        public class EmployeeList : List<Employee> { }
    }
}
