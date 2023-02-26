using SimpleSplines;
using SimpleSplines.Nodes;
using SimpleSplinesEditor.Inspectors.Handlers;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace SimpleSplinesEditor.Inspectors
{
    [CustomEditor(typeof(BezierCurve))]
    internal class BezierCurveInspector : Editor
    {
        #region Data
        private BezierCurveHandler _handler;

        private BezierNodeHandler _selectedNode;
        private int _selectedIndex = -1;
        #endregion

        #region Unity Functions
        private void OnEnable()
        {
            _handler = new BezierCurveHandler(serializedObject);
        }

        public override void OnInspectorGUI()
        {
            serializedObject.DrawPropertyField("_curveNodes");

            serializedObject.DrawPropertyField("Connected");
            serializedObject.DrawPropertyField("_sampleDistance");

            if (serializedObject.hasModifiedProperties) {
                serializedObject.ApplyModifiedProperties();
            }
        }
        private void OnSceneGUI()
        {
            Transform transform = ((BezierCurve)target).transform;

            IList<BezierNode> nodes = _handler.GetBezierNodes();

            for (int i = 0; i < nodes.Count; i++)
                if (BezierNodeHandler.DrawBezierHandles(nodes[i], transform)) {

                    _selectedNode = new BezierNodeHandler(nodes[i], transform);
                    _selectedIndex = i;
                }

            if (_selectedNode != null && _selectedIndex < nodes.Count) {

                /* Since the BezierNode is a value type any changes being made on Unity's
                 * end will not be reflected in the cached value so every frame it is
                 * reassigned to prevent any desynchronization.*/
                _selectedNode.targetNode = nodes[_selectedIndex];

                if (_selectedNode.OnSceneGUI() == true) {

                    ((BezierCurve)target).SendMessage("OnValidate", null);
                    serializedObject.MarkAsDirty("Update Points");
                    nodes[_selectedIndex] = _selectedNode.targetNode;
                }
            }

            IList<BezierPoint> points = _handler.GetBezierPoints();

            /* When the bezier curve is first created the points haven't
             * been generated so this creates them in such a scenario.*/
            if(points?.Count < 1 && nodes?.Count > 1) {

                ((BezierCurve)target).SendMessage("OnValidate", null);
                serializedObject.MarkAsDirty("Initialize Curve");
            }

            for (int i = 0; i < points?.Count; i += 1) {

                Vector3 point = transform.TransformPoint(points[i].point);

                Handles.color = new Color(.1f, 1f, .3f, .5f);
                Handles.DrawLine(point, point + (transform.rotation * points[i].normal * .1f), 1.5f);

                Handles.color = new Color(1f, .1f, .3f, .5f);
                Handles.DrawLine(point, point + (transform.rotation * points[i].bitangent * .1f), 1.5f);
            }
        }
        #endregion
    }
}
