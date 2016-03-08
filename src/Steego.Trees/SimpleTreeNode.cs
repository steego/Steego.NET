using System.Collections.Generic;
using System.Linq;

namespace Steego.Trees
{
    public class SimpleTreeNode<T> : ITreeNode<T> {
        public SimpleTreeNode(T value) {
            Value = value;
        }
        public T Value { get; }
        public IEnumerable<ITreeNode<T>> Children => Enumerable.Empty<ITreeNode<T>>();
    }
}