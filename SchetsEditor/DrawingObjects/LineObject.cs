using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SchetsEditor.DrawingObjects
{
    public class LineObject : VectorObject
    {
        public Point[] Points;

        public LineObject() { }
        public LineObject(BinaryReader reader)
            : base(reader)
        {
            int count = reader.ReadInt32();
            Points = new Point[count];
            for (int i = 0; i < count; i++)
            {
                Points[i] = new Point(reader.ReadInt32(), reader.ReadInt32());
            }
        }

        public override void Draw(Graphics g, Color colorOverride, bool picking = false)
        {
            if (Points == null || Points.Length < 2)
                return;
            var gs = g.Save();
            g.TranslateTransform(mRotationCenter.X, mRotationCenter.Y);
            g.RotateTransform(mRotationAngle);
            g.TranslateTransform(-mRotationCenter.X, -mRotationCenter.Y);
            var pen = new Pen(new SolidBrush(colorOverride), (picking ? 5 : 3));
            g.DrawCurve(pen, Points);
            g.Restore(gs);
        }

        public override void Write(BinaryWriter writer)
        {
            writer.Write((byte)DrawingObject.DrawingObjectType.LineObject);
            base.Write(writer);
            writer.Write(Points.Length);
            foreach (Point p in Points)
            {
                writer.Write(p.X);
                writer.Write(p.Y);
            }
        }
    }
}
