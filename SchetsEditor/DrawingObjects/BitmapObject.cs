using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace SchetsEditor.DrawingObjects
{
    public class BitmapObject : DrawingObject
    {
        public Point Position { get; set; }
        public Bitmap Bitmap { get; set; }
        public bool Erasable { get; set; }

        public BitmapObject() { }
        public BitmapObject(Bitmap bitmap, bool erasable = true)
        {
            Bitmap = bitmap;
            Erasable = erasable;
        }
        public BitmapObject(BinaryReader reader)
            : base(reader)
        {
            Position = new Point(reader.ReadInt32(), reader.ReadInt32());
            Erasable = reader.ReadByte() == 1;
            int width = reader.ReadInt32();
            int height = reader.ReadInt32();
            Bitmap = new Bitmap(width, height, PixelFormat.Format32bppArgb);
            BitmapData d = Bitmap.LockBits(new Rectangle(0, 0, Bitmap.Width, Bitmap.Height), System.Drawing.Imaging.ImageLockMode.WriteOnly, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
            byte[] bdata = reader.ReadBytes(d.Stride * d.Height);
            Marshal.Copy(bdata, 0, d.Scan0, d.Stride * d.Height);
            Bitmap.UnlockBits(d);
        }

        public override DrawingObject Clone()
        {
            BitmapObject cloneObject = new BitmapObject();
            CopySettingsTo(cloneObject);
            return cloneObject;
        }

        public override void CopySettingsTo(DrawingObject cloneObject)
        {
            base.CopySettingsTo(cloneObject);
            ((BitmapObject)cloneObject).Position = Position;
            ((BitmapObject)cloneObject).Bitmap = Bitmap;
            ((BitmapObject)cloneObject).Erasable = Erasable;
        }

        public override void Draw(Graphics g, Color colorOverride, bool picking = false)
        {
            if (Bitmap == null)
                return;
            var gs = g.Save();
            g.TranslateTransform(mRotationCenter.X, mRotationCenter.Y);
            g.RotateTransform(mRotationAngle);
            g.TranslateTransform(-mRotationCenter.X, -mRotationCenter.Y);
            if (picking)
                g.FillRectangle(new SolidBrush((Erasable ? colorOverride :Color.Black)), Position.X, Position.Y, Bitmap.Width, Bitmap.Height);
            else if(!picking)
                g.DrawImage(Bitmap, Position.X, Position.Y, Bitmap.Width, Bitmap.Height);
            g.Restore(gs);
        }

        public override void Write(BinaryWriter writer)
        {
            writer.Write((byte)DrawingObject.DrawingObjectType.BitmapObject);
            base.Write(writer);
            writer.Write(Position.X);
            writer.Write(Position.Y);
            writer.Write((byte)(Erasable ? 1 : 0));
            writer.Write(Bitmap.Width);
            writer.Write(Bitmap.Height);
            BitmapData d = Bitmap.LockBits(new Rectangle(0, 0, Bitmap.Width, Bitmap.Height), System.Drawing.Imaging.ImageLockMode.ReadOnly, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
            byte[] bdata = new byte[d.Stride * d.Height];
            Marshal.Copy(d.Scan0, bdata, 0, d.Stride * d.Height);
            Bitmap.UnlockBits(d);
            writer.Write(bdata, 0, bdata.Length);
        }
    }
}
