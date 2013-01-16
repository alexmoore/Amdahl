using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace Amdahl
{
    /// <summary>
    /// A Glossed-Over Demonstration of Amdahl's Law in C# For a StackOverflow question
    /// Author: Alex Moore
    /// </summary>
    class Program
    {
        static void Main(string[] args)
        {
            var degreesOfParallelism = GetDegreesOfParallelism();


            // Single Threaded Run

            var stopwatch = new Stopwatch();
            var serialWorkLoad = new List<Action> { DoHeavyWork, DoHeavyWork };
            var parallelizableWorkLoad = new List<Action> { DoHeavyWork, DoHeavyWork, DoHeavyWork, DoHeavyWork, DoHeavyWork, DoHeavyWork, DoHeavyWork, DoHeavyWork };


            //----- Serial Run -----//
            stopwatch.Start();
            // Run Serial-only batch of work
            foreach (var serialWork in serialWorkLoad)
            {
                serialWork();
            }

            var s1 = stopwatch.ElapsedMilliseconds;

            // Run parallelizable batch of work in serial to get our baseline
            foreach (var notParallelWork in parallelizableWorkLoad)
            {
                notParallelWork();
            }

            stopwatch.Stop();
            var s2 = stopwatch.ElapsedMilliseconds - s1;

            stopwatch.Reset();


            //----- Parallel Run -----//
            stopwatch.Start();
            // Run Serial-only batch of work
            foreach (var serialWork in serialWorkLoad)
            {
                serialWork();
            }

            var p1 = stopwatch.ElapsedMilliseconds;

            // Run parallelizable batch of work in with as many degrees of parallelism as we specified
            Parallel.ForEach(
                parallelizableWorkLoad,
                new ParallelOptions
                    {
                        MaxDegreeOfParallelism = degreesOfParallelism
                    },
                (workToDo) => workToDo());

            stopwatch.Stop();
            var p2 = stopwatch.ElapsedMilliseconds - p1;

            var speedup = (double)(s1 + s2) / (p1 + p2);

            Console.WriteLine("Serial took  : {2}ms, {0}ms for serial work and {1}ms for parallelizable work", s1, s2, s1 + s2);
            Console.WriteLine("Parallel took: {2}ms, {0}ms for serial work and {1}ms for parallelizable work", p1, p2, p1 + p2);
            Console.WriteLine("Speedup was {0:F}x", speedup);
            Console.WriteLine();
            Console.WriteLine("Press any key to exit");

            Console.ReadKey();
        }

        private static int GetDegreesOfParallelism()
        {
            var numberOfLogicalProcessors = Environment.ProcessorCount;

            // Get number of processors you want to use from user
            Console.Write("Enter the number of processors you want to use (1 to {0}, or press <enter> for {0}):",
                          numberOfLogicalProcessors);

            var stringDegreeOfParallelism = Console.ReadLine();
            int degreeOfParallelism;

            if (string.IsNullOrWhiteSpace(stringDegreeOfParallelism) ||
                !int.TryParse(stringDegreeOfParallelism, out degreeOfParallelism) ||
                degreeOfParallelism > numberOfLogicalProcessors)
            {
                degreeOfParallelism = numberOfLogicalProcessors;
            }

            return degreeOfParallelism;
        }


        static void DoHeavyWork()
        {
            Thread.Sleep(500);
        }
    }
}
