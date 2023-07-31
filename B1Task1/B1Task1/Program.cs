using System;
using System.Diagnostics;
using B1Task1.Data;

namespace B1Task1
{
    internal class Program
    {
       //private static volatile int indexOfFile = 1;
        static void Main(string[] args)
        {
            
            Stopwatch stopwatch = new Stopwatch();
            ThreadPool.InitializeThreads();
            stopwatch.Start();
            ThreadPool.StartGenerating();
            ThreadPool.WaitAll(); 
            stopwatch.Stop();
            Console.WriteLine(stopwatch.ElapsedMilliseconds);
            Console.ReadLine();
        }
    }
}