using SimpleSplines.Modelling;
using UnityEditor;
using UnityEngine;

namespace SimpleSplinesEditor.Inspectors
{
    [CustomEditor(typeof(BezierExtrude))]
    internal class BezierExtrudeInspector : Editor
    {
        #region Unity Functions
        public override void OnInspectorGUI()
        {
            SerializedProperty meshFilerProperty = serializedObject.FindProperty("meshFilter");
            EditorGUILayout.PropertyField(meshFilerProperty);

            EditorGUI.BeginDisabledGroup(meshFilerProperty.objectReferenceValue == null);
            serializedObject.DrawPropertyField("visualSettings");

            EditorGUILayout.Space();

            if (GUILayout.Button("Extrude Visual Mesh")) {

                serializedObject.MarkAsDirty("Extrude Visual Mesh");
                ((BezierExtrude)target).ExtrudeVisualMesh();
            }
            EditorGUI.EndDisabledGroup();

            EditorGUILayout.Space();

            SerializedProperty meshColliderProperty = serializedObject.FindProperty("meshCollider");
            EditorGUILayout.PropertyField(meshColliderProperty);

            EditorGUI.BeginDisabledGroup(meshColliderProperty.objectReferenceValue == null);
            serializedObject.DrawPropertyField("collisionSettings");

            EditorGUILayout.Space();

            if(GUILayout.Button("Extrude Collision Mesh")) {

                serializedObject.MarkAsDirty("Extrude Collision Mesh");
                ((BezierExtrude)target).ExtrudeCollisionMesh();
            }

            if (serializedObject.hasModifiedProperties) {
                serializedObject.ApplyModifiedProperties();
            }
        }
        #endregion
    }
}
