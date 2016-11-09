using SchetsEditor.DrawingObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SchetsEditor
{
    class HistoryCollection
    {
        List<List<DrawingObject>> mHistoryObjects = new List<List<DrawingObject>>();
        int mCurrentHistoryLevel;
        public List<DrawingObject> CurrentList { get { return mHistoryObjects[mCurrentHistoryLevel]; } }

        public HistoryCollection()
        {
            mCurrentHistoryLevel = 0;
            mHistoryObjects.Add(new List<DrawingObject>());
            
        }

        public void AddDrawingObject(DrawingObject dObject)
        {
            List<DrawingObject> newList = new List<DrawingObject>();
            newList.AddRange(mHistoryObjects[mCurrentHistoryLevel]);
            newList.Add(dObject);
            CommitAction(newList);
        }

        public void RemoveDrawingObject(DrawingObject dObject)
        {
            List<DrawingObject> newList = new List<DrawingObject>();
            newList.AddRange(mHistoryObjects[mCurrentHistoryLevel]);
            newList.Remove(dObject);
            CommitAction(newList);
        }

        private void CommitAction(List<DrawingObject> dObjectList)
        {
            if(mCurrentHistoryLevel != mHistoryObjects.Count - 1)
                mHistoryObjects.RemoveRange(mCurrentHistoryLevel + 1, mHistoryObjects.Count - 1 - mCurrentHistoryLevel);
            mHistoryObjects.Add(dObjectList);
            mCurrentHistoryLevel++;
        }

        public void Undo()
        {
            if(mCurrentHistoryLevel > 0)
                 mCurrentHistoryLevel--;
        }

        public void Redo()
        {
            if(mCurrentHistoryLevel < mHistoryObjects.Count - 1)
                mCurrentHistoryLevel++;
        }

        public void ClearHistory()
        {
            mCurrentHistoryLevel = 0;
            mHistoryObjects.Clear();
            mHistoryObjects.Add(new List<DrawingObject>());
            
        }
    }
}
