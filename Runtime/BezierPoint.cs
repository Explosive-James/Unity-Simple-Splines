using System.Runtime.CompilerServices;
using UnityEngine;

namespace SimpleSplines
{
    /// <summary>
    /// Bezier data of a point along a bezier curve.
    /// </summary>
    [System.Serializable]
    public struct BezierPoint
    {
        #region Data
        /// <summary>
        /// The position along the bezier curve.
        /// </summary>
        public Vector3 point;
        /// <summary>
        /// The direction of the bezier curve.
        /// </summary>
        public Vector3 tangent; 
        /// <summary>
        /// The upward direction of the bezier curve.
        /// </summary>
        public Vector3 normal; 
        /// <summary>
        /// The rightward direction of the bezier curve.
        /// </summary>
        public Vector3 bitangent;
        #endregion

        #region Constructor
        public BezierPoint(Vector3 point, Vector3 tangent, Vector3 normal)
        {
            this.point = point;
            this.tangent = tangent;
            this.normal = normal;
            bitangent = Vector3.Cross(normal, tangent);
        }
        public BezierPoint(Vector3 point, Vector3 tangent, Vector3 normal, Vector3 bitangent)
        {
            this.point = point;
            this.tangent = tangent;
            this.normal = normal;
            this.bitangent = bitangent;
        }
        #endregion

        #region Static Functions
        /// <summary>
        /// Interpolates between two bezier points.
        /// </summary>
        public static BezierPoint Lerp(BezierPoint a, BezierPoint b, float t)
        {
            Vector3 point = Vector3.Lerp(a.point, b.point, t);

            /* Having to use slerp instead of lerp on the normal and tangents because a lerp doesn't rotate the vector, 
             * this would be fine except it wouldn't smoothly transition if the vector flips between two points, instead 
             * it just creates a vector that shrinks from one value to the other and if normalized would snap half way.*/
            Vector3 normal = Vector3.Slerp(a.normal, b.normal, t);
            Vector3 tangent = Vector3.Slerp(a.tangent, b.tangent, t);

            return new BezierPoint(point, tangent, normal);
        }
        /// <summary>
        /// Multiplies the bezier point data with a matrix.
        /// </summary>
        public static BezierPoint TransformPoint(Transform transform, BezierPoint point)
        {
            return new BezierPoint(
                transform.TransformPoint(point.point),
                transform.rotation * point.tangent,
                transform.rotation * point.normal);
        }
        #endregion
    }
}
