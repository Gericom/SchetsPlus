using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SchetsEditor.DrawingObjects
{
    public class LineObject : VectorObject
    {
        public Point[] Points;

        public override void Draw(Graphics g, Color colorOverride, bool picking = false)
        {
            if (Points == null || Points.Length < 2)
                return;
            var pen = new Pen(new SolidBrush(colorOverride), (picking ? 5 : 3));
            g.DrawCurve(pen, Points);
        }
    }
}
