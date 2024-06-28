using System;
using System.Text;
using System.IO;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Linq;

namespace BusiestPeriod
{
    class Program
    {
        static List<string>? validBreakTimes = new List<string>(); 

        static void Main(string[] args)
        {
            FileMode(args);

            while (true)
            {
                bool isNullOrEmpty = validBreakTimes?.Any() != true; 
                if (!isNullOrEmpty)
                {
                    (string busiestTime, int driversAmount) = CalculateBusiestTime(validBreakTimes);   
                    Console.WriteLine($"The busiest time is: {busiestTime} where {driversAmount} drivers are taking a break");
                }

                Console.WriteLine("Enter time in HH:mmHH:mm format or 'q' to quit and then press ENTER: ");
                bool isValid = false;
                string input = Console.ReadLine()?? "";

                if (input.ToLower() == "q")
                {
                    Environment.Exit(0); // Exit gracefully
                }

                isValid = CheckTimeFormat(input);

                if (!isValid)
                {
                    Console.WriteLine("Invalid time. Please try again.");
                } 
                else
                {
                    validBreakTimes?.Add(input);
                }
            }
        }

        //Checks if the program should access a file
        static bool FileMode(string[] args)
        { 
            
            bool isFileMode = false;
            bool isFileExists = false;
            
            foreach (var arg in args)
            {
                if (arg.StartsWith("filename"))
                {
                    isFileMode = true;
                    break;
                }
            }

            if (args.Length > 2)
            {
                Console.WriteLine($"Too many arguments. File was not read.");
                return false;
            }
            
            if (isFileMode)
            {
                if (args.Length < 2)
                {
                    Console.WriteLine($"Filepath was not provided.");
                    return false;
                }
                string filePath = args[1];
                isFileExists = File.Exists(filePath);

                if (isFileExists)
                { 
                    ReadFile(filePath);
                    return true;
                }

                else
                {  
                    Console.WriteLine($"The file does not exist: {filePath}");
                    return false;
                }
            }
            else
            { 
                return false;
            }
        }
        
        //Reads the file from the provided filepath
        static void ReadFile(string filePath)
        {
            IEnumerable<string> fileContents = File.ReadLines(filePath);

            int index = 1;
            foreach (string line in fileContents)
            {
                if (CheckTimeFormat(line))
                {
                    validBreakTimes?.Add(line);
                }
                else
                {
                    Console.WriteLine($"The breaktime {line} on line {index} is in the wrong format. Only acceptable format is HH:mmHH:mm");
                    
                }
                
                index++;
            }
        }
        
        //Checks if the time is in correct format
        static bool CheckTimeFormat(string timeString)
        {
            //This regex only matches this format: 23:3011:35
            string pattern = @"^((0[0-9]|1[0-9]|2[0-3]):([0-5][0-9]))((0[0-9]|1[0-9]|2[0-3]):([0-5][0-9]))$";
            Regex regex = new Regex(pattern);
            return regex.IsMatch(timeString);
        }

        //calculates the time when the most drivers are on break
        static (string, int) CalculateBusiestTime(List<string>? breakTimes)
        {
            if (breakTimes == null || breakTimes.Count < 1) return ("No break times were provided", 0);

            int driversOnBreak = 0;
            int maxDriversOnBreak = 0;
            int maxIndex = 0;
            string busiestTime = "";
            
            List<(string, bool)> sortedTimes = SortTimes(breakTimes);            

            for (int i = 0; i < sortedTimes.Count; i++)
            {
                if (sortedTimes[i].Item2 == true)
                {
                    driversOnBreak++;
                    if (driversOnBreak > maxDriversOnBreak)
                    {
                        maxIndex = i;
                        maxDriversOnBreak = driversOnBreak;
                    }
                }
                else
                {
                    driversOnBreak--;
                }
                     
            }
            busiestTime = $"{sortedTimes[maxIndex].Item1}{sortedTimes[maxIndex + 1].Item1}";

            return (busiestTime, maxDriversOnBreak);

        }
        //sorts the times from earliest to latest
        static List<(string, bool)> SortTimes(List<string> times) 
        {
            List<(string Time, bool IsStart)> timeFlags = new List<(string, bool)>();

            // Populate the new list
            for (int i = 0; i < times.Count; i ++)
            {
                var startTime = times[i].Substring(0, 5);
                var endTime = times[i].Substring(5, 5);
                timeFlags.Add((startTime, true));
                timeFlags.Add((endTime, false));
            }

            timeFlags.Sort((a, b) => TimeSpan.Parse(a.Time).CompareTo(TimeSpan.Parse(b.Time)));

            var sortedTimes = timeFlags.Select(tf => tf.IsStart? $"({tf.Time}) Start" : $"({tf.Time}) End").ToList();

            return timeFlags;
        }


    }
}

