using System.Diagnostics;

namespace B1Task1.File
{
    internal class Program
    {
        static void Main(string[] args)
        {
            ThreadPool.InitializeThreads(false, "e");
            var sw = new Stopwatch();
            sw.Start();
            ThreadPool.StartAll();
            ThreadPool.WaitAll();
            sw.Stop();
            Console.WriteLine($"deleted rows ${ThreadPool.DeletedRows}");
            Console.WriteLine("Ended");
            Console.WriteLine(sw.ElapsedMilliseconds);
            Console.ReadLine();
        }
    }
} 