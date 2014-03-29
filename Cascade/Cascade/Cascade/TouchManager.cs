using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Windows7.Multitouch.Win32Helper;
using Windows7.Multitouch;
using Microsoft.Xna.Framework;
using System.Drawing;

namespace Cascade
{
    public static class TouchManager
    {
        public static bool SupportsTouch
        {
            get
            {
                return Windows7.Multitouch.Handler.DigitizerCapabilities.IsMultiTouchReady;
            }
        }
        static TouchHandler handler;
        public static List<TouchPoint> TouchPoints = new List<TouchPoint>();
        static List<TouchPoint> toAddToList = new List<TouchPoint>();
        static List<TouchPoint> toRemoveFromList = new List<TouchPoint>();
        public static void init()
        {
            handler = Factory.CreateHandler<TouchHandler>(Global.Game.Window.Handle);
            handler.TouchDown += new EventHandler<TouchEventArgs>(handler_TouchDown);
            handler.TouchUp += new EventHandler<TouchEventArgs>(handler_TouchUp);
            handler.TouchMove += new EventHandler<TouchEventArgs>(handler_TouchMove);
            
        }
        public static void Update()
        {
            for (int i = 0; i < TouchPoints.Count; i++)
            {
                TouchPoints[i].Update();
            }
            foreach (var t in toAddToList)
            {
                TouchPoints.Add(t);
            }
            toAddToList.Clear();
            foreach (var t in toRemoveFromList)
            {
                TouchPoints.Remove(t);
            }
            toRemoveFromList.Clear();
        }
        static void handler_TouchMove(object sender, TouchEventArgs e)
        {
            foreach (var t in TouchPoints)
            {
                if (t.Id == e.Id)
                {
                    t.SetArgs(e);
                }
            }
        }

        static void handler_TouchUp(object sender, TouchEventArgs e)
        {
            foreach (var t in TouchPoints)
            {
                if (t.Id == e.Id)
                {
                    t.SetArgs(e);
                    toRemoveFromList.Add(t);
                }
            }
        }
        static void handler_TouchDown(object sender, TouchEventArgs e)
        {
            toAddToList.Add(new TouchPoint(e));
        }
    }
    public enum TouchState { Touched, Moved, Released, None }
    public class TouchPoint
    {
        public Vector2 Position = Vector2.Zero;
        public int Id = 0;
        public int Timer = 0;
        public TouchState State = TouchState.None;
        public TouchPoint()
        {

        }
        public TouchPoint(TouchEventArgs e)
        {
            SetArgs(e);
        }
        public void Update()
        {
            Timer++;
            if (State == TouchState.Touched)
                State = TouchState.Moved;
            else if (State == TouchState.Released)
                State = TouchState.None;
        }
        public void SetArgs(TouchEventArgs e)
        {
            Position = new Vector2(e.Location.X, e.Location.Y);
            Id = e.Id;
            if (e.IsTouchDown)
                State = TouchState.Touched;
            else if (e.IsTouchMove)
                State = TouchState.Moved;
            else if (e.IsTouchUp)
                State = TouchState.Released;
        }
    }
}
