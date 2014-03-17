//-----------------------------------------------------------------------------
// Copyright (C) Microsoft Corporation. All rights reserved.
//-----------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace mtGesture
{
    /// <summary>
    /// Drawing object is a rectangle that demonstrates gestures 
    /// </summary>
    class DrawingObject
    {
        private Point _middlePosition;
        private Size _size;
        private double _scalingFactor;
        private double _rotationAngle;
        private bool _drawDiagonals;
        private int _colorIndex;

        // This is initialization of colors that we are going to shift through rectangle object
        // whenever user invokes rollover gesture
        private readonly Color [] _colors = new []  
            {
                Color.FromArgb(13,13,13),     // black
                Color.FromArgb(255,139,23),   // yellow   
                Color.FromArgb(146,208,80),   // green
                Color.FromArgb(149,179,215)   // blue
            };

        /// <summary>
        /// Create new instance of the rectangle.
        /// </summary>
        /// <param name="clientSize">The form size</param>
        public DrawingObject(Size clientSize)
        {
            
            ResetObject(clientSize);
        }
       
        /// <summary>
        /// Set the default state for the drawing object
        /// </summary>
        /// <param name="clientSize"></param>
        public void ResetObject(Size clientSize)
        {
             // Initial positon of center point is the middle point of client window
            _middlePosition.X = clientSize.Width / 2;
            _middlePosition.Y = clientSize.Height / 2;
    
            // Initial width and height are half a size of client window
            _size.Width = clientSize.Width / 2;
            _size.Height = clientSize.Height / 2;
    
            // Initial scaling factor is 1.0 (no scaling)
            _scalingFactor = 1.0;

            // Initial rotation angle is 0.0 (no rotation)
            _rotationAngle = 0.0; 

            _drawDiagonals = false; // no drawing of diagonals

            _colorIndex = 0; // set initial collor to black
        }
        
        
        /// <summary>
        /// Draw the rectangle by applying rotation and scaling factors
        /// </summary>
        /// <param name="graphics"></param>
        public void Paint(Graphics graphics)
        {
            double localScale = 1.0;
            localScale = Math.Max(_scalingFactor, 0.05); 

            // first create a polyline that describes rectangle stratched for scaling factor
            Point [] ptRect = new Point[5];    

            ptRect[0].X = -(int)(localScale * _size.Width/2);
            ptRect[0].Y = -(int)(localScale * _size.Height/2);

            ptRect[1].X = (int)(localScale * _size.Width/2);
            ptRect[1].Y = ptRect[0].Y;

            ptRect[2].X = ptRect[1].X;
            ptRect[2].Y = (int)(localScale * _size.Height/2);

            ptRect[3].X = ptRect[0].X;
            ptRect[3].Y = ptRect[2].Y;
            
            ptRect[4].X = ptRect[0].X;
            ptRect[4].Y = ptRect[0].Y;

            // now we should rotate rectangle for rotation angle 
            double cos = Math.Cos(_rotationAngle);
            double sin = Math.Sin(_rotationAngle);

            for(int i = 0; i < 5; ++i)
            {
                int lDX = ptRect[i].X;
                int lDY = ptRect[i].Y;

                ptRect[i].X = (int)(lDX*cos + lDY*sin);
                ptRect[i].Y = (int)(lDY*cos - lDX*sin);
            }

            // finally we should translate this rectangle
            for(int i = 0; i < 5; ++i)
            {
                ptRect[i].X += _middlePosition.X;
                ptRect[i].Y += _middlePosition.Y;
            }    

            using (SolidBrush brush = new SolidBrush(Color.FromArgb(210,0,0)))
            {
                graphics.FillPolygon(brush, ptRect, System.Drawing.Drawing2D.FillMode.Winding);
            }

            using (Pen pen = new Pen(_colors[_colorIndex], 6))
            {
                graphics.DrawPolygon(pen, ptRect);
            }

            if(_drawDiagonals)
            {
                using (Pen pen = new Pen(Color.Black, 6))
                {
                    // draw diagonals
                    graphics.DrawLine(pen, ptRect[0], ptRect[2]);
                    graphics.DrawLine(pen, ptRect[1], ptRect[3]);
                }
            }
        }

        /// <summary>
        /// Translate the rectangle
        /// </summary>
        /// <param name="size">The translation amount</param>
        public void Move(Size size)
        {
            _middlePosition.X += size.Width;
            _middlePosition.Y += size.Height;
        }

        /// <summary>
        /// Start or stop X drawing
        /// </summary>
        public void TogleDrawDiagonals()
        {
            _drawDiagonals = !_drawDiagonals;
        }
        
        /// <summary>
        /// Change the rectangle size
        /// </summary>
        /// <param name="zoomFactor">scaling factor</param>
        /// <param name="center">the zoom center point</param>
        public void Zoom(double zoomFactor, Point center)
        {
            _scalingFactor *= zoomFactor;
        }

        /// <summary>
        /// Rotate the rectangle
        /// </summary>
        /// <param name="angle">The relative angle</param>
        /// <param name="center">Rotation center point</param>
        public void Rotate(double angle, Point center)
        {
            _rotationAngle += angle;
        }

        /// <summary>
        /// Change the frame color
        /// </summary>
        public void ShiftColor()
        {
            _colorIndex = (_colorIndex + 1) % _colors.Length;
        }
    }
}