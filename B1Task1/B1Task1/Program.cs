using System.Diagnostics;
using B1Task1.Extensions;
using B1Task1.Models;
using B1Task1.Services;
using Microsoft.Data.SqlClient;

namespace B1Task1
{
    internal class Program
    {
        static void Main(string[] args)
        {
            if (!Directory.Exists(".\\files"))
            {
                Directory.CreateDirectory(".\\files");
            }

            if (!Directory.Exists(".\\result"))
            {
                Directory.CreateDirectory(".\\result");
            }

            Menu();
        }

        static void Menu()
        {
            bool exit = false;
            int choice = 0;
            Stopwatch sw = new();
            while (!exit)
            {
                try
                {
                    Console.WriteLine("1. Generate Files");
                    Console.WriteLine("2. Merge files with deleting substring");
                    Console.WriteLine("3. Import in DB");
                    Console.WriteLine("4. Exit");
                    Int32.TryParse(Console.ReadLine(), out choice);
                    switch (choice)
                    {
                        case 1:
                        {
                            sw.Start();
                            ThreadPool.InitializeThreads(Operation.GenerateFiles);
                            ThreadPool.StartAll();
                            ThreadPool.WaitAll();
                            sw.Stop();
                            Console.WriteLine($"Generating finished in {sw.ElapsedMilliseconds}ms");
                            break;
                        }
                        case 2:
                        {
                            Console.WriteLine("Enter the substring to delete");
                            string? substring = Console.ReadLine();
                            sw.Start();
                            ThreadPool.InitializeThreads(Operation.MergeFiles, substring);
                            ThreadPool.StartAll();
                            ThreadPool.WaitAll();
                            sw.Stop();
                            Console.WriteLine($"Merging finished in {sw.ElapsedMilliseconds}ms " +
                                              $"and deleted {ThreadPool.DeletedRows} rows");
                            break;
                        }
                        case 3:
                        {
                            try
                            {
                                Console.WriteLine(
                                    "Choose files to import(delimiter space or enter range in format *-*(e.g. 1-5 12 43 50-55))");
                                string? str = Console.ReadLine();
                                sw.Start();
                                if (!string.IsNullOrWhiteSpace(str))
                                {
                                    var indexes = str.GetIndexes(1, 100);
                                    if (indexes?.Count() != 0)
                                    {
                                        ThreadPool.InitializeThreads(Operation.ImportFiles, indexes: indexes.ToArray());
                                        ThreadPool.StartAll();
                                        while (ThreadPool.ImportedRows < ThreadPool.AllRows ||
                                               ThreadPool.AllRows == 0)
                                        {
                                            Console.Write(
                                                $"{ThreadPool.ImportedRows}/{ThreadPool.AllRows}(remaining: {ThreadPool.AllRows - ThreadPool.ImportedRows})\r");
                                            Thread.Sleep(200);
                                        }
                                        ThreadPool.WaitAll();
                                        sw.Stop();
                                        Console.WriteLine($"Importing {ThreadPool.ImportedRows} finished in {sw.ElapsedMilliseconds} ms");
                                    }
                                }
                                else
                                {
                                    Console.WriteLine("Enter the valid string");
                                }
                                sw.Stop();
                            }
                            catch (SqlException e)
                            {
                                Console.WriteLine("DB exc: " + e.Message);
                            }

                            break;
                        }
                        case 4:
                        {
                            exit = true;
                            break;
                        }
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                }
                finally
                {
                    sw.Reset();
                    ThreadPool.Reset();
                }
            }
        }
    }
}