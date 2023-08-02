using System.Diagnostics;
using B1Task1.SQL;
using Microsoft.Data.SqlClient;

namespace B1Task1
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Menu();
            Console.ReadLine();
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
                            ThreadPool.InitializeThreads();
                            ThreadPool.StartAll();
                            ThreadPool.WaitAll();
                            sw.Stop();
                            Console.WriteLine($"Generating finished in {sw.ElapsedMilliseconds}ms");
                            sw.Reset();
                            break;
                        }
                        case 2:
                        {
                            Console.WriteLine("Enter the substring to delete");
                            string? substring = Console.ReadLine();
                            sw.Start();
                            ThreadPool.InitializeThreads(false, substring);
                            ThreadPool.StartAll();
                            ThreadPool.WaitAll();
                            sw.Stop();
                            Console.WriteLine($"Merging finished in {sw.ElapsedMilliseconds}ms " +
                                              $"and deleted {ThreadPool.DeletedRows} rows");
                            sw.Reset();
                            break;
                        }
                        case 3:
                        {
                            try
                            {
                                DatabaseSQL.ConnectToDb();
                                DatabaseSQL.CreateDb();
                            }
                            catch (SqlException e)
                            {
                                Console.WriteLine("DB" + e.Message);
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
            }
        }
        
    }
    
} 