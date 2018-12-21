using System;
using System.Collections.Generic;

namespace BinaryMan.Azure
{
    using System.Collections.Concurrent;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;

    public static class LinqExtensions
    {
        private class ProcessBag<TData>
        {
            public int Index;
            public TData Data;
        }

        public static IList<TTarget> ProcessInParallel<TSource, TTarget>(this IList<TSource> source,
            Func<TSource, TTarget> processor, int parallelNum)
        {
            var ret = new TTarget[source.Count];
            var queue = new ConcurrentQueue<ProcessBag<TSource>>();
            var cts = new CancellationTokenSource();
            var enqueueTask = Task.Run(() =>
            {
                for (var j = 0; j < source.Count && !cts.IsCancellationRequested; j++)
                {
                    queue.Enqueue(new ProcessBag<TSource> { Index = j, Data = source[j] });
                }
            }, cts.Token);

            var dequeueTasks = Enumerable.Range(0, parallelNum).Select(_ => Task.Run(() =>
            {
                while (!queue.IsEmpty && !cts.IsCancellationRequested)
                {
                    try
                    {
                        if (queue.TryDequeue(out var bag) && bag != null)
                        {
                            ret[bag.Index] = processor(bag.Data);
                        }
                    }
                    catch (Exception)
                    {
                        cts.Cancel();
                        throw;
                    }
                }
            },cts.Token));

            var tasks = dequeueTasks.Concat(new[] {enqueueTask}).ToArray();
            Task.WaitAll(tasks);

            return ret;
        }

        public static IList<TTarget> ProcessInParallel<TSource, TTarget>(this IList<TSource> source,
            Func<TSource, Task<TTarget>> processor, int parallelNum)
        {
            var ret = new TTarget[source.Count];
            var queue = new ConcurrentQueue<ProcessBag<TSource>>();
            var cts = new CancellationTokenSource();
            var enqueueTask = Task.Run(() =>
            {
                for (var j = 0; j < source.Count && !cts.IsCancellationRequested; j++)
                {
                    queue.Enqueue(new ProcessBag<TSource> { Index = j, Data = source[j] });
                }
            }, cts.Token);

            var dequeueTasks = Enumerable.Range(0, parallelNum).Select(_ => Task.Run(async () =>
            {
                while (!queue.IsEmpty && !cts.IsCancellationRequested)
                {
                    try
                    {
                        if (queue.TryDequeue(out var bag) && bag != null)
                        {
                            ret[bag.Index] = await processor(bag.Data);
                        }
                    }
                    catch (Exception)
                    {
                        cts.Cancel();
                        throw;
                    }
                }
            }, cts.Token));

            var tasks = dequeueTasks.Concat(new[] { enqueueTask }).ToArray();
            Task.WaitAll(tasks);

            return ret;
        }
    }
}
