using UnityEngine;

namespace SimpleSplines.Nodes
{
    [System.Serializable]
    public struct BezierNode
    {
        #region Data
        [SerializeField] HandleType _type;
        [SerializeField] Vector3 _point, _forward, _backward;
        [SerializeField] float _angle;
        #endregion

        #region Properties
        public Vector3 Point {
            get => _point;
            set => _point = value;
        }
        public float Angle {
            get => _angle;
            set => _angle = value;
        }

        public Vector3 Forward {
            get => _forward;
            set {
                _forward = value;
                _backward = UpdateHandle(_forward, _backward);
            }
        }
        public Vector3 ForwardPoint => _point + _forward;

        public Vector3 Backward {
            get => _backward;
            set {
                _backward = value;
                _forward = UpdateHandle(_backward, _forward);
            }
        }
        public Vector3 BackwardPoint => _point + _backward;

        public HandleType Type {
            get => _type;
        }
        #endregion

        #region Constructor
        public BezierNode(Vector3 point, Vector3 forward, Vector3 backward)
        {
            _type = HandleType.Aligned;
            _point = point;

            _forward = forward;
            _backward = backward;

            _angle = 0;
        }
        public BezierNode(HandleType type, Vector3 point, Vector3 forward, Vector3 backward, float angle)
        {
            _type = type;
            _point = point;

            _forward = forward;
            _backward = backward;

            _angle = angle;
        }
        #endregion

        #region Public Functions
        public void SetHandleType(HandleType type, bool preserveForwards = true)
        {
            _type = type;

            if (preserveForwards) {
                _backward = UpdateHandle(_forward, _backward);
            }
            else {
                _forward = UpdateHandle(_backward, _forward);
            }
        }
        #endregion

        #region Private Functions
        private Vector3 UpdateHandle(Vector3 pointA, Vector3 pointB)
        {
            return _type switch {

                HandleType.Mirrored => -pointA,
                HandleType.Aligned => -pointA.normalized * pointB.magnitude,
                _ => pointB
            };
        }
        #endregion
    }
}
