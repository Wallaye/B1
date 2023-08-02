using System.Text;

namespace B1Task1.Data
{
    internal class DataGenerator : IDataGenerator
    {
        private Random _random = new();
        private const string RussianAlphabet = "абвгдеёжзийклмнопрстуфхцчшщъыьэюяАБВГДЕЁЖЗИЙКЛМНОПРСТУФХЦЧШЩЪЫЬЭЮЯ";
        private const string EnglishAlphabet = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ";
        private const int DaysInFiveYears = 365 * 5;
        
        public DateOnly GenerateDate()
        {
            var now = DateOnly.FromDateTime(DateTime.Now.AddYears(-5));
            var days = _random.Next(0, DaysInFiveYears);
            var result = now.AddDays(days);
            return result;
        }
        
        public string GenerateEnglishString()
        {
            StringBuilder sb = new StringBuilder(10);
            for (byte i = 0; i < 10;i++)
            {
                sb.Append(EnglishAlphabet.ElementAt(_random.Next(EnglishAlphabet.Length)));
            }
            return sb.ToString();
        }
        
        public string GenerateRussianString()
        {
            StringBuilder sb = new StringBuilder(10);
            for (byte i = 0; i < 10;i++)
            {
                sb.Append(RussianAlphabet.ElementAt(_random.Next(RussianAlphabet.Length)));
            }
            return sb.ToString();
        }

        public int GenerateInt()
        {
            return 2 * _random.Next(1, 50_000_000);
        }

        public double GenerateDouble()
        {
            return 1 + _random.NextDouble() * 19.0;
        }
    }
}
