//-----------------------------------------------------------------------------
// Copyright (C) Microsoft Corporation. All rights reserved.
//-----------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Windows7.Multitouch;
using Windows7.Multitouch.WinForms;

namespace mtGesture
{
    public partial class MainForm : Form
    {
        private readonly Windows7.Multitouch.GestureHandler _gestureHandler;
        private DrawingObject _rect;
 
        public MainForm()
        {
            InitializeComponent();

            Load += (s, e) => { _rect = new DrawingObject(this.Size); };
            SizeChanged += (s, e) => { if (_rect != null) _rect.ResetObject(this.Size); Invalidate(); };
            Paint += (s, e) => { if (_rect != null) _rect.Paint(e.Graphics); };

            _gestureHandler = Factory.CreateHandler<Windows7.Multitouch.GestureHandler>(this);

            _gestureHandler.Pan += ProcessPan;
            _gestureHandler.PanBegin += ProcessPan;
            _gestureHandler.PanEnd += ProcessPan;

            _gestureHandler.Rotate += ProcessRotate;
            _gestureHandler.RotateBegin += ProcessRotate;
            _gestureHandler.RotateEnd += ProcessRotate;

            _gestureHandler.PressAndTap += ProcessRollOver;

            _gestureHandler.TwoFingerTap += ProcessTwoFingerTap;

            _gestureHandler.Zoom += ProcessZoom;
            _gestureHandler.ZoomBegin += ProcessZoom;
            _gestureHandler.ZoomEnd += ProcessZoom;


        }

        private void ProcessPan(object sender, GestureEventArgs args)
        {
            _rect.Move(args.PanTranslation);
            //Text = String.Format("{0}", args.DistanceBetweenFingers);
            Invalidate();
        }

        private void ProcessRotate(object sender, GestureEventArgs args)
        {
            _rect.Rotate(args.RotateAngle, args.Center);
            Invalidate();
        }

        private void ProcessRollOver(object sender, GestureEventArgs args)
        {
            _rect.ShiftColor();
            Invalidate();
        }

        private void ProcessTwoFingerTap(object sender, GestureEventArgs args)
        {
            _rect.TogleDrawDiagonals();
            Invalidate();
        }

        private void ProcessZoom(object sender, GestureEventArgs args)
        {
            _rect.Zoom(args.ZoomFactor, args.Center);
            Invalidate();
        }
    }
}