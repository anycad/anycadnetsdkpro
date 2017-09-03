using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AnyCAD.Platform;

namespace AnyCAD.Basic
{
    class MeasureDistanceEditor : AnyCAD.Platform.CustomEditor
    {
        private int m_Step = 0;
        Vector3 m_Pt1 = null;

        enum EditStep
        {
            ES_Init,
            ES_Drawing,
            ES_Finish,
        };

        Vector3 GetSelectedShape(int cx, int cy)
        {
            Renderer rv = GetRenderer();

            PickHelper ph = new PickHelper();
            ph.Initialize(rv);
            if (!ph.Pick(cx, cy))
                return null;

            return ph.GetPointOnShape();
        }

        public override void OnStartEvent()
        {
        }

        public override void OnExitEvent()
        {
  
        }

        public override void OnButtonDownEvent(InputEvent evt)
        {
            Vector3 pt = GetSelectedShape((int)evt.GetMousePosition().X, (int)evt.GetMousePosition().Y);
            if (pt == null)
                return;

            Renderer renderer = GetRenderer();
            m_Step += 1;
            if (m_Step == (int)EditStep.ES_Finish)
            {
                m_Step = (int)EditStep.ES_Init;

                LineNode lineNode = new LineNode();
                lineNode.SetShowText(true);
                lineNode.Set(m_Pt1, pt);

                renderer.GetSceneManager().AddNode(lineNode);
                renderer.RequestDraw(1);
                return;
            }

            if (evt.IsLButtonDown())
            {

                m_Pt1 = pt;
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
