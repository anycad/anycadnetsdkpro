using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using AnyCAD.Platform;

namespace DxfViewer
{
    public partial class MainForm : Form
    {
        private AnyCAD.Presentation.RenderWindow3d renderView = null;
        private ElementId mBeginId = new ElementId();
        private ElementId mEndId = new ElementId();

        public MainForm()
        {
            InitializeComponent();

            this.renderView = new AnyCAD.Presentation.RenderWindow3d();
            this.renderView.Size = this.panel3d.ClientSize;
            this.renderView.TabIndex = 1;
            panel3d.Controls.Add(this.renderView);
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            renderView.ShowWorkingGrid(false);
            renderView.ExecuteCommand("ShadeWithEdgeMode");
            renderView.ShowCoordinateAxis(false);

            ColorValue clr = new ColorValue(33f / 255f, 40f / 255f, 48f / 255f, 1);
            renderView.SetBackgroundColor(clr, clr, clr);

            Renderer renderer = renderView.Renderer;

            // Customize the Axis
            ScreenWidget coodinateNode = new ScreenWidget();
            AxesWidget axesNode = new AxesWidget();
            axesNode.SetArrowText((int)EnumAxesDirection.Axes_Z, "");
            coodinateNode.SetNode(axesNode);
            coodinateNode.SetWidgetPosition(2);     
            renderer.SetCoordinateWidget(coodinateNode);

            // Set the fixed Top View
            renderer.SetStandardView(EnumStandardView.SV_Top);
            renderer.SetViewType(EnumStandardView.SV_Top);

            this.renderView.RequestDraw();

        }

        private void MainForm_Resize(object sender, EventArgs e)
        {
            if (renderView != null)
                renderView.Size = this.panel3d.ClientSize;
        }

        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFileDialog dlg = new OpenFileDialog();
            dlg.Filter = "DXF (*.dxf)|*.dxf";

            if (DialogResult.OK == dlg.ShowDialog())
            {
                AnyCAD.Exchange.DxfReader reader = new AnyCAD.Exchange.DxfReader();
                renderView.ClearScene();
                AnyCAD.Exchange.ShowShapeReaderContext context = new AnyCAD.Exchange.ShowShapeReaderContext(renderView.SceneManager);
                context.NextShapeId = mBeginId;
                if (reader.Read(dlg.FileName, context, false))
                {
                    renderView.RequestDraw();
                    mEndId = context.NextShapeId;
                }

            }

            renderView.View3d.FitAll();
        }

        private void pDFToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SaveFileDialog dlg = new SaveFileDialog();
            dlg.Filter = "PDF (*.pdf)|*.pdf";
            if (DialogResult.OK == dlg.ShowDialog())
            {
                renderView.Renderer.Print(dlg.FileName);
            }

        }

        private void imageToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SaveFileDialog dlg = new SaveFileDialog();
            dlg.Filter = "Image File (*.jpg;*.png)|*.jpg;*.png";
            if (DialogResult.OK == dlg.ShowDialog())
            {
                renderView.CaptureImage(dlg.FileName);
            }
        }

        private void dToolStripMenuItem_Click(object sender, EventArgs e)
        {
            renderView.ExecuteCommand("IsoView");
            renderView.View3d.FitAll();
        }

        private void exportIgesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SceneManager sceneManager = renderView.SceneManager;



            TopoShapeGroup group = new TopoShapeGroup();
            for (ElementId ii = mBeginId; ii < mEndId; ++ii)
            {
                SceneNode node = sceneManager.FindNode(ii);
                if (node != null)
                {
                    TopoShapeGroup shapeGroup = GlobalInstance.TopoShapeConvert.ToTopoShape(node);
                    if (shapeGroup != null)
                    {
                        for (Int32 jj = 0, len = shapeGroup.Size(); jj < len; ++ii)
                        {
                            TopoShape shape = shapeGroup.GetTopoShape(jj);
                            Matrix4 trf = GlobalInstance.MatrixBuilder.MakeRotation(90, Vector3.UNIT_X);
                            shape = GlobalInstance.BrepTools.Transform(shape, trf);
                            group.Add(shape);
                        }
                    }
                }
            }

            if (group.Size() > 0)
            {
                SaveFileDialog dlg = new SaveFileDialog();
                dlg.Filter = "IGES File (*.igs;*.iges)|*.igs;*.iges";
                if (DialogResult.OK != dlg.ShowDialog())
                {
                    return;
                }
                TopoDataExchangeIges igsWriter = new TopoDataExchangeIges();
                igsWriter.Write(group, new AnyCAD.Platform.Path(dlg.FileName));
            }
            else
            {
                MessageBox.Show("No shape to save!");
            }

        }

        private void dToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            renderView.ExecuteCommand("TopView");
            this.renderView.RequestDraw();
        }

    }
}
