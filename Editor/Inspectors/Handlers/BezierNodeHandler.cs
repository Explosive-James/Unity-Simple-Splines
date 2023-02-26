using SimpleSplines.Nodes;
using UnityEditor;
using UnityEngine;

namespace SimpleSplinesEditor.Inspectors.Handlers
{
    public enum NodeHandle { Forward, Middle, Backward }

    internal class BezierNodeHandler
    {
        #region Data
        public BezierNode targetNode;
        private Transform _transform;

        private NodeHandle _selectedNode = NodeHandle.Middle;
        #endregion

        #region Properties
        public NodeHandle SelectedNode => _selectedNode;
        #endregion

        #region Constructor
        public BezierNodeHandler(BezierNode targetNode, Transform transform)
        {
            this.targetNode = targetNode;
            _transform = transform;
        }
        #endregion

        #region Public Functions
        public bool OnSceneGUI()
        {
            if (EditorExtensions.DrawHandleButton(_transform.TransformPoint(targetNode.ForwardPoint), .055f, .2f)) {
                _selectedNode = NodeHandle.Forward;
            }
            if (EditorExtensions.DrawHandleButton(_transform.TransformPoint(targetNode.BackwardPoint), .055f, .2f)) {
                _selectedNode = NodeHandle.Backward;
            }

            EditorGUI.BeginChangeCheck();
            Vector3 handlePosition = _transform.TransformPoint(GetHandlePosition(_selectedNode));

            handlePosition = Handles.PositionHandle(handlePosition, Quaternion.identity);
            SetHandlePosition(_selectedNode, _transform.InverseTransformPoint(handlePosition));

            return EditorGUI.EndChangeCheck();
        }
        public static bool DrawBezierHandles(BezierNode node, Transform transform)
        {
            EditorExtensions.DrawHandleLine(transform.TransformPoint(node.Point),
                transform.TransformPoint(node.ForwardPoint), new Color(.3f, .1f, 1f));
            EditorExtensions.DrawHandleLine(transform.TransformPoint(node.Point),
                transform.TransformPoint(node.BackwardPoint), new Color(1f, .1f, .3f));

            return EditorExtensions.DrawHandleButton(transform.TransformPoint(node.Point), .075f, .2f);
        }
        #endregion

        #region Private Functions
        private Vector3 GetHandlePosition(NodeHandle handle)
        {
            return handle switch {

                NodeHandle.Middle => targetNode.Point,
                NodeHandle.Forward => targetNode.ForwardPoint,
                NodeHandle.Backward => targetNode.BackwardPoint,

                _ => throw new System.NotImplementedException()
            };
        }
        private void SetHandlePosition(NodeHandle handle, Vector3 position)
        {
            switch (handle) {

                case NodeHandle.Middle:
                    targetNode.Point = position;
                    break;

                case NodeHandle.Forward:
                    targetNode.Forward = position - targetNode.Point;
                    break;
                case NodeHandle.Backward:
                    targetNode.Backward = position - targetNode.Point;
                    break;

                default:
                    throw new System.NotImplementedException();
            }
        }
        #endregion
    }
}
