#if false
using System;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading;

namespace HEXPLAY
{
	public static class PerformanceMeter
	{
		static Stopwatch stopwatch = new Stopwatch();

		static long seed;
		static IntPtr saveProcessorAffinity;
		static ProcessPriorityClass savePriorityClass;
		static ThreadPriority savePriority;

		public static void Prepare()
		{
			seed = Environment.TickCount;  // Prevents the JIT Compiler from optimizing Fkt calls away

			saveProcessorAffinity = Process.GetCurrentProcess().ProcessorAffinity;
			savePriorityClass = Process.GetCurrentProcess().PriorityClass;
			savePriority = Thread.CurrentThread.Priority;

			Process.GetCurrentProcess().ProcessorAffinity = new IntPtr(2); // Uses the second Core or Processor for the Test
			Process.GetCurrentProcess().PriorityClass = ProcessPriorityClass.High; // Prevents "Normal" processes from interrupting Threads
			Thread.CurrentThread.Priority = ThreadPriority.Highest; // Prevents "Normal" Threads from interrupting this thread
		}

		public static void UnPrepare()
		{
			Process.GetCurrentProcess().ProcessorAffinity = saveProcessorAffinity;
			Process.GetCurrentProcess().PriorityClass = savePriorityClass;
			Thread.CurrentThread.Priority = savePriority;
		}

		public static void Sart(bool doPrepare = true, bool doWarmup = true)
		{
			if(doPrepare)
				Prepare();

			Warmup();

			stopwatch.Reset();
			stopwatch.Start();
		}

		public static long Stop(bool doUnPrepare = true, bool printElapsedTime = true)
		{
			stopwatch.Stop();

			if(printElapsedTime)
				Console.WriteLine("Ticks: " + stopwatch.ElapsedTicks + " ms: " + stopwatch.ElapsedMilliseconds);

			if(doUnPrepare)
				UnPrepare();

			return stopwatch.ElapsedMilliseconds;
		}

		public static long Warmup()
		{
			stopwatch.Reset();
			stopwatch.Start();

			long result = seed;
			int count = 100000000;

			while(stopwatch.ElapsedMilliseconds < 1200) // A Warmup of 1000-1500 ms stabilizes the CPU cache and pipeline.
				result = WarmupFunction(seed, count); // Warmup

			stopwatch.Stop();

			return result;
		}

		static long WarmupFunction(long seed, int count)
		{
			long result = seed;

			for(int i = 0; i < count; ++i)
				result ^= i ^ seed; // Some useless bit operations

			return result;
		}
	}
}
#endif