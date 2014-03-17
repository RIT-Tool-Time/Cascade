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

namespace MultiControls
{
    public partial class TouchManipulationEdit : TextBox
    {
        private TouchHandler _handler;
        private ManipulationProcessor _processor;
        private int _startValue;

        public TouchManipulationEdit()
        {
            InitializeComponent();
            _handler = Factory.CreateHandler<TouchHandler>(this);
            _processor = new ManipulationProcessor(ProcessorManipulations.TRANSLATE_X);

            _handler.TouchDown += (s, e) => { _processor.ProcessDown((uint)e.Id, e.Location); };
            _handler.TouchUp += (s, e) => { _processor.ProcessUp((uint)e.Id, e.Location); };
            _handler.TouchMove += (s, e) => { _processor.ProcessMove((uint)e.Id, e.Location); };

            _processor.ManipulationStarted += (s, e) => { CaptureStartValue(); };
            _processor.ManipulationDelta += (s, e) => { UpdateValue(e); };
            
        }

        private void CaptureStartValue()
        {
            try
            {
                _startValue = int.Parse(base.Text);
            }
            catch
            {
                _startValue = 0;
            }
        }

        private void UpdateValue(ManipulationDeltaEventArgs e)
        {
            Text = ((int)(_startValue + e.CumulativeTranslation.Width)).ToString();
        }

        protected override void OnPaint(PaintEventArgs pe)
        {
            base.OnPaint(pe);
        }
    }
}