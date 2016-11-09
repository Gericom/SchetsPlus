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
        public bool mAtomicActionRunning = false;
        public List<DrawingObject> mAtomicNewList;

        public HistoryCollection()
        {
            mCurrentHistoryLevel = 0;
            mHistoryObjects.Add(new List<DrawingObject>()); 
        }

        public void AddDrawingObject(DrawingObject dObject)
        {
            bool wasRunningAtomic = mAtomicActionRunning;
            if (!wasRunningAtomic)
                BeginAtomicAction();
            mAtomicNewList.Add(dObject);
            if (!wasRunningAtomic)
                EndAtomicAction();
        }

        public void RemoveDrawingObject(DrawingObject dObject)
        {
            bool wasRunningAtomic = mAtomicActionRunning;
            if (!wasRunningAtomic)
                BeginAtomicAction();
            mAtomicNewList.Remove(dObject);
            if (!wasRunningAtomic)
                EndAtomicAction();
        }

        public DrawingObject Mutate(DrawingObject dObject)
        {
            bool wasRunningAtomic = mAtomicActionRunning;
            if (!wasRunningAtomic)
                BeginAtomicAction();
            DrawingObject newDObject = dObject.Clone();
            mAtomicNewList[mAtomicNewList.IndexOf(dObject)] = newDObject;
            if (!wasRunningAtomic)
                EndAtomicAction();
            return newDObject;
        }

        private void CommitAction(List<DrawingObject> dObjectList)
        {
            if(mCurrentHistoryLevel != mHistoryObjects.Count - 1)
                mHistoryObjects.RemoveRange(mCurrentHistoryLevel + 1, mHistoryObjects.Count - 1 - mCurrentHistoryLevel);
            mHistoryObjects.Add(dObjectList);
            mCurrentHistoryLevel++;
        }

        public bool Undo()
        {
            if (mCurrentHistoryLevel > 0)
                mCurrentHistoryLevel--;
            else
                return false;
            return true;
        }

        public bool Redo()
        {
            if(mCurrentHistoryLevel < mHistoryObjects.Count - 1)
                mCurrentHistoryLevel++;
            else
                return false;
            return true;
        }

        public void ClearHistory()
        {
            mCurrentHistoryLevel = 0;
            mHistoryObjects.Clear();
            mHistoryObjects.Add(new List<DrawingObject>());
            mAtomicActionRunning = false;
            mAtomicNewList = null;
        }

        public void BeginAtomicAction()
        {
            mAtomicActionRunning = true;
            mAtomicNewList = new List<DrawingObject>();
            mAtomicNewList.AddRange(mHistoryObjects[mCurrentHistoryLevel]);
        }

        public void EndAtomicAction()
        {
            CommitAction(mAtomicNewList);
            mAtomicActionRunning = false;
            mAtomicNewList = null;
        }
    }
}
