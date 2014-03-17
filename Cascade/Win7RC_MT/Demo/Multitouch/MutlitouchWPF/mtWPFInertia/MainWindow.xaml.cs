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
using Windows7.Multitouch.Manipulation;
using Windows7.Multitouch;
using Windows7.Multitouch.WPF;
using System.IO;

namespace mtWPFInertia
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly PictureTrackerManager _pictureTrackerManager;
        
        public MainWindow()
        {
            InitializeComponent();

            if (!Windows7.Multitouch.TouchHandler.DigitizerCapabilities.IsMultiTouchReady)
            {
                MessageBox.Show("Multitouch is not availible");
                Environment.Exit(1);
            }

            //Enable stylus events and load pictures
            this.Loaded += (s, e) => { Factory.EnableStylusEvents(this); LoadPictures(); };

            _pictureTrackerManager = new PictureTrackerManager(_canvas);

            //Register for stylus (touch) events
            StylusDown += _pictureTrackerManager.ProcessDown;
            StylusUp += _pictureTrackerManager.ProcessUp;
            StylusMove += _pictureTrackerManager.ProcessMove;
        }

        //Return collection of file/resource picture locations
        private string [] GetPictureLocations()
        {
            string[] pictures = Directory.GetFiles(
                    Environment.GetFolderPath(Environment.SpecialFolder.MyPictures), "*.jpg");
            
            // If there are no pictures in MyPictures
            if (pictures.Length == 0)
                pictures = new string[] { @"images\Pic1.jpg", @"images\Pic2.jpg", @"images\Pic3.jpg",
                                            @"images\Pic4.jpg" };

            return pictures;
        }


        //Load pictures to the canvas
        private void LoadPictures()
        {
            string[] pictureLocations = GetPictureLocations();
            double angle = 0;
            double step = 360 / pictureLocations.Length;

            foreach (string filePath in pictureLocations)
            {
                try
                {
                    Picture p = new Picture();
                    p.ImagePath = filePath;
                    p.Width = 300;
                    p.Angle = 180 - angle;
                    double angleRad = angle * Math.PI / 180.0;
                    p.X = Math.Sin(angleRad) * 300 + (_canvas.ActualWidth - 300) / 2.0;
                    p.Y = Math.Cos(angleRad) * 300 + (_canvas.ActualHeight - 300) / 2.0;
                    _canvas.Children.Add(p);

                    angle += step;
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Trace.WriteLine("Error:" + ex.Message);
                }
            }
        } 
    }
}