#if UNITY_EDITOR

using System;
using UnityEngine;
using UnityEditor.Experimental.GraphView;
using UnityEngine.UIElements;
using UnityEditor;
using UnityEditor.UIElements;

namespace GloryDay.BehaviourTree.Editor
{
    public class NodeView : UnityEditor.Experimental.GraphView.Node
    {
        #region PRIVATE CONSTANTS FIELD API

        private const string UxmlFilePath = "Assets/Script/Util/Tool/BehaviourTree/Style/NodeView.uxml";

        #endregion

        public readonly Node Node;

        public Port InputPort;
        public Port OutputPort;

        public Action<NodeView> OnNodeSelected;

        public NodeView(Node node) : base(UxmlFilePath)
        {
            Node = node;

            title = node.name;
            viewDataKey = node.Guid;

            style.left = node.Position.x;
            style.top = node.Position.y;

            InitializeInputPort();
            InitializeOutputPort();

            SetupClasses();

            var label = this.Q<Label>("description");
            label.bindingPath = "description";
            label.Bind(new SerializedObject(node));
        }

        public sealed override string title
        {
            get { return base.title; }
            set { base.title = value; }
        }

        private void SetupClasses()
        {
            switch (Node)
            {
                case ActionNode action:
                    AddToClassList(action.ToString());
                    break;
                case CompositeNode composite:
                    AddToClassList(composite.ToString());
                    break;
                case DecoratorNode decorator:
                    AddToClassList(decorator.ToString());
                    break;
                case RootNode root:
                    AddToClassList(root.ToString());
                    break;
            }
        }

        private void InitializeOutputPort()
        {
            switch (Node)
            {
                case ActionNode:
                    break;
                case CompositeNode:
                    OutputPort = InstantiatePort(Orientation.Vertical, Direction.Output, Port.Capacity.Multi, typeof(bool));
                    break;
                case DecoratorNode:
                case RootNode:
                    OutputPort = InstantiatePort(Orientation.Vertical, Direction.Output, Port.Capacity.Single, typeof(bool));
                    break;
            }

            if (OutputPort == null)
            {
                return;
            }

            OutputPort.portName = "";
            OutputPort.style.flexDirection = FlexDirection.ColumnReverse;

            outputContainer.Add(OutputPort);
        }

        private void InitializeInputPort()
        {
            switch (Node)
            {
                case ActionNode:
                case CompositeNode:
                case DecoratorNode:
                    InputPort = InstantiatePort(Orientation.Vertical, Direction.Input, Port.Capacity.Single, typeof(bool));
                    break;
                case RootNode:
                    break;
            }

            if (InputPort == null)
            {
                return;
            }

            InputPort.portName = "";
            InputPort.style.flexDirection = FlexDirection.Column;

            inputContainer.Add(InputPort);
        }

        public override void SetPosition(Rect position)
        {
            base.SetPosition(position);

            Undo.RecordObject(Node, "Behaviour Tree (Set Position)");
            Node.Position.x = position.xMin;
            Node.Position.y = position.yMin;

            EditorUtility.SetDirty(Node);
        }

        public override void OnSelected()
        {
            base.OnSelected();

            OnNodeSelected?.Invoke(this);
        }

        public void SortChildren()
        {
            var composite = Node as CompositeNode;
            composite?.Children.Sort(SortByHorizontalPosition);
        }

        private int SortByHorizontalPosition(Node left, Node right)
        {
            return left.Position.x < right.Position.x ? -1 : 1;
        }

        public void UpdateState()
        {
            var runningStateName = nameof(Node.State.Running).ToLower();
            var failureStateName = nameof(Node.State.Failure).ToLower();
            var successStateName = nameof(Node.State.Success).ToLower();

            RemoveFromClassList(runningStateName);
            RemoveFromClassList(failureStateName);
            RemoveFromClassList(successStateName);

            if (Application.isPlaying == false)
            {
                return;
            }

            switch (Node.mState)
            {
                case Node.State.Running:
                {
                    if (Node.Started)
                    {
                        AddToClassList(runningStateName);
                    }

                    break;
                }
                case Node.State.Failure:
                    AddToClassList(failureStateName);
                    break;
                case Node.State.Success:
                    AddToClassList(successStateName);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}

#endif
