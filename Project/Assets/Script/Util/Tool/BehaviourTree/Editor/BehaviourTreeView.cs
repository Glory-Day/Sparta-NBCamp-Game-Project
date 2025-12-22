#if UNITY_EDITOR

using UnityEngine.UIElements;
using UnityEditor.Experimental.GraphView;
using UnityEditor;
using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;

namespace GloryDay.BehaviourTree.Editor
{
    public class BehaviourTreeView : GraphView
    {
        public new class UxmlFactory : UxmlFactory<BehaviourTreeView, UxmlTraits> { }

        private BehaviourTree _tree;

        public Action<NodeView> OnNodeSelected;

        public BehaviourTreeView()
        {
            Insert(0, new GridBackground());

            this.AddManipulator(new ContentZoomer());
            this.AddManipulator(new ContentDragger());
            this.AddManipulator(new SelectionDragger());
            this.AddManipulator(new RectangleSelector());

            var styleSheet = AssetDatabase.LoadAssetAtPath<StyleSheet>("Assets/Script/Util/Tool/BehaviourTree/Style/BehaviourTreeEditor.uss");
            styleSheets.Add(styleSheet);

            Undo.undoRedoPerformed += OnUndoRedoPerformed;
        }

        private void OnUndoRedoPerformed()
        {
            PopulateView(_tree);

            AssetDatabase.SaveAssets();
        }

        private NodeView FindNodeView(Node node)
        {
            return GetNodeByGuid(node.Guid) as NodeView;
        }

        internal void PopulateView(BehaviourTree tree)
        {
            _tree = tree;

            graphViewChanged -= OnGraphViewChanged;
            DeleteElements(graphElements);
            graphViewChanged += OnGraphViewChanged;

            if (tree.RootNode is null)
            {
                tree.RootNode = tree.CreateNode(typeof(RootNode)) as RootNode;
                EditorUtility.SetDirty(_tree);
                AssetDatabase.SaveAssets();
            }

            tree.Nodes.ForEach(CreateNodeView);
            tree.Nodes.ForEach(node =>
                               {
                                   var children = BehaviourTree.GetChildNodes(node);
                                   children.ForEach(child => Connect(node, child));
                               });
        }

        private void Connect(Node input, Node output)
        {
            var a = FindNodeView(input);
            var b = FindNodeView(output);
            var edge = a.OutputPort.ConnectTo(b.InputPort);

            AddElement(edge);
        }

        public override List<Port> GetCompatiblePorts(Port input, NodeAdapter adapter)
        {
            var list = ports.ToList();

            return list.Where(output => IsCompatiblePort(input, output)).ToList();
        }

        private bool IsCompatiblePort(Port input, Port output)
        {
            return output.direction != input.direction &&  output.node != input.node;
        }

        private GraphViewChange OnGraphViewChanged(GraphViewChange graphViewChange)
        {
            graphViewChange.elementsToRemove?.ForEach(RemoveGraphElement);
            graphViewChange.edgesToCreate?.ForEach(CreateGraphEdge);

            if (graphViewChange.movedElements != null)
            {
                nodes.ForEach(MoveNode);
            }

            return graphViewChange;
        }

        private void RemoveGraphElement(GraphElement element)
        {
            switch (element)
            {
                case NodeView nodeView:
                    _tree.DeleteNode(nodeView.Node);
                    break;
                case Edge edge:
                {
                    var output = (NodeView)edge.output.node;
                    var input = (NodeView)edge.input.node;

                    BehaviourTree.RemoveChildNode(output.Node, input.Node);

                    break;
                }
            }
        }

        private void CreateGraphEdge(Edge edge)
        {
            var output = (NodeView)edge.output.node;
            var input = (NodeView)edge.input.node;

            BehaviourTree.AddChildNode(output.Node, input.Node);
        }

        private void MoveNode(UnityEditor.Experimental.GraphView.Node node)
        {
            var view = node as NodeView;
            view?.SortChildren();
        }

        public override void BuildContextualMenu(ContextualMenuPopulateEvent evt)
        {
            var types = TypeCache.GetTypesDerivedFrom<ActionNode>();
            foreach (var type in types.Where(type => type != null))
            {
                evt.menu.AppendAction($"[{type.BaseType?.Name}] {type.Name}", _ => CreateNode(type));
            }

            types = TypeCache.GetTypesDerivedFrom<CompositeNode>();
            foreach (var type in types.Where(type => type != null))
            {
                evt.menu.AppendAction($"[{type.BaseType?.Name}] {type.Name}", _ => CreateNode(type));
            }

            types = TypeCache.GetTypesDerivedFrom<DecoratorNode>();
            foreach (var type in types.Where(type => type != null))
            {
                evt.menu.AppendAction($"[{type.BaseType?.Name}] {type.Name}", _ => CreateNode(type));
            }
        }

        private void CreateNode(System.Type type)
        {
            if (!_tree)
            {
                BehaviourTree newtree = ScriptableObject.CreateInstance("BehaviourTree") as BehaviourTree;
                string path = $"Assets/DEBUGTREE.asset";
                AssetDatabase.CreateAsset(newtree, path);
                // AssetDatabase.AddObjectToAsset(newtree, newtree);
                AssetDatabase.Refresh();
                EditorUtility.SetDirty(newtree);
                AssetDatabase.SaveAssets();
                _tree = newtree;
            }

            Node node = _tree?.CreateNode(type);

            CreateNodeView(node);
        }

        private void CreateNodeView(Node node)
        {
            var view = new NodeView(node)
            {
                OnNodeSelected = OnNodeSelected
            };

            AddElement(view);
        }

        public void UpdateNodeStates()
        {
            nodes.ForEach(node =>
                          {
                              var view = node as NodeView;

                              view?.UpdateState();
                          });
        }
    }
}


#endif
