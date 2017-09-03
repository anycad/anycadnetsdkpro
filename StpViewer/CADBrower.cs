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
        private System.Collections.Generic.Dictionary<int, FaceStyle> faceStyleDict = new System.Collections.Generic.Dictionary<int, FaceStyle>();
        public CADBrower(System.Windows.Forms.TreeView _treeView, AnyCAD.Presentation.RenderWindow3d _renderView)
        {
            treeView = _treeView;
            renderView = _renderView;
            faceStyle = new FaceStyle();

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
            nodeStack.Push(nodeStack.Peek().Nodes.Add(String.Format("{0}", nShapeCount), "Shape"));
            ++nShapeCount;
            SceneNode node = renderView.ShowGeometry(shape, nShapeCount);
            node.SetFaceStyle(faceStyle);
            return false;
        }

        public override void OnEndComplexShape()
        {
            nodeStack.Pop();
        }

        public override void OnFace(TopoShape shape)
        {
            ++nShapeCount;
            nodeStack.Peek().Nodes.Add(String.Format("{0}", nShapeCount), "Face");
            SceneNode node = renderView.ShowGeometry(shape, nShapeCount);
            node.SetFaceStyle(faceStyle);
        }
    }
}
