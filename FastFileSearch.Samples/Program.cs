using System;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace FastFileSearch.Samples
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Count() != 1)
            {
                Console.WriteLine(@"Please set path to search duplicates (sample C:\temp)");
                Console.ReadKey();
                Environment.Exit(0);
            }

            var folder = args.First();

            if (!Directory.Exists(folder))
            {
                Console.WriteLine($"folder [{folder}] does not exists");
                Console.ReadKey();
                Environment.Exit(0);
            }

            var watch = new Stopwatch();

            watch.Start();
            var searchService = new WinApiSearch();
            try
            {
                var files = searchService.RecursiveScan(folder);
                foreach (var file in files)
                {
                    Console.WriteLine(file.Path);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
            

            watch.Stop();
            Console.WriteLine($"Search was done for {watch.Elapsed}");
            

            Console.ReadKey();
        }
    }
}
