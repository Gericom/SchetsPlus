using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SchetsEditor.DrawingObjects
{
    public abstract class DrawingObject
    {
        public enum DrawingObjectType : byte
        {
            RectangleObject,
            LineObject,
            EllipseObject,
            TextObject,
            BitmapObject
        }

        protected Point mRotationCenter;
        protected int mRotationAngle;

        public DrawingObject() { }
        public DrawingObject(BinaryReader reader)
        {
            mRotationCenter = new Point(reader.ReadInt32(), reader.ReadInt32());
            mRotationAngle = reader.ReadInt32();
        }

        public abstract DrawingObject Clone();

        public virtual void CopySettingsTo(DrawingObject cloneObject)
        {
            cloneObject.mRotationAngle = mRotationAngle;
            cloneObject.mRotationCenter = mRotationCenter;
        }

        public virtual void Draw(Graphics g)
        {
            Draw(g, Color.Black);
        }

        public abstract void Draw(Graphics g, Color colorOverride, bool picking = false);

        public void FixRot(ref Point p)
        {
            if (mRotationAngle == 0) ;
            else if (mRotationAngle == 90)
            {
                int swap = p.Y;
                p.Y = -p.X;
                p.X = swap;
            }
            else if (mRotationAngle == 180)
            {
                p.X = -p.X;
                p.Y = -p.Y;
            }
            else if (mRotationAngle == 270)
            {
                int swap = p.Y;
                p.Y = p.X;
                p.X = -swap;
            }
        }

        public void Rotate(int width, int height)
        {
            mRotationCenter = new Point(width / 2, height / 2);
            mRotationAngle += 90;
            if (mRotationAngle >= 360)
                mRotationAngle -= 360;
        }

        public virtual void Write(BinaryWriter writer)
        {
            writer.Write(mRotationCenter.X);
            writer.Write(mRotationCenter.Y);
            writer.Write(mRotationAngle);
        }

        public abstract void Translate(Point delta);
    }
}
