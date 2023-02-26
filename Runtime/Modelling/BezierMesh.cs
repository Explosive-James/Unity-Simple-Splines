using SimpleSplines.Utilities;
using UnityEngine;

namespace SimpleSplines.Modelling
{
    [RequireComponent(typeof(BezierCurve))]
    public sealed class BezierMesh : MonoBehaviour
    {
        #region Data
        private BezierCurve _bezierCurve;

        public MeshFilter meshFilter;
        public MeshSettings visualSettings = new MeshSettings(
            null, Vector3.zero, Vector3.zero, Vector3.one, true);

        public MeshCollider meshCollider;
        public MeshSettings collisionSettings = new MeshSettings(
            null, Vector3.zero, Vector3.zero, Vector3.one, true);
        #endregion

        #region Properties
        public BezierCurve BezierCurve {
            get {
                if (_bezierCurve == null)
                    _bezierCurve = GetComponent<BezierCurve>();
                return _bezierCurve;
            }
            set {
                _bezierCurve = value;
            }
        }
        #endregion

        #region Unity Functions
#if UNITY_EDITOR
        private void OnDrawGizmosSelected()
        {
            Gizmos.matrix = transform.localToWorldMatrix;

            if (visualSettings.mesh != null && meshFilter != null) {

                Gizmos.color = new Color(.4f, .4f, .4f, .8f);

                Gizmos.DrawMesh(ModelUtilities.TransformMesh(visualSettings.mesh,
                    visualSettings.GetTransformMatrix()));
            }

            if (collisionSettings.mesh != null && meshCollider != null) {

                Gizmos.color = new Color(1, 1, 1, .1f);

                Gizmos.DrawWireMesh(ModelUtilities.TransformMesh(collisionSettings.mesh,
                    collisionSettings.GetTransformMatrix()));
            }
        }
#endif
        #endregion

        #region Public Functions
        public void CreateVisualMesh()
        {
            Mesh visualMesh = CreateBezierMesh(visualSettings);
            visualMesh.name = $"{gameObject.name}-visualMesh";

            meshFilter.sharedMesh = visualMesh;
        }
        public void CreateCollisionMesh()
        {
            Mesh collisionMesh = CreateBezierMesh(collisionSettings);
            collisionMesh.name = $"{gameObject.name}-collisionMesh";

            meshCollider.sharedMesh = collisionMesh;
        }

        public Mesh CreateBezierMesh(MeshSettings meshSettings, bool worldSpace = false)
        {
            Mesh originalMesh = ModelUtilities.TransformMesh(
                meshSettings.mesh, meshSettings.GetTransformMatrix());

            float sizeFactor = BezierCurve.CurveLength / originalMesh.bounds.size.z;
            int duplicationCount = (int)Mathf.Max(sizeFactor, 1);

            float stretchFactor = meshSettings.stretchToFit ? sizeFactor / duplicationCount : 1;

            Vector3[] vertices = new Vector3[originalMesh.vertexCount * duplicationCount];
            Vector3[] normals = new Vector3[vertices.Length];

            Vector2[] uvCoords = new Vector2[vertices.Length];
            int[] triangles = new int[originalMesh.triangles.Length * duplicationCount];

            for (int duplicate = 0; duplicate < duplicationCount; duplicate++) {

                float zOffset = originalMesh.bounds.size.z * duplicate;
                int vertexOffset = duplicate * originalMesh.vertexCount;

                for (int vertex = 0; vertex < originalMesh.vertexCount; vertex++) {

                    Vector3 originalVertex = originalMesh.vertices[vertex];

                    float curvePosition = (zOffset + originalVertex.z) / BezierCurve.CurveLength;
                    int vertexIndex = vertexOffset + vertex;

                    BezierPoint point = BezierCurve.SampleCurve(curvePosition * stretchFactor, worldSpace);
                    Quaternion rotation = Quaternion.LookRotation(point.tangent, point.normal);

                    vertices[vertexIndex] = point.point + (rotation * new Vector3(originalVertex.x, originalVertex.y));
                    normals[vertexIndex] = rotation * originalMesh.normals[vertex];
                    uvCoords[vertexIndex] = originalMesh.uv[vertex];
                }

                for (int triangle = 0; triangle < originalMesh.triangles.Length; triangle++) {

                    int triangleIndex = (duplicate * originalMesh.triangles.Length) + triangle;
                    triangles[triangleIndex] = vertexOffset + originalMesh.triangles[triangle]; ;
                }
            }

            Mesh result = new Mesh() {

                vertices = vertices,
                normals = normals,

                triangles = triangles,
                uv = uvCoords,
            };
            result.RecalculateBounds();

            return result;
        }
        #endregion
    }
}
