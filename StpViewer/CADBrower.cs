using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AnyCAD.Platform;
using AnyCAD.Presentation;
using System.IO;

namespace StpViewer
{
    class CADBrower : AnyCAD.Platform.TopoShapeReaderContext
    {

        private System.Windows.Forms.TreeView treeView = null;
        private AnyCAD.Presentation.RenderWindow3d renderView = null;
        private Stack<System.Windows.Forms.TreeNode> nodeStack = new Stack<System.Windows.Forms.TreeNode>();
        private int nShapeCount = 100;
        private FaceStyle faceStyle;
        private LineStyle holeStyle;
        private System.Collections.Generic.Dictionary<int, FaceStyle> faceStyleDict = new System.Collections.Generic.Dictionary<int, FaceStyle>();
        public CADBrower(System.Windows.Forms.TreeView _treeView, AnyCAD.Presentation.RenderWindow3d _renderView)
        {
            treeView = _treeView;
            renderView = _renderView;
            faceStyle = new FaceStyle();
            holeStyle = new LineStyle();
            holeStyle.SetLineWidth(3);
            holeStyle.SetColor(0, 256, 0);

            System.Windows.Forms.TreeNode node = treeView.Nodes.Add("AnyCAD.net");
            nodeStack.Push(node);
        }

        ~CADBrower()
        {
            //fileSys.Close();
        }
        public override void OnSetFaceColor(ColorValue clr)
        {
            if (clr.ToRGBA() == faceStyle.GetColor().ToRGBA())
                return;

            FaceStyle fs = null;
            if (!faceStyleDict.TryGetValue(clr.ToRGBA(), out fs))
            {
                fs = new FaceStyle();
                fs.SetColor(clr);
                faceStyleDict.Add(clr.ToRGBA(), fs);
            }
           // fs.SetTransparecy(0.5f);
            //fs.SetTransparent(true);
            faceStyle = fs;
        }
        

        public override void OnBeginGroup(String name)
        {
            if (name.Length == 0)
            {
                name = "<UNKNOWN>";
            }

            if (nodeStack.Count == 0)
            {
                System.Windows.Forms.TreeNode node = treeView.Nodes.Add(name);
                nodeStack.Push(node);
            }
            else
            {
                nodeStack.Push(nodeStack.Peek().Nodes.Add(name));
            }
        }

        public override void OnEndGroup()
        {
            nodeStack.Pop();
        }

        public override bool OnBeiginComplexShape(TopoShape shape)
        {
            ++nShapeCount;
            String type = "Shape";
            var st = shape.GetShapeType();
            if (st == EnumTopoShapeType.Topo_COMPOUND)
            {
                type = "Compound";
            }
            else if(st == EnumTopoShapeType.Topo_COMPSOLID)
            {
                type = "CompSolid";
            }
            else if(st == EnumTopoShapeType.Topo_SOLID)
            {
                type = "Solid";
            }
            else if(st == EnumTopoShapeType.Topo_SHELL)
            {
                type = "Shell";
            }
            nodeStack.Push(nodeStack.Peek().Nodes.Add(String.Format("{0}", nShapeCount), type));
            //SceneNode node = renderView.ShowGeometry(shape, nShapeCount);
            //node.SetFaceStyle(faceStyle);
            return true;
        }

        public override void OnEndComplexShape()
        {
            nodeStack.Pop();
        }

        public override void OnFace(TopoShape face)
        {
            ++nShapeCount;
            nodeStack.Peek().Nodes.Add(String.Format("{0}", nShapeCount), String.Format("Face {0}", nShapeCount));

            SceneNode node = renderView.ShowGeometry(face, nShapeCount);
            node.SetFaceStyle(faceStyle);

            GeomSurface gs = new GeomSurface();
            gs.Initialize(face);
            if (gs.IsUClosed() || gs.IsVClosed())
            {
                //SceneNode node = renderView.ShowGeometry(face, nShapeCount);
                //node.SetFaceStyle(faceStyle);
                WireClassifier wc = new WireClassifier();
                if (!wc.Initialize(face))
                    return;

                var holes = wc.GetInnerWires();
                for (int ii = 0, len = holes.Size(); ii < len; ++ii)
                {
                    var holeEdge = holes.GetAt(ii);
                    ++nShapeCount;
                    var holeNode = renderView.ShowGeometry(holeEdge, nShapeCount);
                    holeNode.SetLineStyle(holeStyle);
                }
            }





            //var outer = wc.GetOuterWire();
            //TopoExplor te = new TopoExplor();
            //var edges = te.ExplorEdges(outer);
            //for (int ii = 0, len = edges.Size(); ii < len; ++ii)
            //{
            //    var edge = edges.GetAt(ii);
            //    GeomCurve curve = new GeomCurve();
            //    curve.Initialize(edge);
            //    if (curve.IsClosed())
            //    {
            //        var holeNode = renderView.ShowGeometry(edge, ++nShapeCount);
            //        holeNode.SetLineStyle(holeStyle);
            //    }
            //}
        }
    }
}
