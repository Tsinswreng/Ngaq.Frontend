// namespace Ngaq.Windows;

// using System;
// using System.Collections.Concurrent;
// using System.Diagnostics;
// using System.Threading.Tasks;

// [AttributeUsage(AttributeTargets.Method | AttributeTargets.Constructor)]
// public sealed class TimeEveryMethodAttribute : Attribute { }

// /* 下面这个类会被 Fody 织入到所有打了 TimeEveryMethodAttribute 的方法前后 */
// public static class MethodTimeLogger {
// 	// 线程安全队列，暂存结果
// 	private static readonly BlockingCollection<Entry> _queue = new();
// 	// 后台落盘线程
// 	static MethodTimeLogger() {
// 		Task.Run(() => {
// 			foreach (var e in _queue.GetConsumingEnumerable())
// 				Console.WriteLine($"{e.Timestamp:HH:mm:ss.fff}  {e.Method}  {e.Elapsed.TotalMilliseconds:0.00} ms");
// 		});
// 	}

// 	public static void Log(string methodFullName, long startTick) {
// 		_queue.Add(new Entry {
// 			Timestamp = DateTime.Now,
// 			Method = methodFullName,
// 			Elapsed = TimeSpan.FromTicks(Stopwatch.GetTimestamp() - startTick)
// 		});
// 	}

// 	private struct Entry {
// 		public DateTime Timestamp;
// 		public string Method;
// 		public TimeSpan Elapsed;
// 	}
// }
using System.Reflection;

namespace Ngaq.Windows;

public static class MethodTimeLogger{
	public static void Log(MethodBase methodBase, long milliseconds, string message){
		Console.WriteLine($"方法名:{methodBase.Name}  耗时:{milliseconds}");
	}
}
