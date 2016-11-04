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
        public virtual void Draw(Graphics g)
        {
            Draw(g, Color.Black);
        }

        public abstract void Draw(Graphics g, Color colorOverride, bool picking = false);
    }
}
