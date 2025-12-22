#if UNITY_EDITOR

using UnityEngine.UIElements;

namespace GloryDay.BehaviourTree.Editor
{
    public class InspectorView : VisualElement
    {
        public new class UxmlFactory : UxmlFactory<InspectorView, UxmlTraits> { }

        private UnityEditor.Editor _editor;

        internal void UpdateSelection(NodeView nodeView)
        {
            Clear();

            UnityEngine.Object.DestroyImmediate(_editor);
            _editor = UnityEditor.Editor.CreateEditor(nodeView.Node);

            var container = new IMGUIContainer(() =>
                                               {
                                                   if (_editor.target)
                                                   {
                                                       _editor.OnInspectorGUI();
                                                   }
                                               });

            Add(container);
        }
    }
}

#endif
