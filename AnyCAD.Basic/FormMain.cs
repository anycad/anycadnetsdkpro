using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using AnyCAD.Platform;
using System.IO;

namespace AnyCAD.Basic
{
    public partial class FormMain : Form
    {
        // Render Control
        private Presentation.RenderWindow3d renderView;
        private int shapeId = 100;
        public FormMain()
        {
            InitializeComponent();
            //MessageBox.Show("AnyCAD .Net SDK PRO");
            // 
            // Create renderView
            // 
            this.renderView = new AnyCAD.Presentation.RenderWindow3d();

            //this.renderView.Location = new System.Drawing.Point(0, 27);
            System.Drawing.Size size = this.panel1.ClientSize;
            this.renderView.Size = size;
            
            this.renderView.TabIndex = 1;
            this.panel1.Controls.Add(this.renderView);

            this.renderView.MouseClick += new System.Windows.Forms.MouseEventHandler(this.OnRenderWindow_MouseClick);

            GlobalInstance.EventListener.OnChangeCursorEvent += OnChangeCursor;
            GlobalInstance.EventListener.OnSelectElementEvent += OnSelectElement;


        }

        private void OnSelectElement(SelectionChangeArgs args)
        {
            if (!args.IsHighlightMode())
            {
                SelectedShapeQuery query = new SelectedShapeQuery();
                renderView.QuerySelection(query);
                var shape = query.GetGeometry();
                if (shape != null)
                {
                    GeomCurve curve = new GeomCurve();
                    if (curve.Initialize(shape))
                    {
                        TopoShapeProperty property = new TopoShapeProperty();
                        property.SetShape(shape);
                        Console.WriteLine("Edge Length {0}", property.EdgeLength());
                    }
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

        private void FormMain_Load(object sender, EventArgs e)
        {
            
        }
        private void FormMain_SizeChanged(object sender, EventArgs e)
        {
            if (renderView != null)
            {

                System.Drawing.Size size = this.panel1.ClientSize;
                renderView.Size = size;
            }
        }

        private void sphereToolStripMenuItem_Click(object sender, EventArgs e)
        {
            TopoShape sphere = GlobalInstance.BrepTools.MakeSphere(new Vector3(0, 0, 40), 40);
            SceneNode node = renderView.ShowGeometry(sphere, ++shapeId);


            Texture texture = new Texture();
            texture.SetName("mytexture3");
            texture.SetFilePath(new AnyCAD.Platform.Path("#test\\land_ocean_ice_2048.jpg"));

            FaceStyle style = new FaceStyle();
            style.SetTexture(0, texture);

            node.SetFaceStyle(style);
        }

        private void boxToolStripMenuItem_Click(object sender, EventArgs e)
        {
            TopoShape box = GlobalInstance.BrepTools.MakeBox(new Vector3(40, -20, 0), new Vector3(0, 0, 1), new Vector3(30, 40, 60));

            SceneNode sceneNode = renderView.ShowGeometry(box, ++shapeId);

            FaceStyle style = new FaceStyle();
            style.SetColor(new ColorValue(0.5f, 0.3f, 0, 0.5f));

            Texture texture = new Texture();
            texture.SetName("mytexture2");
            texture.SetFilePath(new AnyCAD.Platform.Path("E:\\198.png"));
            style.SetTexture(0, texture);

            style.SetTransparent(true);
            sceneNode.SetFaceStyle(style);
        }

        private void cylinderToolStripMenuItem_Click(object sender, EventArgs e)
        {
            TopoShape cylinder = GlobalInstance.BrepTools.MakeCylinder(new Vector3(180, 0, 0), new Vector3(0, 0, 1), 20, 100, 360);

            Matrix4 mat= GlobalInstance.MatrixBuilder.MakeRotation(90, Vector3.UNIT_X);
            cylinder = GlobalInstance.BrepTools.Transform(cylinder, mat);
            SceneNode sceneNode = renderView.ShowGeometry(cylinder, ++shapeId);
            FaceStyle style = new FaceStyle();
            
            Texture texture = new Texture();
            texture.SetName("mytexture2");
            texture.SetFilePath(new AnyCAD.Platform.Path(@"E:\Depot\sdk.net.pro.2\weixin.jpg"));
            style.SetTexture(0, texture);

            sceneNode.SetFaceStyle(style);

            sceneNode.SetPickable(false);
        }

        private void coneToolStripMenuItem_Click(object sender, EventArgs e)
        {
            TopoShape cylinder = GlobalInstance.BrepTools.MakeCone(new Vector3(100, 0, 0), new Vector3(0, 0, 1), 20, 100, 40, 315);
            SceneNode node = renderView.ShowGeometry(cylinder, ++shapeId);

            for (int i = 1; i < 10; ++i)
            {
                Matrix4 trf = GlobalInstance.MatrixBuilder.MakeTranslate(i * 100, 0, 0);
                SceneNode aNode = renderView.ShowGeometry(cylinder, ++shapeId);
                aNode.SetTransform(trf);
            }

            Texture texture = new Texture();
            texture.SetName("mytexture");
            texture.SetFilePath(new AnyCAD.Platform.Path("#Terrain\\BeachStones.jpg"));

            FaceStyle style = new FaceStyle();
            style.SetTexture(0, texture);

            node.SetFaceStyle(style);
        }

        private void extrudeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            int size = 20;
            // Create the outline edge
            TopoShape arc = GlobalInstance.BrepTools.MakeArc3Pts(new Vector3(-size, 0, 0), new Vector3(size, 0, 0), new Vector3(0, size, 0));
            TopoShape line1 = GlobalInstance.BrepTools.MakeLine(new Vector3(-size, -size, 0), new Vector3(-size, 0, 0));
            TopoShape line2 = GlobalInstance.BrepTools.MakeLine(new Vector3(size, -size, 0), new Vector3(size, 0, 0));
            TopoShape line3 = GlobalInstance.BrepTools.MakeLine(new Vector3(-size, -size, 0), new Vector3(size, -size, 0));

            TopoShapeGroup shapeGroup = new TopoShapeGroup();
            shapeGroup.Add(line1);
            shapeGroup.Add(arc);
            shapeGroup.Add(line2);
            shapeGroup.Add(line3);

            TopoShape wire = GlobalInstance.BrepTools.MakeWire(shapeGroup);
            TopoShape face = GlobalInstance.BrepTools.MakeFace(wire);

            // Extrude
            TopoShape extrude = GlobalInstance.BrepTools.Extrude(face, 100, new Vector3(0, 0, 1));
            renderView.ShowGeometry(extrude, ++shapeId);

            // Check find....
            SceneNode findNode = renderView.SceneManager.FindNode(new ElementId(shapeId));
            renderView.SceneManager.SelectNode(findNode);
        }

        private void revoleToolStripMenuItem_Click(object sender, EventArgs e)
        {
            int size = 10;
            // Create the outline edge
            TopoShape arc = GlobalInstance.BrepTools.MakeArc3Pts(new Vector3(-size, 0, 0), new Vector3(size, 0, 0), new Vector3(0, size, 0));
            TopoShape line1 = GlobalInstance.BrepTools.MakeLine(new Vector3(-size, -size, 0), new Vector3(-size, 0, 0));
            TopoShape line2 = GlobalInstance.BrepTools.MakeLine(new Vector3(size, -size, 0), new Vector3(size, 0, 0));
            TopoShape line3 = GlobalInstance.BrepTools.MakeLine(new Vector3(-size, -size, 0), new Vector3(size, -size, 0));

            TopoShapeGroup shapeGroup = new TopoShapeGroup();
            shapeGroup.Add(line1);
            shapeGroup.Add(arc);
            shapeGroup.Add(line2);
            shapeGroup.Add(line3);

            TopoShape wire = GlobalInstance.BrepTools.MakeWire(shapeGroup);

            TopoShape revole = GlobalInstance.BrepTools.Revol(wire, new Vector3(size * 3, 0, 0), new Vector3(0, 1, 0), 145);

            renderView.ShowGeometry(revole, ++shapeId);
        }

        private void clearToolStripMenuItem_Click(object sender, EventArgs e)
        {
            renderView.ClearScene();
        }

        private void sTLToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFileDialog dlg = new OpenFileDialog();
            dlg.Filter = "STL (*.stl)|*.stl|IGES (*.igs;*.iges)|*.igs;*.iges|STEP (*.stp;*.step)|*.stp;*.step|BREP (*.brep)|*.brep|All Files(*.*)|*.*";

            if (DialogResult.OK != dlg.ShowDialog())
                return;


            TopoShape shape = GlobalInstance.BrepTools.LoadFile(new AnyCAD.Platform.Path(dlg.FileName));
            renderView.RenderTimer.Enabled = false;
            if (shape != null)
            {
                TopoShapeGroup group = new TopoShapeGroup();
                group.Add(shape);
                //GlobalInstance.BrepTools.SaveFile(group, "d:\\anycad.brep");
                //PhongMaterial material = new PhongMaterial();
                //material.SetAmbient(new ColorValue(0.24725f, 0.2245f, 0.0645f));
                //material.SetDiffuse(new ColorValue(0.84615f, 0.8143f, 0.2903f));
                //material.SetSpecular(new ColorValue(0.797357f, 0.723991f, 0.208006f));
                //material.SetShininess(83.2f);
                //FaceStyle faceStyle = new FaceStyle();
                //faceStyle.SetMaterial(material);
                //SceneManager sceneMgr = renderView.SceneManager;
                //TopoShapeGroup subGroup = GlobalInstance.TopoExplor.ExplorSubShapes(shape);
                //int nSize = subGroup.Size();
                //for (int ii = 0; ii < nSize; ++ii)
                //{
                //    SceneNode node = GlobalInstance.TopoShapeConvert.ToEntityNode(subGroup.GetTopoShape(ii), 10f);
                //    node.SetId(++shapeId);
                //    node.SetFaceStyle(faceStyle);

                //    sceneMgr.AddNode(node);
                //}
                SceneManager sceneMgr = renderView.SceneManager;
                SceneNode rootNode = GlobalInstance.TopoShapeConvert.ToSceneNode(shape, 0.1f);
                if (rootNode != null)
                {
                    sceneMgr.AddNode(rootNode);
                }
            }
            renderView.RenderTimer.Enabled = true;
        

            renderView.FitAll();
            renderView.RequestDraw(EnumRenderHint.RH_LoadScene);
        }

        private void shadeWithEdgeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            renderView.SetDisplayMode((int)(EnumDisplayStyle.DS_Face | EnumDisplayStyle.DS_Edge | EnumDisplayStyle.DS_Realistic));
        }

        private void shadeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            renderView.SetDisplayMode((int)(EnumDisplayStyle.DS_Face|EnumDisplayStyle.DS_Realistic));
        }

        private void edgeWithPointsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            renderView.SetDisplayMode((int)(EnumDisplayStyle.DS_Edge|EnumDisplayStyle.DS_Vertex));
        }

        private void splitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            TopoShape cylinder = GlobalInstance.BrepTools.MakeCylinder(new Vector3(80, 0, 0), new Vector3(0, 0, 1), 20, 100, 315);

            TopoShape planeFace = GlobalInstance.BrepTools.MakePlaneFace(new Vector3(80, 0, 50), new Vector3(0, 0, 1), -100, 100, -100, 100);

            TopoShape rest = GlobalInstance.BrepTools.MakeSplit(cylinder, planeFace);
            renderView.ShowGeometry(rest, ++shapeId);
        }

        private void customToolStripMenuItem_Click(object sender, EventArgs e)
        {
            float[] vb ={0,0,0,100,0,0,100,100,0};
            uint[] ib = { 0, 1, 2 };
            float[] cb = { 1, 0, 0, 1, 0, 1, 0, 1, 0, 0, 1, 1 };
            float[] nb = {};
            RenderableEntity entity = GlobalInstance.TopoShapeConvert.CreateColoredFaceEntity(vb, ib, nb, cb, new AABox(Vector3.ZERO, new Vector3(100, 100, 1)));

            EntitySceneNode node = new EntitySceneNode();
            node.SetEntity(entity);

            renderView.SceneManager.AddNode(node);
            renderView.RequestDraw();
        }

        private void textToolStripMenuItem_Click(object sender, EventArgs e)
        {

            String fontName = "simhei.ttf";
            AnyCAD.Platform.Font font = GlobalInstance.FontManager.FindFont(fontName);
            if (font != null)
            {
                font.SetCharHeight(24);
                font.SetCharWdith(24);
                font.Update();
            }

            {
                Vector3 position = new Vector3(200, 200, 200);


                TextNode text = new TextNode();
                text.SetFontName(fontName);
                text.SetPosition(position);                
                text.SetText("AnyCAD .Net SDK Pro 专业版");
                text.SetTextColor(new ColorValue(1, 0, 0, 1));
                text.SetOffset(new Vector2(-24 * 7, 0));
                renderView.SceneManager.AddNode(text);

                PointNode pn = new PointNode();
                pn.SetPoint(position);
                renderView.ShowSceneNode(pn);
            }
            {
                TextNode text = new TextNode();
                text.SetPosition(new Vector3(10, 50, 0));
                text.SetText("AnyCAD .Net SDK Pro 2D");
                text.SetTextColor(new ColorValue(0, 1, 0, 1));
                renderView.SceneManager.AddNode2d(text);
            }

            renderView.RequestDraw();
        }

        private bool m_PickPoint = false;
        private void pickPointToolStripMenuItem_Click(object sender, EventArgs e)
        {
            m_PickPoint = !m_PickPoint;
        }

        private void OnRenderWindow_MouseClick(object sender, MouseEventArgs e)
        {
            if (!m_PickPoint)
                return;

            Platform.PickHelper pickHelper = renderView.PickShape(e.X, e.Y);
            if (pickHelper != null)
            {
                // add a ball
                //Platform.TopoShape shape = GlobalInstance.BrepTools.MakeSphere(pt, 2);
                //renderView.ShowGeometry(shape, 100);
            }
            // Try the grid
            Vector3 pt = renderView.HitPointOnGrid(e.X, e.Y);
            if (pt != null)
            {
                //Platform.TopoShape shape = GlobalInstance.BrepTools.MakeSphere(pt, 2);
                //renderView.ShowGeometry(shape, 100);
            }
        }

        private void projectionToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // construct a wire;
            var points = new System.Collections.Generic.List<Vector3>();
            points.Add(new Platform.Vector3(0, 0, 0));
            points.Add(new Platform.Vector3(0, 100, 0));
            points.Add(new Platform.Vector3(100, 100, 0));
            Platform.TopoShape wire = GlobalInstance.BrepTools.MakePolygon(points);
            renderView.ShowGeometry(wire, 200);

            Platform.Vector3 dirPlane1 = new Platform.Vector3(0, 1, 1);
            dirPlane1.Normalize();
            Platform.TopoShape newWire1 = GlobalInstance.BrepTools.ProjectOnPlane(wire, new Platform.Vector3(0, 0, 100),
                dirPlane1, new Platform.Vector3(0, 0, 1));

            Platform.Vector3 dirPlane2 = new Platform.Vector3(0, 1, -1);
            dirPlane2.Normalize();
            Platform.TopoShape newWire2 = GlobalInstance.BrepTools.ProjectOnPlane(wire, new Platform.Vector3(0, 0, 500),
                dirPlane2, new Platform.Vector3(0, 0, 1));

            Platform.TopoShapeGroup tsg = new Platform.TopoShapeGroup();
            tsg.Add(newWire1);
            tsg.Add(newWire2);
            Platform.TopoShape loft = GlobalInstance.BrepTools.MakeLoft(tsg, false);
            renderView.ShowGeometry(loft, 202);

            renderView.RequestDraw();
        }

        private void importDXFToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFileDialog dlg = new OpenFileDialog();
            dlg.Filter = "DXF (*.dxf)|*.dxf";

            if (DialogResult.OK == dlg.ShowDialog())
            {
                AnyCAD.Exchange.DxfReader reader = new AnyCAD.Exchange.DxfReader();
                LoadDxfContext context = new LoadDxfContext();
                if (!reader.Read(dlg.FileName, context, false))
                    return;

                //TopoShape test = context.ShapeGroup.GetTopoShape(0);
                //renderView.ShowGeometry(test, 1);
                //Matrix4 mm = GlobalInstance.MatrixBuilder.MakeRotation(90, Vector3.UNIT_Z);
                //TopoShape test1 = GlobalInstance.BrepTools.Transform(test, mm);
                //renderView.ShowGeometry(test1, 5);
                //LoopsBuilder builder = new LoopsBuilder();
                //builder.InitializeWithClosedWires(context.ShapeGroup);
                //TopoShapeGroup faces = builder.BuildFacesWithHoles();
                //TopoShapeGroup faces = context.ShapeGroup;

                //for (int ii = 0; ii < faces.Size(); ++ii)
                //{
                //    renderView.ShowGeometry(faces.GetTopoShape(ii), ++shapeId);
                //}

                TopoShape test = GlobalInstance.BrepTools.MakeWire(context.ShapeGroup);
                renderView.ShowWorkingGrid(true);
                TopoShape test1 = GlobalInstance.BrepTools.Rotation(test, Vector3.UNIT_Z, 90);
                List<Vector3> pts1 = new List<Vector3>();
                pts1.Add(new Vector3(0, 0, 0));
                pts1.Add(new Vector3(0, 0, 100));
                pts1.Add(new Vector3(100, 0, 200));
                TopoShape line = GlobalInstance.BrepTools.MakePolyline(pts1);
                test1 = GlobalInstance.BrepTools.MakePipe(test1, line, 1);
                renderView.ShowGeometry(test1, 5);
                renderView.ShowGeometry(test, 5);
                renderView.RequestDraw();
            }

            renderView.FitAll();
        }

        private void zoomAllToolStripMenuItem_Click(object sender, EventArgs e)
        {
            renderView.FitAll();
        }

        private void curveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            renderView.SetStandardView(EnumStandardView.SV_Top);

            Platform.LineStyle lineStyle = new Platform.LineStyle();
            lineStyle.SetLineWidth(0.5f);
            lineStyle.SetColor(ColorValue.BLUE);
            Platform.LineStyle lineStyle2 = new Platform.LineStyle();
            lineStyle2.SetLineWidth(0.5f);
            lineStyle2.SetColor(ColorValue.GREEN);

            Platform.TopoShape arc = GlobalInstance.BrepTools.MakeEllipseArc(Vector3.ZERO, 100, 50, 45, 270, Vector3.UNIT_Z);
            renderView.ShowGeometry(arc, 100);
 
            {
                GeomCurve curve = new GeomCurve();
                curve.Initialize(arc);

                double paramStart = curve.FirstParameter();
                double paramEnd = curve.LastParameter();

                double step = (paramEnd - paramStart) * 0.1f;

                for (double uu = paramStart; uu <= paramEnd; uu += step)
                {
                    Vector3 dir = curve.DN(uu, 1);
                    Vector3 pos = curve.Value(uu);

                    // 切线
                    {
                        Platform.TopoShape line = GlobalInstance.BrepTools.MakeLine(pos, pos + dir);
                        Platform.SceneNode node = renderView.ShowGeometry(line, 101);
                        node.SetLineStyle(lineStyle);
                    }
                    // 法线
                    {
                        Vector3 dirN = dir.CrossProduct(Vector3.UNIT_Z);
                        Platform.TopoShape line = GlobalInstance.BrepTools.MakeLine(pos, pos + dirN);
                        Platform.SceneNode node = renderView.ShowGeometry(line, 101);
                        node.SetLineStyle(lineStyle2);
                    }

                }

            }

            TopoShapeProperty property = new TopoShapeProperty();
            property.SetShape(arc);

            double len = property.EdgeLength();

            TextNode text = new TextNode();
            text.SetText(String.Format("Arc Length: {0}", len));
            text.SetPosition(new Vector3(100, 100, 0));

            renderView.SceneManager.ClearNodes2d();
            renderView.SceneManager.AddNode2d(text);

            renderView.RequestDraw();
        }

        private void surfaceToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Platform.LineStyle lineStyle = new Platform.LineStyle();
            lineStyle.SetLineWidth(0.5f);
            lineStyle.SetColor(ColorValue.RED);

            var points = new System.Collections.Generic.List<Vector3>();
            points.Add(new Vector3(0, 0, 0));
            points.Add(new Vector3(50, 0, 0));
            points.Add(new Vector3(100, 0, 0));

            points.Add(new Vector3(0, 50, 0));
            points.Add(new Vector3(50, 50, 5));
            points.Add(new Vector3(100, 50, -5));

            points.Add(new Vector3(0, 150, 5));
            points.Add(new Vector3(50, 150, -5));
            points.Add(new Vector3(100, 150, 0));

            TopoShape face = GlobalInstance.BrepTools.MakeSurfaceFromPoints(points, 3, 3);

            renderView.ShowGeometry(face, 101);

            GeomSurface surface = new GeomSurface();
            surface.Initialize(face);
            double ufirst = surface.FirstUParameter();
            double uLarst = surface.LastUParameter();
            double vfirst = surface.FirstVParameter();
            double vLast = surface.LastVParameter();

            double ustep = (uLarst - ufirst) * 0.1f;
            double vstep = (vLast - vfirst) * 0.1f;
            for(double ii =ufirst; ii<=uLarst; ii+= ustep)
                for (double jj = vfirst; jj <= vLast; jj += vstep)
                {
                    var data = surface.D1(ii, jj);

                    Vector3 pos = data[0];
                    Vector3 dirU = data[1];
                    Vector3 dirV = data[2];
                    Vector3 dir = dirV.CrossProduct(dirU);
                    dir.Normalize();
                    {
                        Platform.TopoShape line = GlobalInstance.BrepTools.MakeLine(pos, pos + dir*10.0f);
                        Platform.SceneNode node = renderView.ShowGeometry(line, 101);

                        node.SetLineStyle(lineStyle);
                    }
                }

            TopoShapeProperty property = new TopoShapeProperty();
            property.SetShape(face);

            double area = property.SurfaceArea();

            TextNode text = new TextNode();
            text.SetText(String.Format("Surface Area: {0}", area));
            text.SetPosition(new Vector3(100, 100, 0));
            renderView.SceneManager.ClearNodes2d();
            renderView.SceneManager.AddNode2d(text);

            renderView.RequestDraw();
        }

        private void faceTriangulateToolStripMenuItem_Click(object sender, EventArgs e)
        {
            TopoShape circle = GlobalInstance.BrepTools.MakeCircle(Vector3.ZERO, 100, Vector3.UNIT_Z);
            TopoShape face = GlobalInstance.BrepTools.MakeFace(circle);
            if (face.GetShapeType() == EnumTopoShapeType.Topo_FACE)
            {
                MessageBox.Show("This is a face!");
            }

            FaceTriangulation ft = new FaceTriangulation();
            ft.SetTolerance(5);
            ft.Perform(face);
            float[] points = ft.GetVertexBuffer();
            int pointCount = points.Length / 3;
            uint[] indexBuffer = ft.GetIndexBuffer();
            int faceCount = indexBuffer.Length / 3;
            float[] normals = ft.GetNormalBuffer();

            MessageBox.Show(String.Format("Point Count: {0}\n Face Count: {1}", pointCount, faceCount));

            float[] colorBuffer = new float[pointCount * 4];
            
            Random num = new Random();
            for (int ii = 0; ii < pointCount; ++ii)
            {
                int idx = ii * 4;
                colorBuffer[idx] = num.Next(0, 256)/256.0f;
                colorBuffer[idx+1] = num.Next(0, 256) / 256.0f;
                colorBuffer[idx+2] = num.Next(0, 256) / 256.0f;
                colorBuffer[idx+3] = 1;
            }

            RenderableEntity entity = GlobalInstance.TopoShapeConvert.CreateColoredFaceEntity(points, indexBuffer, normals, colorBuffer, face.GetBBox());

            EntitySceneNode node = new EntitySceneNode();
            node.SetEntity(entity);

            renderView.SceneManager.AddNode(node);
            renderView.RequestDraw();
            //////////////////////////////////////////////////////////////////////////
            // Code to get the mesh
            /*
            for (int ii = 0; ii < faceCount; ++ii)
            {
                int p0 = (int)indexBuffer[ii * 3];
                int p1 = (int)indexBuffer[ii * 3 + 1];
                int p2 = (int)indexBuffer[ii * 3 + 2];

                Vector3 pt0 = new Vector3(points[p0 * 3], points[p0 * 3 + 1], points[p0 * 3 + 2]);
                Vector3 pt1 = new Vector3(points[p1 * 3], points[p1 * 3 + 1], points[p1 * 3 + 2]);
                Vector3 pt2 = new Vector3(points[p2 * 3], points[p2 * 3 + 1], points[p2 * 3 + 2]);

                // ....
                // use the same way to get the normal data for each point.
            }
             * */
            //////////////////////////////////////////////////////////////////////////
        }

        private void pickNodeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            renderView.SetPickMode((int)(EnumPickMode.RF_SceneNode | EnumPickMode.RF_Face));
        }

        private void pickGroupToolStripMenuItem_Click(object sender, EventArgs e)
        {
            renderView.SetPickMode((int)(EnumPickMode.RF_GroupSceneNode | EnumPickMode.RF_Face));

            //// Test Group
            //TopoShape cylinder = GlobalInstance.BrepTools.MakeCylinder(Vector3.ZERO, Vector3.UNIT_Z, 50, 100, 0);
            //TopoShape sphere = GlobalInstance.BrepTools.MakeSphere(new Vector3(0, 0, 150), 50);

            //GroupSceneNode group = new GroupSceneNode();
            //group.AddNode(GlobalInstance.TopoShapeConvert.ToEntityNode(cylinder, 0.1f));
            //group.AddNode(GlobalInstance.TopoShapeConvert.ToEntityNode(sphere, 0.1f));

            //renderView.SceneManager.AddNode(group);

        }

        private void pickFaceEdgePointToolStripMenuItem_Click(object sender, EventArgs e)
        {
            renderView.SetPickMode((int)(EnumPickMode.RF_Default));
        }

        private bool mShowGrid = true;
        private void showGridToolStripMenuItem_Click(object sender, EventArgs e)
        {
            mShowGrid = !mShowGrid;
            renderView.ShowWorkingGrid(mShowGrid);
        }

        private void projLineToSurfaceToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var points = new System.Collections.Generic.List<Vector3>();
            points.Add(new Vector3(0, 0, 0));
            points.Add(new Vector3(50, 0, 0));
            points.Add(new Vector3(100, 0, 0));

            points.Add(new Vector3(0, 50, 0));
            points.Add(new Vector3(50, 50, 5));
            points.Add(new Vector3(100, 50, -5));

            points.Add(new Vector3(0, 150, 5));
            points.Add(new Vector3(50, 150, -5));
            points.Add(new Vector3(100, 150, 0));

            TopoShape surface = GlobalInstance.BrepTools.MakeSurfaceFromPoints(points, 3, 3);

            TopoShape line = GlobalInstance.BrepTools.MakeLine(new Vector3(0, 0, 200), new Vector3(100, 150, 200));

            TopoShape proj = GlobalInstance.BrepTools.ProjectOnSurface(line, surface);

            renderView.ShowGeometry(proj, 200);
            renderView.ShowGeometry(surface, 200);
            renderView.RequestDraw();
        }

        private void largePointsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            const float len = 100.0f;
            const int nDim = 50;
            float[] pointBuffer = new float[nDim * nDim * nDim * 3];
            float[] colorBuffer = new float[nDim*nDim*nDim*3];
            int idx = -1;
            for (int ii = 0; ii < nDim; ++ii)
                for (int jj = 0; jj < nDim; ++jj)
                    for (int kk = 0; kk < nDim; ++kk)
                    {
                        ++idx;
                        pointBuffer[idx * 3] = ii * len;
                        pointBuffer[idx * 3 + 1] = jj * len;
                        pointBuffer[idx * 3 + 2] = kk * len;

                        colorBuffer[idx*3] = ((float)ii)/((float)nDim);
                        colorBuffer[idx*3 + 1] = ((float)jj)/((float)nDim);
                        colorBuffer[idx * 3 + 2] = ((float)kk) / ((float)nDim);
                    }

            PointStyle pointStyle = new PointStyle();
            pointStyle.SetPointSize(4.0f);

            PointCloudNode pcn = new PointCloudNode();
            pcn.SetPointStyle(pointStyle);
            pcn.SetPoints(pointBuffer);
            pcn.SetColors(colorBuffer);
            pcn.ComputeBBox();
            AABox bbox = pcn.GetBBox();
            Vector3 pt = bbox.MinPt;
            renderView.SceneManager.AddNode(pcn);

            renderView.RequestDraw();
        }

        private void splineToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var points = new System.Collections.Generic.List<Vector3>();
            points.Add(new Vector3(0, 0, 0));
            points.Add(new Vector3(0, 0, 10));
            points.Add(new Vector3(0, 10, 50));
            points.Add(new Vector3(10, 20, 150));

            TopoShape spline = GlobalInstance.BrepTools.MakeSpline(points);
            TopoShape circle = GlobalInstance.BrepTools.MakeCircle(Vector3.ZERO, 10, Vector3.UNIT_Z);
         
            TopoShape shape = GlobalInstance.BrepTools.MakePipe(circle, spline, 0);

            renderView.ShowGeometry(shape, 200);

            renderView.RequestDraw();
        }

        private void text3DToolStripMenuItem_Click(object sender, EventArgs e)
        {
            String fontName = "romand.shx";
            FontManager fontMgr = GlobalInstance.FontManager;
            AnyCAD.Platform.Font font = fontMgr.FindFont(fontName);
            if(font == null)
            {
                font = fontMgr.LoadFont(fontName, "..\\Library\\Font\\romand.shx");
                if(font != null)
                    fontMgr.AddFont(font);
            }


            foreach (String name in fontMgr.ListFontNames())
            {
                //MessageBox.Show(name);
            }


            Text3dNode textNode = new Text3dNode();
            textNode.SetFontName("FangSong (TrueType)");
            textNode.SetText("1234565\nabcdefg\n我爱你");
            textNode.SetLineSpace(10);

            Coordinate3 coord = new Coordinate3();
            coord.Origion = new Vector3(100, 100, 0);
            coord.X = new Vector3(1, 1, 0);
            coord.X.Normalize();
            coord.Y = coord.Z.CrossProduct(coord.X);

            Matrix4 trf = GlobalInstance.MatrixBuilder.ToWorldMatrix(coord);
            textNode.SetTransform(trf);
            textNode.Update();

            renderView.ShowSceneNode(textNode);
        }

        private void fillFaceToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Vector3[] pt = new Vector3[8];
            pt[0] = new Vector3(1, 2, 0.5f);

            pt[1] = new Vector3(2, 3, 0.2f);
            pt[2] = new Vector3(0, 5, 0.5f);
            pt[3] = new Vector3(-1, 2, 0.2f);
            pt[4] = new Vector3(-1, 0, 0.3f);
            pt[5] = new Vector3(-1, -2, 0.4f);
            pt[6] = new Vector3(1, -2, 0.5f);

            var ptlist = new System.Collections.Generic.List<Vector3>();
            for (int i = 0; i < 7; i++)
            {
                ptlist.Add(pt[i]);
            }
            ptlist.Add(pt[0]);

            Vector3[] pd = new Vector3[7];
            for (int i = 0; i < 7; i++)
            {
                pd[i] = new Vector3(pt[i].X, pt[i].Y, pt[i].Z + i+1);
            }
            var pdlist = new System.Collections.Generic.List<Vector3>();
            for (int i = 0; i < 7; i++)
            {
                pdlist.Add(pd[i]);
            }
            pdlist.Add(pd[0]);

            TopoShape sect = GlobalInstance.BrepTools.MakePolyline(ptlist);
            TopoShape sectd = GlobalInstance.BrepTools.MakePolyline(pdlist);
            TopoShapeGroup group = new TopoShapeGroup();
            group.Add(sect);
            group.Add(sectd);
            TopoShape ext_Pad = GlobalInstance.BrepTools.MakeLoft(group, true);

            renderView.ShowGeometry(ext_Pad, ++shapeId);

            renderView.FitAll();
        }

        private void pickByClickToolStripMenuItem_Click(object sender, EventArgs e)
        {
            renderView.ExecuteCommand("Pick");
        }

        private void pickByRectangleToolStripMenuItem_Click(object sender, EventArgs e)
        {
            renderView.ExecuteCommand("RectPick");
        }

        private void zoomByRectToolStripMenuItem_Click(object sender, EventArgs e)
        {
            renderView.ExecuteCommand("ZoomByRect");
        }

        private void orbitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            renderView.ExecuteCommand("Orbit");
        }

        private void querySelectionToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SelectedShapeQuery context = new SelectedShapeQuery();
            renderView.QuerySelection(context);
            TopoShape subShape = context.GetSubGeometry();
            GeomCurve curve = new GeomCurve();
            if (curve.Initialize(subShape))
            {
               Vector3 startPt = curve.D0(curve.FirstParameter());
               Vector3 endPt = curve.D0(curve.LastParameter());
                //...
            }

            ElementId id = context.GetNodeId();
            MessageBox.Show(id.AsInt().ToString());
        }

        private void cutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            TopoShape circle = GlobalInstance.BrepTools.MakeCircle(Vector3.ZERO, 10, Vector3.UNIT_Z);
            TopoShape circle2 = GlobalInstance.BrepTools.MakeCircle(Vector3.ZERO, 8, Vector3.UNIT_Z);
            TopoShape section = GlobalInstance.BrepTools.BooleanCut(
                GlobalInstance.BrepTools.MakeFace(circle),
                GlobalInstance.BrepTools.MakeFace(circle2));
            TopoShape surface = GlobalInstance.BrepTools.Extrude(section, 100, Vector3.UNIT_Z);

            TopoShape cyl = GlobalInstance.BrepTools.MakeCylinder(new Vector3(0, 0, 50), Vector3.UNIT_X, 8, 10, 0);

            TopoShape cut = GlobalInstance.BrepTools.BooleanCut(surface, cyl);
            renderView.ShowGeometry(cut, ++shapeId);

            renderView.RequestDraw();

            
        }
        DrawLineEditor drawLine;
        private void lineEditorToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if(drawLine == null)
                drawLine = new DrawLineEditor();
            renderView.ActiveEditor(drawLine);
        }

        private void planeAngleToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // plane
            Vector3 planeDir = Vector3.UNIT_Z;
            TopoShape plane1 = GlobalInstance.BrepTools.MakePlaneFace(Vector3.ZERO, planeDir, -100, 100, -100, 100);

            {
                GeomSurface gs = new GeomSurface();
                gs.Initialize(plane1);
                List<Vector3> rst = gs.D1(gs.FirstUParameter(), gs.FirstVParameter());
                Vector3 dir2 = rst[1].CrossProduct(rst[2]);

                MessageBox.Show(dir2.ToString());
            }
    

            Vector3 normal = new Vector3(0,1,1);
            normal.Normalize();
            TopoShape plane2 = GlobalInstance.BrepTools.MakePlaneFace(Vector3.ZERO, normal, -100, 100, -100, 100);

            renderView.ShowGeometry(plane1, ++shapeId);
            renderView.ShowGeometry(plane2, ++shapeId);

            LineStyle style = new LineStyle();
            style.SetColor(ColorValue.GREEN);
            // witness
            Vector3 end1 =  new Vector3(0, 0, 100);
            LineNode line1 = new LineNode();
            line1.Set(Vector3.ZERO, end1);
            line1.SetLineStyle(style);
            renderView.ShowSceneNode(line1);

            Vector3 end2 = normal * 100;
            LineNode line2 = new LineNode();
            line2.Set(Vector3.ZERO, end2);
            line2.SetLineStyle(style);
            renderView.ShowSceneNode(line2);

            // angle
            double angle = normal.AngleBetween(planeDir);

            Vector3 dir = normal.CrossProduct(planeDir);
            dir.Normalize();
            TopoShape arc = GlobalInstance.BrepTools.MakeArc(end2, end1, Vector3.ZERO, dir);
            SceneNode arcNode = renderView.ShowGeometry(arc, ++shapeId);
            arcNode.SetLineStyle(style);

            // text
            TextNode text = new TextNode();
            text.SetText(angle.ToString());
            Vector3 pos = end2 + end1;
            pos = pos * 0.5f;
            text.SetPosition(pos);
            renderView.ShowSceneNode(text);

            renderView.RequestDraw();
        }

        private float heightOfObject = 0; //记录小球的高度
        private float timerOfObject = 0;  //记录运动时间
        private float speedOfObject = 60; //起始速度
        private float xspeed = 10;        //X方向的速度
        private float distanceX = -125;   //X方向的位移
        private SceneNode m_Object;       //小球的节点
        
        private void FormMain_RenderTick()
        {
            timerOfObject += 0.5f;

            //z方向的位移
            heightOfObject = speedOfObject * timerOfObject - (9.8f * timerOfObject * timerOfObject) * 0.5f;
            //x方向的位移
            distanceX += xspeed;
            if (heightOfObject <= 0.0f)
            {
                distanceX -= xspeed;
                timerOfObject = 0;
                xspeed = -xspeed;
                heightOfObject = 10;
            }

            //设置位移矩阵
            AnyCAD.Platform.Matrix4 trf = GlobalInstance.MatrixBuilder.MakeTranslate(new Vector3(distanceX, 0, heightOfObject));
            m_Object.SetTransform(trf);

            renderView.RequestDraw();
        }

        private bool bEnabledAnimation = false;

        private void animationToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // create the object.
            if (m_Object == null)
            {
                TopoShape sphere = GlobalInstance.BrepTools.MakeSphere(Vector3.ZERO, 10);
                m_Object = renderView.ShowGeometry(sphere, ++shapeId);
            }

            // Register callback for the RenderTick event.
            timerOfObject = 0;
            if (bEnabledAnimation)
            {
                this.renderView.RenderTick -= new AnyCAD.Presentation.RenderEventHandler(FormMain_RenderTick);
            }
            else
            {
                this.renderView.RenderTick += new AnyCAD.Presentation.RenderEventHandler(FormMain_RenderTick);
            }
            bEnabledAnimation = !bEnabledAnimation;
        }

        private void faceWithHolesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var ptlist = new System.Collections.Generic.List<Vector3>();
            ptlist.Add(Vector3.ZERO);
            ptlist.Add(new Vector3(100, 0, 0));
            ptlist.Add(new Vector3(100, 100, 0));
            ptlist.Add(new Vector3(0, 100, 0));
            TopoShape polygon = GlobalInstance.BrepTools.MakePolygon(ptlist);

            TopoShape cir1 = GlobalInstance.BrepTools.MakeCircle(new Vector3(20, 20, 0), 15, Vector3.UNIT_Z);
            TopoShape cir2 = GlobalInstance.BrepTools.MakeCircle(new Vector3(80, 20, 0), 15, Vector3.UNIT_Z);
            TopoShape cir3 = GlobalInstance.BrepTools.MakeCircle(new Vector3(80, 80, 0), 15, Vector3.UNIT_Z);
            TopoShape cir4 = GlobalInstance.BrepTools.MakeCircle(new Vector3(20, 80, 0), 15, Vector3.UNIT_Z);

            TopoShapeGroup group = new TopoShapeGroup();
            group.Add(polygon);
            group.Add(cir1);
            group.Add(cir2);
            group.Add(cir3);
            group.Add(cir4);

            // build faces with holes
            LoopsBuilder lb = new LoopsBuilder();
            lb.Initialize(group);
            TopoShapeGroup faces = lb.BuildFacesWithHoles();
            for (int ii = 0; ii < faces.Size(); ++ii)
            {
                renderView.ShowGeometry(faces.GetTopoShape(ii), ++shapeId);
            }

            renderView.RequestDraw();
        }

        private void spiralToolStripMenuItem_Click(object sender, EventArgs e)
        {
            TopoShape spiralCurve = GlobalInstance.BrepTools.MakeSpiralCurve(100, 10, 10, Coordinate3.UNIT_XYZ);
            renderView.ShowGeometry(spiralCurve, ++shapeId);
        }

        private void rectangleRToolStripMenuItem_Click(object sender, EventArgs e)
        {
            TopoShape rect = GlobalInstance.BrepTools.MakeRectangle(100, 50, 10, Coordinate3.UNIT_XYZ);
            rect = GlobalInstance.BrepTools.MakeFace(rect);

            SceneNode node = renderView.ShowGeometry(rect, ++shapeId);
            Texture texture = new Texture();
            texture.SetName("mytexture");
            texture.SetFilePath(new AnyCAD.Platform.Path("f:\\dimian.png"));

            FaceStyle style = new FaceStyle();
            style.SetTexture(0, texture);
            style.SetColor(255, 255, 255);
            node.SetFaceStyle(style);


            TopoShape line = GlobalInstance.BrepTools.MakeLine(Vector3.ZERO, new Vector3(0, 0, 100));
            TopoShape face = GlobalInstance.BrepTools.Extrude(line, 100, Vector3.UNIT_X);
            renderView.ShowGeometry(face, ++shapeId);
        }

        private void traverseToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SceneNodeIterator itr = renderView.SceneManager.NewSceneNodeIterator();
            String msg = "All Nodes: ";
            while (itr.More())
            {
                SceneNode node = itr.Next();
                msg += node.GetId().ToString() + ", ";
            }

            MessageBox.Show(msg);
        }

        private void sectionToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // build two surfaces
            TopoShape arc = GlobalInstance.BrepTools.MakeArc(Vector3.ZERO, 100, 0, 135, Vector3.UNIT_Z);
            TopoShape cir = GlobalInstance.BrepTools.MakeCircle(new Vector3(-200,0,0), 50, Vector3.UNIT_X);
            TopoShape surf1 = GlobalInstance.BrepTools.Extrude(arc, 100, Vector3.UNIT_Z);
            TopoShape surf2 = GlobalInstance.BrepTools.Extrude(cir, 400, Vector3.UNIT_X);

            renderView.ShowGeometry(surf1, ++shapeId);
            //renderView.ShowGeometry(surf2, ++shapeId);

            // compute section wire
            TopoShape wire = GlobalInstance.BrepTools.SurfaceSection(surf1, surf2);
            if (wire == null)
                return;


            SceneNode sectionNode = renderView.ShowGeometry(wire, ++shapeId);
            LineStyle lineStyle = new LineStyle();
            lineStyle.SetLineWidth(4);
            lineStyle.SetColor(ColorValue.RED);
            sectionNode.SetLineStyle(lineStyle);
       
            renderView.RequestDraw();
        }

        private void customGridToolStripMenuItem_Click(object sender, EventArgs e)
        {
            WorkingPlane wp = renderView.Renderer.GetWorkingPlane();
            GridNode gridNode = new GridNode();
            Vector3 modelSize = renderView.SceneManager.GetBBox().Size();
            Vector2 cellSize = gridNode.GetCellSize();
            int nCountX = (int)(modelSize.X / cellSize.X + 0.5f) + 1;
            int nCountY = (int)(modelSize.Y / cellSize.Y + 0.5f) + 1;
            if (nCountX < 2)
                nCountX = 2;
            if (nCountY < 2)
                nCountY = 2;

            gridNode.SetCellCount(nCountX, nCountY);

            LineStyle lineStyle = new LineStyle();
            lineStyle.SetColor(new ColorValue(1.0f, 1.0f, 1.0f));
            lineStyle.SetPatternStyle((int)EnumLinePattern.LP_DotLine);
            {
                //Z
                LineNode lineNode = new LineNode();
                lineNode.Set(new Vector3(0, 0, -1000), new Vector3(0, 0, 1000));
                lineNode.SetLineStyle(lineStyle);
                gridNode.AddNode(lineNode);
            }
            {
                //X
                LineNode lineNode = new LineNode();
                lineNode.Set(new Vector3(-1000, 0, 0), new Vector3(1000, 0, 0));
                lineNode.SetLineStyle(lineStyle);
                gridNode.AddNode(lineNode);
            }
            {
                //Y
                LineNode lineNode = new LineNode();
                lineNode.Set(new Vector3(0, -1000, 0), new Vector3(0, 1000, 0));
                lineNode.SetLineStyle(lineStyle);
                gridNode.AddNode(lineNode);
            }

            lineStyle = new LineStyle();
            lineStyle.SetColor(new ColorValue(0.9f, 0.9f, 0.9f));
            gridNode.SetLineStyle(lineStyle);
            for (int ii = -1; ii <= nCountX; ++ii)
            {
                if (ii == 0)
                    continue;

                LineNode lineNode = new LineNode();
                lineNode.Set(new Vector3(ii * cellSize.X, cellSize.Y, 0), new Vector3(ii * cellSize.X, -nCountY * cellSize.Y, 0));

                gridNode.AddNode(lineNode);
            }
            for (int ii = -1; ii <= nCountY; ++ii)
            {
                if (ii == 0)
                    continue;

                LineNode lineNode = new LineNode();
                lineNode.Set(new Vector3(-cellSize.X, -ii * cellSize.Y, 0), new Vector3(nCountX * cellSize.X, -ii * cellSize.Y, 0));
                gridNode.AddNode(lineNode);
            }
            gridNode.Update();
            wp.SetGridNode(gridNode);



            {
                AxesWidget xwh = new AxesWidget();
                xwh.EnableLeftHandCS();
                xwh.SetArrowText((int)EnumAxesDirection.Axes_Y, "w");
                xwh.SetArrowText((int)EnumAxesDirection.Axes_Z, "h");
                ScreenWidget coordWidget = new ScreenWidget();
                coordWidget.SetNode(xwh);
                coordWidget.SetWidgetPosition((int)EnumWidgetPosition.WP_BottomLeft);
                renderView.Renderer.AddWidgetNode(coordWidget);
            }
            {
                AxesWidget yz = new AxesWidget();
                yz.ShowArrow((int)EnumAxesDirection.Axes_X, false);
                ScreenWidget coordWidget = new ScreenWidget();
                coordWidget.SetNode(yz);
                coordWidget.SetWidgetPosition((int)EnumWidgetPosition.WP_BottomRight);
                renderView.Renderer.AddWidgetNode(coordWidget);
            }

            renderView.ShowCoordinateAxis(false);

            renderView.RequestDraw();
        }

        private void scriptToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //String str = "topoShape = theBrepTools:MakeEllipsArc(Vector3(0,0,0), 100, 50, 0, 90, Vector3(0,0,1))\n revolShape = theBrepTools:Revol(topoShape, Vector3(0,0,0), Vector3(0,1,0), 90)\ntheEntityGroup:AddTopoShape(revolShape)";
            //EntityGroup group = GlobalInstance.BrepTools.CreateGeometryByScript("Lua", str);
            //if (group == null)
            //    return;
            //for (int ii = 0; ii < group.Size(); ++ii)
            //{
            //    renderView.ShowGeometry(group.GetTopoShape(ii), ++shapeId);
            //}

            renderView.RequestDraw();
        }

        private void drawCurveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DrawCurveOnSurfaceEditor editor = new DrawCurveOnSurfaceEditor();
            renderView.ActiveEditor(editor);
        }

        private void measureDistanceToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MeasureDistanceEditor editor = new MeasureDistanceEditor();
            renderView.ActiveEditor(editor);
        }

        private void perspectiveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            renderView.ExecuteCommand("ProjectionMode");
            renderView.Renderer.SetSkyBox("Early Morning");
            
            renderView.RequestDraw();
            
        }

        private void edgeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            renderView.Renderer.SetDisplayMode((int)EnumDisplayStyle.DS_Edge);
            renderView.RequestDraw();
        }

        private void topToolStripMenuItem_Click(object sender, EventArgs e)
        {
            renderView.Renderer.SetStandardView( EnumStandardView.SV_Top);
            renderView.RequestDraw();
        }

        private void frontToolStripMenuItem_Click(object sender, EventArgs e)
        {
            renderView.Renderer.SetStandardView( EnumStandardView.SV_Front);
            renderView.RequestDraw();
        }

        private void bufferToolStripMenuItem_Click(object sender, EventArgs e)
        {
            TopoShape sphere = GlobalInstance.BrepTools.MakeSphere(Vector3.ZERO, 100);
            byte[] buffer = GlobalInstance.BrepTools.SaveBuffer(sphere);

            TopoShape newSphere = GlobalInstance.BrepTools.LoadBuffer(buffer);
            SceneNode node = renderView.ShowGeometry(newSphere, ++shapeId);
            node.SetPickable(false);

            renderView.RequestDraw();

            TopoDataExchangeStl expoter = new TopoDataExchangeStl();
            if (!expoter.Write(sphere, new Platform.Path("d:/mystl.stl")))
            {

                MessageBox.Show("Error!");
            }
        }

        private void pointToolStripMenuItem_Click(object sender, EventArgs e)
        {
            PointNode pn = new PointNode();
            pn.SetPoint(new Vector3(100, 100, 100));

            PointStyle ps = new PointStyle();
            ps.SetMarker("plus");
            ps.SetPointSize(10);
            pn.SetPointStyle(ps);

            renderView.ShowSceneNode(pn);
            renderView.RequestDraw();
        }

        private void surfaceSectionToolStripMenuItem_Click(object sender, EventArgs e)
        {
            TopoShape oCircle1 = GlobalInstance.BrepTools.MakeCircle(Vector3.ZERO, 20, Vector3.UNIT_Z);
            TopoShape Pipe01_Surf = GlobalInstance.BrepTools.Extrude(oCircle1, 100, Vector3.UNIT_Z);
            renderView.ShowGeometry(Pipe01_Surf, ++shapeId);

            TopoShape oCircle2 = GlobalInstance.BrepTools.MakeCircle(new Vector3(0.0f, 0.0f, 50.0f), 10, Vector3.UNIT_Y);
            TopoShape Pipe02_Surf = GlobalInstance.BrepTools.Extrude(oCircle2, 80, Vector3.UNIT_Y);
            renderView.ShowGeometry(Pipe02_Surf, ++shapeId);

            TopoShape Inters1 = GlobalInstance.BrepTools.SurfaceSection(Pipe01_Surf, Pipe02_Surf);
            if (Inters1 != null)
            {
                SceneNode node = renderView.ShowGeometry(Inters1, ++shapeId);
                LineStyle ls = new LineStyle();
                ls.SetLineWidth(3);
                ls.SetColor(ColorValue.RED);
                node.SetLineStyle(ls);

                GeomCurve curve = new GeomCurve();
                if (curve.Initialize(Inters1))
                {
                    LineStyle ls2 = new LineStyle();
                    ls2.SetColor(ColorValue.GREEN);

                    double start = curve.FirstParameter();
                    double end = curve.LastParameter();
                    for (double ii = start; ii <= end; ii += 0.1f)
                    {
                        List<Vector3> rt = curve.D1(ii);
                        LineNode ln = new LineNode();
                        ln.SetLineStyle(ls2);
                        ln.Set(rt[0], rt[0] + rt[1]);
                        renderView.ShowSceneNode(ln);
                    }
                }
            }
            renderView.RequestDraw();
        }

        private void panToolStripMenuItem_Click(object sender, EventArgs e)
        {
            renderView.ExecuteCommand("Pan");
        }

        private void zoomToolStripMenuItem_Click(object sender, EventArgs e)
        {
            renderView.ExecuteCommand("Zoom");
        }

        private void singlePickToolStripMenuItem_Click(object sender, EventArgs e)
        {
            renderView.ExecuteCommand("PickClearMode", "SinglePick");
        }

        private void multiPickToolStripMenuItem_Click(object sender, EventArgs e)
        {
            renderView.ExecuteCommand("PickClearMode", "MultiPick");
        }

        private void exportToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void importToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFileDialog openDlg = new OpenFileDialog();
            openDlg.Filter = "STL (*.stl)|*.stl|3ds (*.3ds)|*.3ds|obj (*.obj)|*.obj|Skp (*.skp)|*.skp";
                if (openDlg.ShowDialog() == DialogResult.OK)
                {
                    ModelReader reader = new ModelReader();
                    GroupSceneNode node = reader.LoadFile(new AnyCAD.Platform.Path(openDlg.FileName));
                    if (node != null)
                    {
                        node.SetName(openDlg.SafeFileName);
                        renderView.ShowSceneNode(node);
                        renderView.RequestDraw();
                    }
                }
        }

        private void ecllipsArcToolStripMenuItem_Click(object sender, EventArgs e)
        {
            TopoShape arc = GlobalInstance.BrepTools.MakeEllipseArc(Vector3.ZERO, 100, 50, 0, 90, Vector3.UNIT_Z);
            renderView.ShowGeometry(arc, ++shapeId);


            //TopoShape arc2 = GlobalInstance.BrepTools.MakeEllipseArc(Vector3.ZERO, 50, 100, 0, 90, Vector3.UNIT_Z);
            //renderView.ShowGeometry(arc2, ++shapeId);

            renderView.RequestDraw();
        }

        private void curveIntersectionToolStripMenuItem_Click(object sender, EventArgs e)
        {
            TopoShape ellipseArc = GlobalInstance.BrepTools.MakeEllipseArc(Vector3.ZERO, 100, 50, 0, 90, Vector3.UNIT_Z);
            TopoShape circle = GlobalInstance.BrepTools.MakeCircle(Vector3.ZERO, 60, Vector3.UNIT_Z);

            IntersectionLineCurve intersector = new IntersectionLineCurve();
            intersector.SetCurve(ellipseArc);
            if (intersector.Perform(circle))
            {
                PointStyle ps = new PointStyle();
                ps.SetMarker("plus");
                ps.SetPointSize(10);
                

                int nCount = intersector.GetPointCount();
                for (int ii = 1; ii <= nCount; ++ii)
                {
                    if (intersector.GetSquareDistance(ii) < 0.001)
                    {
                        Vector3 pt = intersector.GetPoint(ii);

                        PointNode pn = new PointNode();
                        pn.SetPoint(pt);
                        pn.SetPointStyle(ps);
                        renderView.ShowSceneNode(pn);
                    }
                }
            }

            renderView.ShowGeometry(ellipseArc, ++shapeId);
            renderView.ShowGeometry(circle, ++shapeId);
            renderView.RequestDraw();
        }

        private void sweepToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var ptlist = new System.Collections.Generic.List<Vector3>();
            ptlist.Add(Vector3.ZERO);
            ptlist.Add(new Vector3(0,0,100));
            ptlist.Add(new Vector3(100, 100, 600));
            
            TopoShape spline = GlobalInstance.BrepTools.MakeSpline(ptlist);
            TopoShape rect = GlobalInstance.BrepTools.MakeRectangle(20, 50, 5, Coordinate3.UNIT_XYZ);
            TopoShape line = GlobalInstance.BrepTools.MakeLine(Vector3.ZERO, new Vector3(100, 100, 0));
            TopoShape sweepShape = GlobalInstance.BrepTools.Sweep(line, spline, true);

            renderView.ShowGeometry(sweepShape, ++shapeId);
            renderView.RequestDraw();

            TopoShape path1 = GlobalInstance.BrepTools.MakeLine(new Vector3(0, 0, 0), new Vector3(0, 0, 100));
            TopoShape ts1 = GlobalInstance.BrepTools.MakeLine(new Vector3(0, 0, 0), new Vector3(100, 0, 100));
            TopoShape loft1 = GlobalInstance.BrepTools.Sweep(ts1, path1, true);

            renderView.ShowGeometry(loft1, ++shapeId);
            renderView.RequestDraw();

        }

        private void queryInfoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            renderView.ActiveEditor(new QueryShapeInfoEditor());
        }

        private void sTEPToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFileDialog dlg = new OpenFileDialog();
            dlg.Filter = "STEP (*.stp;*.step)|*.stp;*.step|All Files(*.*)|*.*";

            if (DialogResult.OK != dlg.ShowDialog())
                return;

            //AnyCAD.Exchange.ShowShapeReaderContext context = new AnyCAD.Exchange.ShowShapeReaderContext(renderView.SceneManager);
            //context.NextShapeId = ++shapeId;
            //AnyCAD.Exchange.StepReader reader = new AnyCAD.Exchange.StepReader();
            //reader.Read(dlg.FileName, context);

            //shapeId = context.NextShapeId + 1;
            //renderView.RequestDraw(EnumRenderHint.RH_LoadScene);
        }

        private void loft2ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            TopoShape circle = GlobalInstance.BrepTools.MakeCircle(new Vector3(0, 0, 50), 10, Vector3.UNIT_Z);
            TopoShape rect =  GlobalInstance.BrepTools.MakeRectangle(40, 40, 5, new Coordinate3(new Vector3(-20,-20,0), Vector3.UNIT_X, Vector3.UNIT_Y, Vector3.UNIT_Z));

            TopoShape loft = GlobalInstance.BrepTools.MakeLoft(rect, circle, true);

            TopoShape cyliner = GlobalInstance.BrepTools.MakeCylinder(new Vector3(0, 0, 0), Vector3.UNIT_Z, 5, 50, 0);

            TopoShape obj = GlobalInstance.BrepTools.BooleanCut(loft, cyliner);

            renderView.ShowGeometry(obj, ++shapeId);
            renderView.RequestDraw();
        }

        private void pipeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            for (int ii = 0; ii < 3; ++ii)
            {
                Vector3 startPt = new Vector3(ii * 100, 0, 0);
                var points = new System.Collections.Generic.List<Vector3>();
                points.Add(startPt);
                points.Add(startPt + new Platform.Vector3(0, 0, 100));
                points.Add(startPt + new Platform.Vector3(50, 50, 150));
                TopoShape path = GlobalInstance.BrepTools.MakePolyline(points);
                TopoShape section = GlobalInstance.BrepTools.MakeCircle(startPt, 10, Vector3.UNIT_Z);

                TopoShape pipe = GlobalInstance.BrepTools.MakePipe(section, path, ii);

                renderView.ShowGeometry(pipe, ++shapeId);
            }
            for (int ii = 0; ii < 3; ++ii)
            {
                Vector3 startPt = new Vector3(ii * 100, 100, 0);
                var points = new System.Collections.Generic.List<Vector3>();
                points.Add(startPt);
                points.Add(startPt + new Platform.Vector3(0, 0, 100));
                points.Add(startPt + new Platform.Vector3(50, 50, 150));
                TopoShape path = GlobalInstance.BrepTools.MakePolyline(points);
                Coordinate3 coord = new Coordinate3();
                coord.Origion = startPt - new Vector3(-5,-5,0);
                TopoShape section = GlobalInstance.BrepTools.MakeRectangle(10, 10, 2, coord);

                TopoShape pipe = GlobalInstance.BrepTools.MakePipe(section, path, ii);

                renderView.ShowGeometry(pipe, ++shapeId);
            }

            renderView.RequestDraw();
        }

        private void swee2ToolStripMenuItem_Click(object sender, EventArgs e)
        {




            AdvFeatureTools advTool = new AdvFeatureTools();

            {
                Coordinate3 coord = new Coordinate3();
                coord.Origion = new Vector3(-0.5f, -1, 0);
                TopoShape profile = GlobalInstance.BrepTools.MakeRectangle(1,2,0.2f, coord);

                TopoShape path = GlobalInstance.BrepTools.MakeLine(Vector3.ZERO, new Vector3(0, 0, 100));

                //define the "S" curve
                float[] S ={0,40,-80,
                            1,10,0};
                TopoShape sweepBody = advTool.MakeSweep(profile, path, S, true);
                renderView.ShowGeometry(sweepBody, ++shapeId);
            }
            {
                Primitive2dTools tool2d = new Primitive2dTools();
                float radius = 50;
                TopoShapeGroup group = new TopoShapeGroup();

                group.Add(tool2d.MakeArc(new Vector2(0, radius), radius, 0, 45));
                group.Add(tool2d.MakeLine(new Vector2(radius, radius), new Vector2(radius * 2, radius)));
                TopoShape spline = tool2d.ToBSplineCurve(group);

                TopoShape profile = GlobalInstance.BrepTools.MakeCircle(new Vector3(100, 100, 0), 1, Vector3.UNIT_Z);
                List<Vector3> pts = new List<Vector3>();
                pts.Add(new Vector3(100, 100, 0));
                pts.Add(new Vector3(100, 100, 100));
                pts.Add(new Vector3(100, 200, 400));

                TopoShape path = GlobalInstance.BrepTools.MakeSpline(pts);
                TopoShape sweepBody = advTool.MakeSweep(profile, path, spline, true);

                renderView.ShowGeometry(sweepBody, ++shapeId);
            }
        }

        private void imageToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFileDialog dlg = new OpenFileDialog();
            dlg.Filter = "Image Files (*.png;*.jpg;*.bmp)|*.png;*.jpg;*.bmp|All Files(*.*)|*.*";

            if (DialogResult.OK != dlg.ShowDialog())
                return;

            ImageNode node = new ImageNode();
            node.SetImage(new AnyCAD.Platform.Path(dlg.FileName));
            node.SetWidth(100);
            node.SetHeight(200);

            renderView.ShowSceneNode(node);
        }

        private void moveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            renderView.ExecuteCommand("MoveNode");
        }

        private void complexToolStripMenuItem_Click(object sender, EventArgs e)
        {
            float y = 10;
            TopoShape line1 =  GlobalInstance.BrepTools.MakeLine(new Vector3(0, 0, 0 + y), new Vector3(4, 0, 0 + y));
            TopoShape line2 =  GlobalInstance.BrepTools.MakeLine(new Vector3(4, 0, 0 + y), new Vector3(4, 0, 0.5f + y));
            TopoShape line3 =  GlobalInstance.BrepTools.MakeLine(new Vector3(4, 0, 0.5f + y), new Vector3(1, 0, 0.5f + y));
            TopoShape line4 =  GlobalInstance.BrepTools.MakeLine(new Vector3(1, 0, 0.5f + y), new Vector3(1, 0, 4 + y));
            TopoShape line5 =  GlobalInstance.BrepTools.MakeLine(new Vector3(1, 0, 4 + y), new Vector3(0.5f, 0, 4 + y));
            TopoShape line6 =  GlobalInstance.BrepTools.MakeLine(new Vector3(0.5f, 0, 4 + y), new Vector3(0.5f, 0, 0.5f + y));
            TopoShape line7 =  GlobalInstance.BrepTools.MakeLine(new Vector3(0.5f, 0, 0.5f + y), new Vector3(0, 0, 0.5f + y));
            TopoShape line8 =  GlobalInstance.BrepTools.MakeLine(new Vector3(0, 0, 0.5f + y), new Vector3(0, 0, 0 + y));
            TopoShapeGroup shapeGroup = new TopoShapeGroup();

            shapeGroup.Add(line2);
            shapeGroup.Add(line3);
            shapeGroup.Add(line4);
            shapeGroup.Add(line5);
            shapeGroup.Add(line6);
            shapeGroup.Add(line7);
            shapeGroup.Add(line8);
            shapeGroup.Add(line1);
            TopoShape profile =  GlobalInstance.BrepTools.MakeWire(shapeGroup);

            TopoShape line9 =  GlobalInstance.BrepTools.MakeLine(new Vector3(0, 0, 0 + y), new Vector3(0, -20, 0 + y));
            TopoShapeGroup lineGroup = new TopoShapeGroup();
            lineGroup.Add(line9);
            TopoShape wire =  GlobalInstance.BrepTools.MakeWire(lineGroup);
            TopoShape sweep =  GlobalInstance.BrepTools.Sweep(profile, wire, true);
            //shapeGroupall.Add(sweep);

            //renderView.ShowGeometry(sweep, ++shapeId);



            TopoShape line10 =  GlobalInstance.BrepTools.MakeLine(new Vector3(1, 0, 0.5f + y), new Vector3(1, 0, 3 + y));
            TopoShape line11 =  GlobalInstance.BrepTools.MakeLine(new Vector3(1, 0, 3 + y), new Vector3(2, 0, 3 + y));
            TopoShape line12 =  GlobalInstance.BrepTools.MakeLine(new Vector3(2, 0, 3 + y), new Vector3(2, 0, 0.5f + y));
            TopoShape line13 =  GlobalInstance.BrepTools.MakeLine(new Vector3(2, 0, 0.5f + y), new Vector3(1, 0, 0.5f + y));
            TopoShapeGroup shapeGroup1 = new TopoShapeGroup();
            shapeGroup1.Add(line10);
            shapeGroup1.Add(line11);
            shapeGroup1.Add(line12);
            shapeGroup1.Add(line13);
            TopoShape profile1 =  GlobalInstance.BrepTools.MakeWire(shapeGroup1);
            TopoShape line14 =  GlobalInstance.BrepTools.MakeLine(new Vector3(1, 0, 0.5f + y), new Vector3(1, -0.5f, 0.5f + y));
            TopoShapeGroup lineGroup1 = new TopoShapeGroup();
            lineGroup1.Add(line14);
            TopoShape wire1 =  GlobalInstance.BrepTools.MakeWire(lineGroup1);

            TopoShape sweep1 =  GlobalInstance.BrepTools.Sweep(profile1, wire1, true);
            //shapeGroupall.Add(sweep1);

            TopoShape comp = GlobalInstance.BrepTools.BooleanAdd(sweep, sweep1);
            RepairTools rt = new RepairTools();
            comp = rt.RemoveExtraEdges(comp);

            renderView.ShowGeometry(comp, ++shapeId);
            renderView.View3d.FitAll();

        }

        private void splitToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            TopoShape box = GlobalInstance.BrepTools.MakeBox(Vector3.ZERO, Vector3.UNIT_Z, new Vector3(150f, 150f, 150f));
            TopoShape sphere = GlobalInstance.BrepTools.MakeSphere(Vector3.ZERO, 100f);
            TopoShape sphere2 = GlobalInstance.BrepTools.MakeSphere(Vector3.ZERO, 50f);

            TopoShapeGroup tools = new TopoShapeGroup();
            tools.Add(sphere);
            tools.Add(sphere2);
            TopoShape split = GlobalInstance.BrepTools.MakeSplit(box, tools);


            TopoExplor expo = new TopoExplor();
            TopoShapeGroup faces = expo.ExplorFaces(split);
            TopoShapeGroup bodies = expo.ExplorSolids(split);


            Random random = new Random();
            for (int ii = 0; ii < bodies.Size(); ++ii)
            {
                SceneNode node = renderView.ShowGeometry(bodies.GetTopoShape(ii), ++shapeId);
                FaceStyle fs = new FaceStyle();
                fs.SetColor(new ColorValue((float)random.NextDouble(), (float)random.NextDouble(), (float)random.NextDouble(), 0.5f));
                fs.SetTransparent(true);
                node.SetFaceStyle(fs);
            }


            renderView.RequestDraw();

            MessageBox.Show(String.Format("Face: {0} Solid: {1}", faces.Size(), bodies.Size()));

        }

        private TopoShape LoadSplineFromFile()
        {
            OpenFileDialog dlg = new OpenFileDialog();
            dlg.Filter = "The Points(*.txt)|*.txt";
            if (dlg.ShowDialog() != DialogResult.OK)
                return null;

            List<Vector3> points = new List<Vector3>();
            String fileName = dlg.FileName;
            StreamReader sr = new StreamReader(fileName, Encoding.Default);
            String line;
            while ((line = sr.ReadLine()) != null)
            {
                String[] items = line.Split(' ');
                if (items.Length == 3)
                {
                    Vector3 pt = new Vector3();
                    pt.X = float.Parse(items[0]);
                    pt.Y = float.Parse(items[1]);
                    pt.Z = float.Parse(items[2]);

                    points.Add(pt);
                }
            }

            if (points.Count < 2)
                return null;

            return GlobalInstance.BrepTools.MakeSpline(points);
        }

        private void loft3ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            TopoShape spline1 = LoadSplineFromFile();
            renderView.ShowGeometry(spline1, ++shapeId);

            TopoShape spline2 = LoadSplineFromFile();
            renderView.ShowGeometry(spline2, ++shapeId);

            TopoShape shell = GlobalInstance.BrepTools.MakeLoft(spline1, spline2, false);
            renderView.ShowGeometry(shell, ++shapeId);

            renderView.RequestDraw();
        }


        private void polygonToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //List<Vector3> points = new List<Vector3>();


            //points.Add(new Vector3());
            //points.Add(new Vector3(100, 0, 0));
            //points.Add(new Vector3(200, 100, 0));
            //points.Add(new Vector3(200, 200, 0));
            //points.Add(new Vector3(100, 300, 0));
            //points.Add(new Vector3(-100, 200, 0));
            // 

            //6176 13667 5096
            //6176 13692 5096 
            //1900 13692 5096
            //1900 13667 5096
            List<Vector3> points = new List<Vector3>();
            points.Add(new Vector3(61.76f, 136.67f, 50.96f));
            points.Add(new Vector3(61.76f, 136.92f, 50.96f));
            points.Add(new Vector3(19.00f, 136.92f, 50.96f));
            points.Add(new Vector3(19.00f, 136.67f, 50.96f));
            TopoShape plygon = GlobalInstance.BrepTools.MakePolygon(points);
            TopoShape face = GlobalInstance.BrepTools.MakeFace(plygon);
            renderView.ShowGeometry(face, ++shapeId);
        }
        private void holeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //TopoShape spline1 = LoadSplineFromFile();
            //renderView.ShowGeometry(spline1, ++shapeId);

            //TopoShape spline2 = LoadSplineFromFile();
            //renderView.ShowGeometry(spline2, ++shapeId);

            //TopoShape shell = GlobalInstance.BrepTools.MakeLoft(spline1, spline2, false);

            //TopoShape hole = LoadSplineFromFile();
            ////renderView.ShowGeometry(hole, ++shapeId);

            //TopoShape faceWithHole = GlobalInstance.BrepTools.AddHole(shell, hole);
            //renderView.ShowGeometry(faceWithHole, ++shapeId);

            var cirle = GlobalInstance.BrepTools.MakeCircle(Vector3.ZERO, 10, Vector3.UNIT_Z);
            var face = GlobalInstance.BrepTools.Extrude(cirle, 100, Vector3.UNIT_Z);

           // renderView.ShowGeometry(face, ++shapeId);

            WireClassifier wc = new WireClassifier();
            wc.Initialize(face);
            TopoExplor te = new TopoExplor();
            var edges = te.ExplorEdges(wc.GetOuterWire());
            for(int ii=0, len = edges.Size(); ii<len; ++ii)
            {
                var edge = edges.GetAt(ii);
                GeomCurve curve = new GeomCurve();
                curve.Initialize(edge);
                if(curve.IsClosed())
                {
                    renderView.ShowGeometry(edge, ++shapeId);
                }
            }
        }

        private void evolvedToolStripMenuItem_Click(object sender, EventArgs e)
        {
            List<Vector3> points = new List<Vector3>();
            points.Add(new Vector3());
            points.Add(new Vector3(200, 0, 0));
            points.Add(new Vector3(200, 200, 0));
            points.Add(new Vector3(0, 200, 0));

            TopoShape polygon = GlobalInstance.BrepTools.MakePolygon(points);

            float radius = 100;
            TopoShape arc = GlobalInstance.BrepTools.MakeArc(Vector3.ZERO, new Vector3(-radius, -radius, 0), new Vector3(0, -radius, 0), Vector3.UNIT_Z);
            TopoShape line = GlobalInstance.BrepTools.MakeLine(new Vector3(-radius, -radius, 0), new Vector3(-radius, -radius * 2, 0));

            TopoShapeGroup edges = new TopoShapeGroup();
            edges.Add(arc);
            edges.Add(line);
            TopoShape wire = GlobalInstance.BrepTools.MakeWire(edges);
            

            Vector3 dirZ = new Vector3(1,-1,0);
            dirZ.Normalize();
            Vector3 dirX = dirZ.CrossProduct(Vector3.UNIT_Z);
            Coordinate3 coord = new Coordinate3(Vector3.ZERO, dirX, Vector3.UNIT_Z, dirZ);

            
            TopoShape path = GlobalInstance.BrepTools.Transform(wire, coord);
            renderView.ShowGeometry(path, ++shapeId);

            AdvFeatureTools advFT = new AdvFeatureTools();
            TopoShape shape = advFT.MakeEvolved(polygon, path, 0, true);

            renderView.ShowGeometry(shape, ++shapeId);
        }

        private void performanceToolStripMenuItem_Click(object sender, EventArgs e)
        {
            TopoShape box = GlobalInstance.BrepTools.MakeBox(Vector3.ZERO, Vector3.UNIT_Z, Vector3.UNIT_SCALE);
            RenderableGeometry geom = new RenderableGeometry();
            geom.SetGeometry(box);
            geom.SetShapeFilter((int)EnumPickMode.RF_Face);// only display face
            for (int ii = 0; ii < 10; ++ii)
            {
                for (int jj = 0; jj < 20; ++jj)
                {
                    EntitySceneNode node = new EntitySceneNode();
                    node.SetEntity(geom);
                    node.SetTransform(GlobalInstance.MatrixBuilder.MakeTranslate(ii * 5 - 250, jj * 5 - 500, 0));
                    renderView.ShowSceneNode(node);
                }
            }

            renderView.RequestDraw();
        }

        private void glueToolStripMenuItem_Click(object sender, EventArgs e)
        {
            TopoShape box1 = GlobalInstance.BrepTools.MakeBox(Vector3.ZERO, Vector3.UNIT_Z, new Vector3(100, 100, 100));
            TopoShape box2 = GlobalInstance.BrepTools.MakeBox(new Vector3(0, 0, -100), Vector3.UNIT_Z, new Vector3(100, 100, 100));

            TopoShapeGroup group = new TopoShapeGroup();
            group.Add(box1);
            group.Add(box2);

            TopoShape compound = GlobalInstance.BrepTools.MakeCompound(group);
            renderView.ShowGeometry(compound, 300);
            //RepairTools repairTool = new RepairTools();
            //TopoShape twoBox = repairTool.GlueFaces(compund, 0.00001f, true);

            //TopoDataExchangeStep step = new TopoDataExchangeStep();
            //step.Write(twoBox, "d:/twobox.stp");
        }


        double radius;
        double decrease;
        double cunterarray;
        public void drawSpiral(Vector3 startPoint, List<TopoShape> shapes)
        {
            radius = Math.Sqrt((startPoint.X * startPoint.X) + (startPoint.Y * startPoint.Y));
            double b = radius;

            double bx = 0;
            double by = 0;
            decrease = 0.01;
            double countersizeX = Math.Round(radius / decrease);
            cunterarray = 0; //countersizeX;
            int i = 1;
            while (b > 0)
            {
                b = b - (decrease);

                bx = Math.Cos(3 * i * Math.PI / 180) * b;
                double bxsqr = Math.Pow(bx, 2);
                double bsqr = Math.Pow(b, 2);

                by = Math.Sqrt(bsqr - bxsqr);

                // Set y value to negative if degrees is below y = 0
                if (i * 3 > 180 && i * 2 <= 360)
                    by = -1 * by;

                // Trying to get Z index of Shape Point by using spiral coordinate...
                Vector3 to = new Vector3((float)bx, (float)by, (float)1000.0);
                Ray r = new Ray(to, new Vector3(0, 0, -1000));

                for (int ii = 0; ii < shapes.Count; ++ii)
                {
                    IntersectionLineSurface intersector = new IntersectionLineSurface();
                    intersector.SetSurface(shapes[ii]);
                    if (intersector.Perform(r) && intersector.GetPointCount() > 0)
                    {
                        Vector3 intersect = intersector.GetPoint(1);
                        to.Z = intersect.Z;

                        break;
                    }
                }

                cunterarray = cunterarray + 1;
                TopoShape line = GlobalInstance.BrepTools.MakeLine(startPoint, to);
                startPoint = to;
                renderView.ShowGeometry(line, 103);
                i = i * 3 > 356 ? i = 0 : i + 1;
            }
        }
        private void intersectionToolStripMenuItem_Click(object sender, EventArgs e)
        {
            List<TopoShape> shapeList = new List<TopoShape>();
            MultiShapeQuery query = new MultiShapeQuery();
            renderView.QuerySelection(query);
            int nCount = query.GetCount();
            for (int ii = 0; ii < nCount; ++ii)
            {
                SelectedShapeQuery shapeQuery = query.GetSubContext(ii);
                shapeList.Add(shapeQuery.GetSubGeometry());
            }

            drawSpiral(Vector3.ZERO, shapeList);
        }

        private void solidToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFileDialog dlg = new OpenFileDialog();
            dlg.Filter = "STL (*.stl)|*.stl|IGES (*.igs;*.iges)|*.igs;*.iges|STEP (*.stp;*.step)|*.stp;*.step|BREP (*.brep)|*.brep|All Files(*.*)|*.*";

            if (DialogResult.OK != dlg.ShowDialog())
                return;


            TopoShape shape = GlobalInstance.BrepTools.LoadFile(new AnyCAD.Platform.Path(dlg.FileName));
            renderView.ShowGeometry(shape, 100);

            //TopoExplor explor = new TopoExplor();
            //TopoShapeGroup solids = explor.ExplorSolids(shape);
            //float vol = 0;
            //for(int ii=0; ii<solids.Size(); ++ii)
            //{
            //    TopoShapeProperty property = new TopoShapeProperty();
            //    property.SetShape(solids.GetAt(ii));
            //    vol += property.SolidVolume();
            //}
            TopoExplor explor = new TopoExplor();
            TopoShapeGroup faces = explor.ExplorFaces(shape);
            TopoShape solid = GlobalInstance.BrepTools.MakeSolid(faces);
            TopoShapeProperty property = new TopoShapeProperty();
            property.SetShape(solid);
            double vol = property.SolidVolume();

            MessageBox.Show(String.Format("{0}", vol));
        }

        private void gridPositionToolStripMenuItem_Click(object sender, EventArgs e)
        {
            WorkingPlane wp = renderView.Renderer.GetWorkingPlane();
            wp.SetPosition(new Vector3(0, 0, 100));

            renderView.RequestDraw();
        }

        int nLoadCount = 0;
        private void loadLinesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFileDialog dlg = new OpenFileDialog();
            dlg.Filter = "The Lines(*.txt;*.out)|*.txt;*.out";
            if (dlg.ShowDialog() != DialogResult.OK)
                return;

            if (nLoadCount > 1)
                nLoadCount = 0;

            LineStyle ls = new LineStyle();
            ls.SetColor(255 * nLoadCount, 0, 0);
            ls.SetLineWidth(2 + nLoadCount);
            ++nLoadCount;
            

            String fileName = dlg.FileName;
            StreamReader sr = new StreamReader(fileName, Encoding.Default);
            String line;
            while ((line = sr.ReadLine()) != null)
            {             
                String[] items = line.Split(' ');
                if (items.Length == 2)
                {
                    int nCount = int.Parse(items[1]);
                    while (nCount > 0)
                    {
                        --nCount;

                        Vector3 pt1 = new Vector3();
                        line = sr.ReadLine();
                        items = line.Split(' ');                       
                        if (items.Length == 3)
                        {

                            pt1.X = float.Parse(items[0]);
                            pt1.Y = float.Parse(items[1]);
                            pt1.Z = float.Parse(items[2]);
                        }

                        Vector3 pt2 = new Vector3();
                        line = sr.ReadLine();
                        items = line.Split(' ');
                        if (items.Length == 3)
                        {

                            pt2.X = float.Parse(items[0]);
                            pt2.Y = float.Parse(items[1]);
                            pt2.Z = float.Parse(items[2]);
                        }

                        LineNode node = new LineNode();
                        node.SetStartPt(pt1);
                        node.SetEndPt(pt2);
                        node.SetLineStyle(ls);

                        renderView.ShowSceneNode(node);
                    }

                }

            }
        }

        private void loadPointsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFileDialog dlg = new OpenFileDialog();
            dlg.Filter = "The Lines(*.txt;*.pts)|*.txt;*.pts";
            if (dlg.ShowDialog() != DialogResult.OK)
                return;

                        String fileName = dlg.FileName;
            StreamReader sr = new StreamReader(fileName, Encoding.Default);
            String line;
            while ((line = sr.ReadLine()) != null)
            {
                String[] items = line.Split('\t');
                if (items.Length == 4)    
                {
                    String id = items[0];

                    Vector3 pt1 = new Vector3();
                    pt1.X = float.Parse(items[1]);
                    pt1.Y = float.Parse(items[2]);
                    pt1.Z = float.Parse(items[3]);

                    PointNode pn = new PointNode();
                    pn.SetName(id);
                    pn.SetPoint(pt1);
                    pn.SetShowText(true);

                    renderView.ShowSceneNode(pn);
                }
            }
        }

        private void iteratorToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SceneNodeIterator itr = renderView.SceneManager.NewSceneNodeIterator();
            while (itr.More())
            {
                SceneNode node = itr.Next();
                MessageBox.Show(node.GetName());
            }
        }

        private void tTToolStripMenuItem_Click(object sender, EventArgs e)
        {
            TopoShape arc = GlobalInstance.BrepTools.MakeArc(Vector3.ZERO, 100, 0, 135, Vector3.UNIT_Z);
            TopoShape cir = GlobalInstance.BrepTools.MakeCircle(new Vector3(-200, 0, 0), 50, Vector3.UNIT_X);
            TopoShape surf1 = GlobalInstance.BrepTools.Extrude(arc, 100, Vector3.UNIT_Z);
            TopoShape surf2 = GlobalInstance.BrepTools.Extrude(cir, 400, Vector3.UNIT_X);
            TopoShape wire = GlobalInstance.BrepTools.SurfaceSection(surf1, surf2);

            renderView.ShowGeometry(surf1, 100);
            renderView.ShowGeometry(surf2, 100);
            var no = renderView.ShowGeometry(wire, 100);
            LineStyle ls = new LineStyle();
            ls.SetColor(255, 0, 0);
            ls.SetLineWidth(4);
            no.SetLineStyle(ls);
        }

        private void axesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            AxesWidget axes = new AxesWidget();
            Matrix4 trf = GlobalInstance.MatrixBuilder.MakeTranslate(100, 100, 100);
            axes.SetTransform(trf);

            renderView.ShowSceneNode(axes);
        } 

        private void arrowToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ArrowWidget arrow = new ArrowWidget();
            Matrix4 trf = GlobalInstance.MatrixBuilder.MakeTranslate(100, 100, 100);
            arrow.SetTransform(trf);

            renderView.ShowSceneNode(arrow);
        }

        private void sKPToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFileDialog dlg = new OpenFileDialog();
            dlg.Filter = "SKP (*.skp)|*.skp|All Files(*.*)|*.*";

            if (DialogResult.OK != dlg.ShowDialog())
                return;
            
            ModelReader reader = new ModelReader();
            GroupSceneNode node = reader.LoadFile(new AnyCAD.Platform.Path(dlg.FileName));
            if (node != null)
            {
                renderView.ShowSceneNode(node);
            }
        }

        Texture animationTexture = null;
        bool useTextureA = true;
        String textureA;
        String textureB;
        private void textureAnimationToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (animationTexture == null)
            {

                OpenFileDialog dlg = new OpenFileDialog();
                dlg.Filter = "Image Files (*.png;*.jpg;*.bmp)|*.png;*.jpg;*.bmp|All Files(*.*)|*.*";

                if (DialogResult.OK != dlg.ShowDialog())
                    return;

                textureA = dlg.FileName;

                if (DialogResult.OK != dlg.ShowDialog())
                    return;
                textureB = dlg.FileName;

                animationTexture = new Texture();
                animationTexture.SetFilePath(new AnyCAD.Platform.Path(textureA));
                
                TopoShape box = GlobalInstance.BrepTools.MakeBox(Vector3.ZERO, Vector3.UNIT_Z, new Vector3(100, 100, 100));

                SceneNode node = renderView.ShowGeometry(box, 100);

                FaceStyle fs = new FaceStyle();
                fs.SetTexture(0, animationTexture);

                node.SetFaceStyle(fs);

                bEnabledAnimation = false;
            }

            if (bEnabledAnimation)
            {
                this.renderView.RenderTick -= new AnyCAD.Presentation.RenderEventHandler(FormMain_RenderTickTextureAnimation);
            }
            else
            {
                this.renderView.RenderTick += new AnyCAD.Presentation.RenderEventHandler(FormMain_RenderTickTextureAnimation);
            }
            bEnabledAnimation = !bEnabledAnimation;
        }

        private void FormMain_RenderTickTextureAnimation()
        {
            if (useTextureA)
            {
                animationTexture.SetFilePath(new AnyCAD.Platform.Path(textureA));
            }
            else 
            {
                animationTexture.SetFilePath(new AnyCAD.Platform.Path(textureB));
            }
            useTextureA = !useTextureA;
            this.renderView.RequestDraw();
        }

        Texture texture = null;
        private void imageBitsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var pathName = new AnyCAD.Platform.Path("dd:xx");
            GlobalInstance.ImageLibrary.RemoveImage(pathName);


            OpenFileDialog dlg = new OpenFileDialog();
            dlg.Filter = "Image Files (*.png)|*.png|All Files(*.*)|*.*";
            if (DialogResult.OK != dlg.ShowDialog())
                return;
            FileStream file = new FileStream(dlg.FileName, FileMode.Open, FileAccess.Read);
            BinaryReader read = new BinaryReader(file);
            int count = (int)file.Length;
            byte[] buffer = new byte[count];
            read.Read(buffer, 0, buffer.Length);


            if (texture == null)
            {
                texture = new Texture();
                TopoShape box = GlobalInstance.BrepTools.MakeBox(Vector3.ZERO, Vector3.UNIT_Z, new Vector3(100, 100, 100));

                SceneNode node = renderView.ShowGeometry(box, 100);
                
                FaceStyle fs = new FaceStyle();
                fs.SetTexture(0, texture);

                node.SetFaceStyle(fs);
            }
            texture.SetFileBuffer(pathName, buffer);

            this.renderView.RequestDraw();
        }

        private void loadHermitSplineToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFileDialog dlg = new OpenFileDialog();
            dlg.Filter = "Hermite Spline(*.txt;*.hs)|*.txt;*.hs";
            if (dlg.ShowDialog() != DialogResult.OK)
                return;

            String fileName = dlg.FileName;
            StreamReader sr = new StreamReader(fileName, Encoding.Default);

            int ww = 0;
            String line;
            while ((line = sr.ReadLine()) != null)
            {
                String[] items = line.Split('\t');
                if (items.Length >= 4)
                {
                    String id = items[0];


                    int ctrlPtCount = int.Parse(items[1]);
                    int tangents = int.Parse(items[2]);
                    int pa = int.Parse(items[3]);

                    String lineOfPts = sr.ReadLine();
                    String lineOfTangents = sr.ReadLine();
                    String lineOfPa = sr.ReadLine();

                    String[] itemPts = lineOfPts.Split('\t');

                    List<Vector3> pts = new List<Vector3>();
                    for (int ii = 0; ii < ctrlPtCount; ++ii)
                    {

                        Vector3 pt1 = new Vector3();
                        pt1.X = float.Parse(itemPts[ii * 3]);
                        pt1.Y = float.Parse(itemPts[ii * 3 + 1]);
                        pt1.Z = float.Parse(itemPts[ii * 3 + 2]);

                        pts.Add(pt1);
                    }
                    var shape = GlobalInstance.BrepTools.MakeSpline(pts);
                    var node = renderView.ShowGeometry(shape, 1);

                    LineStyle ls = new LineStyle();
                    ls.SetLineWidth(1 + ww);
                    node.SetLineStyle(ls);
                    ++ww;
                }

                break;
            }
        }

        private void loadDHermiteRToolStripMenuItem_Click(object sender, EventArgs e)
        {
                        OpenFileDialog dlg = new OpenFileDialog();
            dlg.Filter = "Hermite Spline(*.out;*.hs)|*.out;*.hs";
            if (dlg.ShowDialog() != DialogResult.OK)
                return;

            String fileName = dlg.FileName;
            StreamReader sr = new StreamReader(fileName, Encoding.Default);
            LineStyle ls = new LineStyle();
            ls.SetColor(255, 0, 0);
            ls.SetLineWidth(2);

            String line;
            while ((line = sr.ReadLine()) != null)
            {
                String[] items = line.Split('\t');
                if(items.Length == 0)
                    continue;

                if (items[0] == "Line")
                {
                    line = sr.ReadLine();
                    String[] subItems = line.Split('\t');

                    Vector3 pt = new Vector3();
                    pt.X = double.Parse(subItems[0]);
                    pt.Y = double.Parse(subItems[1]);
                    pt.Z = double.Parse(subItems[2]);

                    Vector3 pt2 = new Vector3();
                    pt2.X = double.Parse(subItems[3]);
                    pt2.Y = double.Parse(subItems[4]);
                    pt2.Z = double.Parse(subItems[5]);

                    LineNode ln = new LineNode();
                    ln.SetStartPt(pt);
                    ln.SetEndPt(pt2);
                    ln.SetLineStyle(ls);
                    renderView.ShowSceneNode(ln);

                }
                else if (items[0] == "Hermite")
                {
                    int ctrlCount = int.Parse(items[2]);
                    line = sr.ReadLine();
                    String[] subItems = line.Split('\t');

                    List<Vector3> pts = new List<Vector3>();
                    for (int ii = 0; ii < ctrlCount; ++ii)
                    {
                        Vector3 pt = new Vector3();
                        pt.X = double.Parse(subItems[ii*3]);
                        pt.Y = double.Parse(subItems[ii*3 + 1]);
                        pt.Z = double.Parse(subItems[ii*3 + 2]);

                        pts.Add(pt);
                    }

                    var shape = GlobalInstance.BrepTools.MakeSpline(pts);
                    var node = renderView.ShowGeometry(shape, 1);
                    node.SetLineStyle(ls);
                }
            }
        }



        private void arcToolStripMenuItem_Click(object sender, EventArgs e)
        {
            {
                //这段显示有问题
                // 下面半径比这段大10
                TopoShape arc = GlobalInstance.BrepTools.MakeArc(new Vector3(1025.445, 932.979, 0),
                                        new Vector3(1740.748, 766.2047, 0),
                                        new Vector3(1346.25, 604.377, 0),
                                        Vector3.UNIT_Z);

                renderView.ShowGeometry(arc, 100);
            }
            {
                TopoShape arc = GlobalInstance.BrepTools.MakeArc(new Vector3(1000, 870, 0),
                                        new Vector3(1750, 770, 0),
                                        new Vector3(1346.25, 604.377, 0),
                                        Vector3.UNIT_Z);
                renderView.ShowGeometry(arc, 101);
            }
        }

        private void hLRToolStripMenuItem_Click(object sender, EventArgs e)
        {
            HiddenLineRemovalTool hlr = new HiddenLineRemovalTool();
            hlr.Initialize(new Vector3(0, 0, 1), new Vector3(0, 0, 100), new Vector3(0, 1, 0), 100, false);

            TopoShape tp = GlobalInstance.BrepTools.MakeCylinder(Vector3.ZERO, Vector3.UNIT_Y, 10, 10, 0);

            hlr.Compute(tp);

            LineStyle ls = new LineStyle();
            ls.SetColor(255, 0, 0);
            ls.SetLineWidth(2);
            
            var visibuleEdges = hlr.GetVisibleEdges();
            for(int ii=0, len = visibuleEdges.Size(); ii<len; ++ii)
            {
               var node =  renderView.ShowGeometry(visibuleEdges.GetAt(ii), ii);
                node.SetLineStyle(ls);
            }


            LineStyle ls2 = new LineStyle();
            ls2.SetColor(0, 255, 0);

            var hiddenEdges = hlr.GetHiddenEdges();
            for (int ii = 0, len = hiddenEdges.Size(); ii < len; ++ii)
            {
              var node =  renderView.ShowGeometry(hiddenEdges.GetAt(ii), ii);
                node.SetLineStyle(ls2);
            }

            renderView.RequestDraw();
        }

        private void manySphereToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var sphere = GlobalInstance.BrepTools.MakeSphere(Vector3.ZERO, 1);
            var entity = GlobalInstance.TopoShapeConvert.ToEntity(sphere, 0.1);
            entity.SetShapeFilter((int)EnumPickMode.RF_Face);
            var random = new Random();
            for(int ii=0; ii<10000; ++ii)
            {
                var node = new EntitySceneNode();
                node.SetEntity(entity);             
                node.SetId(new ElementId(ii));

                double radius = random.NextDouble() * 20;
                var scale = GlobalInstance.MatrixBuilder.MakeScale(radius, radius, radius);
                var position = GlobalInstance.MatrixBuilder.MakeTranslate(random.NextDouble() * 1000, random.NextDouble() * 1000, random.NextDouble() * 1000);

                var trf = GlobalInstance.MatrixBuilder.Multiply(position, scale);

                node.SetTransform(trf);

                renderView.ShowSceneNode(node);
            }

            renderView.RequestDraw();
        }

        private void sweep3ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // create the path
            var path = GlobalInstance.BrepTools.MakeArc(new Vector3(100, 100, 0), new Vector3(-100, 100, 0), Vector3.ZERO, Vector3.UNIT_Z);

            // Get the sketch position and direction
            GeomCurve curve = new GeomCurve();
            curve.Initialize(path);

            var rt = curve.D1(curve.FirstParameter());

            var position = rt[0];
            var dir = rt[1];
            dir.Normalize();

            var sketch = GlobalInstance.BrepTools.MakeCircle(position, 5, dir);

            // Create pipe
            var pipe =  GlobalInstance.BrepTools.Sweep(sketch, path, true);

            renderView.ShowGeometry(pipe, 100);
        }

        private void lineToolStripMenuItem_Click(object sender, EventArgs e)
        {
            TopoShape line = GlobalInstance.BrepTools.MakeLine(new Vector3(1, 1, 1), new Vector3(2, 2, 2));
            var node =  GlobalInstance.TopoShapeConvert.ToSceneNode(line, 1);
            renderView.ShowSceneNode(node);
        }
    }
}
