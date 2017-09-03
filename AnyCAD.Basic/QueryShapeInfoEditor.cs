using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AnyCAD.Platform;

namespace AnyCAD.Basic
{
    class QueryShapeInfoEditor : AnyCAD.Platform.CustomEditor
    {
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
            this.RemoveAllTempNodes();

            Renderer rv = GetRenderer();

            PickHelper pickHelper = new PickHelper();
            pickHelper.Initialize(rv);

            if (!pickHelper.Pick(evt.GetMousePosition()))
                return;


            TopoShape shape = pickHelper.GetGeometry();
            GeomSurface surface = new GeomSurface();
            if (!surface.Initialize(shape))
                return;

            IntersectionLineSurface intersector = new IntersectionLineSurface();
            intersector.SetSurface(shape);
            if (!intersector.Perform(rv.ComputeScreenRay(evt.GetMousePosition())))
            {
                return;
            }

            int nCount = intersector.GetPointCount();
            if(nCount < 1)
                return;

            double u = intersector.GetParameterU(1);
            double v = intersector.GetParameterV(1);

            Vector3 pt = surface.Value(u, v);
            Vector3 normal = surface.GetNormal(u, v);

            LineNode lineNode = new LineNode();
            lineNode.Set(pt, pt + normal);

            this.ShowTempNode(lineNode);
            rv.RequestDraw(1);
        }

        public override void OnButtonUpEvent(InputEvent evt)
        {

        }

        public override void OnMouseMoveEvent(InputEvent evt)
        {

        }
    }
}
