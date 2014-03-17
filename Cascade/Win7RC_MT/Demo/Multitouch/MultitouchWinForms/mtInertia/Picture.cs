//-----------------------------------------------------------------------------
// Copyright (C) Microsoft Corporation. All rights reserved.
//-----------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Windows7.Multitouch.Manipulation;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace mtInertia
{
    class Picture
    {
        private Bitmap _bitmap;

        //Load the picture and initialize the class
        public Picture(string filePath, float dpiX, float dpiY)
        {
            _bitmap = Bitmap.FromFile(filePath) as Bitmap;
            _bitmap.SetResolution(dpiX, dpiY);
            ScalingFactor = new SizeF(1F, 1F);
            Width = 100;
            Height = 100 * ((float)_bitmap.Height / _bitmap.Width);
        }

        /// <summary>
        /// Angle in radians
        /// </summary>
        public float Angle {get; set; }

        public SizeF Translate { get; set; }

        public SizeF ScalingFactor { get; set; }

        public float Width { get; set; }

        public float Height { get; set; }

        private float WidthFactor { get { return (float)Width / _bitmap.Width; } }

        private float HeightFactor { get { return (float)Height / _bitmap.Height; } } 

        //Transform the bitmap (scale & rotate)
        private Bitmap TransformImage()
        {
            //Get the scale & rotate transform matrix
            Matrix matrix = GetTrasformationMatrix();

            //Get an array of the bitmap boundaries after the matrix transformation
            PointF[] bitmapRect = GetBitmapBoundary(matrix);

            float minX = FindMinX(bitmapRect);
            float minY = FindMinY(bitmapRect);
            float maxX = FindMaxX(bitmapRect);
            float maxY = FindMaxY(bitmapRect);
            
            RectangleF boundRect = new RectangleF(
                0, 0, Math.Abs(maxX - minX), Math.Abs(maxY - minY));

            //create a new empty bitmap to hold rotated image
            Bitmap transformedBitmap = new Bitmap((int)boundRect.Width, (int)boundRect.Height);

            //make a graphics object from the empty bitmap
            Graphics g = Graphics.FromImage(transformedBitmap);

            g.TranslateTransform(-minX, -minY);

            //For debugging:
            //g.FillRectangle(Brushes.Azure, g.VisibleClipBounds);
            //g.DrawPolygon(new Pen(Color.Red, 10), bitmapRect);          
            
            g.MultiplyTransform(matrix);

            //draw the image onto transformed graphics
            g.DrawImage(_bitmap, new PointF(0,0));
            
            return transformedBitmap; 
        }

        //Get the bitmap boundaries after transformation
        private PointF[] GetBitmapBoundary(Matrix matrix)
        {
            PointF[] bitmapRect = new PointF[] 
            {   new PointF(0, 0), 
                new PointF(_bitmap.Width, 0),
                new PointF(_bitmap.Width, _bitmap.Height),
                new PointF(0, _bitmap.Height)
            };

            matrix.TransformPoints(bitmapRect);

            return bitmapRect;
        }

        private Matrix GetTrasformationMatrix()
        {
            Matrix matrix = new Matrix();
            matrix.RotateAt(Angle, new PointF(ScalingFactor.Width * Width / 2, ScalingFactor.Height * Height / 2));
            matrix.Scale(ScalingFactor.Width * WidthFactor, ScalingFactor.Height * HeightFactor, MatrixOrder.Prepend);
            
            return matrix;
        }

        private static float FindMaxY(PointF[] bitmapRect)
        {
            return bitmapRect.Max(p => p.Y);
        }

        private static float FindMaxX(PointF[] bitmapRect)
        {
            return bitmapRect.Max(p => p.X);
        }

        private static float FindMinY(PointF[] bitmapRect)
        {
            return bitmapRect.Min(p => p.Y);
        }

        private static float FindMinX(PointF[] bitmapRect)
        {
            return bitmapRect.Min(p => p.X);
        }

        //Find if a point is in the bound rectangle
        public bool HitTest(PointF point)
        {
            Matrix matrix = GetTrasformationMatrix();

            PointF[] points = GetBitmapBoundary(matrix);

            float minX = FindMinX(points);
            float minY = FindMinY(points);
            
            Matrix translateMatrix = new Matrix();
            translateMatrix.Translate(-minX + Translate.Width - Width * ScalingFactor.Width / 2, -minY + Translate.Height - ScalingFactor.Height * Height / 2);
            translateMatrix.TransformPoints(points);

            PointF[] ptRect = new PointF[5];
            Array.Copy(points, ptRect, 4);
            ptRect[4] = ptRect[0];
            
            bool isIn = false;

            for (int j = 0, i = 1; i < 5; ++i, ++j)
            {
                if ((((ptRect[i].Y < point.Y) && (point.Y < ptRect[j].Y)) || ((ptRect[j].Y < point.Y)
                    && (point.Y < ptRect[i].Y))) &&
                    (point.X < (ptRect[j].X - ptRect[i].X) * (point.Y - ptRect[i].Y) / (ptRect[j].Y -ptRect[i].Y) +ptRect[i].X))
                {
                    isIn = !isIn;
                }
            }

            return isIn;        
        }

        //Draw the transformed image
        public void Draw(Graphics graphics)
        {
            Bitmap image = TransformImage();
            graphics.DrawImage(image, new PointF(Translate.Width - Width * ScalingFactor.Width / 2, Translate.Height - ScalingFactor.Height * Height / 2));
        }
    }

    //Hold the pictures
    class Canvas
    {
        private List<Picture> _pictures = new List<Picture>();

        public void Add(Picture picture)
        {
            _pictures.Add(picture);
        }

        public void MovePictureToFront(Picture picture)
        {
            _pictures.Remove(picture);
            _pictures.Add(picture);
        }

        public void Draw(Graphics graphics)
        {
            foreach (Picture picture in _pictures)
                picture.Draw(graphics);
        }

        public int Count
        {
            get
            {
                return _pictures.Count;
            }
        }

        public Picture HitTest(PointF location)
        {
            for (int i = _pictures.Count - 1; i >= 0; --i)
            {
                if (_pictures[i].HitTest(location))
                    return _pictures[i];
            }
            return null;
        }
    }
}