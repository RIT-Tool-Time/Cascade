//-----------------------------------------------------------------------------
// Copyright (C) Microsoft Corporation. All rights reserved.
//-----------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Drawing;
using Windows7.Multitouch.Interop;

namespace Windows7.Multitouch.WinForms
{
    /// <summary>
    /// Represents a WinForms Control
    /// </summary>
    class ControlWrapper : IHwndWrapper
    {
        private readonly Control _control;

        public ControlWrapper(Control control)
        {
            //It is better to add this to the application manifest, but if the user
            //forgot to do that, we do.
            User32.SetProcessDPIAware();

            _control = control;
 
        }

        #region IHwndWrapper Members

        public IntPtr Handle
        {
            get { return _control.Handle; }
        }

        public object Source
        {
            get { return _control; }
        }


        public event EventHandler HandleCreated
        {
            add
            {
                _control.HandleCreated += value;
            }
            remove
            {
                _control.HandleCreated -= value;
            }
        }


        public event EventHandler HandleDestroyed
        {
            add
            {
                _control.HandleDestroyed += value;
            }
            remove
            {
                _control.HandleDestroyed -= value;
            }
        }
        
        public bool IsHandleCreated
        {
            get { return _control.IsHandleCreated; }
        }

        public Point PointToClient(Point point)
        {
            return _control.PointToClient(point);
        }


        public IGUITimer CreateTimer()
        {
            return new GUITimer();
        }

        #endregion
    }

    /// <summary>
    /// The WinForm GUI Timer for Inerta Events
    /// </summary>
    public class GUITimer : Timer, IGUITimer
    {
    }


    /// <summary>
    /// A factory that creates touch or gesture handler for a Windows Forms controls
    /// </summary>
    public class Factory
    {
        /// <summary>
        /// A factory that creates touch or gesture handler for a Windows Forms controls
        /// </summary>
        /// <remarks>We use factory to ensure that only one handler will be created for a control, since Gesture and Touch are mutually exclude</remarks>
        /// <typeparam name="T">The handler type</typeparam>
        /// <param name="control">The control that need touch or gesture events</param>
        /// <returns>TouchHandler or Gesture Handler</returns>
        public static T CreateHandler<T>(Control control) where T : Windows7.Multitouch.Handler
        {
            return Windows7.Multitouch.Handler.CreateHandler<T>(new ControlWrapper(control));
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
}