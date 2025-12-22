using System.Collections.Generic;
using UnityEngine;

namespace GloryDay.BehaviourTree
{
    public abstract class CompositeNode : Node
    {
        [HideInInspector] public List<Node> Children = new();

        public override Node Clone()
        {
            var clone = Instantiate(this);
            clone.Children = Children.ConvertAll(node => node.Clone());

            return clone;
        }

        public override string ToString()
        {
            return "composite";
        }
    }
}
