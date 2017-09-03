using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AnyCAD.Platform;

namespace AnyCAD.Designer
{
    class EditStep
    {
        public const int ES_Init = 0;
        public const int ES_Drawing = 1;
        public const int ES_Finish = 2;
    };

    delegate void AddLineEvent(Vector3 startPt, Vector3 endPt);

    class DrawLineEditor : AnyCAD.Platform.CustomEditor
    {
        Vector3 m_StartPos = new Vector3();       
        LineNode tempLineNode = new LineNode();

        int m_Step = EditStep.ES_Init;
        AddLineEvent OnAddLineEvent;

        public DrawLineEditor(AddLineEvent callback)
        {
            SetId("AD::DrawLine");
            OnAddLineEvent = callback;

            LineStyle lineStyle = new LineStyle();
            lineStyle.SetPatternStyle((int)EnumLinePattern.LP_DashedLine);
            lineStyle.SetColor(255, 0, 100);
            lineStyle.SetLineWidth(2);

            tempLineNode.SetLineStyle(lineStyle);
        }

        public override void OnStartEvent()
        {            
            m_Step = EditStep.ES_Init;

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
            if (m_Step == EditStep.ES_Finish)
            {               
                LineNode lineNode = new LineNode();

                // Screen position to world position
                Vector3 endPos = ToWorldPoint(evt.GetMousePosition());
                if (OnAddLineEvent != null)
                {
                    OnAddLineEvent(m_StartPos, endPos);
                }

                // Start the next line
                m_Step = EditStep.ES_Drawing;
                m_StartPos = endPos;
                lineNode.Set(m_StartPos, endPos);

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
