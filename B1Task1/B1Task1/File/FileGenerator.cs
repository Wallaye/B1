using System.Text;
using B1Task1.Data;

namespace B1Task1.File;

public static class FileGenerator
{
    public static int RowsToGenerate = 100000;

    private static int _rowsInOneTime = 10;
    public static int RowsInOneTime
    {
        get => _rowsInOneTime;
        set {
            if (value < 0)
            {
                _rowsInOneTime = 1;
            }
            else if (value > RowsToGenerate)
            {
                _rowsInOneTime = RowsToGenerate;
            }
            else
            {
                _rowsInOneTime = value;
            }
        }
    }
    public static IDataGenerator DataGenerator = new DataGenerator();
    
    private static void GenerateFile(int index)
    {
        try
        {
            string filename = $".\\files\\{index}.txt";
            int j = 0;
            var sb = new StringBuilder();
            using (var file = System.IO.File.Create(filename))
            {
                using (var sw = new StreamWriter(file))
                {
                    for (int i = 0; i < RowsToGenerate / _rowsInOneTime; i++)
                    {
                        sb.Clear();
                        for (j = 0; j < _rowsInOneTime; j++)
                        {
                            var date = DataGenerator.GenerateDate();
                            var englishString = DataGenerator.GenerateEnglishString();
                            var russianString = DataGenerator.GenerateRussianString();
                            var intNum = DataGenerator.GenerateInt();
                            var floatNum = DataGenerator.GenerateDouble();
                            sb.Append($"{date}||{englishString}||{russianString}||{intNum}||{floatNum}\n");
                        }
                        sw.Write(sb.ToString());
                    }
                }
            }
        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message);
        }
    }

    public static void GenerateFiles(int startingIndex, int endingIndex)
    {
        for (int i = startingIndex; i <= endingIndex; i++)
        {
            GenerateFile(i);
        }
    }
}