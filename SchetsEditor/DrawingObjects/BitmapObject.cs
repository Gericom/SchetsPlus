using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SchetsEditor.DrawingObjects
{
    public class BitmapObject : DrawingObject
    {
        public Point Position { get; set; }
        public Bitmap Bitmap { get; set; }
        public bool Erasable { get; set; }

        public BitmapObject(Bitmap bitmap, bool erasable = true)
        {
            Bitmap = bitmap;
            Erasable = erasable;
        }

        public override void Draw(Graphics g, Color colorOverride, bool picking = false)
        {
            if (Bitmap == null)
                return;
            if (picking)
                g.FillRectangle(new SolidBrush((Erasable ? colorOverride :Color.Black)), Position.X, Position.Y, Bitmap.Width, Bitmap.Height);
            else if(!picking)
                g.DrawImage(Bitmap, Position.X, Position.Y, Bitmap.Width, Bitmap.Height);
        }
    }
}
