using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.Graphics;

namespace XamarinFloatingActionButton
{
    public class TouchDelegateGroup : TouchDelegate
    {
        private static Rect USELESS_HACKY_RECT = new Rect();
        private List<TouchDelegate> mTouchDelegates = new List<TouchDelegate>();
        private TouchDelegate mCurrentTouchDelegate;
        private bool mEnabled;

        public TouchDelegateGroup(View uselessHackyView)
            : base(USELESS_HACKY_RECT, uselessHackyView)
        {
        }

        public void addTouchDelegate(TouchDelegate touchDelegate)
        {
            mTouchDelegates.Add(touchDelegate);
        }

        public void removeTouchDelegate(TouchDelegate touchDelegate)
        {
            mTouchDelegates.Remove(touchDelegate);
            if (mCurrentTouchDelegate == touchDelegate)
            {
                mCurrentTouchDelegate = null;
            }
        }

        public void clearTouchDelegates()
        {
            mTouchDelegates.Clear();
            mCurrentTouchDelegate = null;
        }

        public override bool OnTouchEvent(MotionEvent e)
        {
            if (!mEnabled) return false;

            TouchDelegate deleg = null;

            switch (e.Action)
            {
                case MotionEventActions.Down:
                    for (int i = 0; i < mTouchDelegates.Count; i++)
                    {
                        TouchDelegate touchDelegate = mTouchDelegates[i];
                        if (touchDelegate.OnTouchEvent(e))
                        {
                            mCurrentTouchDelegate = touchDelegate;
                            return true;
                        }
                    }
                    break;

                case MotionEventActions.Move:
                    deleg = mCurrentTouchDelegate;
                    break;

                case MotionEventActions.Cancel:
                case MotionEventActions.Up:
                    deleg = mCurrentTouchDelegate;
                    mCurrentTouchDelegate = null;
                    break;
            }

            return deleg != null && deleg.OnTouchEvent(e);
        }

        public void setEnabled(bool enabled)
        {
            mEnabled = enabled;
        }
    }
}