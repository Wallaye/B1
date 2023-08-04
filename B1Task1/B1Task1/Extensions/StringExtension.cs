namespace B1Task1.Extensions;

public static class StringExtension
{
    /// <summary>
    /// Parses string in array of indexes. Delimiter - space. Or can add range with using '-'.
    /// E.g. 1-5 10 35 - returns 1 2 3 4 5 10 35
    /// </summary>
    /// <param name="str">string to process</param>
    /// <param name="minNumber">minimal number to add</param>
    /// <param name="maxNumber">maximal number to add</param>
    /// <returns>return IEnumerable of indexes</returns>
    public static IEnumerable<int> GetIndexes(this string str, int minNumber, int maxNumber)
    {
        List<int> result = new();
        var values = str.Split(" ", StringSplitOptions.RemoveEmptyEntries);
        foreach (var value in values)
        {
            if (!value.Contains('-'))
            {
                if (int.TryParse(value, out int val) && val <= maxNumber && val >= minNumber)
                {
                    result.Add(val);
                }
            }
            else
            {
                var endIndexes = value.Split('-', StringSplitOptions.RemoveEmptyEntries);
                if (int.TryParse(endIndexes[0], out int start) && int.TryParse(endIndexes[1], out int end))
                {
                    for (int i = start; i <= end; i++)
                    {
                        if (i >= minNumber && i <= maxNumber)
                        {
                            result.Add(i);
                        }
                    }
                }
            }
        }
        return result.Distinct();
    }
}