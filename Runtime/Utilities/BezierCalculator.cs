using SimpleSplines.Nodes;
using System.Collections.Generic;
using UnityEngine;

namespace SimpleSplines.Utilities
{
    internal static class BezierCalculator
    {
        #region Public Functions
        public static BezierPoint[] CalculateSplinePoints(BezierNode[] nodes, float sampleDistance, bool connected)
        {
            int length = connected ? nodes.Length : nodes.Length - 1;
            List<BezierPoint> results = new List<BezierPoint>();

            for(int index = 0; index < length; index++) {

                int next = (index + 1) % nodes.Length;
                
                float distance = ApproximateDistance(nodes[index], nodes[next]);
                int resolution = (int)Mathf.Max(distance / sampleDistance, 2);

                results.AddRange(CalculateBezierPoints(nodes[index], nodes[next], resolution));
            }

            return CalculateEqualDistancePoints(results.ToArray(), sampleDistance);
        }
        #endregion

        #region Private Functions
        private static BezierPoint[] CalculateBezierPoints(BezierNode nodeA, BezierNode nodeB, int resolution)
        {
            BezierPoint[] points = new BezierPoint[resolution];

            Vector3 p0 = nodeA.Point, p1 = nodeA.ForwardPoint;
            Vector3 p2 = nodeB.BackwardPoint, p3 = nodeB.Point;

            /* Calculating the first point seperately because the normals are calculated from the previous
             * normals, if this isn't calculated seperately all the normals will be equal to zero, althought
             * these normals aren't the final normals they are used to generate the final normals.*/
            points[0] = new BezierPoint(nodeA.Point,
                GetBezierTangent(p0, p1, p2, p3, 0).normalized,
                GetBezeirNormal(nodeA.Forward.normalized, nodeA.Angle));

            for (int i = 1; i < resolution; i++) {

                float t = i / (resolution - 1f);

                points[i].point = GetBezierPoint(p0, p1, p2, p3, t);
                points[i].tangent = GetBezierTangent(p0, p1, p2, p3, t).normalized;

                Quaternion difference = Quaternion.FromToRotation(points[i - 1].tangent, points[i].tangent);
                points[i].normal = difference * points[i - 1].normal;
            }

            /* Converting the correct start and finish normals to local space so we can calculate the 
             * rotation between them along the local z axis so we can apply it to the normals.*/
            Quaternion startRotation = Quaternion.LookRotation(points[0].tangent, points[0].normal);
            Vector3 startNormal = Quaternion.Inverse(startRotation) * points[0].normal;

            Quaternion finishRotation = Quaternion.LookRotation(points[^1].tangent, points[^1].normal);
            Vector3 finishNormal = Quaternion.Inverse(finishRotation) * GetBezeirNormal(-nodeB.Backward.normalized, nodeB.Angle);

            /* The normals should only rotate on the relative z axis however because the quaternion could
             * decide to rotate along the x or z axis the values are added to prevent it from ever rotating
             * along the wrong axis.*/
            Vector3 reflectedEuler = Quaternion.FromToRotation(startNormal, finishNormal).eulerAngles;
            Quaternion reflectedAngle = Quaternion.Euler(0, 0, reflectedEuler.x + reflectedEuler.z);

            for (int i = 1; i < resolution; i++) {

                float t = i / (resolution - 1f);
                Vector3 reflectedNormals = Quaternion.Slerp(Quaternion.identity, reflectedAngle, t) * startNormal;

                Quaternion rotation = Quaternion.LookRotation(points[i].tangent, points[i].normal);
                points[i].normal = rotation * reflectedNormals;
            }

            return points;
        }
        private static BezierPoint[] CalculateEqualDistancePoints(BezierPoint[] points, float distance)
        {
            List<BezierPoint> results = new List<BezierPoint>() { points[0] };

            float totalDistance = 0;
            float targetDistance = distance;

            for (int i = 0; i < points.Length - 1; i++) {

                float pointDistance = Vector3.Distance(points[i].point, points[i + 1].point);

                while(totalDistance + pointDistance >= targetDistance) {

                    float percent = (targetDistance - totalDistance) / pointDistance;
                    results.Add(BezierPoint.Lerp(points[i], points[i + 1], percent));

                    targetDistance += distance;
                }

                totalDistance += pointDistance;
            }

            results.Add(points[^1]); 
            return results.ToArray();
        }
        private  static float ApproximateDistance(BezierNode nodeA, BezierNode nodeB)
        {
            return nodeA.Forward.magnitude + nodeB.Backward.magnitude +
                Vector3.Distance(nodeA.ForwardPoint, nodeB.BackwardPoint); ;
        }
        private static Vector3 GetBezierPoint(Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3, float t)
        {
            float it = 1 - t;
            float it2 = it * it;
            float t2 = t * t;

            return it2 * it * p0 +
                3 * it2 * t * p1 +
                3 * it * t2 * p2 +
                t2 * t * p3;
        }
        private static Vector3 GetBezierTangent(Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3, float t)
        {
            float it = 1 - t;
            float t2 = t * t;

            return 3 * it * it * (p1 - p0) +
                6 * it * t * (p2 - p1) +
                3 * t2 * (p3 - p2);
        }
        private static Vector3 GetBezeirNormal(Vector3 tangent, float angle)
        {
            if (Mathf.Abs(Vector3.Dot(tangent, Vector3.up)) == 1)
                return Quaternion.Euler(0, angle, 0) * Vector3.back;

            // ToDo: This might cause issues.
            Quaternion orientation = Quaternion.LookRotation(tangent);
            Vector3 bitangent = Vector3.Cross(tangent, orientation * Quaternion.Euler(0, 0, angle) * Vector3.up);
            return Vector3.Cross(bitangent, tangent).normalized;
        }
        #endregion
    }
}
