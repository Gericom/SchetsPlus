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
        private DrawingObject mWorkingObject;
        private Size mSchetsSize;

        public Boolean HasUnsavedChanges { get; private set; } = false;

        //Acknowledge that changes have been processed (saved)
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

        //Draw the drawing objects in the list and the currently-being-drawed object
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
        //Clear the canvas in as a one-step atomic action
        public void Clear()
        {
            mHistoryObjects.BeginAtomicAction();
            foreach (DrawingObject d in mHistoryObjects.CurrentList)
                mHistoryObjects.RemoveDrawingObject(d);
            mHistoryObjects.EndAtomicAction();
            HasUnsavedChanges = true;
        }
        //Rotate the canvas as a one-step atomic action
        public void Rotate()
        {
            mHistoryObjects.BeginAtomicAction();
            foreach (DrawingObject d in mHistoryObjects.CurrentList)
                mHistoryObjects.Mutate(d).Rotate(mSchetsSize.Width, mSchetsSize.Height);
            mHistoryObjects.EndAtomicAction();
            HasUnsavedChanges = true;
        }

        public void Undo()
        {
            if(mHistoryObjects.Undo())
                HasUnsavedChanges = true;
        }

        public void Redo()
        {
            if(mHistoryObjects.Redo())
                HasUnsavedChanges = true;
        }

        public void MoveObjectUp(DrawingObject dObject)
        {
            mHistoryObjects.MoveObjectUp(dObject);
            HasUnsavedChanges = true;
        }

        public void MoveObjectDown(DrawingObject dObject)
        {
            mHistoryObjects.MoveObjectDown(dObject);
            HasUnsavedChanges = true;
        }

        //Mutate an object, to make it possible to safely change its properties
        //This is also recorded as an undoable action
        public DrawingObject Mutate(DrawingObject dObject)
        {
            DrawingObject mutated = mHistoryObjects.Mutate(dObject);
            HasUnsavedChanges = true;
            return mutated;
        }

        //Start adding an object, makes it possible to show what one's drawing
        public void BeginAddObject(DrawingObject dObject)
        {
            mWorkingObject = dObject;
        }

        //Finish adding the object; add it to the list
        public void EndAddObject()
        {
            mHistoryObjects.AddDrawingObject(mWorkingObject);
            mWorkingObject = null;
            HasUnsavedChanges = true;
        }

        //Find the object at a certain pixel using a method called 'picking'
        //How this works is described in Veranderingen.txt
        public DrawingObject FindObjectByPoint(Point p)
        {
            if (p.X < 0 || p.Y < 0)
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
            mHistoryObjects.RemoveDrawingObject(dObject);
            HasUnsavedChanges = true;
        }

        public Bitmap ToBitmap()
        {
            Bitmap b = new Bitmap(mSchetsSize.Width, mSchetsSize.Height);
            Graphics g = Graphics.FromImage(b);
            Teken(g);
            return b;
        }

        //Read a schets from a project file
        public void Read(byte[] data)
        {
            BinaryReader r = new BinaryReader(new GZipStream(new MemoryStream(data), CompressionMode.Decompress));
            //File Signature
            if (r.ReadByte() != (byte)'S')
                throw new Exception("Invalid Signature!");
            if (r.ReadByte() != (byte)'P')
                throw new Exception("Invalid Signature!");
            if (r.ReadByte() != (byte)'P')
                throw new Exception("Invalid Signature!");
            if (r.ReadByte() != (byte)'P')
                throw new Exception("Invalid Signature!");
            //Schets Size
            mSchetsSize = new Size(r.ReadInt32(), r.ReadInt32());
            //Number of objects
            int count = r.ReadInt32();
            mHistoryObjects.ClearHistory();
            //Read each object
            for (int i = 0; i < count; i++)
            {
                byte type = r.ReadByte();
                switch ((DrawingObject.DrawingObjectType)type)
                {
                    case DrawingObject.DrawingObjectType.EllipseObject:
                        mHistoryObjects.CurrentList.Add(new EllipseObject(r));
                        break;
                    case DrawingObject.DrawingObjectType.TextObject:
                        mHistoryObjects.CurrentList.Add(new TextObject(r));
                        break;
                    case DrawingObject.DrawingObjectType.RectangleObject:
                        mHistoryObjects.CurrentList.Add(new RectangleObject(r));
                        break;
                    case DrawingObject.DrawingObjectType.LineObject:
                        mHistoryObjects.CurrentList.Add(new LineObject(r));
                        break;
                    case DrawingObject.DrawingObjectType.BitmapObject:
                        mHistoryObjects.CurrentList.Add(new BitmapObject(r));
                        break;
                }
            }
            r.Close();
            HasUnsavedChanges = false;
        }

        public byte[] Write()
        {
            MemoryStream m = new MemoryStream();
            //Use GZip compression, to bring the file size down quite a lot
            GZipStream gs = new GZipStream(m, CompressionLevel.Optimal);
            BinaryWriter writer = new BinaryWriter(gs);
            //Signature
            writer.Write((byte)'S');
            writer.Write((byte)'P');
            writer.Write((byte)'P');
            writer.Write((byte)'P');
            //Schets Size
            writer.Write(mSchetsSize.Width);
            writer.Write(mSchetsSize.Height);
            //Number of objects
            writer.Write(mHistoryObjects.CurrentList.Count);
            //Write each object
            foreach (DrawingObject o in mHistoryObjects.CurrentList)
                o.Write(writer);
            writer.Flush();
            gs.Close();
            byte[] result = m.ToArray();
            writer.Close();
            return result;
        }
    }
}
