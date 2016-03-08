using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Steego.Trees {
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
            return (from n in node.RecurseDepth(n => n.Children)
                    select n.Value).Fold(seed, combine);
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
            var toGet = new Queue<ITreeNode<T>>();
            toGet.Enqueue(root);
            while(toGet.Any()) {
                var current = toGet.Dequeue();
                yield return current.Value;
                foreach(var next in current.Children) {
                    toGet.Enqueue(next);
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
                        yield return cursor.Current.Value;
                        var subEnumerable = cursor.Current.Children;
                        var subEnumerator = subEnumerable?.GetEnumerator();
                        if(subEnumerator == null || !subEnumerator.MoveNext())
                            continue;
                        cursors.Push(cursor);
                        cursor = subEnumerator;
                    }
                    cursor.Dispose();
                    cursor = (cursors.Count > 0) ? cursors.Pop() : null;
                } while(cursor != null);
            } finally {
                foreach(var c in cursors) {
                    c.Dispose();
                }
                cursor?.Dispose();
            }
        }

    }
}
