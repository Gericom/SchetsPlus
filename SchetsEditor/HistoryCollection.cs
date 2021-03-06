﻿using SchetsEditor.DrawingObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SchetsEditor
{
    public class HistoryCollection
    {
        //The working of HistoryCollection is explained in Veranderingen.txt
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

        //Makes it possible to safely change the properties of an object
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

        //Registers the action in the history
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

        //Begin a one-step action
        public void BeginAtomicAction()
        {
            mAtomicActionRunning = true;
            mAtomicNewList = new List<DrawingObject>();
            mAtomicNewList.AddRange(mHistoryObjects[mCurrentHistoryLevel]);
        }

        //End a one-step action
        public void EndAtomicAction()
        {
            CommitAction(mAtomicNewList);
            mAtomicActionRunning = false;
            mAtomicNewList = null;
        }

        //Move object down in the layout hierarchy
        public void MoveObjectDown(DrawingObject dObject)
        {
            if (mHistoryObjects[mCurrentHistoryLevel].IndexOf(dObject) == 0)
                return;
            bool wasRunningAtomic = mAtomicActionRunning;
            if (!wasRunningAtomic)
                BeginAtomicAction();
            int idx = mAtomicNewList.IndexOf(dObject);
            var swap = mAtomicNewList[idx];
            mAtomicNewList[idx] = mAtomicNewList[idx - 1];
            mAtomicNewList[idx - 1] = swap;
            if (!wasRunningAtomic)
                EndAtomicAction();
        }

        //Move object up in the layout hierarchy
        public void MoveObjectUp(DrawingObject dObject)
        {
            if (mHistoryObjects[mCurrentHistoryLevel].IndexOf(dObject) == mHistoryObjects[mCurrentHistoryLevel].Count - 1)
                return;
            bool wasRunningAtomic = mAtomicActionRunning;
            if (!wasRunningAtomic)
                BeginAtomicAction();
            int idx = mAtomicNewList.IndexOf(dObject);
            var swap = mAtomicNewList[idx];
            mAtomicNewList[idx] = mAtomicNewList[idx + 1];
            mAtomicNewList[idx + 1] = swap;
            if (!wasRunningAtomic)
                EndAtomicAction();
        }
    }
}
