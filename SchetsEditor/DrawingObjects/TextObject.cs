using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SchetsEditor.DrawingObjects
{
    public class TextObject : VectorObject
    {
        public Point Position { get; set; }
        public String Text { get; set; }

        public TextObject() { }
        public TextObject(BinaryReader reader)
            : base(reader)
        {
            Position = new Point(reader.ReadInt32(), reader.ReadInt32());
            byte[] textbytes = reader.ReadBytes(reader.ReadInt32());
            Text = Encoding.Unicode.GetString(textbytes);
        }

        public override DrawingObject Clone()
        {
            TextObject cloneObject = new TextObject();
            CopySettingsTo(cloneObject);
            return cloneObject;
        }

        public override void CopySettingsTo(DrawingObject cloneObject)
        {
            base.CopySettingsTo(cloneObject);
            ((TextObject)cloneObject).Position = Position;
            ((TextObject)cloneObject).Text = Text;
        }

        public override void Draw(Graphics g, Color colorOverride, bool picking = false)
        {
            var gs = g.Save();
            g.TranslateTransform(mRotationCenter.X, mRotationCenter.Y);
            g.RotateTransform(mRotationAngle);
            g.TranslateTransform(-mRotationCenter.X, -mRotationCenter.Y);
            Font font = new Font("Calibri", 40);
            g.DrawString(Text, font, new SolidBrush(colorOverride), Position, StringFormat.GenericTypographic);
            g.Restore(gs);
        }

        public override void Write(BinaryWriter writer)
        {
            writer.Write((byte)DrawingObject.DrawingObjectType.TextObject);
            base.Write(writer);
            writer.Write(Position.X);
            writer.Write(Position.Y);
            byte[] textbytes = Encoding.Unicode.GetBytes(Text);
            writer.Write(textbytes.Length);
            writer.Write(textbytes, 0, textbytes.Length);
        }

        public override void Translate(Point delta)
        {
            FixRot(ref delta);
            Position = new Point(Position.X + delta.X, Position.Y + delta.Y);
        }
    }
}
