using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SchetsEditor.DrawingObjects
{
    public class RectangleObject : ShapeObject
    {
        public override void Draw(Graphics g, Color colorOverride, bool picking = false)
        {
            if (Filled)
                g.FillRectangle(new SolidBrush(colorOverride), new Rectangle(Position, Size));
            else
                g.DrawRectangle(new Pen(new SolidBrush(colorOverride), (picking ? 4 : 2)), new Rectangle(Position, Size));
        }
    }
}
