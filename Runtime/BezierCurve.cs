using SimpleSplines.Utilities;
using SimpleSplines.Nodes;
using System;
using UnityEngine;

namespace SimpleSplines
{
    [DisallowMultipleComponent, ExecuteAlways]
    public sealed class BezierCurve : MonoBehaviour
    {
        #region Data
        [SerializeField, HideInInspector]
        BezierPoint[] _curvePoints;

#if UNITY_EDITOR
        [SerializeField, Min(.01f)]
        internal float _sampleDistance = .1f;

        [SerializeField]
        internal BezierNode[] _curveNodes = new BezierNode[] {
            new BezierNode(new Vector3(1, 0, -1), Vector3.forward, Vector3.back),
            new BezierNode(new Vector3(-1, 0, 1), Vector3.forward, Vector3.back),
        };
#endif
        #endregion

        #region Properties
        /// <summary>
        /// Does the end of the curve connect to the start.
        /// </summary>
        [field: SerializeField]
        public bool Connected { get; private set; }

        /// <summary>
        /// How long is the bezier curve.
        /// </summary>
        [field: SerializeField, HideInInspector]
        public float CurveLength { get; private set; }
        #endregion

        #region Unity Functions
#if UNITY_EDITOR
        private void OnDrawGizmos()
        {
            if (_curvePoints == null || _curvePoints.Length == 0) return;

            for (int i = 0; i < _curvePoints.Length - 1; i++) {

                Vector3 p1 = transform.TransformPoint(_curvePoints[i].point);
                Vector3 p2 = transform.TransformPoint(_curvePoints[i + 1].point);

                /* Gizmos lines can only be one pixel thick which doesn't look very nice,
                 * however Handles lines cannot be selected in the editor so this is the
                 * only way to have a nice looking selectable line.*/
                Gizmos.DrawLine(p1, p2);
                UnityEditor.Handles.DrawLine(p1, p2, 3);
            }
        }
        private void OnValidate()
        {
            RecalculateCurve(_curveNodes, Connected, _sampleDistance);
        }
#endif
        #endregion

        #region Public Functions
        public void RecalculateCurve(BezierNode[] bezierNodes, bool connected, float sampleDistance)
        {
            if (bezierNodes.Length < 2) {

                _curvePoints = new BezierPoint[0];
                return;
            }

            BezierPoint[] bezierPoints = BezierCalculator.CalculateSplinePoints(
                bezierNodes, sampleDistance, connected);
            _curvePoints = bezierPoints;

            float finishDistance = Vector3.Distance(bezierPoints[^1].point, bezierPoints[^2].point);
            CurveLength = ((bezierPoints.Length - 1) * sampleDistance) + finishDistance;
        }

        public BezierPoint SampleCurve(float percent, bool worldSpace = true)
        {
            percent = Mathf.Clamp01(percent);

            if (percent == 1) {

                /* If percent is one then the result is already known and there is no need to sample, 
                 * this also prevents an index out of range exception as the next curve point
                 * doesn't actually exist if the starting point is the last in the collection.*/
                if (!worldSpace) return _curvePoints[^1];
                return BezierPoint.TransformPoint(transform, _curvePoints[^1]);
            }

            int length = _curvePoints.Length - 1;
            int index = (int)(length * percent);

            /* To get more accurate results we can lerp between two curve points, however in order to
             * do that we must first convert the percent along the entire length of the curve to a local
             * percent which is the distance along two points.*/
            float segmentLength = 1f / length;
            float localPercent = (percent - (segmentLength * index)) / segmentLength;

            /* Converting the result to world space as the bezier curve points are stored in local space.*/
            BezierPoint result = BezierPoint.Lerp(_curvePoints[index], _curvePoints[index + 1], localPercent);

            if (!worldSpace) return result;
            return BezierPoint.TransformPoint(transform, result);
        }
        public float GetNearestCurvePoint(Vector3 point)
        {
            /* Since the bezier curve points are stored in local space 
             * it's cheaper to convert the point into local space.*/
            point = transform.InverseTransformPoint(point);

            int startIndex = 0, finishIndex = _curvePoints.Length - 1;
            int nearestIndex = 0;

            while (finishIndex - startIndex >= 2) {

                float nearestDistance = Mathf.Infinity;
                float delta = finishIndex - startIndex;

                for (int i = 0; i < 5; i++) {

                    /* Because the curve could be a loop this needs to itterate over both the start and end of the curve, 
                     * so this allows it to loop over the start and end if an over or underflow occurs, this also helps
                     * in edge cases where the start and end get close to each other.*/
                    int targetIndex = GetLoopedIndex(startIndex + (int)(delta / 4f * i));
                    float currentDistance = Vector3.Distance(_curvePoints[targetIndex].point, point);

                    if (currentDistance < nearestDistance) {

                        nearestDistance = currentDistance;
                        nearestIndex = targetIndex;
                    }
                }

                startIndex = (int)(nearestIndex - (delta / 4));
                finishIndex = (int)(nearestIndex + (delta / 4));
            }

            nearestIndex = GetLoopedIndex(nearestIndex - 1);
            float localPercent = 0;

            for (int i = 0; i < 2; i++) {

                int indexA = (nearestIndex + i) % _curvePoints.Length;
                int indexB = (indexA + 1) % _curvePoints.Length;

                if (indexB == 0) (indexA, indexB) = (0, 1);

                float samplePercent = ClosestLinePoint(_curvePoints[indexA], _curvePoints[indexB], point);
                localPercent += Mathf.Clamp01(samplePercent);
            }

            float pointPercent = 1f / (_curvePoints.Length - 1f);
            return ((pointPercent * nearestIndex) + (pointPercent * localPercent)) % 1f;
        }
        #endregion

        #region Private Functions
        private float ClosestLinePoint(BezierPoint pointA, BezierPoint pointB, Vector3 point)
        {
            Vector3 a2b = pointB.point - pointA.point;
            Vector3 a2p = point - pointA.point;

            float a2b2 = a2b.sqrMagnitude;
            float dot = Vector3.Dot(a2p, a2b);

            return dot / a2b2;
        }

        private int GetLoopedIndex(int curveIndex)
        {
            if (curveIndex < 0 && Connected) {
                curveIndex = _curvePoints.Length - 1 + curveIndex;
            }
            return Mathf.Max(curveIndex % _curvePoints.Length, 0);
        }
        #endregion
    }
}
