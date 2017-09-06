using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AnyCAD.Platform;
using AnyCAD.Presentation;

namespace AnyCAD.Designer
{

    class PartElement
    {
        public ElementId Id;
        public TopoShape Shape = null;
        public SceneNode Node = null;
    }

    class BoltElement : PartElement
    {

        public double Length = 100;
        public double Diameter = 10;
        public double Thickness = 2;



        public BoltElement(int id)
        {
            Id = new ElementId(id);
        }

        public void UpdateShape()
        {
            double radius = Diameter * 0.5;
            if (Thickness >= radius)
                throw new Exception("INVALID PARAMETERS!");

            Shape = GlobalInstance.BrepTools.MakeTube(Vector3.ZERO, Vector3.UNIT_Z, radius - Thickness, Thickness, Length);


        }
    }

    class NutElement: PartElement
    {
        public double Length = 100;
        public double Diameter = 14;
        public double Thickness = 2;

        public NutElement(int id)
        {
            Id = new ElementId(id);
        }

        public void UpdateShape()
        {
            double radius = Diameter * 0.5;
            if (Thickness >= radius)
                throw new Exception("INVALID PARAMETERS!");

            double radiusIn = radius - Thickness;
            //var bottom = GlobalInstance.BrepTools.MakeCylinder(Vector3.ZERO, Vector3.UNIT_Z, radius, Thickness, 0);
            //var tube = GlobalInstance.BrepTools.MakeTube(new Vector3(0, 0, Thickness), Vector3.UNIT_Z, radiusIn, Thickness, Length);

            //Shape = GlobalInstance.BrepTools.BooleanAdd(tube, bottom);
            List<Vector3> points = new List<Vector3>();
            points.Add(Vector3.ZERO);
            points.Add(new Vector3(radius,0,0));
            points.Add(new Vector3(radius, 0, Length));
            points.Add(new Vector3(radiusIn, 0, Length));
            points.Add(new Vector3(radiusIn, 0, Thickness));
            points.Add(new Vector3(0, 0, Thickness));

            var sketch =  GlobalInstance.BrepTools.MakePolygon(points);
            Shape = GlobalInstance.BrepTools.Revol(sketch, Vector3.ZERO, Vector3.UNIT_Z, 360);
        }
    }

     

    class TubeSystem
    {
        public BoltElement Bolt;
        public NutElement Nut;

        public double Deep = 0;
        public double Step = 1;

        FaceStyle RedStyle;
        FaceStyle BlueStyle;

        public delegate void UpdateViewEvent();

        public UpdateViewEvent OnUpdate;

        public TubeSystem()
        {
            Bolt = new BoltElement(100);
            Nut = new NutElement(101);

            RedStyle = new FaceStyle();
            RedStyle.SetColor(255, 100, 100);

            BlueStyle = new FaceStyle();
            BlueStyle.SetColor(100, 100, 255);
        }

        public void Init(RenderWindow3d renderView)
        {
            // TODO: change parameters
            Bolt.UpdateShape();
            Nut.UpdateShape();

            Bolt.Node = renderView.ShowGeometry(Bolt.Shape, Bolt.Id);

            Bolt.Node.SetFaceStyle(RedStyle);


            Nut.Node = renderView.ShowGeometry(Nut.Shape, Nut.Id);
            FaceStyle faceStyle = new FaceStyle();
            faceStyle.SetTransparecy(0.5f);
            faceStyle.SetTransparent(true);
            Nut.Node.SetFaceStyle(faceStyle);
        }

        public void Connect(double deep)
        {
            //move Nut
            var trf = GlobalInstance.MatrixBuilder.MakeTranslate(0, 0, deep);
            Bolt.Node.SetTransform(trf);

            if (deep > 50)
                Bolt.Node.SetFaceStyle(BlueStyle);
            else
                Bolt.Node.SetFaceStyle(RedStyle);
        }

        public void OnTimer()
        {
            Deep += Step;
            if (Deep >= 100)
            {
                Deep = 100;
                Step = -1;
            }
            else if (Deep <= 2)
            {
                Step = 1;
                Deep = 2;
            }

            Connect(Deep);

            if (OnUpdate!=null)
                OnUpdate();
        }
    }
}
