using UnityEditor;
using UnityEngine;

namespace SimpleSplinesEditor
{
    internal static class EditorExtensions
    {
        #region Public Functions
        public static bool DrawPropertyField(this SerializedProperty property, Rect position, GUIContent label = default)
        {
            label ??= new GUIContent(property.displayName, property.tooltip);

            EditorGUI.BeginChangeCheck();
            EditorGUI.PropertyField(position, property, label);

            return EditorGUI.EndChangeCheck();
        }

        public static bool DrawPropertyField(this SerializedObject obj, string propertyName, GUIContent label= default)
        {
            SerializedProperty property = obj.FindPropertyComplete(propertyName);
            label ??= new GUIContent(property.displayName, property.tooltip);

            EditorGUI.BeginChangeCheck();
            EditorGUILayout.PropertyField(property, label);

            return EditorGUI.EndChangeCheck();
        }
        public static SerializedProperty FindPropertyComplete(this SerializedObject obj, string propertyName)
        {
            SerializedProperty property = obj.FindProperty(propertyName);
            property ??= obj.FindProperty($"<{propertyName}>k__BackingField");

            return property;
        }

        public static void DrawHandleLine(Vector3 positionA, Vector3 positionB, Color colour)
        {
            Handles.color = colour;
            Handles.DrawLine(positionA, positionB, 2);
        }
        public static bool DrawHandleButton(Vector3 position, float visualSize, float selectSize)
        {
            visualSize *= HandleUtility.GetHandleSize(position);
            selectSize *= HandleUtility.GetHandleSize(position);

            Handles.color = Color.white;
            return Handles.Button(position, Quaternion.identity, visualSize, selectSize, Handles.DotHandleCap);
        }

        public static void MarkAsDirty(this SerializedObject serializedObject, string message)
        {
            Undo.RegisterCompleteObjectUndo(serializedObject.targetObject, message);
            EditorUtility.SetDirty(serializedObject.targetObject);
        }
        #endregion
    }
}
