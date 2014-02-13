using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using Microsoft.Kinect;
using System.Threading;

namespace KinectXNATest
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Game1 : Microsoft.Xna.Framework.Game
    {
        Effect effect;
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        KinectSensor kinect;
        DepthImagePixel[] depthPixels;
        ColorImagePoint[] colorPixels;
        RenderTarget2D depthTarget, colorTarget, depthCoordMap;
        Color[] depthArray, depthCoordArray;
        byte[] colorArray;
        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            Console.WriteLine("Looking for Kinect... ");
            foreach (var sensor in KinectSensor.KinectSensors)
            {
                Console.WriteLine(sensor.Status);
                if (sensor.Status == KinectStatus.Connected)
                {
                    kinect = sensor;
                    Console.WriteLine("Kinect connected");
                    break;
                }
            }
            if (kinect != null)
            {
                kinect.Start();
                kinect.DepthStream.Enable(DepthImageFormat.Resolution320x240Fps30);
                kinect.ColorStream.Enable(ColorImageFormat.RgbResolution640x480Fps30);
                depthPixels = new DepthImagePixel[kinect.DepthStream.FramePixelDataLength];
                
            }
            this.TargetElapsedTime = TimeSpan.FromSeconds(1d / 30d);
        }
        int ad = 0;
        void kinect_DepthFrameReady(object sender, DepthImageFrameReadyEventArgs e)
        {
            ad++;
            if (kinect != null && !depthFramyBusy)
            {
                var ts = new ThreadStart(delegate { depthFrameSetUp(e.OpenDepthImageFrame(), depthPixels, depthArray, depthTarget); });
                new Thread(ts).Start();
                
            }
        }
        bool depthFramyBusy = false;
        void depthFrameSetUp(DepthImageFrame frame, DepthImagePixel[] depthPixels, Color[] depthArray, RenderTarget2D depthTarget)
        {
            depthFramyBusy = true;
            using (frame)
            {
                if (frame != null)
                {
                    //Console.WriteLine("Has frame");
                    frame.CopyDepthImagePixelDataTo(depthPixels);

                    for (int i = 0; i < depthPixels.Length; i++)
                    {

                        int b = (depthPixels[i].Depth >= frame.MinDepth && depthPixels[i].Depth <= frame.MaxDepth && depthPixels[i].IsKnownDepth) ? depthPixels[i].Depth : 0;
                        if (depthPixels[i].Depth >= frame.MaxDepth)
                        {
                            b = frame.MaxDepth;
                        }
                        float f = (float)((float)b - frame.MinDepth) / (float)(frame.MaxDepth - frame.MinDepth);

                        depthArray[i] = new Color(f, f, f, 1);
                    }

                    depthTarget.SetData(depthArray);
                    DepthImagePoint[] dip = new DepthImagePoint[kinect.ColorStream.FrameWidth * kinect.ColorStream.FrameHeight];
                    kinect.CoordinateMapper.MapColorFrameToDepthFrame(kinect.ColorStream.Format,
                        kinect.DepthStream.Format, depthPixels, dip);
                    for (int i = 0; i < depthCoordArray.Length; i++)
                    {
                        depthCoordArray[i] = new Color((float)dip[i].X / (float)kinect.ColorStream.FrameWidth, (float)dip[i].Y / (float)kinect.ColorStream.FrameHeight, 0, 1);
                    }
                    depthCoordMap.SetData(depthCoordArray);

                }
                else
                {

                }
            }
            depthFramyBusy = false;
        }
        int ad2 = 0;
        void clearTextures()
        {
            for (int i = 0; i < 16; i++)
            {
                GraphicsDevice.Textures[i] = null;
            }
        }
        void kinect_ColorFrameReady(object sender, ColorImageFrameReadyEventArgs e)
        {
            ad2++;
            if (kinect != null && !colorFrameBusy)
            {
                ThreadStart ts = new ThreadStart(delegate { colorFrameSetUp(e.OpenColorImageFrame(), colorArray, colorTarget); });
                Thread t = new Thread(ts);
                t.Start();
            }
        }
        bool colorFrameBusy = false;
        void colorFrameSetUp(ColorImageFrame frame, byte[] colorArray, RenderTarget2D colorTarget)
        {
            colorFrameBusy = true;
            using (frame)
            {

                if (frame != null)
                {
                    frame.CopyPixelDataTo(colorArray);
                    if (frame.Format == ColorImageFormat.RgbResolution640x480Fps30)
                    {
                        for (int i = 0; i < colorArray.Length; i += 4)
                        {
                            if (i < colorArray.Length - 3)
                            {
                                 byte b = colorArray[i];
                                 byte g = colorArray[i + 1];
                                 byte r = colorArray[i + 2];
                                 byte a = colorArray[i + 3];
                                 colorArray[i] = r;
                                 colorArray[i + 1] = g;
                                 colorArray[i + 2] = b;
                                 colorArray[i + 3] = 255;
                            }
                        }
                    }
                    else if (frame.Format == ColorImageFormat.InfraredResolution640x480Fps30)
                    {
                        UInt16 int16 = 0;
                        for (int i = 0; i < colorArray.Length; i += 2)
                        {
                            if (i < colorArray.Length - 1)
                            {
                                byte b1 = colorArray[i], b2 = colorArray[i + 1];
                                int16 = (UInt16)(b2 << 8 | b1);
                                float f = (int16 / (float)ushort.MaxValue) * (16f);
                                byte b = (byte)f;
                                byte b12 = (byte)(b << 4 | b), b22 = (byte)(b << 4 | b);
                                colorArray[i] = b12;
                                colorArray[i + 1] = b22;
                            }
                        }
                    }
                    //kinect.CoordinateMapper.MapDepthFrameToColorFrame(
                    GraphicsDevice.Textures[0] = null;
                    colorTarget.SetData(colorArray);
                }
            }
            colorFrameBusy = false;
        }
        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            // TODO: Add your initialization logic here

            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);
            if (kinect != null)
            {
                depthTarget = new RenderTarget2D(this.GraphicsDevice, kinect.DepthStream.FrameWidth, kinect.DepthStream.FrameHeight);
                colorTarget = new RenderTarget2D(this.GraphicsDevice, kinect.ColorStream.FrameWidth, kinect.ColorStream.FrameHeight, false, SurfaceFormat.Color, DepthFormat.None);
                depthCoordMap = new RenderTarget2D(this.GraphicsDevice, kinect.ColorStream.FrameWidth, kinect.ColorStream.FrameHeight);
                depthArray = new Color[kinect.DepthStream.FrameHeight * kinect.DepthStream.FrameWidth];
                depthCoordArray = new Color[kinect.ColorStream.FrameWidth * kinect.ColorStream.FrameHeight];
                colorArray = new byte[kinect.ColorStream.FramePixelDataLength];
                Console.WriteLine("Depth: " + depthPixels.Length + ", Color: " + depthArray.Length);

                kinect.DepthFrameReady += new EventHandler<DepthImageFrameReadyEventArgs>(kinect_DepthFrameReady);
                kinect.ColorFrameReady += new EventHandler<ColorImageFrameReadyEventArgs>(kinect_ColorFrameReady);
            }
            graphics.PreferredBackBufferWidth = 640;
            graphics.PreferredBackBufferHeight = 480;
            graphics.ApplyChanges();
            effect = Content.Load<Effect>("Effect1");
            
            // TODO: use this.Content to load your game content here
        }

        

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            // Allows the game to exit
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
                this.Exit();

            // TODO: Add your update logic here

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);

            // TODO: Add your drawing code here
            spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend);
            if (kinect != null)
            {
                effect.CurrentTechnique = effect.Techniques["NormalDepth"];
                effect.Parameters["depthTexture"].SetValue(depthTarget);
                effect.Parameters["depthCoordTexture"].SetValue(depthCoordMap);
                Matrix m = Matrix.CreateOrthographicOffCenter(0, GraphicsDevice.Viewport.Width, GraphicsDevice.Viewport.Height, 0, 0, 1);
                effect.Parameters["MatrixTransform"].SetValue(m);
                foreach (var p in effect.CurrentTechnique.Passes)
                {
                    p.Apply();
                    spriteBatch.Draw(colorTarget, new Rectangle(0, 0, GraphicsDevice.Viewport.Width, GraphicsDevice.Viewport.Height), Color.White);
                }
            }

            spriteBatch.End();

            base.Draw(gameTime);
            clearTextures();
        }
    }
}
