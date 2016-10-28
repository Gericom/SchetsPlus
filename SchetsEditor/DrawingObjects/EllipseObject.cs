﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SchetsEditor.DrawingObjects
{
    public class EllipseObject : DrawingObject
    {
        public Point Position { get; set; }
        public Size Size { get; set; }
        public Color Color { get; set; }
        public bool Filled { get; set; }

        public override void Draw(Graphics g, bool picking, Color pickingColor)
        {
            Color color = (picking ? pickingColor : Color);
            if (Filled)
                g.FillEllipse(new SolidBrush(color), new Rectangle(Position, Size));
            else
                g.DrawEllipse(new Pen(new SolidBrush(color), 3), new Rectangle(Position, Size));
        }
    }
}
