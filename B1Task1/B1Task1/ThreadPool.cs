using System.Text;
using B1Task1.File;

namespace B1Task1;

public static class ThreadPool
{
    private const int CountFiles = 100;
    private const int CountThreads = 10;
    private static readonly Thread[] Threads = new Thread[CountThreads];
    private static StreamWriter _streamWriter = StreamWriter.Null;
    private static FileStream? _fileStream = null;

    public static long DeletedRows = 0;

    public static void InitializeThreads(bool generating = true, string? substring = null)
    {
        int startingIndex = 1;
        int endingIndex = CountFiles / CountThreads;
        for (int i = 0; i < Threads.Length; i++)
        {
            int start = startingIndex;
            int end = endingIndex;
            if (i != Threads.Length - 1)
            {
                if (generating)
                {
                    Threads[i] = new Thread(() => FileGenerator.GenerateFiles(start, end));
                }
                else
                {
                    Threads[i] = new Thread(() => ReadFilesAndDeleteSubstring(start, end, substring));
                }
                startingIndex = endingIndex + 1;
                endingIndex = startingIndex + CountFiles / CountThreads - 1;
            }
            else
            {
                if (generating)
                {
                    Threads[i] = new Thread(() => FileGenerator.GenerateFiles(start, CountFiles));
                }
                else
                {
                    Threads[i] = new Thread(() => FileGenerator.GenerateFiles(start, end));
                }
            }
        }
    }
    
    public static void ReadFilesAndDeleteSubstring(int start, int end, string? substring)
    {
        try
        {
            if (_streamWriter == StreamWriter.Null)
            {
                lock (_streamWriter)
                {
                    if (_fileStream == null)
                    {
                        _fileStream = new FileStream("..\\..\\..\\result.txt",
                            FileMode.Create, FileAccess.Write, FileShare.Write);
                    }
                    if (_streamWriter == StreamWriter.Null)
                    {
                        _streamWriter = new StreamWriter(_fileStream);
                    }
                }
            }
            for (int i = start; i <= end; i++)
            {
                ReadFileAndDeleteSubstring(i, substring);
            }
        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message);
        }
    }
    
    public static void ReadFileAndDeleteSubstring(int index, string? substring)
    {
        string filename = $"..\\..\\..\\files\\{index}.txt";
        Task<string>? getFileContentTask = null;
        var sb = new StringBuilder();
        int deletedRows = 0;
        try
        {
            using var sr = new StreamReader(filename);

            if (!string.IsNullOrWhiteSpace(substring))
            {
                string? line;
                while ((line = sr.ReadLine()) != null)
                {
                    if (!line.Contains(substring) && !string.IsNullOrWhiteSpace(line))
                    {
                        sb.AppendLine(line);
                    }
                    else
                    {
                        deletedRows++;
                    }
                }
                Interlocked.Add(ref DeletedRows, deletedRows);
            }
            else
            {
                getFileContentTask = sr.ReadToEndAsync();
            }
            lock (_streamWriter)
            {
                if (getFileContentTask != null)
                {
                    sb.Append(getFileContentTask.GetAwaiter().GetResult());
                }
                _streamWriter.Write(sb.ToString());
            }
        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message);
        }
        
    }
    
    public static void StartAll()
    {
        for (int i = 0; i < Threads.Length; i++)
        {
            Threads[i].Start();
        }
    }

    public static void WaitAll()
    {
        for (int i = 0; i < Threads.Length; i++)
        {
            Threads[i].Join();
        }
        DisposeStreams();
    }

    public static void DisposeStreams()
    {
        try
        {
            if (_streamWriter != StreamWriter.Null)
            {
                _streamWriter.Close();
                _streamWriter = StreamWriter.Null;
            }

            if (_fileStream != null) 
            {
                _fileStream.Close();
                _fileStream = null;
            }
            GC.Collect();
            GC.WaitForPendingFinalizers();
        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message);
        }
        
    }
}