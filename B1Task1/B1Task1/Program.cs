using System;

namespace B1Task1
{
    internal class Program
    {
        static void Main(string[] args)
        {
            IGenerator generator = new Generator();
            Console.WriteLine(generator.GenerateDouble());
            Console.WriteLine(generator.GenerateInt());
            Console.WriteLine(generator.GenerateEnglishString());
            Console.WriteLine(generator.GenerateRussianString());
            Console.ReadLine();
        }
    }
}