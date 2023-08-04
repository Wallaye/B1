using System.Globalization;
using System.Runtime.InteropServices.JavaScript;
using System.Text;
using B1Task1.File;
using B1Task1.Models;
using B1Task1.Services;
using FileStream = System.IO.FileStream;

namespace B1Task1;

public static class ThreadPool
{
    /// <summary>
    /// Files to process
    /// </summary>
    public static int CountFiles = 100;
    /// <summary>
    /// Count of threads
    /// </summary>
    public static int CountThreads = 10;
    /// <summary>
    /// Thread array
    /// </summary>
    private static Thread[]? _threads;
    /// <summary>
    /// Stream writer for resulting file
    /// </summary>
    private static StreamWriter _streamWriter = StreamWriter.Null;
    /// <summary>
    /// File stream for resulting file
    /// </summary>
    private static FileStream? _fileStream;
    /// <summary>
    /// Synchronization object
    /// </summary>
    private static object _sync = new();
    
    public static volatile int ImportedRows = 0;
    public static volatile int AllRows = 0;
    public static long DeletedRows = 0;

    /// <summary>
    /// Initializing threads in thread pool
    /// </summary>
    /// <param name="op">operation to perform</param>
    /// <param name="substring">substring to delete</param>
    /// <param name="indexes">indexes of files to import</param>
    public static void InitializeThreads(Operation op = Operation.GenerateFiles, string? substring = null,
        int[]? indexes = null)
    {
        if (indexes?.Length < CountThreads && op == Operation.ImportFiles)
        {
            CountThreads = indexes.Length;
        }

        _threads = new Thread[CountThreads];
        int startingIndex = 1;
        int endingIndex = CountFiles / CountThreads;
        for (int i = 0; i < _threads.Length; i++)
        {
            int start = startingIndex;
            int end = endingIndex;
            if (i != _threads.Length - 1)
            {
                switch (op)
                {
                    case Operation.GenerateFiles:
                    {
                        _threads[i] = new Thread(() => FileGenerator.GenerateFiles(start, end));
                        break;
                    }
                    case Operation.MergeFiles:
                    {
                        _threads[i] = new Thread(() => ReadFilesAndDeleteSubstring(start, end, substring));
                        break;
                    }
                    case Operation.ImportFiles:
                    {
                        int j = 0;
                        int filePerThread = indexes.Length / CountThreads;
                        var coll = indexes.Skip(i * filePerThread).Take(filePerThread).ToArray();
                        _threads[i] = new Thread(() =>
                            ImportFilesInDb(coll));
                        break;
                    }
                }

                startingIndex = endingIndex + 1;
                endingIndex = startingIndex + CountFiles / CountThreads - 1;
            }
            //if last thread then delegate it all of files
            else
            {
                switch (op)
                {
                    case Operation.GenerateFiles:
                    {
                        _threads[i] = new Thread(() => FileGenerator.GenerateFiles(start, CountFiles));
                        break;
                    }
                    case Operation.MergeFiles:
                    {
                        _threads[i] = new Thread(() => ReadFilesAndDeleteSubstring(start, CountFiles, substring));
                        break;
                    }
                    case Operation.ImportFiles:
                    {
                        int filePerThread = indexes.Length / CountThreads;
                        var coll = indexes.Skip(i * filePerThread).ToArray();
                        _threads[i] = new Thread(() =>
                            ImportFilesInDb(coll));
                        break;
                    }
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
                //Locking streamwriter for single creation
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
        //If substring is null then use task for async reading
        Task<string>? getFileContentTask = null;
        var sb = new StringBuilder();
        int deletedRows = 0;
        try
        {
            using var sr = new StreamReader(filename);
            //reading file
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
                //Atomic update of deleted rows
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

    /// <summary>
    /// Start all threads
    /// </summary>
    public static void StartAll()
    {
        for (int i = 0; i < _threads?.Length; i++)
        {
            _threads[i].Start();
        }
    }

    /// <summary>
    /// Wait finishing of all threads and disposing FileStream and StreamWriter
    /// </summary>
    public static void WaitAllAndDisposeStreams()
    {
        for (int i = 0; i < _threads?.Length; i++)
        {
            _threads[i].Join();
        }

        DisposeStreams();
    }

    /// <summary>
    /// Disposing FileStream StreamWriter
    /// </summary>
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

    /// <summary>
    /// Reseting ThreadPool
    /// </summary>
    public static void Reset()
    {
        _threads = null;
        CountFiles = 100;
        CountThreads = 10;
        DeletedRows = 0;
        ImportedRows = 0;
        AllRows = 0;
    }

    /// <summary>
    /// Import files in Database
    /// </summary>
    /// <param name="indexes">indexes of files</param>
    public static void ImportFilesInDb(int[] indexes)
    {
        for (int i = 0; i < indexes.Length; i++)
        {
            ImportFile(indexes[i]);
        }
    }

    /// <summary>
    /// import single file
    /// </summary>
    /// <param name="index">index of file</param>
    private static void ImportFile(int index)
    {
        string filename = $".\\files\\{index}.txt";
        var reader = new StreamReader(filename);
        int rowsInFile = 0;
        //Count lines
        while (reader.ReadLine() != null)
        {
            rowsInFile++;
        }
            
        Interlocked.Add(ref AllRows, rowsInFile);
        //return File pos to the start of file.
        reader.DiscardBufferedData();
        reader.BaseStream.Seek(0, SeekOrigin.Begin);
        string? line;
        int rows = 0;
        int rowsImported = 0;

        StringBuilder sb = new();
        while ((line = reader.ReadLine()) != null)
        {
            //parse string
            var data = line.Split("||");
            DateOnly date = DateOnly.Parse(data[0]);
            string eng = data[1];
            string rus = data[2];
            int intValue = int.Parse(data[3]);
            double doubleValue = double.Parse(data[4]);
            Table table = new Table()
            {
                Date = date,
                EngString = eng,
                RusString = rus,
                DoubleValue = doubleValue,
                IntValue = intValue
            };
            //represent date for SQL query
            var str = $"{table.Date.Year}-{table.Date.Month}-{table.Date.Day}";

            //SQL query
            var sqlQuery =
                $"INSERT INTO [B1].[dbo].[Tables] (EngString, RusString, [Date], IntValue, DoubleValue) VALUES (" +
                $"'{table.EngString}', '{table.RusString}', '{str}', {table.IntValue}, {table.DoubleValue.ToString("G", CultureInfo.InvariantCulture)});";
            sb.AppendLine(sqlQuery);
            rows++;
            rowsImported++;

            //make 20 inserts at once
            if (rows % 20 == 0)
            {
                lock (_sync)
                {
                    TableService.ExecuteQuery(sb.ToString());
                }
                Interlocked.Add(ref ImportedRows, rows);
                rows = 0;
                sb.Clear();
            }
        }

        //perform inserts of remaining rows
        lock (_sync)
        {
            if (sb.Length != 0)
                TableService.ExecuteQuery(sb.ToString());
            Interlocked.Add(ref ImportedRows, rows);
            sb.Clear();
        }
    }
}