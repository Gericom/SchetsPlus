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
       

        public void Draw(Graphics g)
        {
            Draw(g, false, Color.Black);
        }

        public abstract void Draw(Graphics g, bool picking, Color pickingColor);
    }
}
