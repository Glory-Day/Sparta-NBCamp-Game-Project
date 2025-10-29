using System.Collections.Generic;
using Backend.Object.Character.Enemy;
using UnityEditor;
using UnityEngine;

[CreateAssetMenu()]
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
        node.name = type.Name;
        node.Guid = GUID.Generate().ToString();

#else

        var node = CreateInstance(type) as Node;
        node.name = type.Name;

#endif

#if UNITY_EDITOR

        Undo.RecordObject(this, "Behaviour Tree (CreateNode)");
        Nodes.Add(node);

#endif

#if UNITY_EDITOR

        if (!Application.isPlaying)
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

    public void AddChild(Node parent, Node child)
    {
        var decorator = parent as DecoratorNode;
        if (decorator)
        {
#if UNITY_EDITOR

            Undo.RecordObject(decorator, "Behaviour Tree (AddChild)");

#endif

            decorator.Child = child;

#if UNITY_EDITOR

            EditorUtility.SetDirty(decorator);

#endif
        }

        var rootNode = parent as RootNode;
        if (rootNode)
        {
#if UNITY_EDITOR

            Undo.RecordObject(rootNode, "Behaviour Tree (AddChild)");

#endif

            rootNode.Child = child;

#if UNITY_EDITOR

            EditorUtility.SetDirty(rootNode);

#endif
        }

        var composite = parent as CompositeNode;
        if (composite == false)
        {
            return;
        }

#if UNITY_EDITOR

        Undo.RecordObject(composite, "Behaviour Tree (AddChild)");

#endif

        composite.Children.Add(child);

#if UNITY_EDITOR

        EditorUtility.SetDirty(composite);

#endif
    }

    public void RemoveChild(Node parent, Node child)
    {
        DecoratorNode decorator = parent as DecoratorNode;
        if (decorator)
        {
#if UNITY_EDITOR

            Undo.RecordObject(decorator, "Behaviour Tree (RemoveChild)");

#endif

            decorator.Child = null;

#if UNITY_EDITOR

            EditorUtility.SetDirty(decorator);

#endif
        }

        RootNode rootNode = parent as RootNode;
        if (rootNode)
        {
#if UNITY_EDITOR

            Undo.RecordObject(rootNode, "Behaviour Tree (RemoveChild)");

#endif

            rootNode.Child = null;

#if UNITY_EDITOR

            EditorUtility.SetDirty(rootNode);

#endif
        }

        CompositeNode composite = parent as CompositeNode;
        if (composite)
        {
#if UNITY_EDITOR

            Undo.RecordObject(composite, "Behaviour Tree (RemoveChild)");

#endif

            composite.Children.Remove(child);

#if UNITY_EDITOR

            EditorUtility.SetDirty(composite);

#endif
        }
    }

    public List<Node> GetChildren(Node parent)
    {
        List<Node> children = new List<Node>();
        DecoratorNode decorator = parent as DecoratorNode;
        if (decorator && decorator.Child != null)
        {
            children.Add(decorator.Child);
        }

        RootNode rootNode = parent as RootNode;
        if (rootNode && rootNode.Child != null)
        {
            children.Add(rootNode.Child);
        }

        CompositeNode composite = parent as CompositeNode;
        if (composite)
        {
            return composite.Children;
        }

        return children;
    }

    public void Traverse(Node node, System.Action<Node> visiter)
    {
        if (node)
        {
            visiter.Invoke(node);
            var children = GetChildren(node);
            children.ForEach((n) => Traverse(n, visiter));
        }
    }
    public BehaviourTree Clone()
    {
        BehaviourTree tree = Instantiate(this);
        tree.RootNode = tree.RootNode.Clone();
        tree.Nodes = new List<Node>();
        Traverse(tree.RootNode, (n) => { tree.Nodes.Add(n); });
        return tree;
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
