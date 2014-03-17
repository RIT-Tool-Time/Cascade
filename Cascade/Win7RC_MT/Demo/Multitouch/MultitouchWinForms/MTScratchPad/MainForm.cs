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
using System.Diagnostics;
using Windows7.Multitouch.WinForms;

namespace MTScratchPad
{
    public partial class MainForm : Form
    {
        private readonly TouchColor _touchColor = new TouchColor ();
        private readonly Dictionary<int, Stroke> _activeStrokes = new Dictionary<int, Stroke>();
        private readonly List<Stroke> _finishedStrokes = new List<Stroke>();
        private readonly Windows7.Multitouch.TouchHandler _touchHandler;

        public MainForm()
        {
            InitializeComponent();
        
            _touchHandler = Factory.CreateHandler<Windows7.Multitouch.TouchHandler>(this);

            _touchHandler.TouchDown += OnTouchDownHandler;
            _touchHandler.TouchMove += OnTouchMoveHandler;
            _touchHandler.TouchUp += OnTouchUpHandler;

            Paint += new PaintEventHandler(this.OnPaintHandler);
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            if (!Windows7.Multitouch.TouchHandler.DigitizerCapabilities.IsMultiTouchReady)
            {
                MessageBox.Show("Multitouch is not availible");
                Environment.Exit(1);
            }
            Text += " - Maximum Inputs: " + Windows7.Multitouch.TouchHandler.DigitizerCapabilities.MaxumumTouches;

        }

        // Touch down event handler.
        private void OnTouchDownHandler(object sender, Windows7.Multitouch.TouchEventArgs e)
        {

            // If there exist stroke with this ID, finish it.
            Stroke stroke;

            if(_activeStrokes.TryGetValue(e.Id, out stroke))
            {
                FinishStroke(stroke);
                // Request redraw of the window.
                Invalidate();
                return;
            }

            // Create new stroke, add point and assign a color to it.
            Stroke newStroke = new Stroke ();
            newStroke.Color = _touchColor.GetColor(e.IsPrimaryContact);
            newStroke.Id = e.Id;

            // Add new stroke to the collection of strokes in drawing.
            _activeStrokes[newStroke.Id] = newStroke;
        }

        private void FinishStroke(Stroke stroke)
        {
            //Seal stroke for better performance
            stroke.Seal();

            // Add finished stroke to the collection of finished strokes.
            _finishedStrokes.Add(stroke);

            // Remove finished stroke from the collection of strokes in drawing.
            _activeStrokes.Remove(stroke.Id);
        }

        // Touch up event handler.
        private void OnTouchUpHandler(object sender, Windows7.Multitouch.TouchEventArgs e)
        {
            // Find the stroke in the collection of the strokes in drawing.
            Stroke stroke;
            if (_activeStrokes.TryGetValue(e.Id, out stroke))
            {
                FinishStroke(stroke);

                // Request full redraw.
                Invalidate();
            }
        }

        // Touch move event handler.
        private void OnTouchMoveHandler(object sender, Windows7.Multitouch.TouchEventArgs e)
        {
            // Find the stroke in the collection of the strokes in drawing.
            Stroke stroke;
            if(_activeStrokes.TryGetValue(e.Id, out stroke))
            {
                // Add contact point to the stroke
                stroke.Add(e.Location);

                // Partial redraw: only the last line segment
                Graphics g = this.CreateGraphics();
                stroke.DrawLast(g);
            }
        }

        // OnPaint event handler.
        private void OnPaintHandler(object sender, PaintEventArgs e)
        {
            // Erase the background
            Brush brush = new SolidBrush(SystemColors.Window);
            e.Graphics.FillRectangle(brush, ClientRectangle);

            // Full redraw: draw complete collection of finished strokes and
            // also all the strokes that are currently in drawing.
            DrawStrokes(e.Graphics, _finishedStrokes);
            DrawStrokes(e.Graphics, _activeStrokes.Values.ToList());
       }

        //Draw all strokes
        private static void DrawStrokes(Graphics graphics, IEnumerable<Stroke> strokes)
        {
            foreach (Stroke stroke in strokes)
                stroke.Draw(graphics);
        }
    }

    // Color generator: assigns a color to the new stroke.
    public class TouchColor
    {
        private int _count = 0;  // Rotating secondary color index

        // Returns color for the newly started stroke.
        public Color GetColor(bool primary)
        {
            if (primary)
            {
                // The primary contact is drawn in black.
                return Color.Black;
            }
            else
            {
                // Take current secondary color.
                Color color = _secondaryColors[_count];

                // Move to the next color in the array.
                _count = (_count + 1) % _secondaryColors.Length;

                return color;
            }
        }

        static private Color[] _secondaryColors =    // Secondary colors
        {
            Color.Red,
            Color.LawnGreen,
            Color.Blue,
            Color.Cyan,
            Color.Magenta,
            Color.Yellow
        };
    }
}