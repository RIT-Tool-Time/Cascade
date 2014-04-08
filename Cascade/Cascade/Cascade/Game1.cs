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
using System.Threading;
using System.Text;
using System.Globalization;
using Windows7.Multitouch.Win32Helper;
using Windows7.Multitouch;
using System.ComponentModel;

namespace Cascade
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Game1 : Microsoft.Xna.Framework.Game
    {
        public GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        ThreadStart socketThreadStart;
        Thread socketThread;
        TcpObject tcp;
        bool threadRunning = false;
        Random rand = new Random();
        ColorManager clearColor = new ColorManager();
        RenderTarget2D colorTarget, depthTarget, finalTarget;
        TouchEmitter[] emitters = new TouchEmitter[10];
        public List<TouchPoint> Touches = new List<TouchPoint>();
        string socketBuffer = "";
        public BackgroundWorker startUpWorker;
        System.Timers.Timer threadTimer;
        public Game1()
        {
            threadTimer = new System.Timers.Timer(1000d / 240d);
            threadTimer.Elapsed += new System.Timers.ElapsedEventHandler(threadTimer_Elapsed);
            Global.Game = this;
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            tcp = new TcpObject();
            this.TargetElapsedTime = TimeSpan.FromSeconds(1d / 60d);
            GC.KeepAlive(this.Window);
            startUpWorker = new BackgroundWorker();
            startUpWorker.DoWork += new DoWorkEventHandler(startUpWorker_DoWork);
        }

        void threadTimer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            if (tcp.Connected && !threadRunning)
            {
                socketThread = new Thread(socketThreadStart);
                socketThread.Priority = ThreadPriority.Highest;
                socketThread.IsBackground = false;
                socketThread.Start();
            }
        }

        void startUpWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            try
            {
                tcp.Connect("129.21.64.202", 8124);
                tcp.Write("Yo");
                tcp.Write("I am connected");
                Global.Output += "TCP Client connected";
            }
            catch
            {
                Global.Output += "TCP Client not connected";
            }
        }

        void th_TouchDown(object sender, TouchEventArgs e)
        {
            Global.Output += "Touchdown!";
            
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
            Global.init();
            TouchManager.init();

            for (int i = 0; i < emitters.Length; i++)
            {
                emitters[i] = new TouchEmitter(Global.ParticleManager, Vector3.Zero)
                {
                    Step = 6,
                    Speed = new Vector3(0, 0, 0),
                    SpeedRange = new Vector3(0, 0, 0),
                    Color = new Color(210, 235, 255),
                    ColorRange = new Color(0.0f, 0.03f, 0.0f, 0.0f),
                    SpeedTransferMultiplier = 1f
                };
                emitters[i].Emitted += new ParticleEmittedEventHandler(Game1_Emitted);
            }

            Global.Camera.Pos = new Vector3(0, 0, -1000);
            Global.Camera.LookAtPos = new Vector3(0);
            /*panelManager.Add(Color.White);
            panelManager.Add(Color.White);
            panelManager.Add(Color.White);
            panelManager.Add(Color.White);
            panelManager.Add(Color.White);
            panelManager.Add(Color.White);*/
            Global.PanelManager.NoteOffset = 48;
            MusicManager.Update();
            for (int i = 0; i < MusicManager.PentatonicScale.Length; i++)
            {
                Global.PanelManager.Add(
                    new MusicPanel()
                    {
                        NoteOffset = MusicManager.PentatonicScale[i % MusicManager.PentatonicScale.Length]
                    }
                    );
            }

            for (int i = 0; i < 10; i++)
            {
                for (int o = 0; o < 10; o++)
                {
                    //var p = new Ellipse(Global.ParticleManager, new Vector3(i * 300, 0, o * 300), 24) { Color = MyMath.Between(Color.Aqua, Color.Red, (float)o / 10), Speed = new Vector3(0, 1, 0) };
                    //p.Behaviors.Add(new Behaviors.Disappear(100, 0.1f, 0.01f, 1));
                    //p.Behaviors.Add(new Behaviors.Spin(new Vector3(100, 50, 25), new Vector3(5, 4, 2)));
                }
            }

            //new Ellipse(Global.ParticleManager, new Vector3(0, 0, 1000), 30) { Color = Color.Aqua, Speed = new Vector3(0, 0, 0) };
            spriteBatch = new SpriteBatch(GraphicsDevice);
            

            this.IsMouseVisible = true;

            startUpWorker.RunWorkerAsync();
            threadTimer.Start();
            graphics.PreferredBackBufferWidth = 1920; graphics.PreferredBackBufferHeight = (int)(graphics.PreferredBackBufferWidth * (9f / 16f));
            graphics.IsFullScreen = true;
            graphics.ApplyChanges();
            socketThreadStart = new ThreadStart(SocketMethod);
            Global.Output += "LoadContent completed";
            
            CreateRenderTargets(1920, (9f / 16f));
            // TODO: use this.Content to load your game content here
        }

        void Game1_Emitted(ParticleEmittedEventArgs e)
        {
            if (e.Emitter is TouchEmitter)
            {
                TouchEmitter emit = e.Emitter as TouchEmitter;
                if (emit.Touch == null)
                {

                }
                else
                {
                    if (emit.Touch.Holding)
                    {
                        e.Particle.Scale = new Vector2(MyMath.RandomRange(0.1f, 0.3f));
                    }
                    else
                    {
                        e.Particle.Scale = new Vector2(MyMath.RandomRange(0.25f, 0.75f));
                    }
                }
                e.Particle.Gravity = 0.0f;
                e.Particle.Behaviors.Add(new Behaviors.Disappear(60, 0.025f, 0.05f, 0.5f));
                e.Particle.Behaviors.Add(new Behaviors.SpeedDamping(0.6f, 0.5f));
                float ang = MyMath.RandomRange(0, 360);
                e.Particle.Speed += new Vector3(MyMath.LengthDirX(30, ang), MyMath.LengthDirY(30, ang), 0);

                e.Particle.BlendState = BlendState.AlphaBlend;
                //e.Particle.Behaviors.Add(new Behaviors.Bounce(720, 0.5f));
                e.Particle.Alpha = 0;
                
                e.Particle.ScaleSpeed = new Vector2(MyMath.RandomRange(0.01f, 0.02f)) * 0.1f;
                e.Particle.MotionStretch = true;
            }
        }
        private void CreateRenderTargets(int width, float aspectRatio)
        {
            int height = (int)(width * aspectRatio);
            colorTarget = new RenderTarget2D(GraphicsDevice, width, height, false, GraphicsDevice.PresentationParameters.BackBufferFormat, DepthFormat.None, 0, RenderTargetUsage.PreserveContents);
            depthTarget = new RenderTarget2D(GraphicsDevice, width, height, false, GraphicsDevice.PresentationParameters.BackBufferFormat, DepthFormat.None, 0, RenderTargetUsage.PreserveContents);
            finalTarget = new RenderTarget2D(GraphicsDevice, width, height, false, GraphicsDevice.PresentationParameters.BackBufferFormat, DepthFormat.None, 0, RenderTargetUsage.PreserveContents);
            Global.SpriteEffect.Parameters["depthTexture"].SetValue(depthTarget);
        }
        public void SocketMethod()
        {
            threadRunning = true;
            if (tcp.Connected)
            {
                bool checkValue = false;
                string newBuffer = "";
                string value = tcp.ReadString(Encoding.ASCII).Replace("\0", "");
                for (int i = 0; i < value.Length; i++)
                {
                    if (value[i] != ';' && !checkValue)
                    {
                        socketBuffer += value[i];
                    }
                    else
                    {
                        Global.Output += socketBuffer;
                        if (socketBuffer.Contains("move"))
                        {
                            var vals = socketBuffer.Split(':');
                            foreach (var t in Touches)
                            {
                                int id = (int)(long.Parse(vals[2]) % int.MaxValue);
                                if (t.Id == id)
                                {
                                    string[] pos = vals[1].Split(',');
                                    t.Position.X = float.Parse(pos[0]) * Global.ScreenSize.X;
                                    t.Position.Y = float.Parse(pos[1]) * Global.ScreenSize.Y;
                                }
                            }
                        }
                        else if (socketBuffer.Contains("touch"))
                        {
                            var vals = socketBuffer.Split(':');
                            var touch = new TouchPoint();
                            long l = long.Parse(vals[2]);
                            touch.Id = (int)(l % int.MaxValue);
                            touch.State = TouchState.Touched;
                            string[] pos = vals[1].Split(',');
                            touch.Position.X = float.Parse(pos[0]) * Global.ScreenSize.X;
                            touch.Position.Y = float.Parse(pos[1]) * Global.ScreenSize.Y;
                            Touches.Add(touch);
                            Touches = Touches;
                        }
                        else if (socketBuffer.Contains("cancel"))
                        {
                            var vals = socketBuffer.Split(':');
                            if (vals[1] != "null")
                            {
                                TouchPoint t1 = null;
                                string[] strings = vals[1].Split(',');
                                int[] ids = new int[strings.Length];
                                for (int o = 0; o < ids.Length; o++)
                                {
                                    ids[o] = (int)(long.Parse(strings[o]) % int.MaxValue);
                                }
                                foreach (var t in Touches)
                                {
                                    if (!ids.Contains(t.Id))
                                    {
                                        t1 = t;
                                        break;
                                    }
                                }
                                if (t1 != null)
                                {
                                    t1.State = TouchState.Released;
                                    //Touches.Remove(t1);
                                }
                            }
                            else
                            {
                                foreach (var t in Touches)
                                {
                                    t.State = TouchState.Released;
                                }
                                //Touches.Clear();
                            }
                        }
                        socketBuffer = "";
                    }
                    
                }
                
            }
            threadRunning = false;
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
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Controls.GetKey(Keys.Escape) == ControlState.Pressed)
            {
                this.Exit();
                tcp.Close();
            } 
            
            
            
            TouchManager.Update();
            Global.Update(gameTime);
            MusicManager.Update();
            VolumeMeter.Update();
            Global.Output += VolumeMeter.Volume;
            clearColor.Update();
            Global.PanelManager.Update();
            if (Controls.GetKey(Keys.Space) == ControlState.Pressed)
            {
                Global.SetSpeed(0.1f, 0.1f);
            }
            else if (Controls.GetKey(Keys.Space) == ControlState.Released)
            {
                Global.SetSpeed(1f, 0.1f);
            }
            if (Controls.GetKey(Keys.Down) == ControlState.Held)
            {
                Global.Camera.LookAtPos = new Vector3(Global.Camera.LookAtPos.X, Global.Camera.LookAtPos.Y - 5, Global.Camera.LookAtPos.Z);
            }
            foreach (var touch in Global.Touches)
            {
                //Global.Output += touch.State;
                foreach (var emit in emitters)
                {
                    if (emit.Touch == null && touch.State == TouchState.Touched)
                    {
                        emit.Touch = touch;
                        emit.Pos = touch.Position.ToVector3();
                       // emit.EmitParticle();
                        break;
                    }
                }
            }
            for (int i = 0; i < Touches.Count; i++)
            {
                Touches[i].Update();
                if (Touches[i].State == TouchState.None)
                {
                    Touches.RemoveAt(i);
                    i--;
                }
            }
            //emitter.Pos = new Vector3(-Controls.MousePos, 1000);
            //Global.Output += Global.ParticleManager.NumberofParticles + ", " + Controls.MousePos + ", " + emitter.Pos;
            //Global.Output += GC.GetTotalMemory(false) / 1000000f;
            // TODO: Add your update logic here

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {

            
            GraphicsDevice.SetRenderTarget(depthTarget);
            GraphicsDevice.Clear(Color.Red);

            GraphicsDevice.SetRenderTarget(colorTarget);
            GraphicsDevice.Clear(Color.Transparent);

            GraphicsDevice.SetRenderTargets(colorTarget, depthTarget);

            //GraphicsDevice.Clear(Color.Black);
            RasterizerState rast = new RasterizerState()
            {
                CullMode = CullMode.None,
                FillMode = FillMode.Solid,
                DepthBias = 0
            };
            DepthStencilState depth = new DepthStencilState()
            {
                DepthBufferEnable = true,
                DepthBufferWriteEnable = false
            };
            GraphicsDevice.DepthStencilState = DepthStencilState.None;
            GraphicsDevice.RasterizerState = rast;

            //Set matrices for panels
            Global.Effect.View = Matrix.CreateTranslation(0, 0, 0);
            Global.Effect.Projection = Matrix.CreateOrthographicOffCenter(0, 1280, 720, 0, 0, 1);
            Global.Effect.World = Matrix.CreateTranslation(0, 0, 0);
            Global.Effect.Alpha = 1;
            Global.Effect.CurrentTechnique.Passes[0].Apply();
            Global.Effect.Parameters["depth"].SetValue(0);
            //panelManager.Draw(GraphicsDevice, graphics, spriteBatch, null, 1280, 720);

            //Set matrices for particles
            Global.Effect.View = Matrix.CreateTranslation(0, 0, 0);
           // Global.Effect.Projection = Matrix.CreatePerspectiveFieldOfView(MathHelper.ToRadians(45), 16f / 9f, 1, 1000000);
            Global.Effect.World = Matrix.CreateTranslation(0, 0, 0);
           
            Global.ParticleManager.Draw(GraphicsDevice, graphics, spriteBatch, colorTarget, Global.ScreenSize.X, Global.ScreenSize.Y);

            //sprite batch stuff
            GraphicsDevice.BlendState = BlendState.AlphaBlend;
            GraphicsDevice.SetRenderTarget(finalTarget);
            GraphicsDevice.Clear(new Color(170, 220, 240));
            //Global.SpriteEffect.Parameters["depthTexture"].SetValue(depthTarget);
            Matrix sbMatrix = Matrix.CreateOrthographicOffCenter(0, Global.ScreenSize.X, Global.ScreenSize.Y, 0, 0, 1);
            Global.SpriteEffect.Parameters["MatrixTransform"].SetValue(sbMatrix);
            Global.Effect.World = Matrix.CreateTranslation(Vector3.Zero);
            Global.Effect.Alpha = 1;
            GraphicsDevice.RasterizerState = RasterizerState.CullNone;
            Global.Effect.Color = Color.White;
            foreach (var pass in Global.Effect.CurrentTechnique.Passes)
            {
                pass.Apply();
                //spriteBatch.Draw(colorTarget, new Rectangle(0, 0, (int)Global.ScreenSize.X, (int)Global.ScreenSize.Y), Color.White);
                Global.PanelManager.Draw(GraphicsDevice, graphics, spriteBatch, null, 1280, 720);
            }

            spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, null, null, null);
            Global.SpriteEffect.SetTechnique("Normal");
            foreach (var pass in Global.SpriteEffect.CurrentTechnique.Passes)
            {
                pass.Apply();
                spriteBatch.Draw(colorTarget, new Rectangle(0, 0, (int)Global.ScreenSize.X, (int)Global.ScreenSize.Y), Color.White);
            }
            
            

            GraphicsDevice.SetRenderTarget(null);
            GraphicsDevice.Clear(Color.Black);
            Global.SpriteEffect.SetTechnique("Normal");
            foreach (var pass in Global.SpriteEffect.CurrentTechnique.Passes)
            {
                pass.Apply();
                spriteBatch.Draw(finalTarget, new Rectangle(0, 0, (int)Global.ScreenSize.X, (int)Global.ScreenSize.Y), Color.White);
                float y = 0;
                Vector2 size = Fonts.Output.MeasureString(Global.Output);
                if (size.Y > 500)
                    y = -(size.Y - 500);
                spriteBatch.DrawString(Fonts.Output, Global.Output, new Vector2(0, y), Color.Black);
            }
            

            
            spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
