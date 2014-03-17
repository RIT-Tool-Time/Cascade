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

namespace MultiControls
{
    public partial class TouchGestureEdit : TextBox
    {
        private GestureHandler _handler;

        public TouchGestureEdit()
        {
            InitializeComponent();
            _handler = Factory.CreateHandler<GestureHandler>(this);
            _handler.Pan += (s, e) => { UpdateValue(e); };
        }

        private void UpdateValue(GestureEventArgs e)
        {
            try
            {
                int v = int.Parse(base.Text);
                base.Text = (v + e.PanTranslation.Width).ToString();
            }
            catch
            {

            }
        }

        protected override void OnPaint(PaintEventArgs pe)
        {
            base.OnPaint(pe);
        }
    }
}