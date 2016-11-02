﻿using SchetsEditor.DrawingObjects;
using System;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace SchetsEditor
{
    public interface ISchetsTool
    {
        void MuisVast(SchetsControl s, Point p);
        void MuisDrag(SchetsControl s, Point p);
        void MuisLos(SchetsControl s, Point p);
        void Letter(SchetsControl s, char c);
    }

    public abstract class StartpuntTool : ISchetsTool
    {
        protected Point startpunt;
        protected Brush kwast;
        protected DrawingObject mDrawingObject;
        protected bool mDragging = false;

        public virtual void MuisVast(SchetsControl s, Point p)
        {
            startpunt = p;
            mDragging = true;
        }
        public virtual void MuisLos(SchetsControl s, Point p)
        {
            mDragging = false;
            kwast = new SolidBrush(s.PenKleur);
        }
        public abstract void MuisDrag(SchetsControl s, Point p);
        public abstract void Letter(SchetsControl s, char c);
    }

    public class TekstTool : StartpuntTool
    {
        public override string ToString() { return "Text"; }

        public override void MuisDrag(SchetsControl s, Point p) { }

        public override void Letter(SchetsControl s, char c)
        {
            if (c >= 32)
            {
                Graphics gr = s.MaakBitmapGraphics();
                Font font = new Font("Tahoma", 40);
                string tekst = c.ToString();
                SizeF sz =
                gr.MeasureString(tekst, font, this.startpunt, StringFormat.GenericTypographic);
                gr.DrawString(tekst, font, kwast,
                                              this.startpunt, StringFormat.GenericTypographic);
                // gr.DrawRectangle(Pens.Black, startpunt.X, startpunt.Y, sz.Width, sz.Height);
                startpunt.X += (int)sz.Width;
                s.Invalidate();
            }
        }
    }

    public abstract class TweepuntTool : StartpuntTool
    {
        public static Rectangle Punten2Rechthoek(Point p1, Point p2)
        {
            return new Rectangle(new Point(Math.Min(p1.X, p2.X), Math.Min(p1.Y, p2.Y))
                                , new Size(Math.Abs(p1.X - p2.X), Math.Abs(p1.Y - p2.Y))
                                );
        }
        public static Pen MaakPen(Brush b, int dikte)
        {
            Pen pen = new Pen(b, dikte);
            pen.StartCap = LineCap.Round;
            pen.EndCap = LineCap.Round;
            return pen;
        }
        public override void MuisVast(SchetsControl s, Point p)
        {
            base.MuisVast(s, p);
            kwast = Brushes.Gray;
        }
        public override void MuisDrag(SchetsControl s, Point p)
        {
            s.Refresh();
            this.Bezig(s.CreateGraphics(), this.startpunt, p);
        }
        public override void MuisLos(SchetsControl s, Point p)
        {
            base.MuisLos(s, p);
            this.Compleet(s.MaakBitmapGraphics(), this.startpunt, p);
            s.Invalidate();
        }
        public override void Letter(SchetsControl s, char c)
        {
        }
        public abstract void Bezig(Graphics g, Point p1, Point p2);

        public virtual void Compleet(Graphics g, Point p1, Point p2)
        {
            this.Bezig(g, p1, p2);
        }
    }

    public class RechthoekTool : TweepuntTool
    {
        public override string ToString() { return "Frame"; }

        public override void MuisVast(SchetsControl s, Point p)
        {
            base.MuisVast(s, p);
            mDrawingObject = new RectangleObject() { Color = s.PenKleur, Filled = false, Position = p, Size = new Size(0, 0) };
        }

        public override void Bezig(Graphics g, Point p1, Point p2)
        {
            Rectangle r = Punten2Rechthoek(p1, p2);
            ((RectangleObject)mDrawingObject).Position = r.Location;
            ((RectangleObject)mDrawingObject).Size = r.Size;
            if (mDragging)
                mDrawingObject.Draw(g, Color.Gray);
            else
                mDrawingObject.Draw(g);
            //g.DrawRectangle(MaakPen(kwast, 3), TweepuntTool.Punten2Rechthoek(p1, p2));
        }
    }

    public class VolRechthoekTool : RechthoekTool
    {
        public override string ToString() { return "Plane"; }

        public override void Compleet(Graphics g, Point p1, Point p2)
        {
            Rectangle r = Punten2Rechthoek(p1, p2);
            ((RectangleObject)mDrawingObject).Position = r.Location;
            ((RectangleObject)mDrawingObject).Size = r.Size;
            ((RectangleObject)mDrawingObject).Filled = true;
            mDrawingObject.Draw(g);
            //g.FillRectangle(kwast, TweepuntTool.Punten2Rechthoek(p1, p2));
        }
    }

    public class CircleTool : TweepuntTool
    {
        public override string ToString() { return "Ellipse"; }

        public override void Bezig(Graphics g, Point p1, Point p2)
        {
            g.DrawEllipse(MaakPen(kwast, 3), TweepuntTool.Punten2Rechthoek(p1, p2));
        }
    }

    public class FillCircle : CircleTool
    {
        public override string ToString() { return "FilledEllipse"; }

        public override void Compleet(Graphics g, Point p1, Point p2)
        {
            g.FillEllipse(kwast, TweepuntTool.Punten2Rechthoek(p1, p2));
        }
    }


    public class LijnTool : TweepuntTool
    {
        public override string ToString() { return "Line"; }

        public override void Bezig(Graphics g, Point p1, Point p2)
        {
            g.DrawLine(MaakPen(this.kwast, 3), p1, p2);
        }
    }

    public class PenTool : LijnTool
    {
        public override string ToString() { return "Pen"; }

        public override void MuisDrag(SchetsControl s, Point p)
        {
            this.MuisLos(s, p);
            this.MuisVast(s, p);
        }
    }

    public class GumTool : PenTool
    {
        public override string ToString() { return "Eraser"; }

        public override void Bezig(Graphics g, Point p1, Point p2)
        {
            g.DrawLine(MaakPen(Brushes.White, 7), p1, p2);
        }
    }
}
