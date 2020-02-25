using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using AnyCAD.Platform;
namespace StpViewer
{
    public partial class MainForm : Form
    {
        private AnyCAD.Presentation.RenderWindow3d renderView = null;

        public MainForm()
        {
            InitializeComponent();

            this.renderView = new AnyCAD.Presentation.RenderWindow3d();
            this.renderView.Location = new System.Drawing.Point(0, 0);
            this.renderView.Size = this.Size;
            this.renderView.TabIndex = 1;
            this.splitContainer1.Panel2.Controls.Add(this.renderView);

            GlobalInstance.EventListener.OnSelectElementEvent += OnSelectionChanged;
        }

        void OnSelectionChanged(SelectionChangeArgs args)
        {

        }

        private void MainForm_Resize(object sender, EventArgs e)
        {
            if (renderView != null)
                renderView.Size = this.splitContainer1.Panel2.Size;
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            renderView.ExecuteCommand("ShadeWithEdgeMode");
            renderView.ShowCoordinateAxis(true);
            //renderView.SetPickMode((int)EnumPickMode.RF_Edge);
            this.renderView.RequestDraw();
        }

        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFileDialog dlg = new OpenFileDialog();
            dlg.Filter = "STEP File(*.stp;*.step)|*.stp;*.step|All Files(*.*)|*.*";

            if (DialogResult.OK == dlg.ShowDialog())
            {
                this.treeViewStp.Nodes.Clear();
                this.renderView.ClearScene();

                CADBrower browser = new CADBrower(this.treeViewStp, this.renderView);
                AnyCAD.Exchange.StepReader reader = new AnyCAD.Exchange.StepReader();
                reader.Read(dlg.FileName, browser);
            }

            renderView.FitAll();
        }

        private void treeViewStp_AfterSelect(object sender, TreeViewEventArgs e)
        {
           
        }

        private void openIGESToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFileDialog dlg = new OpenFileDialog();
            dlg.Filter = "IGES File(*.iges;*.igs)|*.igs;*.igesp|All Files(*.*)|*.*";

            if (DialogResult.OK == dlg.ShowDialog())
            {
                this.treeViewStp.Nodes.Clear();
                this.renderView.ClearScene();

                CADBrower browser = new CADBrower(this.treeViewStp, this.renderView);
                AnyCAD.Exchange.IgesReader reader = new AnyCAD.Exchange.IgesReader();
                reader.Read(dlg.FileName, browser);
            }

            renderView.View3d.FitAll();
        }

        private void saveImageToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SaveFileDialog dlg = new SaveFileDialog();
            dlg.Filter = "Image File(*.png;*.jpg)|*.png;*.jpg|All Files(*.*)|*.*";

            if (DialogResult.OK == dlg.ShowDialog())
            {
                renderView.CaptureImage(dlg.FileName);
            }
        }

        private void zoomFitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            renderView.FitAll();
        }
    }


}
