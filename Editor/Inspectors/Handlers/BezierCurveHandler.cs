using SimpleSplines;
using SimpleSplines.Nodes;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;

namespace SimpleSplinesEditor.Inspectors.Handlers
{
    internal class BezierCurveHandler
    {
        #region Data
        private readonly SerializedObject _serializedObject;

        private readonly FieldInfo _curveNodes, _curvePoints;
        #endregion

        #region Constructor
        public BezierCurveHandler(SerializedObject serializedObject)
        {
            _serializedObject = serializedObject;

            BindingFlags bindingFlags = (BindingFlags)int.MaxValue;

            _curveNodes = typeof(BezierCurve).GetField("_curveNodes", bindingFlags);
            _curvePoints = typeof(BezierCurve).GetField("_curvePoints", bindingFlags);
        }
        #endregion

        #region Public Functions
        public IList<BezierNode> GetBezierNodes()
        {
            return GetFieldValue<BezierNode>(_curveNodes);
        }
        public void SetBezierNodes(IList<BezierNode> nodes)
        {
            _curveNodes.SetValue(_serializedObject.targetObject, nodes);
        }

        public IList<BezierPoint> GetBezierPoints()
        {
            return GetFieldValue<BezierPoint>(_curvePoints);
        }
        public void SetBezierPoints(IList<BezierPoint> points)
        {
            _curvePoints.SetValue(_serializedObject.targetObject, points);
        }
        #endregion

        #region Private Functions
        private IList<T> GetFieldValue<T>(FieldInfo fieldInfo)
        {
            return (IList<T>)fieldInfo.GetValue(_serializedObject.targetObject);
        }
        #endregion
    }
}
