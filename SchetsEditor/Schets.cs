using SchetsEditor.DrawingObjects;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace SchetsEditor
{
    public class Schets
    {
        private List<DrawingObject> mDrawingObjectList = new List<DrawingObject>();
        private DrawingObject mWorkingObject;
        private Size mSchetsSize;
        
        public Schets()
        {
            mSchetsSize = new Size(1, 1);
        }

        public void VeranderAfmeting(Size sz)
        {
            if (sz.Width > mSchetsSize.Width || sz.Height > mSchetsSize.Height)
                mSchetsSize =
                    new Size(Math.Max(sz.Width, mSchetsSize.Width), Math.Max(sz.Height, mSchetsSize.Height));
        }

        public void Teken(Graphics gr)
        {
            gr.SmoothingMode = SmoothingMode.AntiAlias;
            gr.CompositingQuality = CompositingQuality.HighQuality;
            gr.PixelOffsetMode = PixelOffsetMode.HighQuality;
            gr.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAliasGridFit;
            gr.Clear(Color.White);
            foreach (DrawingObject d in mDrawingObjectList)
                d.Draw(gr);
            if (mWorkingObject != null)
                mWorkingObject.Draw(gr);
        }
        public void Clear()
        {
            mDrawingObjectList.Clear();
        }
        public void Rotate()
        {
            //bitmap.RotateFlip(RotateFlipType.Rotate90FlipNone);
        }

        public void BeginAddObject(DrawingObject dObject)
        {
            mWorkingObject = dObject;
        }

        public void EndAddObject()
        {
            mDrawingObjectList.Add(mWorkingObject);
            mWorkingObject = null;
        }

        public DrawingObject FindObjectByPoint(Point p)
        {
            if (p.X < 1 || p.Y < 1)
                return null;
            Bitmap b = new Bitmap(p.X + 1, p.Y + 1);
            Graphics g = Graphics.FromImage(b);
            g.SmoothingMode = SmoothingMode.None;
            g.CompositingQuality = CompositingQuality.HighQuality;
            g.PixelOffsetMode = PixelOffsetMode.HighQuality;
            g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.SingleBitPerPixelGridFit;
            g.Clear(Color.Transparent);
            for (int i = 0; i < mDrawingObjectList.Count; i++)
                mDrawingObjectList[i].Draw(g, Color.FromArgb((int)(0xFF000000 | (i + 1))), true);
            int id = b.GetPixel(p.X, p.Y).ToArgb() & 0xFFFFFF;
            if (id > 0)
                return mDrawingObjectList[id - 1];
            return null;
        }

        public void RemoveObject(DrawingObject dObject)
        {
            mDrawingObjectList.Remove(dObject);
        }

        public Bitmap ToBitmap()
        {
            Bitmap b = new Bitmap(mSchetsSize.Width, mSchetsSize.Height);
            Graphics g = Graphics.FromImage(b);
            Teken(g);
            return b;
        }
    }
}
