//-----------------------------------------------------------------------------
// Copyright (C) Microsoft Corporation. All rights reserved.
//-----------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Interop;
using Windows7.Multitouch.Interop;

namespace Windows7.Multitouch.WPF
{
    /// <summary>
    /// Represents a WPF Window
    /// </summary>
    class WindowWrapper : IHwndWrapper
    {
        private readonly System.Windows.Window _window;

        public WindowWrapper(System.Windows.Window window)
        {
            _window = window;
  

            HandleCreated += (s,e) => {};
            _window.Loaded += (s,e) => { HandleCreated(s, EventArgs.Empty); };
        }


        #region IHwndWrapper Members

        public IntPtr Handle
        {
            get { return System.Diagnostics.Process.GetCurrentProcess().MainWindowHandle; }
        }

        public object Source
        {
            get { return _window; }
        }

        public event EventHandler HandleCreated;


        public event EventHandler HandleDestroyed
        {
            add
            {
                _window.Closed += value;
            }
            remove
            {
                _window.Closed -= value;
            }
        }

        public bool IsHandleCreated
        {
            get { return Handle != IntPtr.Zero; }
        }

        public System.Drawing.Point PointToClient(System.Drawing.Point point)
        {
            System.Windows.Point sourcePoint = new System.Windows.Point(point.X, point.Y);
            System.Windows.Point destinationPoint = _window.PointFromScreen(sourcePoint);
            return new System.Drawing.Point((int)(0.5 + destinationPoint.X), (int)(0.5 + destinationPoint.Y));
        }

        #endregion
    }

    /// <summary>
    /// The WPF GUI Timer for Inerta Events
    /// </summary>
    public class GUITimer : System.Windows.Threading.DispatcherTimer, IGUITimer
    {
        /// <summary>
        /// Do Nothing
        /// </summary>
        public void Dispose()
        {
            
        }

        /// <summary>
        /// Get/Set the timer interval
        /// </summary>
        int IGUITimer.Interval
        {
            get
            {
                return (int)base.Interval.Ticks;
            }
            set
            {
                base.Interval = TimeSpan.FromTicks(value);
            }
        }

        bool IGUITimer.Enabled
        {
            get
            {
                return base.IsEnabled;
            }
            set
            {
                base.IsEnabled = value;
            }
        }
    }

    /// <summary>
    /// Helper class for creating Gesture handler and Enabling touch for WPF application
    /// </summary>
    public class Factory
    {
        /// <summary>
        /// A factory that creates gesture handler for a WPF Window
        /// </summary>
        /// <remarks>since WPF does not support Touch events, only Gesture handler can be created</remarks>
        /// <param name="window">The window that need touch or gesture events</param>
        /// <returns>The Gesture Handler</returns>
        public static GestureHandler CreateGestureHandler(System.Windows.Window window) 
        {
            return Windows7.Multitouch.Handler.CreateHandler<GestureHandler>(new WindowWrapper(window));
        }

        /// <summary>
        /// Enable Stylus events, that represent touch events. 
        /// </summary>
        /// <remarks>Each stylus device has an Id that is corelate to the touch Id</remarks>
        /// <param name="window">The WPF window that needs stylus events</param>
        public static void EnableStylusEvents(System.Windows.Window window)
        {
            WindowInteropHelper windowInteropHelper = new WindowInteropHelper(window);

            // Set the window property to enable multitouch input on inking context.
            User32.SetProp(windowInteropHelper.Handle, "MicrosoftTabletPenServiceProperty", new IntPtr(0x01000000));
        }


        /// <summary>
        /// Create a wrapper for a UI based timer 
        /// </summary>
        /// <remarks>This timer is called in the context of the UI thread</remarks>
        /// <returns>A timer Wrapper</returns>
        public static IGUITimer CreateTimer()
        {
            return new GUITimer();
        }
    }

    /// <summary>
    /// Helper class to convert drawing point tp WPF point and vice versa
    /// </summary>
    public static class PointUtil
    {
        /// <summary>
        /// Convert WPF point to drawing point
        /// </summary>
        /// <param name="p">WPF point</param>
        /// <returns>Drawing Point</returns>
        public static System.Drawing.PointF ToDrawingPointF(this System.Windows.Point p)
        {
            return new System.Drawing.PointF((float)p.X, (float)p.Y);
        }

        /// <summary>
        /// Convert drawing point to WPF point
        /// </summary>
        /// <param name="p">Drawing Point</param>
        /// <returns>WPF Point</returns>
        public static System.Windows.Point ToDrawingPointF(this System.Drawing.PointF p)
        {
            return new System.Windows.Point(p.X, p.Y);
        }
    }
}