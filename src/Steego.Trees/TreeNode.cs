using System;
using System.Collections.Generic;
using System.Linq;

namespace Steego.Trees
{
    public class TreeNode<T> : ITreeNode<T> {
        private readonly Func<T, IEnumerable<T>> GenChildren;

        public TreeNode(T value, Func<T, IEnumerable<T>> genChildren) {
            Value = value;
            GenChildren = genChildren;
        }

        public T Value { get; }
        public IEnumerable<ITreeNode<T>> Children {
            get {
                if(GenChildren == null)
                    return Enumerable.Empty<ITreeNode<T>>();
                return from t in GenChildren(Value)
                    select (new TreeNode<T>(t, GenChildren)) as ITreeNode<T>;
            }
        }
    }
}