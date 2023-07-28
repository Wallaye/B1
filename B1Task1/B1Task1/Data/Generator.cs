using System;
using System.Linq;
using System.Text;

namespace B1Task1
{
    internal class Generator : IGenerator
    {
        private Random _random = new();
        private readonly string _russianAlphabet = "абвгдеёжзийклмнопрстуфхцчшщъыьэюяАБВГДЕЁЖЗИЙКЛМНОПРСТУФХЦЧШЩЪЫЬЭЮЯ";
        private readonly string _englishAlphabet = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ";
        
        public DateOnly GenerateDate()
        {
            throw new NotImplementedException();
        }
        
        public string GenerateEnglishString()
        {
            StringBuilder sb = new StringBuilder(10);
            for (byte i = 0; i < 10;i++)
            {
                sb.Append(_englishAlphabet.ElementAt(_random.Next(_englishAlphabet.Length)));
            }
            return sb.ToString();
        }
        
        public string GenerateRussianString()
        {
            StringBuilder sb = new StringBuilder(10);
            for (byte i = 0; i < 10;i++)
            {
                sb.Append(_russianAlphabet.ElementAt(_random.Next(_russianAlphabet.Length)));
            }
            return sb.ToString();
        }

        public int GenerateInt()
        {
            return _random.Next();
        }

        public double GenerateDouble()
        {
            return 1 + _random.NextDouble() * 19.0;
        }
    }
}
