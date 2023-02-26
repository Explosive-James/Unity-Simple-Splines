using UnityEngine;

namespace SimpleSplines.Modelling
{
    [System.Serializable]
    public class MeshSettings
    {
        #region Data
        public Mesh mesh;
        public bool stretchToFit;

        [Space]
        public Vector3 position = Vector3.zero;
        public Vector3 rotation = Vector3.zero;
        public Vector3 scale = Vector3.one;
        #endregion

        #region Constructor
        public MeshSettings(Mesh mesh, Vector3 position,
            Vector3 rotation, Vector3 scale, bool stretchToFit)
        {
            this.mesh = mesh;
            this.stretchToFit = stretchToFit;

            this.position = position;
            this.rotation = rotation;
            this.scale = scale;
        }
        #endregion

        #region Public Functions
        public Matrix4x4 GetTransformMatrix() =>
            Matrix4x4.TRS(position, Quaternion.Euler(rotation), scale);
        #endregion
    }
}
