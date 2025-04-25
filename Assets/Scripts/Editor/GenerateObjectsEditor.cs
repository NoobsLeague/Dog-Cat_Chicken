using UnityEditor;
using UnityEngine;

namespace Editor
{
    [CustomEditor(typeof(GenerateObject))]
    [CanEditMultipleObjects]
    public class GenerateObjectsEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            if (GUILayout.Button("Generate"))
            {
                (target as GenerateObject)?.RegenerateObjects();
            }

            if (GUILayout.Button("Clear"))
            {
                (target as GenerateObject)?.RemoveChildren();
            }
        }
    }
}