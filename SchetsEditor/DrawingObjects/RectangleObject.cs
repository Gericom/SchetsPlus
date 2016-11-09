using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SchetsEditor.DrawingObjects
{
    public class RectangleObject : ShapeObject
    {
        public RectangleObject() { }
        public RectangleObject(BinaryReader reader)
            : base(reader)
        { }

        public override DrawingObject Clone()
        {
            RectangleObject cloneObject = new RectangleObject();
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
                g.FillRectangle(new SolidBrush(colorOverride), new Rectangle(Position, Size));
            else
                g.DrawRectangle(new Pen(new SolidBrush(colorOverride), (picking ? 4 : 2)), new Rectangle(Position, Size));
            g.Restore(gs);
        }

        public override void Write(BinaryWriter writer)
        {
            writer.Write((byte)DrawingObject.DrawingObjectType.RectangleObject);
            base.Write(writer);
        }
    }
}
