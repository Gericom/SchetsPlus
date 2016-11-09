using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SchetsEditor.DrawingObjects
{
    public abstract class ShapeObject : VectorObject
    {
        public Point Position { get; set; }
        public Size Size { get; set; }
        public bool Filled { get; set; }
        public int LineWidth { get; set; }

        public ShapeObject() { }
        public ShapeObject(BinaryReader reader)
            : base(reader)
        {
            Position = new Point(reader.ReadInt32(), reader.ReadInt32());
            Size = new Size(reader.ReadInt32(), reader.ReadInt32());
            Filled = reader.ReadByte() == 1;
            LineWidth = reader.ReadInt32();
        }

        public override void CopySettingsTo(DrawingObject cloneObject)
        {
            base.CopySettingsTo(cloneObject);
            ((ShapeObject)cloneObject).Position = Position;
            ((ShapeObject)cloneObject).Size = Size;
            ((ShapeObject)cloneObject).Filled = Filled;
            ((ShapeObject)cloneObject).LineWidth = LineWidth;
        }

        public override void Write(BinaryWriter writer)
        {
            base.Write(writer);
            writer.Write(Position.X);
            writer.Write(Position.Y);
            writer.Write(Size.Width);
            writer.Write(Size.Height);
            writer.Write((byte)(Filled ? 1 : 0));
            writer.Write(LineWidth);
        }
    }
}
