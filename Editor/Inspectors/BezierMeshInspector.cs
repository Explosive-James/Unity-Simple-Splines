using SimpleSplines.Modelling;
using UnityEditor;
using UnityEngine;

namespace SimpleSplinesEditor.Inspectors
{
    [CustomEditor(typeof(BezierMesh))]
    internal class BezierMeshInspector : Editor
    {
        #region Unity Functions
        public override void OnInspectorGUI()
        {
            SerializedProperty meshFilerProperty = serializedObject.FindProperty("meshFilter");
            EditorGUILayout.PropertyField(meshFilerProperty);

            EditorGUI.BeginDisabledGroup(meshFilerProperty.objectReferenceValue == null);
            serializedObject.DrawPropertyField("visualSettings");

            EditorGUILayout.Space();

            if (GUILayout.Button("Create Visual Mesh")) {

                serializedObject.MarkAsDirty("Create Visual Mesh");
                ((BezierMesh)target).CreateVisualMesh();
            }
            EditorGUI.EndDisabledGroup();

            EditorGUILayout.Space();

            SerializedProperty meshColliderProperty = serializedObject.FindProperty("meshCollider");
            EditorGUILayout.PropertyField(meshColliderProperty);

            EditorGUI.BeginDisabledGroup(meshColliderProperty.objectReferenceValue == null);
            serializedObject.DrawPropertyField("collisionSettings");

            EditorGUILayout.Space();

            if (GUILayout.Button("Create Collision Mesh")) {

                serializedObject.MarkAsDirty("Create Collision Mesh");
                ((BezierMesh)target).CreateCollisionMesh();
            }

            if (serializedObject.hasModifiedProperties) {
                serializedObject.ApplyModifiedProperties();
            }
        }
        #endregion
    }
}
