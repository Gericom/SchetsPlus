using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SchetsEditor.DrawingObjects
{
    public abstract class VectorObject : DrawingObject
    {
        public Color Color { get; set; }

        public VectorObject() { }
        public VectorObject(BinaryReader reader)
            : base(reader)
        {
            Color = Color.FromArgb(reader.ReadInt32());
        }

        public override void CopySettingsTo(DrawingObject cloneObject)
        {
            base.CopySettingsTo(cloneObject);
            ((VectorObject)cloneObject).Color = Color;
        }

        public override void Draw(Graphics g)
        {
            Draw(g, Color);
        }

        public override void Write(BinaryWriter writer)
        {
            base.Write(writer);
            writer.Write(Color.ToArgb());
        }
    }
}
