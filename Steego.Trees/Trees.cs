using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Steego.Trees
{
  public interface ITreeNode<T> {
    T value { get; }
    IEnumerable<ITreeNode<T>> children { get; }
  }

  public static class Trees {

    /// <summary>
    /// Projects transforms a tree of type T to type U
    /// </summary>
    /// <typeparam name="T">The type of the input tree</typeparam>
    /// <typeparam name="U">The type of the resulting transformed tree</typeparam>
    /// <param name="node">The tree to transform</param>
    /// <param name="selector">The transforming function</param>
    /// <returns></returns>
    public static ITreeNode<U> Select<T, U>(this ITreeNode<T> node, Func<T, U> selector) {
      return new ProjectedNode<T, U>(node, selector);
    }

    /// <summary>
    /// Filters a tree given a filtering function
    /// </summary>
    /// <typeparam name="T">The type of the input tree</typeparam>
    /// <param name="node">The tree node to be filtered</param>
    /// <param name="predicate">The filtering function</param>
    /// <returns></returns>
    public static ITreeNode<T> Where<T>(this ITreeNode<T> node, Func<T, Boolean> predicate) {
      return new FilteredNode<T>(node, predicate);
    }

    public static ITreeNode<T> GetChildren<T>(this ITreeNode<T> node, Func<T, IEnumerable<T>> f) {
      return null;
    }

    public static IEnumerable<U> Fold<T, U>(this ITreeNode<T> node, U seed, Func<U, T, U> combine) {
      return (from n in node.RecurseDepth(n => n.children)
              select n.value).Fold(seed, combine);
    }

    public static ITreeNode<T> Create<T>(T value) {
      return new SimpleTreeNode<T>(value);
    }

    public static ITreeNode<T> Create<T>(T value, Func<T, IEnumerable<T>> genChildren) {
      return new TreeNode<T>(value, genChildren);
    }

    public static ITreeNode<T> MaxDepth<T>(this ITreeNode<T> node, int max) {
      return new MaxTree<T>(node, max);
    }

    public static IEnumerable<T> FlattenBreadth<T>(this ITreeNode<T> root) {
      var ToGet = new Queue<ITreeNode<T>>();
      ToGet.Enqueue(root);
      while(ToGet.Any()) {
        var current = ToGet.Dequeue();
        yield return current.value;
        foreach(var next in current.children) {
          ToGet.Enqueue(next);
        }
      }
    }

    //  Todo:  Fix bug in flatten
    public static IEnumerable<T> Flatten<T>(this ITreeNode<T> root) {
      var cursors = new Stack<IEnumerator<ITreeNode<T>>>();
      var cursor = Enumerable.Repeat(root, 1).GetEnumerator();
      try {

        do {
          while(cursor.MoveNext()) {
            yield return cursor.Current.value;
            var subEnumerable = cursor.Current.children;
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

  public class FilteredNode<T> : ITreeNode<T> {

    private ITreeNode<T> node;
    private Func<T, Boolean> predicate;
    public FilteredNode(ITreeNode<T> node, Func<T, Boolean> predicate) {
      this.node = node;
      this.predicate = predicate;
    }

    public T value {
      get { return node.value; }
    }

    public IEnumerable<ITreeNode<T>> children {
      get {
        return from c in node.children
               where predicate(c.value)
               select new FilteredNode<T>(c, predicate) as ITreeNode<T>;
      }
    }
  }

  public class ProjectedNode<T, U> : ITreeNode<U> {

    private ITreeNode<T> node;
    private Func<T, U> selector;
    public ProjectedNode(ITreeNode<T> node, Func<T, U> selector) {
      this.node = node;
      this.selector = selector;
    }

    public U value {
      get { return selector(node.value); }
    }

    public IEnumerable<ITreeNode<U>> children {
      get {
        return from c in node.children
               select new ProjectedNode<T, U>(c, selector) as ITreeNode<U>;
      }
    }
  }

  public class SimpleTreeNode<T> : ITreeNode<T> {
    public SimpleTreeNode(T value) {
      this.value = value;
    }
    public T value { get; set; }
    public IEnumerable<ITreeNode<T>> children {
      get {
        return Enumerable.Empty<ITreeNode<T>>();
      }
    }
  }

  public class TreeNode<T> : ITreeNode<T> {
    private Func<T, IEnumerable<T>> genChildren;

    public TreeNode(T value, Func<T, IEnumerable<T>> genChildren) {
      this.value = value;
      this.genChildren = genChildren;
    }

    public T value { get; set; }
    public IEnumerable<ITreeNode<T>> children {
      get {
        if(genChildren == null)
          return Enumerable.Empty<ITreeNode<T>>();
        return from t in genChildren(value)
               select (new TreeNode<T>(t, genChildren)) as ITreeNode<T>;
      }
    }
  }

  public class MaxTree<T> : ITreeNode<T> {

    private ITreeNode<T> node;
    private int max;
    public MaxTree(ITreeNode<T> node, int max) {
      this.node = node;
      this.max = max;
    }

    public T value {
      get { return node.value; }
    }

    public IEnumerable<ITreeNode<T>> children {
      get {
        if(max <= 1)
          return Enumerable.Empty<ITreeNode<T>>();
        var nextLevel = max - 1;
        return (from c in node.children
                select new MaxTree<T>(c, nextLevel) as ITreeNode<T>);
      }
    }
  }

}
