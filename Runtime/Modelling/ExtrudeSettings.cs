using UnityEngine;

namespace SimpleSplines.Modelling
{
    [System.Serializable]
    public class ExtrudeSettings
    {
        #region Data
        [SerializeField, Min(3)] int _resolution = 12;
        [SerializeField, Min(0)] float _radius = .5f;
        [SerializeField, Min(1)] int _segments = 12;
        #endregion

        #region Properties
        public int Resolution {
            get => _resolution;
            set => _resolution = Mathf.Max(value, 3);
        }
        public float Radius {
            get => _radius;
            set => Mathf.Max(value, 0);
        }

        public int Segments {
            get => _segments;
            set => Mathf.Max(value, 1);
        }
        #endregion

        #region Constructor
        public ExtrudeSettings(int resolution, float radius, int segments)
        {
            Resolution = resolution;
            Radius = radius;
            Segments = segments;
        }
        #endregion
    }
}
