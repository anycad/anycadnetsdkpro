using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AnyCAD.Platform;

namespace AnyCAD.Basic
{
    class DrawLineEditor : AnyCAD.Platform.CustomEditor
    {
        private Vector3 m_StartPos = new Vector3();
        private int m_Step = 0;
        LineNode tempLineNode = new LineNode();

        enum EditStep
        {
            ES_Init,
            ES_Drawing,
            ES_Finish,
        };

        public override void OnStartEvent()
        {
            LineStyle lineStyle = new LineStyle();
            lineStyle.SetPatternStyle((int)EnumLinePattern.LP_DashedLine);
            lineStyle.SetColor(100, 0, 100);

            tempLineNode.SetLineStyle(lineStyle);

            this.ShowTempNode(tempLineNode);
            GetRenderer().RequestDraw(1);
        }

        public override void OnExitEvent()
        {
            this.RemoveAllTempNodes();
        }

        public override void OnButtonDownEvent(InputEvent evt)
        {
            Renderer renderer = GetRenderer();
            m_Step += 1;
            if (m_Step == (int)EditStep.ES_Finish)
            {
                m_Step = (int)EditStep.ES_Init;

                LineNode lineNode = new LineNode();
                lineNode.Set(m_StartPos, ToWorldPoint(evt.GetMousePosition()));

                renderer.GetSceneManager().AddNode(lineNode);
                tempLineNode.SetVisible(false);

                renderer.RequestDraw(1);
                return;
            }

            if (evt.IsLButtonDown())
            {
                m_StartPos = ToWorldPoint(evt.GetMousePosition());
                tempLineNode.SetVisible(true);
                renderer.RequestDraw(1);
            }
        }

        public override void OnButtonUpEvent(InputEvent evt)
        {

        }

        public override void OnMouseMoveEvent(InputEvent evt)
        {
            if (m_Step == (int)EditStep.ES_Drawing)
            {
                Renderer renderer = GetRenderer();
                tempLineNode.Set(m_StartPos, ToWorldPoint(evt.GetMousePosition()));
                renderer.RequestDraw(1);
            }
        }
    }
}
