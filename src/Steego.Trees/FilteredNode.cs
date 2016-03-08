using System;
using System.Collections.Generic;
using System.Linq;

namespace Steego.Trees
{
    public class FilteredNode<T> : ITreeNode<T> {

        private readonly ITreeNode<T> Node;
        private readonly Func<T, bool> Predicate;
        public FilteredNode(ITreeNode<T> node, Func<T, bool> predicate) {
            Node = node;
            Predicate = predicate;
        }

        public T Value => Node.Value;

        public IEnumerable<ITreeNode<T>> Children => 
            from c in Node.Children
            where Predicate(c.Value)
            select new FilteredNode<T>(c, Predicate) as ITreeNode<T>;
    }
}