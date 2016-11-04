using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SchetsEditor.DrawingObjects
{
    public class TextObject : VectorObject
    {
        public Point Position { get; set; }
        public String Text { get; set; }

        public override void Draw(Graphics g, Color colorOverride, bool picking = false)
        {
            Font font = new Font("Calibri", 40);
            g.DrawString(Text, font, new SolidBrush(colorOverride),
                                          Position, StringFormat.GenericTypographic);
        }
    }
}
