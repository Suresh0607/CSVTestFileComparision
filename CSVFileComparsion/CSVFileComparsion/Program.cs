using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSVFileComparsion
{
    class Program
    {
        static void Main(string[] args)
        {
            Program program1 = new Program();
            string path = Directory.GetCurrentDirectory();
            Console.ReadLine();
            string csvfilePathOne = ConfigurationManager.AppSettings["csvfilePathOne"];
            string csvfilePathTwo = ConfigurationManager.AppSettings["csvfilePathTwo"];
            string resultfilePath = ConfigurationManager.AppSettings["resultfilePath"];

            bool filesAreSimilar = program1.CompareCsvFiles(csvfilePathOne, csvfilePathTwo, resultfilePath);
            if (!filesAreSimilar)
            {
                Console.WriteLine("Files are not Similar");
            }
            else
            {
                Console.WriteLine("Files are  Similar");
            }
            Console.ReadLine();
        }

        //Compare Length of Files 
        private bool CompareFileLength(string[] fileContent1, string[] fileContent2)
        {
            if (!fileContent1.Length.Equals(fileContent2.Length))
            {
                Console.WriteLine("Files Length are not Same");
                return false;
            }
            else
            {
                Console.WriteLine("Files Length are Same");
                return true;
            }
        }

        //Compare Column Length of Files 
        private bool CompareCSVFileColumnLength(string[] fileContent1, string[] fileContent2)
        {
            for (int i = 0; i < fileContent1.Length; ++i)
            {
                string[] columnsOne = fileContent1[i].Split(new char[] { ',' });
                string[] columnsTwo = fileContent2[i].Split(new char[] { ',' });

                if (!columnsOne.Length.Equals(columnsTwo.Length))
                {
                    Console.WriteLine("Files Total column number differs");
                    return false;
                }
            }
            Console.WriteLine("Files Total column number are same");
            return true;
        }

        //Compare Length of Files 
        private bool CompareCsvFiles(string filePathOne, string filePathTwo, string resultPath)
        {
            var csv = new StringBuilder();
            string[] fileContentsOne = File.ReadAllLines(filePathOne);
            string[] fileContentsTwo = File.ReadAllLines(filePathTwo);

            if (!CompareFileLength(fileContentsOne, fileContentsTwo))
                return false;

            if (!CompareCSVFileColumnLength(fileContentsOne, fileContentsTwo))
                return false;

            string[] columnshead1 = fileContentsOne[0].Split(new char[] { ';' });
            List<string> heading1 = new List<string>();
            Dictionary<string, string>[] dict1 = new Dictionary<string, string>[fileContentsOne.Length];
            Dictionary<string, string>[] dict2 = new Dictionary<string, string>[fileContentsTwo.Length];
            string[] headingsplit = columnshead1[0].Split(',');
            for (int i = 0; i < headingsplit.Length; i++)
            {
                heading1.Add(headingsplit[i]);
            }

            var newLine = "";
            newLine = string.Format("{0},{1},{2},{3},{4}", "File1_ColumnName", "File1_ColumnValue", "File2_ColumnName", "File2_ColumnValue", "RowNumber");
            csv.AppendLine(newLine);

            for (int i = 0; i < fileContentsOne.Length - 1; ++i)
            {
                string[] columnsOne = fileContentsOne[i + 1].Split(new char[] { ';' });
                string[] columnsTwo = fileContentsTwo[i + 1].Split(new char[] { ';' });

                string[] cellOne = columnsOne[0].Split(',');
                string[] cellTwo = columnsTwo[0].Split(',');
                dict1[i] = new Dictionary<string, string>();
                dict2[i] = new Dictionary<string, string>();
                for (int j = 0; j < headingsplit.Length; j++)
                {
                    dict1[i].Add(heading1[j], cellOne[j]);
                }
                for (int j = 0; j < headingsplit.Length; j++)
                {
                    dict2[i].Add(heading1[j], cellTwo[j]);
                }
                foreach (KeyValuePair<string, string> entry in dict1[i])
                {
                    if (dict2[i][entry.Key].Equals(entry.Value) != true)
                    {
                        Console.WriteLine("Mismatch Values on row " + i + 2 + ":\n File1 " + entry.Key + "-" + entry.Value + "\n File2 " + entry.Key + "-" + dict2[i][entry.Key]);
                        newLine = string.Format("{0},{1},{2},{3},{4}", entry.Key, entry.Value, entry.Key, dict2[i][entry.Key], i + 2);
                        csv.AppendLine(newLine);
                    }
                }
            }

            string reportFile = "Errorlist"+DateTime.Now.ToString().Replace("/", "_").Replace(":", "_").Replace(" ", "_") + ".csv";
            File.WriteAllText(resultPath+ reportFile, csv.ToString());
            return true;
        }
    }
}
