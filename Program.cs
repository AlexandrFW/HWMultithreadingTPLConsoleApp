using System.Diagnostics;

namespace HWMultithreadingTPLConsoleApp;

internal class Program
{
    private static readonly Stopwatch stopwatch = new();

    private static List<int> listForProcessing = new();

    private static int sumArrayElements = 0;

    [STAThread]
    static void Main(string[] args)
    {
        Console.WriteLine("HW: Multithreading and TPL!");
        Console.WriteLine();

        FillListByInt(10000);

        SumListItemsCommon();

        SumListItemsInThreads();

        SumListItemsUsingLINQ();

        Console.ReadKey();
    }

    private static void FillListByInt(int amountOfElements)
    {
        listForProcessing.Clear();

        listForProcessing = Enumerable.Range(1, amountOfElements).ToList();
    }

    private static void SumListItemsCommon()
    {
        stopwatch.Reset();

        stopwatch.Start();

        int sumListItemsResult = 0;
        for (int i = 0; i < listForProcessing.Count; i++)
        {
            sumListItemsResult += listForProcessing[i]; 
        }

        var elapsed = stopwatch.Elapsed;        

        stopwatch.Stop();

        Console.WriteLine($"Время, затраченное на обычный подсчёт суммы элементов массива {elapsed.TotalMilliseconds}ms\r\n" +
                          $"Сумма элементов: {sumListItemsResult}");
        Console.WriteLine();
    }

    private static void SumListItemsInThreads()
    {
        stopwatch.Reset();

        stopwatch.Start();

        var sumListItemsResult = 0;

        Parallel.ForEach(listForProcessing, i => Interlocked.Add(ref sumListItemsResult, i));

        var elapsed = stopwatch.Elapsed;

        stopwatch.Stop();

        Console.WriteLine($"Время, затраченное на подсчёт суммы элементов массива через Threads {elapsed.TotalMilliseconds}ms\r\n" +
                          $"Сумма элементов: {sumListItemsResult}");
        Console.WriteLine();
    }

    private static void SumListItemsUsingLINQ()
    {
        stopwatch.Reset();

        stopwatch.Start();

        var sumListItemsResult = listForProcessing.AsParallel().Sum();

        var elapsed = stopwatch.Elapsed;

        stopwatch.Stop();

        Console.WriteLine($"Время, затраченное на подсчёт суммы элементов массива через LINQ {elapsed.TotalMilliseconds}ms\r\n" +
                          $"Сумма элементов: {sumListItemsResult}");
        Console.WriteLine();
    }  


}