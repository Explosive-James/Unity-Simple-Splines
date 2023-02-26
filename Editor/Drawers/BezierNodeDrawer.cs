using SimpleSplines.Nodes;
using UnityEditor;
using UnityEngine;

namespace SimpleSplinesEditor.Drawers
{
    [CustomPropertyDrawer(typeof(BezierNode))]
    internal class BezierNodeDrawer : PropertyDrawer
    {
        #region Unity Functions
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            /* Changing the label name when the property is in a collection.*/
            string[] propertyPath = property.propertyPath.Split('.');

            if (propertyPath[^1].StartsWith("data[")) {
                label.text = $"Node {propertyPath[^1][5..^1]} Settings";
            }

            SerializedProperty handleProperty = property.FindPropertyRelative("_type");
            SerializedProperty pointProperty = property.FindPropertyRelative("_point");

            HandleType type = (HandleType)handleProperty.enumValueFlag;

            SerializedProperty forwardProperty = property.FindPropertyRelative("_forward");
            SerializedProperty backwardProperty = property.FindPropertyRelative("_backward");

            /* The position rect height is the size of the entire property which needs adjusting.*/
            position.height = EditorGUIUtility.singleLineHeight;
            EditorGUI.LabelField(position, label);

            EditorGUI.indentLevel++;

            position.y += EditorGUIUtility.singleLineHeight;
            if (handleProperty.DrawPropertyField(position)) {

                type = (HandleType)handleProperty.enumValueFlag;

                backwardProperty.vector3Value = GetModifiedHandle(
                    forwardProperty.vector3Value, backwardProperty.vector3Value, type);
            }

            position.y += EditorGUIUtility.singleLineHeight;
            EditorGUI.PropertyField(position, property.FindPropertyRelative("_angle"));

            position.y += EditorGUIUtility.singleLineHeight * 1.5f;
            EditorGUI.PropertyField(position, pointProperty);

            position.y += EditorGUIUtility.singleLineHeight * 1.5f;
            if (forwardProperty.DrawPropertyField(position)) {

                backwardProperty.vector3Value = GetModifiedHandle(
                    forwardProperty.vector3Value, backwardProperty.vector3Value, type);
                backwardProperty.serializedObject.ApplyModifiedProperties();
            }

            position.y += EditorGUIUtility.singleLineHeight;
            if (backwardProperty.DrawPropertyField(position)) {

                forwardProperty.vector3Value = GetModifiedHandle(
                    backwardProperty.vector3Value, forwardProperty.vector3Value, type);
                forwardProperty.serializedObject.ApplyModifiedProperties();
            }

            EditorGUI.indentLevel--;
        }
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return EditorGUIUtility.singleLineHeight * 7f;
        }
        #endregion

        #region Private Functions
        private Vector3 GetModifiedHandle(Vector3 a, Vector3 b, HandleType type)
        {
            return type switch {

                HandleType.Mirrored => -a,
                HandleType.Aligned => -a.normalized * b.magnitude,

                _ => b,
            };
        }
        #endregion
    }
}
