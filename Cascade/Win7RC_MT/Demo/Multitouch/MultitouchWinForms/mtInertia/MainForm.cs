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
using System.IO;

namespace mtInertia
{
    public partial class MainForm : Form
    {
        private readonly TouchHandler _touchHandler;
        private readonly PictureTrackerManager _pictureTrackerManager;
        private readonly Canvas _canvas = new Canvas();
        private const int NumOfPictures = 4;

        public MainForm()
        {
            InitializeComponent();
            
            //Load pictures from MyPictures
            if (!LoadPictures())
            {
                MessageBox.Show("No Pictures in MyPicture folder");
                Environment.Exit(0);
            }

            //Create the touch handler
            _touchHandler = Factory.CreateHandler<TouchHandler>(this);

            _pictureTrackerManager = new PictureTrackerManager(_canvas, this);

            //Register for touch events
            _touchHandler.TouchDown += _pictureTrackerManager.ProcessDown;
            _touchHandler.TouchUp += _pictureTrackerManager.ProcessUp;
            _touchHandler.TouchMove += _pictureTrackerManager.ProcessMove;

            Paint += (s, e) => { _canvas.Draw(e.Graphics); };
        }

        
        /// <summary>
        /// Load pictures from MyPictures
        /// </summary>
        /// <returns></returns>
        private bool LoadPictures()
        {
            int nPicturesLoaded = 0;
            float dpiX, dpiY;
            using (Graphics g = Graphics.FromHwnd(Handle))
            {
                dpiX = g.DpiX;
                dpiY = g.DpiY;
            }

            try
            {
                foreach (string filePath in Directory.GetFiles(
                    Environment.GetFolderPath(Environment.SpecialFolder.MyPictures), "*.jpg"))
                {
                    try
                    {
                        Picture p = new Picture(filePath, dpiX, dpiY);
                        _canvas.Add(p);
                        p.Translate = new SizeF((Width - p.Width) / 2, (Height - p.Height) / 2); 
                        ++nPicturesLoaded;
                        
                        if (nPicturesLoaded == NumOfPictures)
                            return true;
                    }
                    catch
                    {
                    }

                }
                return _canvas.Count > 0;
            }
            catch
            {

            }
            return false;
        }

        /// <summary>
        /// Managing the manipulation of multiple pictures in the same time
        /// </summary>
        class PictureTrackerManager
        {
            //Cache for re-use of picture trackers 
            private readonly Stack<PictureTracker> _pictureTrackers = new Stack<PictureTracker>();
            
            //Map between touch ids and picture trackers
            private readonly Dictionary<uint, PictureTracker> _pictureTrackerMap = new Dictionary<uint, PictureTracker>();
            private readonly Canvas _canvas;
            private Form _form;

            public PictureTrackerManager(Canvas canvas, Form form)
            {
                _canvas = canvas;
                _form = form;
            }

            public void ProcessDown(object sender, TouchEventArgs args)
            {
                PictureTracker pictureTracker = GetPictureTracker((uint)args.Id, args.Location);
                if (pictureTracker == null)
                    return;
                
                pictureTracker.ProcessDown((uint)args.Id, args.Location);
            }

            public void ProcessUp(object sender, TouchEventArgs args)
            {
                PictureTracker pictureTracker = GetPictureTracker((uint)args.Id, args.Location);
                if (pictureTracker == null)
                    return;
                
                pictureTracker.ProcessUp((uint)args.Id, args.Location);
            }

            public void ProcessMove(object sender, TouchEventArgs args)
            {
                PictureTracker pictureTracker = GetPictureTracker((uint)args.Id, args.Location);
                if (pictureTracker == null)
                    return;
                
                pictureTracker.ProcessMove((uint)args.Id, args.Location);
            }

            private PictureTracker GetPictureTracker(uint touchId, System.Drawing.Point location)
            {
                PictureTracker pictureTracker;

                //See if we already track the picture with the touchId
                if (_pictureTrackerMap.TryGetValue(touchId, out pictureTracker))
                    return pictureTracker;

                //Get the picture under the touch location
                Picture picture = FindPicture(location);

                if (picture == null)
                    return null;

                //See if we track the picture with other ID
                pictureTracker = (from KeyValuePair<uint, PictureTracker> entry in _pictureTrackerMap
                                  where entry.Value.Picture == picture
                                  select entry.Value).FirstOrDefault();
                
                //First time
                if (pictureTracker == null)
                {
                    //take from stack
                    if (_pictureTrackers.Count > 0)
                        pictureTracker = _pictureTrackers.Pop();
                    else //create new
                        pictureTracker = new PictureTracker(this, _form);

                    pictureTracker.Picture = picture;
                    BringPictureToFront(picture);
                }
                
                //remember the corelation between the touch id and the picture
                _pictureTrackerMap[touchId] = pictureTracker;

                return pictureTracker;
            }

            //We remove the touchID from the tracking map since the fingers are no longer touch
            //the picture
            public void InInertia(PictureTracker pictureTracker)
            {
                //remove all touch id from the map
                foreach (uint id in
                    (from KeyValuePair<uint, PictureTracker> entry in _pictureTrackerMap
                     where entry.Value == pictureTracker
                     select entry.Key).ToList())
                {
                    _pictureTrackerMap.Remove(id);
                }
            }

            //Inertia is completed, we can reuse the object
            public void Completed(PictureTracker pictureTracker)
            {
                pictureTracker.Picture = null;
                _pictureTrackers.Push(pictureTracker);
            }

            /// <summary>
            /// Find the picture in the touch location
            /// </summary>
            /// <param name="pointF">touch location</param>
            /// <returns>The picture or null if no picture exists in the touch location</returns>
            private Picture FindPicture(System.Drawing.PointF pointF)
            {
                return _canvas.HitTest(pointF); 
            }

            private void BringPictureToFront(Picture picture)
            {
                _canvas.MovePictureToFront(picture);
            }

        }

        /// <summary>
        /// Track a single picture
        /// </summary>
        class PictureTracker
        {
            //Calculate the inertia start velocity
            private readonly InertiaParam _inertiaParam = new InertiaParam();

            private readonly ManipulationInertiaProcessor _processor = 
                new ManipulationInertiaProcessor(ProcessorManipulations.ALL, Factory.CreateTimer());
            
            private readonly PictureTrackerManager _pictureTrackerManager;
            private readonly Form _form;

            public PictureTracker(PictureTrackerManager pictureTrackerManager, Form form)
            {
                _pictureTrackerManager = pictureTrackerManager;
                _form = form;

                //Start inertia velocity calculations
               _processor.ManipulationStarted += (s, e) =>
               {
                   _inertiaParam.Reset();
               };

                //All completed, inform the tracker manager that the current tracker can be reused
               _processor.ManipulationCompleted += (s, e) => { _inertiaParam.Stop(); pictureTrackerManager.Completed(this); };
               _processor.ManipulationDelta += ProcessManipulationDelta;
               _processor.BeforeInertia += OnBeforeInertia;
            }

            public Picture Picture { get; set; } 

            public void ProcessDown(uint id, System.Drawing.PointF location)
            {
                _processor.ProcessDown(id, location);
            }

            public void ProcessMove(uint id, System.Drawing.PointF location)
            {
                _processor.ProcessMove(id, location);
            }

            public void ProcessUp(uint id, System.Drawing.PointF location)
            {
                _processor.ProcessUp(id, location);
            }

            //Update picture state
            private void ProcessManipulationDelta(object sender, ManipulationDeltaEventArgs e)
            {
                if (Picture == null)
                    return;

                Picture.Translate = new SizeF(Picture.Translate.Width + e.TranslationDelta.Width,
                                    Picture.Translate.Height + e.TranslationDelta.Height);

                Picture.Angle += e.RotationDelta * (float)( 180 / Math.PI);

                Picture.ScalingFactor = new SizeF(Picture.ScalingFactor.Width * e.ScaleDelta, Picture.ScalingFactor.Height * e.ScaleDelta); 

                //Update inertia calculation. Take 40 percent from the previos data
                _inertiaParam.Update(e, 0.4F);

                _form.Invalidate();
            }

            //Fingers removed, start inertia
            void OnBeforeInertia(object sender, BeforeInertiaEventArgs e)
            {
                //Tell the tracker manager that the user removed the fingers
                _pictureTrackerManager.InInertia(this);

                _processor.InertiaProcessor.InertiaTimerInterval = 15;
                _processor.InertiaProcessor.MaxInertiaSteps = 500;
                _processor.InertiaProcessor.InitialVelocity = _inertiaParam.InitialVelocity;
                _processor.InertiaProcessor.DesiredDisplacement = _inertiaParam.InitialVelocity.Magnitude * 50;
                _processor.InertiaProcessor.InitialAngularVelocity = _inertiaParam.InitialAngularVelocity;
                _processor.InertiaProcessor.DesiredRotation = Math.Abs(_inertiaParam.InitialAngularVelocity * _processor.InertiaProcessor.InertiaTimerInterval * 540F / (float)Math.PI);
                _processor.InertiaProcessor.InitialExpansionVelocity = _inertiaParam.InitialExpansionVelocity;
                _processor.InertiaProcessor.DesiredExpansion = Math.Abs(_inertiaParam.InitialExpansionVelocity * 4F);
            }

            //Keep track of object velocities
            private class InertiaParam
            {
                public VectorF InitialVelocity { get; set; }
                public float InitialAngularVelocity { get; set; }
                public float InitialExpansionVelocity { get; set; }
                public System.Diagnostics.Stopwatch _stopwatch = new System.Diagnostics.Stopwatch();
                public void Reset()
                {
                    InitialVelocity = new VectorF(0, 0);
                    InitialAngularVelocity = 0;
                    InitialExpansionVelocity = 0;
                    _stopwatch.Reset();
                    _stopwatch.Start();
                }
                
                public void Stop()
                {
                    _stopwatch.Stop();
                }

                //update velocities, velocity = distance/time
                public void Update(ManipulationDeltaEventArgs e, float history)
                {
                    float elappsedMS = (float)_stopwatch.ElapsedMilliseconds;
                    if (elappsedMS == 0)
                        elappsedMS = 1;

                    InitialVelocity = InitialVelocity * history + ((VectorF)e.TranslationDelta * (1F - history)) / elappsedMS;
                    InitialAngularVelocity = InitialAngularVelocity * history + (e.RotationDelta * (1F - history)) / elappsedMS;
                    InitialExpansionVelocity = InitialExpansionVelocity * history + (e.ExpansionDelta * (1F - history)) / elappsedMS;
                    _stopwatch.Reset();
                    _stopwatch.Start();
                }
            }
        }
    }
}