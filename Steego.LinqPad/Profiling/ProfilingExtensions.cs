using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Steego.LinqPad.Profiling {
  public static class ProfilingExtensions {
    public static void Profile<T>(this IEnumerable<T> sequence, int interval = 1000) {
      var watch = new System.Diagnostics.Stopwatch();
      watch.Start();
      var count = 0;
      foreach(var item in sequence) {
        count += 1;
        if(watch.ElapsedMilliseconds >= interval) {
          Console.WriteLine("Time: {0}, Count: {1}", DateTime.Now, count);
          count = 0;
          watch.Restart();
        }
      }
      Console.WriteLine("Profile all done");
    }
  }
}
