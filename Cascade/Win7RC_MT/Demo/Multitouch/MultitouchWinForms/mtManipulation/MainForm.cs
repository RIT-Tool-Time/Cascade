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
using Windows7.Multitouch.Manipulation;
using Windows7.Multitouch.WinForms;

namespace mtManipulation
{
    public partial class MainForm : Form
    {
        private readonly TouchHandler _touchHandler;
        private readonly ManipulationProcessor _processor;
        private readonly List<DrawingObject> _objectList;

        

        public MainForm()
        {
            InitializeComponent();
            _touchHandler = Factory.CreateHandler<TouchHandler>(this);
            
            _processor = new ManipulationProcessor(ProcessorManipulations.ALL);

            _objectList = new List<DrawingObject>
                { new DrawingObject(Size),
                    new DrawingObject(Size),
                    new DrawingObject(Size)
                };

            _touchHandler.TouchDown += (s,e ) => { _processor.ProcessDown((uint)e.Id, e.Location); };
            _touchHandler.TouchUp += (s, e) => { _processor.ProcessUp((uint)e.Id, e.Location); };
            _touchHandler.TouchMove += (s, e) => { _processor.ProcessMove((uint)e.Id, e.Location); };

            _processor.ManipulationDelta += ProcessManipulationDelta;
            _processor.PivotRadius = 2;
        }

        private void ProcessManipulationDelta(object sender, ManipulationDeltaEventArgs e)
        {
            DrawingObject obj = FindObject(Point.Round(e.Location));
            
            if (obj == null)
                return;

            obj.Move(e.TranslationDelta.ToSize());
            obj.Rotate(-e.RotationDelta, Point.Round(e.Location));
            obj.Zoom(e.ScaleDelta, Point.Round(e.Location));
            
            Invalidate();
        }

        private DrawingObject FindObject(Point location)
        {
            DrawingObject obj = (from o in _objectList
                                 orderby o.RangeFromCenter(location)
                                 select o).First();

            return obj;
        }

        private void MainForm_Paint(object sender, PaintEventArgs e)
        {
            foreach (DrawingObject obj in _objectList)
                obj.Paint(e.Graphics);
        }
    }
}