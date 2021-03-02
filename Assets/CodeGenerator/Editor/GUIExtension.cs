using UnityEditor;
using UnityEngine;

namespace CodeGenerator
{
    public static class GUIExtension
    {
        public static bool ButtonLabel(string label, GUIStyle style, params GUILayoutOption[] options)
        {
            EditorGUILayout.LabelField(label, style, options);

            Event e = Event.current;
            if (e.type == EventType.MouseDown)
            {
                Rect r = GUILayoutUtility.GetLastRect();
                if (r.Contains(e.mousePosition))
                {
                    e.Use();
                    return true;
                }
            }

            return false;
        }
    }
}
