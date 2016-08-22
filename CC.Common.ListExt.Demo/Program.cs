using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CC.Common.ListExt.Demo
{
    class Program
    {
        /// <summary>
        /// Class used to test the Export functionality
        /// </summary>
        public class ExportTest
        {
            private EmployeeList list;
            //private List<string> fields;
            private Random _gen = new Random();

            private DateTime GetRandomDate(DateTime from, DateTime to)
            {
                TimeSpan range = new TimeSpan(to.Ticks - from.Ticks);
                return from + new TimeSpan((long)(range.Ticks * _gen.NextDouble()));
            }

            private DateTime RandomDate()
            {
                //DateTime ret = new DateTime(1950, 1, 1);
                //int range = ((TimeSpan)(DateTime.Today - ret)).Days;
                //return ret.AddDays(_gen.Next(range));
                return GetRandomDate(new DateTime(1930, 1, 1), DateTime.Now);
            }

            public string Test(string sort)
            {
                string ret = String.Empty;

                if (null == list)
                {
                    list = new EmployeeList();
                    list.Add(new Employee("Ken", "Wilcox", "Programmer", new DateTime(1974, 05, 25, 21, 35, 40)));
                    //System.Threading.Thread.Sleep(1000);
                    list.Add(new Employee("Jane", "Doe", "Programmer", RandomDate()));
                    //System.Threading.Thread.Sleep(1000);
                    list.Add(new Employee("John", "Doe", "Marketting", RandomDate()));
                    //System.Threading.Thread.Sleep(1000);
                    list.Add(new Employee("Adelle", "Wilcox", "Programmer", RandomDate()));
                }

                //if (null == fields)
                //{
                //  // What fields we want to export, and in what order
                //  fields = new List<string>();
                //  fields.Add("FirstName");
                //  fields.Add("LastName");
                //  fields.Add("Title");
                //}

                try
                {
                    list.Sort(sort); // Is this cool or what!!!11!!!
                                     //list.CSVFieldSep(',');
                                     //list.CSVFieldPrefixPostfix('\'');
                                     //list.CSVFieldPrefix('[');
                                     //list.CSVFieldPostfix(']');

                    //list.DisablePrefixPostFix();
                    list.ExportToCSV(@"C:\Temp\data.csv");
                    //ret = data;
                }
                catch (Exception ex)
                {
                    ret = ex.Message;
                }

                return ret;
            }

            public static void Main()
            {
                ExportTest t = new ExportTest();
                Console.WriteLine(t.Test("LastName desc, FirstName asc"));
            }
        }

        public class Employee
        {
            [CSVExport("First Name")]
            public string FirstName { get; set; }
            [CSVExport("Last Name")]
            public string LastName { get; set; }
            public string Title { get; set; }
            [CSVExport("Hire Date")]
            public DateTime Date { get; set; }

            public Employee(string firstName, string lastName, string title, DateTime date)
            {
                this.FirstName = firstName;
                this.LastName = lastName;
                this.Title = title;
                this.Date = date;
            }

            public override string ToString()
            {
                return String.Format("Title: {0} - Name: {1}, {2} Date: {3}", Title, LastName, FirstName, Date);
            }
        }

        public class EmployeeList : List<Employee> { }
    }
}
