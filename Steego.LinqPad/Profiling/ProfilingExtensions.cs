using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Steego.LinqPad.Profiling {
  public static class ProfilingExtensions {
    public static IEnumerable<T> Profile<T>(this IEnumerable<T> sequence, string name, int interval = 1000) {
      var watch = new System.Diagnostics.Stopwatch();
      watch.Start();
      var count = 0;
      foreach(var item in sequence) {
        count += 1;
        yield return item;
        if(watch.ElapsedMilliseconds >= interval) {
          Console.WriteLine("Name: {0}, Time: {1}, Count: {2}", name, DateTime.Now, count);
          count = 0;
          watch.Restart();
        }
      }
      Console.WriteLine("Name: {0}, Profile all done", name);
    }
  }
}
