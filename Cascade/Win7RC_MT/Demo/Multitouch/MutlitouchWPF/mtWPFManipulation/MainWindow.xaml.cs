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
using Windows7.Multitouch.Manipulation;

namespace mtWPFGesture
{
    /// <summary>
    /// Interaction logic for Window1.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        ManipulationProcessor _processor = new ManipulationProcessor(ProcessorManipulations.ALL);

        public MainWindow()
        {
            InitializeComponent();

            if (!Windows7.Multitouch.TouchHandler.DigitizerCapabilities.IsMultiTouchReady)
            {
                MessageBox.Show("Multitouch is not availible");
                Environment.Exit(1);
            }


            Loaded += (s, e) => { Factory.EnableStylusEvents(this); };
            SizeChanged += (s, e) => 
                {
                    _image.Width = e.NewSize.Width / 4; _image.Height = e.NewSize.Height/4;
                    Canvas.SetLeft(_image, e.NewSize.Width * 0.375);
                    Canvas.SetTop(_image, e.NewSize.Height * 0.375);
                };


           StylusDown += (s, e) => { _processor.ProcessDown((uint)e.StylusDevice.Id, e.GetPosition(_canvas).ToDrawingPointF()); };
           StylusUp += (s, e) => { _processor.ProcessUp((uint)e.StylusDevice.Id, e.GetPosition(_canvas).ToDrawingPointF()); };
           StylusMove += (s, e) => { _processor.ProcessMove((uint)e.StylusDevice.Id, e.GetPosition(_canvas).ToDrawingPointF()); };

            _processor.ManipulationDelta += ProcessManipulationDelta;
            _processor.PivotRadius = 2;
            
        }

        private void ProcessManipulationDelta(object sender, ManipulationDeltaEventArgs e)
        {
            _translate.X += e.TranslationDelta.Width;
            _translate.Y += e.TranslationDelta.Height;

            _rotate.Angle += e.RotationDelta * 180 / Math.PI;

            _scale.ScaleX *= e.ScaleDelta;
            _scale.ScaleY *= e.ScaleDelta;         
        }
    }
}