using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SchetsEditor.DrawingObjects
{
    public class LineObject : DrawingObject
    {
        public Point[] Points;

        public override void Draw(Graphics g, Color colorOverride)
        {
            if (Points.Length < 2)
                return;
            var pen = new Pen(new SolidBrush(colorOverride), 3);
            for (int i = 0; i < Points.Length - 1; i++)
            {
                g.DrawLine(pen, Points[i], Points[i + 1]);
            }
        }
    }
}
