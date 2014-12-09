using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Steego.Trees {
  public static class EnumerableExt {
    public static IEnumerable<U> Fold<T, U>(this IEnumerable<T> list, U seed, Func<U, T, U> combine) {
      var current = seed;
      foreach(var item in list) {
        current = combine(current, item);
        yield return current;
      }
    }
  }
}
