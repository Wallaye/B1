namespace B1Task1;

public static class ThreadPool
{
    private static volatile int indexFile = 1;
    public static int CountFiles = 100;
    public const int CountThreads = 10;
    private static Thread[] _threads = new Thread[CountThreads];

    public static void InitializeThreads()
    {
        int startingIndex = 1;
        int endingIndex = CountFiles / CountThreads;
        for (int i = 0; i < _threads.Length; i++)
        {
            if (i != _threads.Length - 1)
            {
                int start = startingIndex;
                int end = endingIndex;
                _threads[i] = new Thread(() => FileWorker.GenerateFiles(start, end));
                startingIndex = endingIndex + 1;
                endingIndex = startingIndex + CountFiles / CountThreads - 1;
            }
            else
            {
                _threads[i] = new Thread(() => FileWorker.GenerateFiles(startingIndex, CountFiles));
            }
        }
    }

    public static void StartGenerating()
    {
        for (int i = 0; i < _threads.Length; i++)
        {
            _threads[i].Start();
        }
    }
}