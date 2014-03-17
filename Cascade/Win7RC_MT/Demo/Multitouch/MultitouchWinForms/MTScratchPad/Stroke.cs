//-----------------------------------------------------------------------------
// Copyright (C) Microsoft Corporation. All rights reserved.
//-----------------------------------------------------------------------------

// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
// PARTICULAR PURPOSE.
//
// Copyright (c) Microsoft Corporation. All rights reserved.

using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;


namespace MTScratchPad
{
    // Stroke object represents a single stroke, trajectory of the finger from
    // touch-down to touch-up. Object has two properties: color of the stroke,
    // and ID used to distinguish strokes coming from different fingers.
    public class Stroke
    {
        private List<Point> _points = new List<Point>();  // the list of stroke points
        private Point[] _sealedPointsArray;

        private const float _penWidth = 3.0f;    // pen width for drawing the stroke

        /// <summary>
        /// Seal the object.
        /// </summary>
        /// <remarks>
        /// To improve performance, Seal replaces the list with fixed array.
        /// </remarks>
        public void Seal()
        {
            _sealedPointsArray = _points.ToArray();
            _points = null;
        }

        /// <summary>
        /// Indicate if we can add nore points to the stroke
        /// </summary>
        public bool IsSealed
        {
            get
            {
                return _sealedPointsArray != null;
            }
        }

        /// <summary>
        ///  Draws the complete stroke
        /// </summary>
        /// <param name="graphics">the form's graphics object</param>
        public void Draw(Graphics graphics)
        {
            Point[] pointArray = PointArray;

            if (pointArray.Length < 2)
            {
                return;
            }

            Pen pen = new Pen(Color, _penWidth);
            graphics.DrawLines(pen, pointArray);
        }

        private Point[] PointArray
        {
            get
            {
                return IsSealed ? _sealedPointsArray : _points.ToArray();
            }
        }
    
        /// <summary>
        /// Draws only last segment of the stroke.
        /// </summary>
        /// <param name="graphics">the drawing surface</param>
        public void DrawLast(Graphics graphics)
        {
            Point[] pointArray = PointArray;

            if (pointArray.Length >= 2)
            {
                Pen pen = new Pen(Color, _penWidth);
                graphics.DrawLine(pen, pointArray[pointArray.Length - 2], pointArray[pointArray.Length - 1]);
            }
        }

        /// <summary>
        /// Access to the property stroke color
        /// </summary>
        public Color Color { get; set; }


        /// <summary>
        /// Access to the property stroke ID 
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Adds a point to the stroke.
        /// </summary>
        /// <param name="pt">point to be added to the stroke</param>
        public void Add(Point pt)
        {
            if (IsSealed)
                throw new InvalidOperationException("This object is sealed");

            _points.Add(pt);
        }
    }
 }