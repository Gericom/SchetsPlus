using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SchetsEditor.DrawingObjects
{
    public abstract class VectorObject : DrawingObject
    {
        public Color Color { get; set; }

        public override void Draw(Graphics g)
        {
            Draw(g, Color);
        }
    }
}
