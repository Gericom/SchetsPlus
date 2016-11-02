using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SchetsEditor.DrawingObjects
{
    public abstract class DrawingObject
    {
        public Color Color { get; set; }

        public void Draw(Graphics g)
        {
            Draw(g, Color);
        }

        public abstract void Draw(Graphics g, Color colorOverride);
    }
}
