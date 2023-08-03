using System.Text;
using B1Task1.File;

namespace B1Task1;

public static class ThreadPool
{
    public static int CountFiles = 100;
    public static int CountThreads = 10;
    private static Thread[]? _threads;
    private static StreamWriter _streamWriter = StreamWriter.Null;
    private static FileStream? _fileStream;
 
    public static long DeletedRows    = 0;

    public static void InitializeThreads(bool generating = true, string? substring = null)
    {
        _threads = new Thread[CountThreads];
        int startingIndex = 1;
        int endingIndex = CountFiles / CountThreads;
        for (int i = 0; i < _threads.Length; i++)
        {
            int start = startingIndex;
            int end = endingIndex;
            if (i != _threads.Length - 1)
            {
                if (generating)
                {
                    _threads[i] = new Thread(() => FileGenerator.GenerateFiles(start, end));
                }
                else
                {
                    _threads[i] = new Thread(() => ReadFilesAndDeleteSubstring(start, end, substring));
                }
                startingIndex = endingIndex + 1;
                endingIndex = startingIndex + CountFiles / CountThreads - 1;
            }
            else
            {
                if (generating)
                {
                    _threads[i] = new Thread(() => FileGenerator.GenerateFiles(start, CountFiles));
                }
                else
                {
                    _threads[i] = new Thread(() => ReadFilesAndDeleteSubstring(start, CountFiles, substring));
                }
            }
        }
    }
    
    private static void ReadFilesAndDeleteSubstring(int start, int end, string? substring)
    {
        try
        {
            if (_streamWriter == StreamWriter.Null)
            {
                lock (_streamWriter)
                {
                    if (_fileStream == null)
                    {
                        _fileStream = new FileStream(".\\result\\result.txt",
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
    
    private static void ReadFileAndDeleteSubstring(int index, string? substring)
    {
        string filename = $".\\files\\{index}.txt";
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
        for (int i = 0; i < _threads.Length; i++)
        {
            _threads[i].Start();
        }
    }

    public static void WaitAll()
    {
        for (int i = 0; i < _threads.Length; i++)
        {
            _threads[i].Join();
        }
        DisposeStreams();
    }

    private static void DisposeStreams()
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

    public static void Reset()
    {
        _threads = null;
        CountFiles = 100;
        CountThreads = 10;
        DeletedRows = 0;
    }
}