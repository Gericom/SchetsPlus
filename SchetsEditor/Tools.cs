using SchetsEditor.DrawingObjects;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace SchetsEditor
{   
    public interface ISchetsTool
    {
        void MouseDown(SchetsControl s, Point p);
        void MouseDrag(SchetsControl s, Point p);
        void MouseUp(SchetsControl s, Point p);
        void Letter(SchetsControl s, char c);
    }

    public abstract class StartpuntTool : ISchetsTool
    {
        protected Point startpunt;
        protected DrawingObject mDrawingObject;
        protected bool mDragging = false;

        public virtual void MouseDown(SchetsControl s, Point p)
        {
            startpunt = p;
            mDragging = true;
        }
        public virtual void MouseUp(SchetsControl s, Point p)
        {
            mDragging = false;
            if(mDrawingObject != null)
                s.Schets.EndAddObject();
        }
        public abstract void MouseDrag(SchetsControl s, Point p);
        public virtual void Letter(SchetsControl s, char c) { }
    }

    public class TekstTool : StartpuntTool
    {
        public override string ToString() { return "Text"; }

        public override void MouseDrag(SchetsControl s, Point p) { }

        public override void MouseDown(SchetsControl s, Point p)
        {
            base.MouseDown(s, p);
            mDrawingObject = new TextObject() { Position = p, Text = "", Color = s.PenColor };
            s.Schets.BeginAddObject(mDrawingObject);
        }

        public override void Letter(SchetsControl s, char c)
        {
            if (c >= 32)
            {
                ((TextObject)mDrawingObject).Text += c;
                s.Invalidate();
            }
            else if (c == '\b' && ((TextObject)mDrawingObject).Text.Length > 0)
            {
                ((TextObject)mDrawingObject).Text = ((TextObject)mDrawingObject).Text.Substring(0, ((TextObject)mDrawingObject).Text.Length - 1);
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
        public override void MouseDown(SchetsControl s, Point p)
        {
            base.MouseDown(s, p);
        }
        public override void MouseDrag(SchetsControl s, Point p)
        {
            this.Bezig(this.startpunt, p);
            s.Invalidate();
        }
        public override void MouseUp(SchetsControl s, Point p)
        {
            base.MouseUp(s, p);
            this.Compleet(this.startpunt, p);
            s.Invalidate();
            mDrawingObject = null;
        }

        public abstract void Bezig(Point p1, Point p2);

        public virtual void Compleet(Point p1, Point p2)
        {
            this.Bezig(p1, p2);
        }
    }

    public class RechthoekTool : TweepuntTool
    {
        public override string ToString() { return "Frame"; }

        public override void MouseDown(SchetsControl s, Point p)
        {
            base.MouseDown(s, p);
            mDrawingObject = new RectangleObject() { Color = s.PenColor, Filled = false, Position = p, Size = new Size(0, 0), LineWidth = s.PenSize };
            s.Schets.BeginAddObject(mDrawingObject);
        }

        public override void Bezig(Point p1, Point p2)
        {
            Rectangle r = Punten2Rechthoek(p1, p2);
            ((RectangleObject)mDrawingObject).Position = r.Location;
            ((RectangleObject)mDrawingObject).Size = r.Size;
        }
    }

    public class VolRechthoekTool : RechthoekTool
    {
        public override string ToString() { return "Plane"; }

        public override void MouseDown(SchetsControl s, Point p)
        {
            base.MouseDown(s, p);
            ((RectangleObject)mDrawingObject).LineWidth = 3;
        }

        public override void Compleet(Point p1, Point p2)
        {
            Rectangle r = Punten2Rechthoek(p1, p2);
            ((RectangleObject)mDrawingObject).Position = r.Location;
            ((RectangleObject)mDrawingObject).Size = r.Size;
            ((RectangleObject)mDrawingObject).Filled = true;
        }
    }

    public class EllipseTool : TweepuntTool
    {
        public override string ToString() { return "Ellipse"; }

        public override void MouseDown(SchetsControl s, Point p)
        {
            base.MouseDown(s, p);
            mDrawingObject = new EllipseObject() { Color = s.PenColor, Filled = false, Position = p, Size = new Size(0,0), LineWidth = s.PenSize };
            s.Schets.BeginAddObject(mDrawingObject);
        }

        public override void Bezig(Point p1, Point p2)
        {
            Rectangle r = Punten2Rechthoek(p1, p2);
            ((EllipseObject)mDrawingObject).Position = r.Location;
            ((EllipseObject)mDrawingObject).Size = r.Size;
        }
    }

    public class FillEllipse : EllipseTool
    {
        public override string ToString() { return "FilledEllipse"; }

        public override void MouseDown(SchetsControl s, Point p)
        {
            base.MouseDown(s, p);
            ((EllipseObject)mDrawingObject).LineWidth = 3;
        }

        public override void Compleet(Point p1, Point p2)
        {
            Rectangle r = Punten2Rechthoek(p1, p2);
            ((EllipseObject)mDrawingObject).Position = r.Location;
            ((EllipseObject)mDrawingObject).Size = r.Size;
            ((EllipseObject)mDrawingObject).Filled = true;
        }
    }


    public class LijnTool : TweepuntTool
    {
        public override string ToString() { return "Line"; }

        public override void MouseDown(SchetsControl s, Point p)
        {
            base.MouseDown(s, p);
            mDrawingObject = new LineObject() { Color = s.PenColor, LineWidth = s.PenSize };
            s.Schets.BeginAddObject(mDrawingObject);
        }

        public override void Bezig(Point p1, Point p2)
        {
            ((LineObject)mDrawingObject).Points = new Point[] { p1, p2 };
        }
    }

    public class PenTool : LijnTool
    {
        public override string ToString() { return "Pen"; }

        public override void MouseDown(SchetsControl s, Point p)
        {
            base.MouseDown(s, p);
            mPoints = new List<Point>();
            mPoints.Add(p);
        }

        private List<Point> mPoints;

        public override void Bezig(Point p1, Point p2)
        {
            mPoints.Add(p2);
            ((LineObject)mDrawingObject).Points = mPoints.ToArray();
        }

    }
    public class GumTool : StartpuntTool
    {
        public override string ToString() { return "Eraser"; }

        public override void MouseDown(SchetsControl s, Point p)
        {
            base.MouseDown(s, p);
            
        }
        public override void MouseDrag(SchetsControl s, Point p)
        {
            
        }
        public override void MouseUp(SchetsControl s, Point p)
        {
            base.MouseUp(s, p);
            var obj = s.Schets.FindObjectByPoint(p);
            if (obj != null)
                s.Schets.RemoveObject(obj);
            s.Invalidate();
        }
    }
    public class MoveUpTool : StartpuntTool
    {
        public override string ToString() { return "MoveUp"; }

        public override void MouseDown(SchetsControl s, Point p)
        {
            base.MouseDown(s, p);

        }
        public override void MouseDrag(SchetsControl s, Point p)
        {

        }
        public override void MouseUp(SchetsControl s, Point p)
        {
            base.MouseUp(s, p);
            var obj = s.Schets.FindObjectByPoint(p);
            if (obj != null)
                s.Schets.MoveObjectUp(obj);
            s.Invalidate();
        }
    }
    public class MoveDownTool : StartpuntTool
    {
        public override string ToString() { return "MoveDown"; }

        public override void MouseDown(SchetsControl s, Point p)
        {
            //base.MouseDown(s, p);

        }
        public override void MouseDrag(SchetsControl s, Point p)
        {

        }
        public override void MouseUp(SchetsControl s, Point p)
        {
            //base.MouseUp(s, p);
            var obj = s.Schets.FindObjectByPoint(p);
            if (obj != null)
                s.Schets.MoveObjectDown(obj);
            s.Invalidate();
        }
    }

    public class DragTool : StartpuntTool
    {
        public override string ToString() { return "Drag"; }

        public override void MouseDown(SchetsControl s, Point p)
        {
            base.MouseDown(s, p);
            var obj = s.Schets.FindObjectByPoint(p);
            if(obj != null)
                mDrawingObject = s.Schets.Mutate(obj);
        }
        public override void MouseDrag(SchetsControl s, Point p)
        {
            if (mDrawingObject != null)
            {
                mDrawingObject.Translate(new Point(p.X - startpunt.X, p.Y - startpunt.Y));
                startpunt = p;
                s.Invalidate();
            }
        }
        public override void MouseUp(SchetsControl s, Point p)
        {
            /*base.MouseUp(s, p);
            var obj = s.Schets.FindObjectByPoint(p);
            if (obj != null)
                s.Schets.MoveObjectDown(obj);*/
            MouseDrag(s, p);
            mDrawingObject = null;
            s.Invalidate();
        }
    }
}
