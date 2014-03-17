//-----------------------------------------------------------------------------
// Copyright (C) Microsoft Corporation. All rights reserved.
//-----------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Windows7.Multitouch.WPF;
using Windows7.Multitouch;
using System.Windows.Media.Effects;

namespace mtWPFGesture
{
    /// <summary>
    /// Interaction logic for Window1.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly Windows7.Multitouch.GestureHandler _gestureHandler;
        private readonly BlurEffect _blur = new BlurEffect();
        private readonly DropShadowEffect _dropShadow = new DropShadowEffect();

        public MainWindow()
        {
            InitializeComponent();

            if (!Windows7.Multitouch.TouchHandler.DigitizerCapabilities.IsMultiTouchReady)
            {
                MessageBox.Show("Multitouch is not availible");
                Environment.Exit(1);
            }

            _dropShadow.Color = Colors.Black;
            _dropShadow.Direction = 320;
            _dropShadow.ShadowDepth = 30;
            _dropShadow.BlurRadius = 1;
            _dropShadow.Opacity = 0.5;

            _blur.Radius = 10;

            Loaded += (s, e) => {  };
            SizeChanged += (s, e) => 
                {
                    _image.Width = e.NewSize.Width / 4; _image.Height = e.NewSize.Height/4;
                    Canvas.SetLeft(_image, e.NewSize.Width * 0.375);
                    Canvas.SetTop(_image, e.NewSize.Height * 0.375);
                };

            _gestureHandler = Factory.CreateGestureHandler(this);

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
            _translate.X += args.PanTranslation.Width;
            _translate.Y += args.PanTranslation.Height;
        }

        private void ProcessRotate(object sender, GestureEventArgs args)
        {
            _rotate.Angle -= args.RotateAngle * 180 / Math.PI;
        }

        private void ProcessRollOver(object sender, GestureEventArgs args)
        {
            if (_image.Effect == _dropShadow)
            {
                _image.Effect = null;
            }
            else
            {
                _image.Effect = _dropShadow;
            }
        }

        private void ProcessTwoFingerTap(object sender, GestureEventArgs args)
        {
            if (_image.Effect == _blur)
            {
                _image.Effect = null;
            }
            else
            {
                _image.Effect = _blur;
            }
        }

        private void ProcessZoom(object sender, GestureEventArgs args)
        {
            _scale.ScaleX *= args.ZoomFactor;
            _scale.ScaleY *= args.ZoomFactor;
        }
    }
}