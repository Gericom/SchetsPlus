using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SchetsEditor.DrawingObjects
{
    public class EllipseObject : ShapeObject
    {
        public EllipseObject() { }
        public EllipseObject(BinaryReader reader)
            : base(reader)
        {

        }

        public override DrawingObject Clone()
        {
            EllipseObject cloneObject = new EllipseObject();
            CopySettingsTo(cloneObject);
            return cloneObject;
        }

        public override void Draw(Graphics g, Color colorOverride, bool picking = false)
        {
            var gs = g.Save();
            g.TranslateTransform(mRotationCenter.X, mRotationCenter.Y);
            g.RotateTransform(mRotationAngle);
            g.TranslateTransform(-mRotationCenter.X, -mRotationCenter.Y);
            if (Filled)
                g.FillEllipse(new SolidBrush(colorOverride), new Rectangle(Position, Size));
            else
                g.DrawEllipse(new Pen(new SolidBrush(colorOverride), (picking ? LineWidth + 2 : LineWidth)), new Rectangle(Position, Size));
            g.Restore(gs);
        }

        public override void Write(BinaryWriter writer)
        {
            writer.Write((byte)DrawingObject.DrawingObjectType.EllipseObject);
            base.Write(writer);
        }
    }
}
