using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AnyCAD.Platform;

namespace AnyCAD.Basic
{
    class MoveNodeEditor : AnyCAD.Platform.CustomEditor
    {
        private Vector3 m_StartPos = new Vector3();
        private EditStep m_Step = 0;
        private SceneNode m_TargetNode = null;
        private Matrix4 m_InitTrf = null;

        enum EditStep
        {
            ES_PickNode,
            ES_BeginMove,
        };

        public override void OnStartEvent()
        {
            
        }

        public override void OnExitEvent()
        {

        }

        public override void OnButtonDownEvent(InputEvent evt)
        {
            Renderer renderer = GetRenderer();
            if (m_Step == EditStep.ES_PickNode)
            {
                if (renderer.Select(true) > 0)
                {
                    SelectedShapeQuery query = new SelectedShapeQuery();
                    renderer.QuerySelection(query);
                    m_TargetNode = query.GetRootNode();
                    if (m_TargetNode != null)
                    {
                        m_Step = EditStep.ES_BeginMove;
                        m_InitTrf = m_TargetNode.GetTransform();
                    }
                 }
            }
            else
            {
                m_StartPos = ToWorldPoint(evt.GetMousePosition());
            }
        }

        public override void OnButtonUpEvent(InputEvent evt)
        {
            m_Step = EditStep.ES_PickNode;
            GetRenderer().ClearSelection();
            GetRenderer().RequestDraw(1);
        }

        public override void OnMouseMoveEvent(InputEvent evt)
        {
            if (m_Step == EditStep.ES_PickNode)
            {
                Vector2 pt = evt.GetMousePosition();
                GetRenderer().Highlight((int)pt.X, (int)pt.Y);
            }
            else
            {
                Vector3 endPt = ToWorldPoint(evt.GetMousePosition());
                Matrix4 trf = GlobalInstance.MatrixBuilder.MakeTranslate(endPt - m_StartPos);
                m_TargetNode.SetTransform(GlobalInstance.MatrixBuilder.Multiply(m_InitTrf, trf));

                GetRenderer().RequestDraw(1);
            }
        }
    }
}
