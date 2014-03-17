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

namespace mtWPFScratchPad
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly Dictionary<int, Stroke> _activeStrokes = new Dictionary<int, Stroke>();
        private readonly TouchColor _touchColor = new TouchColor();

        public MainWindow()
        {
            InitializeComponent();
            
            if (!Windows7.Multitouch.TouchHandler.DigitizerCapabilities.IsMultiTouchReady)
            {
                MessageBox.Show("Multitouch is not availible");
                Environment.Exit(1);
            }

            Loaded += (s, e) => { Factory.EnableStylusEvents(this); };

            StylusDown += OnTouchDownHandler;
            StylusMove += OnTouchMoveHandler;
            StylusUp += OnTouchUpHandler;
        }

        // Touch down event handler.
        private void OnTouchDownHandler(object sender, StylusEventArgs e)
        {

            // If there exist stroke with this ID, finish it.
            Stroke stroke;

            if(_activeStrokes.TryGetValue(e.StylusDevice.Id, out stroke))
            {
                FinishStroke(stroke);
                return;
            }

            // Create new stroke, add point and assign a color to it.
            Stroke newStroke = new Stroke ();
            newStroke.Color = _touchColor.GetColor();
            newStroke.Id = e.StylusDevice.Id;

            // Add new stroke to the collection of strokes in drawing.
            _activeStrokes[newStroke.Id] = newStroke;
        }

        private void FinishStroke(Stroke stroke)
        {
            //Seal stroke for better performance
            stroke.Freeze();

            // Remove finished stroke from the collection of strokes in drawing.
            _activeStrokes.Remove(stroke.Id);
        }

        // Touch up event handler.
        private void OnTouchUpHandler(object sender, StylusEventArgs e)
        {
            // Find the stroke in the collection of the strokes in drawing.
            Stroke stroke;
            if (_activeStrokes.TryGetValue(e.StylusDevice.Id, out stroke))
            {
                FinishStroke(stroke);
            }
        }

        // Touch move event handler.
        private void OnTouchMoveHandler(object sender, StylusEventArgs e)
        {
            // Find the stroke in the collection of the strokes in drawing.
            Stroke stroke;
            if (_activeStrokes.TryGetValue(e.StylusDevice.Id, out stroke))
            {
                // Add contact point to the stroke
                stroke.Add(e.GetPosition(_canvas));
                stroke.AddToCanvas(_canvas);
            }
        }

        // Color generator: assigns a color to the new stroke.
        public class TouchColor
        {
            private int _count = 0;  // Rotating secondary color index

            // Returns color for the newly started stroke.
            public Color GetColor()
            {
                // Take current secondary color.
                Color color = _secondaryColors[_count];

                // Move to the next color in the array.
                _count = (_count + 1) % _secondaryColors.Length;

                return color;
            }

            static private Color[] _secondaryColors =    // Secondary colors
            {
                Colors.Black,
                Colors.Red,
                Colors.LawnGreen,
                Colors.Blue,
                Colors.Cyan,
                Colors.Magenta,
                Colors.Yellow
            };
        }
    }
}