using System.Text;
using B1Task1.File.Data;

namespace B1Task1.File;

public static class FileGenerator
{
    public static int RowsToGenerate = 100000;
    public static int RowsInOneTime = 10;
    public static IDataGenerator DataGenerator = new DataGenerator();
    
    public static void GenerateFile(int index)
    {
        string filename = $"..\\..\\..\\files\\{index}.txt";
        int j = 0;
        var sb = new StringBuilder();
        using (var file = System.IO.File.Create(filename))
        {
            using (var sw = new StreamWriter(file))
            {
                for (int i = 0; i < RowsToGenerate / RowsInOneTime; i++)
                {
                    sb.Clear();
                    for (j = 0; j < RowsInOneTime; j++)
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

    public static void GenerateFiles(int startingIndex, int endingIndex)
    {
        for (int i = startingIndex; i <= endingIndex; i++)
        {
            GenerateFile(i);
        }
    }
}