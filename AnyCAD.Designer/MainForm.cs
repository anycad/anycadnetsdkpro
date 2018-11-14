using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using AnyCAD.Platform;

namespace AnyCAD.Designer
{
    public partial class MainForm : Form
    {
        private AnyCAD.Presentation.RenderWindow3d renderView;

        private int ShapeId = 100;
        private DrawLineEditor mDrawLineEditor = null;

        private LineStyle mDefaultLineStyle;

        public MainForm()
        {
            InitializeComponent();

            this.renderView = new AnyCAD.Presentation.RenderWindow3d();

            System.Drawing.Size size = this.panel1.ClientSize;
            this.renderView.Size = size;

            this.renderView.TabIndex = 1;
            this.panel1.Controls.Add(this.renderView);

            GlobalInstance.EventListener.OnChangeCursorEvent += OnChangeCursor;
            GlobalInstance.EventListener.OnSelectElementEvent += OnSelectElement;

        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            mDefaultLineStyle = new LineStyle();
            mDefaultLineStyle.SetColor(255, 0, 0);
            mDefaultLineStyle.SetLineWidth(2);

            renderView.ExecuteCommand("PickClearMode", "Single");
        }

        private void OnSelectElement(SelectionChangeArgs args)
        {
            if (!args.IsHighlightMode())
            {
                SelectedShapeQuery query = new SelectedShapeQuery();
                renderView.QuerySelection(query);
                var ids = query.GetIds();
                if(ids.Length > 0)
                {
                    var shape = query.GetGeometry();
             
                    if (shape != null)
                    {                    
                        GeomCurve curve = new GeomCurve();
                        if (curve.Initialize(shape))
                        {
                            TopoShapeProperty property = new TopoShapeProperty();
                            property.SetShape(shape);
                            var str = String.Format("Length:{0}", property.EdgeLength());
                            toolStripStatusLabel2.Text = str;
                            
                            return;
                        }
                    }
                }
                else
                {
                    toolStripStatusLabel2.Text = "";
                }
            }            
        }

        private void OnChangeCursor(String commandId, String cursorHint)
        {

            if (cursorHint == "Pan")
            {
                this.renderView.Cursor = System.Windows.Forms.Cursors.SizeAll;
            }
            else if (cursorHint == "Orbit")
            {
                this.renderView.Cursor = System.Windows.Forms.Cursors.Hand;
            }
            else if (cursorHint == "Cross")
            {
                this.renderView.Cursor = System.Windows.Forms.Cursors.Cross;
            }
            else
            {
                if (commandId == "Pick")
                {
                    this.renderView.Cursor = System.Windows.Forms.Cursors.Arrow;
                }
                else
                {
                    this.renderView.Cursor = System.Windows.Forms.Cursors.Default;
                }
            }

        }

        private void MainForm_SizeChanged(object sender, EventArgs e)
        {
            if (renderView != null)
            {

                System.Drawing.Size size = this.panel1.ClientSize;
                renderView.Size = size;
            }
        }

        
        private void OnAddLine(Vector3 startPt, Vector3 endPt)
        {
            var shape = GlobalInstance.BrepTools.MakeLine(startPt, endPt);
            if (shape == null)
                return;

            var node = renderView.ShowGeometry(shape, ++ShapeId);
            node.SetLineStyle(mDefaultLineStyle);

        }
        private void lineToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if(mDrawLineEditor == null)
                mDrawLineEditor = new DrawLineEditor(OnAddLine);
            renderView.ActiveEditor(mDrawLineEditor);
            toolStripStatusLabel2.Text = "Draw Line";
        }

        private void pickToolStripMenuItem_Click(object sender, EventArgs e)
        {
            renderView.ExecuteCommand("Pick", "");
        }

        private void dToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //renderView.ShowCoordinateAxis(false);
            //renderView.ShowWorkingGrid(false);

            //ColorValue clr = new ColorValue(33f / 255f, 40f / 255f, 48f / 255f, 1);
            //renderView.SetBackgroundColor(clr, clr, clr);

            Renderer renderer = renderView.Renderer;

            //// Customize the Axis
            //ScreenWidget coodinateNode = new ScreenWidget();
            //AxesWidget axesNode = new AxesWidget();
            //axesNode.SetArrowText((int)EnumAxesDirection.Axes_Z, "");
            //coodinateNode.SetNode(axesNode);
            //coodinateNode.SetWidgetPosition(2);
            //renderer.SetCoordinateWidget(coodinateNode);

            // Set the fixed Top View
            renderer.SetStandardView(EnumStandardView.SV_Top);
            renderer.SetViewType(EnumStandardView.SV_Top);
        }

        void UpdateView()
        {
            this.renderView.RequestDraw();
        }

        TubeSystem tubeSystem = null;
        private void tubeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (tubeSystem == null)
            {
                tubeSystem = new TubeSystem();
                tubeSystem.Init(this.renderView);

                renderView.RenderTick += new Presentation.RenderEventHandler(tubeSystem.OnTimer);
                tubeSystem.OnUpdate += UpdateView;
            }
        }

        private void MainForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            
        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            mDrawLineEditor = null;
        }
    }
}
