using System.Collections.Generic;
using UnityEngine;

namespace SimpleSplines.Utilities
{
    // ToDo: Merge By Distance
    public static class ModelUtilities
    {
        #region Public Functions
        public static Vector3[] CalculateNormals(Vector3[] vertices, int[] triangles)
        {
            Vector3[] normals = new Vector3[vertices.Length];

            for (int a = 0; a < triangles.Length; a += 3) {

                int b = a + 1, c = a + 2;

                Vector3 c2b = vertices[triangles[c]] - vertices[triangles[b]];
                Vector3 b2a = vertices[triangles[a]] - vertices[triangles[b]];

                Vector3 faceNormals = Vector3.Cross(c2b, b2a).normalized;

                normals[triangles[a]] += faceNormals;
                normals[triangles[b]] += faceNormals;
                normals[triangles[c]] += faceNormals;
            }

            for (int i = 0; i < normals.Length; i++) {
                normals[i] = normals[i].normalized;
            }

            return normals;
        }

        public static Mesh TransformMesh(Mesh mesh, Matrix4x4 matrix)
        {
            Vector3[] vertices = new Vector3[mesh.vertexCount];
            Vector3[] normals = new Vector3[vertices.Length];

            Quaternion rotation = matrix.rotation;

            for(int i = 0; i < vertices.Length; i++) {

                vertices[i] = matrix.MultiplyPoint(mesh.vertices[i]);
                normals[i] = rotation * mesh.normals[i];
            }

            Mesh result = Object.Instantiate(mesh);
            (result.vertices, result.normals) = (vertices, normals);

            result.RecalculateBounds();
            return result;
        }
        #endregion
    }
}
