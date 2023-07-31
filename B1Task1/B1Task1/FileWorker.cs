using System.Text;
using B1Task1.Data;

namespace B1Task1;

public static class FileWorker
{
    public static int RowsToGenerate = 100000;
    public static int RowsInOneTime = 10;
    public static IDataGenerator DataGenerator = new DataGenerator();
    
    public static void GenerateFile(object? index)
    {
        string filename = $"..\\..\\..\\files\\{(int)index}.txt";
        int j = 0;
        var sb = new StringBuilder();
        using (var file = File.Create(filename))
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

    public static void GenerateFiles(object? startingIndex, object? endingIndex)
    {
        for (int i = (int)startingIndex; i <= (int)endingIndex; i++)
        {
            GenerateFile(i);
        }
    }

    public static bool SetRowsInfo(int rowsToGenerate, int rowsInOneTime)
    {
        if (rowsInOneTime >= rowsToGenerate)
        {
            rowsInOneTime = rowsToGenerate;
            return true;
        }
        return rowsToGenerate % rowsInOneTime != 0;
    }
}