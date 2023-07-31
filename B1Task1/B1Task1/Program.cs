using System;
using B1Task1.Data;

namespace B1Task1
{
    internal class Program
    {
        static void Main(string[] args)
        {
            IGenerator generator = new Generator();
            for (int i = 0; i < 1000; i++)
            {
                Console.WriteLine(generator.GenerateDate());
            }
            Console.ReadLine();
        }
    }
}