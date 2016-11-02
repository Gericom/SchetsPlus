using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SchetsEditor.DrawingObjects
{
    public abstract class ShapeObject : DrawingObject
    {
        public Point Position { get; set; }
        public Size Size { get; set; }
        public bool Filled { get; set; }
    }
}
