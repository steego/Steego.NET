using System;
using System.Collections.Generic;
using System.Linq;

namespace Steego.Trees
{
    public class ProjectedNode<T, U> : ITreeNode<U> {

        private readonly ITreeNode<T> Node;
        private readonly Func<T, U> Selector;
        public ProjectedNode(ITreeNode<T> node, Func<T, U> selector) {
            Node = node;
            Selector = selector;
        }

        public U Value => Selector(Node.Value);

        public IEnumerable<ITreeNode<U>> Children => 
            from c in Node.Children
            select new ProjectedNode<T, U>(c, Selector) as ITreeNode<U>;
    }
}