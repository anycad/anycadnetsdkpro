using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AnyCAD.Platform;

namespace AnyCAD.Basic
{
    class DrawCurveOnSurfaceEditor : AnyCAD.Platform.CustomEditor
    {
        System.Collections.Generic.List<Vector3> m_Points = new System.Collections.Generic.List<Vector3>();


        Vector3 GetPointOnShape(int cx, int cy)
        {
            Platform.Vector3 hitPoint = null;
            Renderer rv = GetRenderer();

            PickHelper pickHelper = new PickHelper();
            pickHelper.Initialize(rv);

            if (pickHelper.Pick(cx, cy))
            {
                hitPoint = pickHelper.GetPointOnShape();
            }

            return hitPoint;
        }

        public override void OnStartEvent()
        {
        }

        public override void OnExitEvent()
        {
            this.RemoveAllTempNodes();
        }

        public override void OnButtonDownEvent(InputEvent evt)
        {
            if (evt.IsRButtonDown())
            {
                this.Exit(1);
                return;
            }

            Vector3 pt = GetPointOnShape((int)evt.GetMousePosition().X, (int)evt.GetMousePosition().Y);
            if (pt == null)
                return;

            m_Points.Add(pt);

            if (m_Points.Count > 1)
            {
                TopoShape spline = GlobalInstance.BrepTools.MakeSpline(m_Points);
                if (spline != null)
                {
                    SceneNode node = GlobalInstance.TopoShapeConvert.ToEdgeNode(spline, 0.0f);

                    this.RemoveAllTempNodes();
                    this.ShowTempNode(node);
                    GetRenderer().RequestDraw(1);
                }
            }
        }

        public override void OnButtonUpEvent(InputEvent evt)
        {

        }

        public override void OnMouseMoveEvent(InputEvent evt)
        {

        }
    }
}
