using SimpleSplines.Utilities;
using UnityEngine;

namespace SimpleSplines.Modelling
{
    [RequireComponent(typeof(BezierCurve))]
    public sealed class BezierExtrude : MonoBehaviour
    {
        #region Data
        private BezierCurve _bezierCurve;

        public MeshFilter meshFilter;
        public ExtrudeSettings visualSettings = new ExtrudeSettings(12, .5f, 64);

        public MeshCollider meshCollider;
        public ExtrudeSettings collisionSettings = new ExtrudeSettings(6, .5f, 24);
        #endregion

        #region Properties
        public BezierCurve BezierCurve {
            get {
                if(_bezierCurve == null )
                    _bezierCurve = GetComponent<BezierCurve>();
                return _bezierCurve;
            }
            set {
                _bezierCurve = value;
            }
        }
        #endregion

        #region Public Functions
        public void ExtrudeVisualMesh()
        {
            Mesh visualMesh = ExtrudeCurveMesh(visualSettings);
            visualMesh.name = $"{gameObject.name}-visualMesh";

            meshFilter.sharedMesh = visualMesh;
        }
        public void ExtrudeCollisionMesh()
        {

            Mesh collisionMesh = ExtrudeCurveMesh(collisionSettings);
            collisionMesh.name = $"{gameObject.name}-collisionMesh";

            meshCollider.sharedMesh = collisionMesh;
        }

        public Mesh ExtrudeCurveMesh(ExtrudeSettings settings, bool worldSpace = false)
        {
            Vector3[] vertices = new Vector3[(settings.Resolution + 1) * (settings.Segments + 1)];
            Vector2[] uvCoords = new Vector2[vertices.Length];

            float textureHeight = BezierCurve.CurveLength / (settings.Radius * Mathf.PI * 2);

            for (int segment = 0; segment < settings.Segments + 1; segment++) {

                BezierPoint bezierPoint = BezierCurve.SampleCurve((float)segment / settings.Segments, worldSpace);
                Quaternion orientation = Quaternion.LookRotation(bezierPoint.tangent, bezierPoint.normal);

                for (int vertex = 0; vertex < settings.Resolution + 1; vertex++) {

                    float angle = ((float)vertex / settings.Resolution) * Mathf.PI * 2;
                    int vertexIndex = (segment * (settings.Resolution + 1)) + vertex;

                    Vector3 position = new Vector3(Mathf.Sin(angle), Mathf.Cos(angle));

                    vertices[vertexIndex] = bezierPoint.point + orientation * position * settings.Radius;
                    uvCoords[vertexIndex] = new Vector2((float)vertex / settings.Resolution, ((float)segment / settings.Segments) * textureHeight);
                }
            }

            int[] triangles = new int[settings.Resolution * 6 * settings.Segments];

            for (int segment = 0; segment < settings.Segments; segment++)
                for (int triangle = 0; triangle < settings.Resolution; triangle++) {

                    int vertexOffset = segment * (settings.Resolution + 1);
                    int triangleIndex = (segment * settings.Resolution * 6) + (triangle * 6);

                    triangles[triangleIndex] = vertexOffset + triangle;
                    triangles[triangleIndex + 1] = vertexOffset + triangle + settings.Resolution + 1;
                    triangles[triangleIndex + 2] = triangles[triangleIndex] + 1;

                    triangles[triangleIndex + 3] = triangles[triangleIndex + 1];
                    triangles[triangleIndex + 4] = triangles[triangleIndex + 3] + 1;
                    triangles[triangleIndex + 5] = triangles[triangleIndex + 2];
                }

            Vector3[] normals = ModelUtilities.CalculateNormals(vertices, triangles);

            for (int segment = 0; segment < settings.Segments + 1; segment++) {

                int vertexA = segment * (settings.Resolution + 1);
                int vertexB = vertexA + settings.Resolution;

                normals[vertexA] = (normals[vertexA] + normals[vertexB]).normalized;
                normals[vertexB] = normals[vertexA];
            }

            Mesh result = new Mesh() {

                name = gameObject.name + "-extrudeMesh",

                vertices = vertices,
                triangles = triangles,

                normals = normals,
                uv = uvCoords,
            };
            result.RecalculateBounds();

            return result;
        }
        #endregion
    }
}
