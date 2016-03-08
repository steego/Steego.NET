using System.Collections.Generic;

namespace Steego.Trees
{
    public interface ITreeNode<T> {
        T Value { get; }
        IEnumerable<ITreeNode<T>> Children { get; }
    }
}