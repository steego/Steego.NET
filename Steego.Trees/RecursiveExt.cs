using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Steego.Trees {
  public static class RecursiveExt {

    public static IEnumerable<T> AtLongest<T>(this IEnumerable<T> source, TimeSpan max) {
      var start = DateTime.Now;

      foreach(var element in source) {
        var now = DateTime.Now.Subtract(start);
        if(now > max)
          break;
        yield return element;
      }
    }

    public static IEnumerable<T> RecurseBreadth<T>(this T root, Func<T, IEnumerable<T>> getMore) {
      var ToGet = new Queue<T>();
      ToGet.Enqueue(root);
      while(ToGet.Any()) {
        var current = ToGet.Dequeue();
        yield return current;
        foreach(var next in getMore(current)) {
          ToGet.Enqueue(next);
        }
      }
    }

    public static IEnumerable<T> RecurseDepth<T>(this T root, Func<T, IEnumerable<T>> getMore) {
      var cursors = new Stack<IEnumerator<T>>();
      var cursor = Enumerable.Repeat(root, 1).GetEnumerator();
      try {

        do {
          while(cursor.MoveNext()) {
            yield return cursor.Current;
            var subEnumerable = getMore(cursor.Current);
            var subEnumerator = (subEnumerable != null) ? subEnumerable.GetEnumerator() : null;
            if(subEnumerator != null && subEnumerator.MoveNext()) {
              cursors.Push(cursor);
              cursor = subEnumerator;
            }
          }
          cursor.Dispose();
          cursor = (cursors.Count > 0) ? cursors.Pop() : null;
        } while(cursor != null);
      } finally {
        foreach(var c in cursors) {
          c.Dispose();
        }
        if(cursor != null)
          cursor.Dispose();
      }
    }

  } 
}
