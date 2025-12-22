using System.Collections.Generic;
using Backend.Object.Character.Enemy;
using UnityEngine;

#if UNITY_EDITOR

using UnityEditor;

#endif

namespace GloryDay.BehaviourTree
{
    public class BehaviourTree : ScriptableObject
    {
        public Node RootNode;
        public Node.State TreeState = Node.State.Running;
        public List<Node> Nodes = new List<Node>();
        public Blackboard blackboard = new Blackboard();

        public Node.State Update()
        {
            if (RootNode.mState == Node.State.Running)
            {
                TreeState = RootNode.Update();
            }

            return TreeState;
        }

        public Node CreateNode(System.Type type)
        {
#if UNITY_EDITOR

            var node = CreateInstance(type) as Node;
            if (node)
            {
                node.name = type.Name;
                node.Guid = GUID.Generate().ToString();
            }

#else

            var node = CreateInstance(type) as Node;
            if (node)
            {
                node.name = type.Name;
            }

#endif

#if UNITY_EDITOR

            Undo.RecordObject(this, "Behaviour Tree (CreateNode)");

#endif

            Nodes.Add(node);

#if UNITY_EDITOR

            if (Application.isPlaying == false)
            {
                AssetDatabase.AddObjectToAsset(node, this);
            }

            Undo.RegisterCreatedObjectUndo(node, "Behaviour Tree (CreateNode)");
            AssetDatabase.SaveAssets();

#endif

            return node;
        }

        public void DeleteNode(Node node)
        {
#if UNITY_EDITOR

            Undo.RecordObject(this, "Behaviour Tree (DeleteNode)");

#endif

            Nodes.Remove(node);

#if UNITY_EDITOR

            //AssetDatabase.RemoveObjectFromAsset(node);
            Undo.DestroyObjectImmediate(node);
            AssetDatabase.SaveAssets();

#endif
        }

        public static void AddChildNode(Node parent, Node child)
        {
            switch (parent)
            {
                case DecoratorNode decorator:
                {
#if UNITY_EDITOR

                    Undo.RecordObject(decorator, "Behaviour Tree (AddChild)");

#endif

                    decorator.Child = child;

#if UNITY_EDITOR

                    EditorUtility.SetDirty(decorator);

#endif

                    break;
                }
                case RootNode root:
                {
#if UNITY_EDITOR

                    Undo.RecordObject(root, "Behaviour Tree (AddChild)");

#endif

                    root.Child = child;

#if UNITY_EDITOR

                    EditorUtility.SetDirty(root);

#endif

                    break;
                }
                case CompositeNode compositor:
                {
#if UNITY_EDITOR

                    Undo.RecordObject(compositor, "Behaviour Tree (AddChild)");

#endif

                    compositor.Children.Add(child);

#if UNITY_EDITOR

                    EditorUtility.SetDirty(compositor);

#endif

                    break;
                }
            }
        }

        public static void RemoveChildNode(Node parent, Node child)
        {
            switch (parent)
            {
                case DecoratorNode decorator:
                {
#if UNITY_EDITOR

                    Undo.RecordObject(decorator, "Behaviour Tree (RemoveChild)");

#endif

                    decorator.Child = null;

#if UNITY_EDITOR

                    EditorUtility.SetDirty(decorator);

#endif

                    break;
                }
                case RootNode root:
                {
#if UNITY_EDITOR

                    Undo.RecordObject(root, "Behaviour Tree (RemoveChild)");

#endif

                    root.Child = null;

#if UNITY_EDITOR

                    EditorUtility.SetDirty(root);

#endif

                    break;
                }
                case CompositeNode compositor:
                {
#if UNITY_EDITOR

                    Undo.RecordObject(compositor, "Behaviour Tree (RemoveChild)");

#endif

                    compositor.Children.Remove(child);

#if UNITY_EDITOR

                    EditorUtility.SetDirty(compositor);

#endif

                    break;
                }
            }
        }

        public static List<Node> GetChildNodes(Node parent)
        {
            var children = new List<Node>();

            switch (parent)
            {
                case DecoratorNode decorator:
                    children.Add(decorator.Child);
                    break;
                case RootNode root:
                    children.Add(root.Child);
                    break;
                case CompositeNode compositor:
                    return compositor.Children;
            }

            return children;
        }

        public static void Traverse(Node child, System.Action<Node> visitor)
        {
            if (child is null)
            {
                return;
            }

            visitor.Invoke(child);

            var children = GetChildNodes(child);
            children.ForEach(node => Traverse(node, visitor));
        }

        public BehaviourTree Clone()
        {
            BehaviourTree clone = Instantiate(this);
            clone.RootNode = clone.RootNode.Clone();
            clone.Nodes = new List<Node>();

            Traverse(clone.RootNode, node => { clone.Nodes.Add(node); });

            return clone;
        }

        public void Bind(EnemyComponent agent)
        {
            Traverse(RootNode, node =>
            {
                node.tree = this;
                node.agent = agent;
                node.blackboard = blackboard;
            });
        }
    }
}
