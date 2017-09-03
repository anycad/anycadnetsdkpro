using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AnyCAD.Platform;

namespace AnyCAD.Basic
{
    public class ReadShapeContext : AnyCAD.Platform.TopoShapeReaderContext
    {
        private int nShapeCount = 100;

        AnyCAD.Platform.SceneManager sceneManager = null;

        public ReadShapeContext(AnyCAD.Platform.SceneManager sm)
        {
            sceneManager = sm;
        }

        private void ShowGeometry(TopoShape topoShape)
        {
            AnyCAD.Platform.SceneNode faceNode = AnyCAD.Platform.GlobalInstance.TopoShapeConvert.ToEntityNode(topoShape, 0.5f);
            faceNode.SetId(++nShapeCount);
            sceneManager.AddNode(faceNode);
        }

        public override void OnBeginGroup(String name)
        {
        }

        public override void OnEndGroup()
        {

        }

        public override void OnCompound(TopoShape shape)
        {
      
        }

        public override void OnSolid(TopoShape shape)
        {
      
        }

        public override void OnShell(TopoShape shape)
        {
  
        }

        public override void OnFace(TopoShape shape)
        {
 
        }

        public override void OnWire(TopoShape shape)
        {
            ShowGeometry(shape);
        }

        public override void OnEdge(TopoShape shape)
        {
            ShowGeometry(shape);
        }

        public override void OnPoint(TopoShape shape)
        {

        }
    }
}
