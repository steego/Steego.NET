using System.Collections.Generic;
using System.Linq;

namespace Steego.Trees
{
    public class MaxTree<T> : ITreeNode<T> {

        private readonly ITreeNode<T> Node;
        private readonly int Max;
        public MaxTree(ITreeNode<T> node, int max) {
            Node = node;
            Max = max;
        }

        public T Value => Node.Value;

        public IEnumerable<ITreeNode<T>> Children {
            get {
                if(Max <= 1)
                    return Enumerable.Empty<ITreeNode<T>>();
                var nextLevel = Max - 1;
                return (from c in Node.Children
                    select new MaxTree<T>(c, nextLevel) as ITreeNode<T>);
            }
        }
    }
}