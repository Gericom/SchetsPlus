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
        public Point[] Points { get; set; }
        public int LineWidth { get; set; }

        public LineObject() { }
        public LineObject(BinaryReader reader)
            : base(reader)
        {
            LineWidth = reader.ReadInt32();
            int count = reader.ReadInt32();
            Points = new Point[count];
            for (int i = 0; i < count; i++)
            {
                Points[i] = new Point(reader.ReadInt32(), reader.ReadInt32());
            }
        }

        public override DrawingObject Clone()
        {
            LineObject cloneObject = new LineObject();
            CopySettingsTo(cloneObject);
            return cloneObject;
        }

        public override void CopySettingsTo(DrawingObject cloneObject)
        {
            base.CopySettingsTo(cloneObject);
            ((LineObject)cloneObject).Points = Points;
            ((LineObject)cloneObject).LineWidth = LineWidth;
        }

        public override void Draw(Graphics g, Color colorOverride, bool picking = false)
        {
            if (Points == null || Points.Length < 2 || LineWidth < 1)
                return;
            var gs = g.Save();
            g.TranslateTransform(mRotationCenter.X, mRotationCenter.Y);
            g.RotateTransform(mRotationAngle);
            g.TranslateTransform(-mRotationCenter.X, -mRotationCenter.Y);
            if (Points.Length == 2 && Points[0] == Points[1])
            {
                int lwidth = (picking ? LineWidth + 2 : LineWidth);
                g.FillEllipse(new SolidBrush(colorOverride), Points[0].X - lwidth / 2f, Points[0].Y - lwidth / 2f, lwidth, lwidth);
            }
            else
            {
                var pen = new Pen(new SolidBrush(colorOverride), (picking ? LineWidth + 2 : LineWidth))
                { StartCap = System.Drawing.Drawing2D.LineCap.Round, EndCap = System.Drawing.Drawing2D.LineCap.Round, LineJoin = System.Drawing.Drawing2D.LineJoin.Round };
                g.DrawLines(pen, Points);
            }
            g.Restore(gs);
        }

        public override void Write(BinaryWriter writer)
        {
            writer.Write((byte)DrawingObject.DrawingObjectType.LineObject);
            base.Write(writer);
            writer.Write(LineWidth);
            writer.Write(Points.Length);
            foreach (Point p in Points)
            {
                writer.Write(p.X);
                writer.Write(p.Y);
            }
        }

        public override void Translate(Point delta)
        {
            FixRot(ref delta);
            for(int i = 0; i < Points.Length; i++)
                Points[i] = new Point(Points[i].X + delta.X, Points[i].Y + delta.Y);
        }
    }
}
