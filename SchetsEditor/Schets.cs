using SchetsEditor.DrawingObjects;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.IO.Compression;

namespace SchetsEditor
{
    public class Schets
    {
        private HistoryCollection mHistoryObjects = new HistoryCollection();
        //private List<DrawingObject> mDrawingObjectList = new List<DrawingObject>();
        //private List<DrawingObject> BackupDrawingObjectList = new List<DrawingObject>();
        private DrawingObject mWorkingObject;
        private Size mSchetsSize;

        public Boolean HasUnsavedChanges { get; private set; } = false;
       // public Boolean HasRemovedObject { get; private set; } = false;

        public void AcknowledgeChanges()
        {
            HasUnsavedChanges = false;
        }
        
        public Schets()
        {
            mSchetsSize = new Size(1, 1);
        }

        public void VeranderAfmeting(Size sz)
        {
            if (sz.Width > mSchetsSize.Width || sz.Height > mSchetsSize.Height)
            {
                mSchetsSize =
                    new Size(Math.Max(sz.Width, mSchetsSize.Width), Math.Max(sz.Height, mSchetsSize.Height));
                HasUnsavedChanges = true;
            }
        }

        public void Teken(Graphics gr)
        {
            gr.SmoothingMode = SmoothingMode.AntiAlias;
            gr.CompositingQuality = CompositingQuality.HighQuality;
            gr.PixelOffsetMode = PixelOffsetMode.HighQuality;
            gr.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAliasGridFit;
            gr.Clear(Color.White);
            foreach (DrawingObject d in mHistoryObjects.CurrentList)
                d.Draw(gr);
            if (mWorkingObject != null)
                mWorkingObject.Draw(gr);
        }
        public void Clear()
        {
            //mDrawingObjectList.Clear();
            HasUnsavedChanges = true;
        }
        public void Rotate()
        {
           /* foreach (DrawingObject d in mDrawingObjectList)
                d.Rotate(mSchetsSize.Width, mSchetsSize.Height);*/
            HasUnsavedChanges = true;
        }

        public void Undo()
        {
            mHistoryObjects.Undo();
            HasUnsavedChanges = true;

        }

        public void Redo()
        {
            HasUnsavedChanges = true;
        }

        public void BeginAddObject(DrawingObject dObject)
        {
            mWorkingObject = dObject;
        }

        public void EndAddObject()
        {
            //mDrawingObjectList.Add(mWorkingObject);
            mHistoryObjects.AddDrawingObject(mWorkingObject);
            mWorkingObject = null;
            HasUnsavedChanges = true;
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
            for (int i = 0; i < mHistoryObjects.CurrentList.Count; i++)
                mHistoryObjects.CurrentList[i].Draw(g, Color.FromArgb((int)(0xFF000000 | (i + 1))), true);
            int id = b.GetPixel(p.X, p.Y).ToArgb() & 0xFFFFFF;
            if (id > 0)
                return mHistoryObjects.CurrentList[id - 1];
            return null;
        }

        public void RemoveObject(DrawingObject dObject)
        {
            //mDrawingObjectList.Remove(dObject);
            mHistoryObjects.RemoveDrawingObject(mWorkingObject);
            HasUnsavedChanges = true;
        }

        public Bitmap ToBitmap()
        {
            Bitmap b = new Bitmap(mSchetsSize.Width, mSchetsSize.Height);
            Graphics g = Graphics.FromImage(b);
            Teken(g);
            return b;
        }

        public void Read(byte[] data)
        {
            BinaryReader r = new BinaryReader(new GZipStream(new MemoryStream(data), CompressionMode.Decompress));
            if (r.ReadByte() != (byte)'S')
                throw new Exception("Invalid Signature!");
            if (r.ReadByte() != (byte)'P')
                throw new Exception("Invalid Signature!");
            if (r.ReadByte() != (byte)'P')
                throw new Exception("Invalid Signature!");
            if (r.ReadByte() != (byte)'P')
                throw new Exception("Invalid Signature!");
            mSchetsSize = new Size(r.ReadInt32(), r.ReadInt32());
            int count = r.ReadInt32();
            mHistoryObjects.ClearHistory();
          /*  for (int i = 0; i < count; i++)
            {
                byte type = r.ReadByte();
                switch ((DrawingObject.DrawingObjectType)type)
                {
                    case DrawingObject.DrawingObjectType.EllipseObject:
                        mDrawingObjectList.Add(new EllipseObject(r));
                        break;
                    case DrawingObject.DrawingObjectType.TextObject:
                        mDrawingObjectList.Add(new TextObject(r));
                        break;
                    case DrawingObject.DrawingObjectType.RectangleObject:
                        mDrawingObjectList.Add(new RectangleObject(r));
                        break;
                    case DrawingObject.DrawingObjectType.LineObject:
                        mDrawingObjectList.Add(new LineObject(r));
                        break;
                    case DrawingObject.DrawingObjectType.BitmapObject:
                        mDrawingObjectList.Add(new BitmapObject(r));
                        break;
                }
            }*/
            r.Close();
            HasUnsavedChanges = false;
        }

        public byte[] Write()
        {
            MemoryStream m = new MemoryStream();
            GZipStream gs = new GZipStream(m, CompressionLevel.Optimal);
            BinaryWriter writer = new BinaryWriter(gs);
            writer.Write((byte)'S');
            writer.Write((byte)'P');
            writer.Write((byte)'P');
            writer.Write((byte)'P');
            writer.Write(mSchetsSize.Width);
            writer.Write(mSchetsSize.Height);
          /*  writer.Write(mDrawingObjectList.Count);
            foreach (DrawingObject o in mDrawingObjectList)
                o.Write(writer);*/
            writer.Flush();
            gs.Close();
            byte[] result = m.ToArray();
            writer.Close();
            return result;
        }
    }
}
