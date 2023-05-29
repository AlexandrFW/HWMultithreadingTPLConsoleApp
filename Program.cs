using System.Collections.Concurrent;
using System.Diagnostics;
using System.Threading;

namespace HWMultithreadingTPLConsoleApp;

internal class Program
{
    private static readonly Stopwatch stopwatch = new();

    private static List<long> listForProcessing = new();

    [STAThread]
    static void Main(string[] args)
    {
        Console.WriteLine("HW: Multithreading and TPL!");
        Console.WriteLine();

        FillListByInt(10000000);

        SumListItemsCommon();

        SumListItemsInParallelForEach();

        SumListItemsInThreads();

        SumListItemsUsingLINQ();

        Console.ReadKey();
    }

    private static void FillListByInt(long amountOfElements)
    {
        listForProcessing = ExtentionLong.Range(1, amountOfElements).ToList(); 
    }

    private static void SumListItemsCommon()
    {
        stopwatch.Reset();

        stopwatch.Start();

        long sumListItemsResult = 0;

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

    private static void SumListItemsInParallelForEach()
    {
        stopwatch.Reset();

        stopwatch.Start();

        long sumListItemsResult = 0;

        Parallel.ForEach(listForProcessing, i => Interlocked.Add(ref sumListItemsResult, i));

        var elapsed = stopwatch.Elapsed;

        stopwatch.Stop();

        Console.WriteLine($"Время, затраченное на подсчёт суммы элементов массива через Parallel.ForEach {elapsed.TotalMilliseconds}ms\r\n" +
                          $"Сумма элементов: {sumListItemsResult}");
        Console.WriteLine();
    }

    private static void SumListItemsInThreads()
    {
        object obj = new object();

        long totalSum = 0;
        int numberOfThreads = 10;

        var totalSumList = new ConcurrentBag<long>();

        var countDownEvent = new CountdownEvent(numberOfThreads);

        var firstIndex = 0;

        var rowLines = listForProcessing.Count;

        var rangeLength = rowLines / numberOfThreads;

        stopwatch.Reset();

        stopwatch.Start();

        for (int i = 1; i <= numberOfThreads; i++)
        {
            var endOfRange = firstIndex + rangeLength;

            if (endOfRange > rowLines)
                endOfRange = rowLines - 1;

            (int, int) tuple = (firstIndex, endOfRange);

            var thread = new Thread((randeParameters) => 
            {
                long currentSum = 0;
                
                (int minimum, int maximum) = ((int, int))randeParameters!;

                for (int j = minimum; j <= maximum; j++)
                {
                    currentSum += listForProcessing[j];
                }

                totalSumList.Add(currentSum);

                countDownEvent.Signal();
            });
            thread.Start(tuple);

            firstIndex = endOfRange + 1;
        }              

        countDownEvent.Wait();

        totalSum = totalSumList.Sum();

        var elapsed = stopwatch.Elapsed;

        stopwatch.Stop();

        Console.WriteLine($"Время, затраченное на подсчёт суммы элементов массива через Threads {elapsed.TotalMilliseconds}ms\r\n" +
                          $"Сумма элементов: {totalSum}");
        Console.WriteLine();
    }

    private static void SumListItemsUsingLINQ()
    {
        stopwatch.Reset();

        stopwatch.Start();

        long sumListItemsResult = 0;

        sumListItemsResult = listForProcessing.AsParallel().Select(x => x).Sum();

        var elapsed = stopwatch.Elapsed;

        stopwatch.Stop();

        Console.WriteLine($"Время, затраченное на подсчёт суммы элементов массива через LINQ {elapsed.TotalMilliseconds}ms\r\n" +
                          $"Сумма элементов: {sumListItemsResult}");
        Console.WriteLine();
    }
}

public static class ExtentionLong
{
    public static IEnumerable<long> Range(this long source, long length)
    {
        for (long i = source; i <= length; i++)
        {
            yield return i;
        }
    }
}