namespace B1Task1;

public static class ThreadPool
{
    private const int CountFiles = 100;
    private const int CountThreads = 10;
    private static readonly Thread[] Threads = new Thread[CountThreads];

    public static void InitializeThreads()
    {
        int startingIndex = 1;
        int endingIndex = CountFiles / CountThreads;
        for (int i = 0; i < Threads.Length; i++)
        {
            int start = startingIndex;
            int end = endingIndex;
            if (i != Threads.Length - 1)
            {
                Threads[i] = new Thread(() => FileWorker.GenerateFiles(start, end));
                startingIndex = endingIndex + 1;
                endingIndex = startingIndex + CountFiles / CountThreads - 1;
            }
            else
            {
                Threads[i] = new Thread(() => FileWorker.GenerateFiles(start, CountFiles));
            }
        }
    }

    public static void StartGenerating()
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
    }
}