#if UNITY_EDITOR

using System;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEngine;
using UnityEngine.UIElements;

namespace GloryDay.BehaviourTree.Editor
{
    public class BehaviourTreeEditor : EditorWindow
    {
        #region PRIVATE CONSTANTS FIELD API

        private const string UxmlFilePath = "Assets/Script/Util/Tool/BehaviourTree/Style/BehaviourTreeEditor.uxml";
        private const string UssFilePath = "Assets/Script/Util/Tool/BehaviourTree/Style/BehaviourTreeEditor.uss";

        #endregion

        private BehaviourTreeView _treeView;
        private InspectorView _inspectorView;
        private IMGUIContainer _blackboardView;

        private SerializedObject _data;
        private SerializedProperty _property;

        [MenuItem("Tools/Behaviour Tree Editor")]
        public static void OpenWindow()
        {
            var window = GetWindow<BehaviourTreeEditor>();
            window.titleContent = new GUIContent("Behaviour Tree Editor");
        }

        [OnOpenAsset]
        public static bool OnOpenAsset(int instanceId, int line)
        {
            if (Selection.activeObject is not BehaviourTree)
            {
                return false;
            }

            OpenWindow();

            return true;
        }

        public void CreateGUI()
        {
            // Each editor window contains a root VisualElement object.
            var root = rootVisualElement;

            // Import UXML.
            var asset = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(UxmlFilePath);
            asset.CloneTree(root);

            // A stylesheet can be added to a VisualElement.
            // The style will be applied to the VisualElement and all of its children.
            var sheet = AssetDatabase.LoadAssetAtPath<StyleSheet>(UssFilePath);
            root.styleSheets.Add(sheet);

            _treeView = root.Q<BehaviourTreeView>();
            _inspectorView = root.Q<InspectorView>();
            _blackboardView = root.Q<IMGUIContainer>();
            _blackboardView.onGUIHandler = () =>
            {
                if (_property == null)
                {
                    return;
                }

                if (Application.isPlaying)
                {
                    _data.Update();
                }

                EditorGUILayout.PropertyField(_property);

                if (Application.isPlaying)
                {
                    _data.ApplyModifiedProperties();
                }
            };

            if (_treeView != null)
            {
                _treeView.OnNodeSelected = OnNodeSelectionChanged;
            }

            OnSelectionChange();
        }

        private void OnEnable()
        {
            EditorApplication.playModeStateChanged -= OnPlayModeStateChanged;
            EditorApplication.playModeStateChanged += OnPlayModeStateChanged;
        }


        private void OnDisable()
        {
            EditorApplication.playModeStateChanged -= OnPlayModeStateChanged;
        }

        private void OnPlayModeStateChanged(PlayModeStateChange mode)
        {
            switch (mode)
            {
                case PlayModeStateChange.EnteredEditMode:
                    OnSelectionChange();
                    break;
                case PlayModeStateChange.ExitingEditMode:
                    break;
                case PlayModeStateChange.EnteredPlayMode:
                    OnSelectionChange();
                    break;
                case PlayModeStateChange.ExitingPlayMode:
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(mode), mode, null);
            }
        }

        private void OnSelectionChange()
        {
            var tree = Selection.activeObject as BehaviourTree;
            if (tree is null)
            {
                if (Selection.activeGameObject)
                {
                    var runner = Selection.activeGameObject.GetComponent<BehaviourTreeRunner>();
                    if (runner)
                    {
                        tree = runner.Tree;
                    }
                }
            }

            if (Application.isPlaying)
            {
                if (tree)
                {
                    _treeView?.PopulateView(tree);
                }
            }
            else
            {
                if (tree)
                {
                    if (AssetDatabase.CanOpenAssetInEditor(tree.GetInstanceID()))
                    {
                        _treeView.PopulateView(tree);
                    }
                }
            }

            if (tree == null || _treeView == null)
            {
                return;
            }

            _data = new SerializedObject(tree);
            _property = _data.FindProperty("blackboard");

        }

        private void OnNodeSelectionChanged(NodeView node)
        {
            _inspectorView.UpdateSelection(node);
        }

        private void OnInspectorUpdate()
        {
            _treeView?.UpdateNodeStates();
        }
    }
}

#endif
