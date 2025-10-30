#if UNITY_EDITOR

using System;
using UnityEngine.UIElements;
using UnityEditor;

namespace GloryDay.BehaviourTree.Editor
{
    public class InspectorView : VisualElement
    {
        public new class UxmlFactory : UxmlFactory<InspectorView, VisualElement.UxmlTraits>
        {
        }

        private UnityEditor.Editor editor;
        public InspectorView() { }

        internal void UpdateSelection(NodeView nodeView)
        {
            Clear();

            UnityEngine.Object.DestroyImmediate(editor);
            editor = UnityEditor.Editor.CreateEditor(nodeView.node);
            IMGUIContainer container = new IMGUIContainer(() =>
                                                          {
                                                              if (editor.target)
                                                              {
                                                                  editor.OnInspectorGUI();
                                                              }
                                                          });

            Add(container);
        }
    }
}

#endif
