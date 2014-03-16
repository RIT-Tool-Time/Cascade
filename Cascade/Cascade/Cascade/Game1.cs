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
        PanelManager panelManager;
        RenderTarget2D colorTarget, depthTarget;
        ParticleEmitter emitter;
        public Game1()
        {
            Global.Game = this;
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            tcp = new TcpObject();
            this.TargetElapsedTime = TimeSpan.FromSeconds(1d / 60d);
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
            
            Global.Camera.Pos = new Vector3(0, 0, -1000);
            Global.Camera.LookAtPos = new Vector3(0);
            
            panelManager = new PanelManager();
            panelManager.Add(Color.White);
            panelManager.Add(Color.White);
            panelManager.Add(Color.White);
            panelManager.Add(Color.White);
            panelManager.Add(Color.White);
            panelManager.Add(Color.White);

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
            emitter = new TriangleEmitter(Global.ParticleManager, Vector3.Zero)
            {
                Step = 0.1f,
                Speed = new Vector3(0, -10, 0),
                SpeedRange = new Vector3(5, 4, 10),
                ColorRange = new Color(0.5f, 0.5f, 0.5f, 0.0f),
            };
            emitter.Emitted += delegate(ParticleEmittedEventArgs e)
            {
                e.Particle.Gravity = -0.2f;
                e.Particle.Behaviors.Add(new Behaviors.Disappear(360, 0.1f, 0.1f, 1));
                e.Particle.Behaviors.Add(new Behaviors.Bounce(-250, 0.2f));
                e.Particle.Alpha = 0;
                e.Particle.Scale = new Vector2(0.1f);
                e.Particle.MotionStretch = true;
            };
            var emit = new CircleEmitter(Global.ParticleManager, new Vector3(-800, -900, 2300))
            {
                Speed = new Vector3(10, 30, 10),
                SpeedRange = new Vector3(2, 4, 5),
                ColorRange = new Color(0.5f, 0.5f, 0.5f, 0.0f),
                PosRange = new Vector3(100),
                Scale = new Vector2(1.0f),
                Step = 100
            };
            emit.Emitted += delegate(ParticleEmittedEventArgs e)
            {
                e.Particle.BlendState = BlendState.Opaque;
                e.Particle.Alpha = 0;
                e.Particle.Gravity = -0.4f;
                //e.Particle.BlendState = BlendState.Additive;
                e.Particle.Behaviors.Add(new Behaviors.Disappear(400, 0.1f, 0.02f, 1f));
            };

            var emit2 = new CircleEmitter(Global.ParticleManager, new Vector3(10000, -4000, 10000))
            {
                Step = 2,
                Speed = new Vector3(-25, 15, -10),
                SpeedRange = new Vector3(5, 2, 0),
                PosRange = new Vector3(500),
                ColorRange = new Color(0.5f, 0.5f, 0.5f, 0.0f),
            };
            emit2.Emitted += delegate(ParticleEmittedEventArgs e)
            {
                e.Particle.Alpha = 0;
                e.Particle.Behaviors.Add(new Behaviors.Disappear(400, 0.01f, 0.02f, 1f));
                e.Particle.Behaviors.Add(new Behaviors.Spin(new Vector3(200, 0, 0), new Vector3(5, 0, 0)));
            };

            this.IsMouseVisible = true;
            try
            {
                /*tcp.Connect("129.21.65.69", 8124);
                tcp.Write("Yo");
                tcp.Write("I am connected");
                Global.Output += "TCP Client connected";*/
            }
            catch
            {
                Global.Output += "TCP Client not connected";
            }
            graphics.PreferredBackBufferWidth = 1280; graphics.PreferredBackBufferHeight = (int)(graphics.PreferredBackBufferWidth * (9f / 16f)); ;
            graphics.ApplyChanges();
            socketThreadStart = new ThreadStart(SocketMethod);
            Global.Output += "LoadContent completed";
            
            CreateRenderTargets(1280, (9f / 16f));
            // TODO: use this.Content to load your game content here
        }
        private void CreateRenderTargets(int width, float aspectRatio)
        {
            int height = (int)(width * aspectRatio);
            colorTarget = new RenderTarget2D(GraphicsDevice, width, height, false, GraphicsDevice.PresentationParameters.BackBufferFormat, DepthFormat.None, 0, RenderTargetUsage.PreserveContents);
            depthTarget = new RenderTarget2D(GraphicsDevice, width, height, false, GraphicsDevice.PresentationParameters.BackBufferFormat, DepthFormat.None, 0, RenderTargetUsage.PreserveContents);
            Global.SpriteEffect.Parameters["depthTexture"].SetValue(depthTarget);
        }
        public void SocketMethod()
        {
            threadRunning = true;
            if (tcp.Connected)
            {
                string value = tcp.ReadString(Encoding.ASCII).Replace("\0", "");
                string[] values = value.Split(' ');
                foreach (var val in values)
                {
                    Global.Output += val;
                    if (val.StartsWith("#") && val.Length > 1)
                    {
                        string hex = val.Substring(1);
                        uint num = uint.Parse(hex, System.Globalization.NumberStyles.HexNumber, CultureInfo.InvariantCulture);
                        clearColor.R = (byte)(num >> 16);
                        clearColor.G = (byte)(num >> 8);
                        clearColor.B = (byte)num;
                        clearColor.A = (byte)255;
                        foreach (var mp in panelManager.Panels)
                        {
                            mp.ColorManager.Color = clearColor;
                        }
                    }
                    else if (val.Contains("Tap"))
                    {
                        //clearColor.Animate(Color.White, 15);
                        int index = 0;
                        try
                        {
                            string s = val.Substring(3);
                            int.TryParse(s, out index);
                            index--;
                            panelManager[index].ColorManager.Animate(Color.White, 15);
                        }
                        catch
                        {

                        }
                    }
                }
                Console.Write(value);
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
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
                this.Exit();
            if (tcp.Connected && !threadRunning)
            {
                socketThread = new Thread(socketThreadStart);
                socketThread.Start();
            }
            
            Global.Update(gameTime);
            clearColor.Update();
            panelManager.Update();
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
            if (Controls.MouseLeft == ControlState.Held)
            {
                emitter.Step += (0.2f - emitter.Step) * 0.1f * Global.Speed;
                emitter.Emit = true;
            }
            else
            {
                emitter.Step += (30f - emitter.Step) * 0.01f * Global.Speed;
                if (emitter.Step > 29)
                    emitter.Emit = false;
            }
            //Global.Output += emitter.Step;
            Vector3 mouse = new Vector3(Controls.MousePos, 0);
            Viewport v = new Viewport(0, 0, 1280, 720) { MinDepth = 0, MaxDepth = 1000000 };
            emitter.Pos = v.Unproject(mouse, Global.Effect.Projection, Global.Effect.View, Matrix.CreateTranslation(Global.Camera.Pos - Global.Camera.LookAtPos) * Matrix.CreateScale(1f / 1280, 1f / 720, 1));
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

            
            GraphicsDevice.SetRenderTarget(colorTarget);
            GraphicsDevice.Clear(Color.Wheat);
            GraphicsDevice.SetRenderTarget(depthTarget);
            GraphicsDevice.Clear(Color.Red);
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
            //panelManager.Draw(GraphicsDevice, graphics, spriteBatch, null, 1280, 720);

            //Set matrices for particles
            Global.Effect.View = Matrix.CreateLookAt(Global.Camera.Pos, Global.Camera.LookAtPos, Vector3.Up);
            Global.Effect.Projection = Matrix.CreatePerspectiveFieldOfView(MathHelper.ToRadians(45), 16f / 9f, 1, 1000000);
            Global.Effect.World = Matrix.CreateTranslation(0, 0, 0);
            Global.ParticleManager.Draw(GraphicsDevice, graphics, spriteBatch, colorTarget, Global.ScreenSize.X, Global.ScreenSize.Y);

            //sprite batch stuff
            GraphicsDevice.BlendState = BlendState.AlphaBlend;
            GraphicsDevice.SetRenderTarget(null);
            GraphicsDevice.Clear(Color.Black);
            //Global.SpriteEffect.Parameters["depthTexture"].SetValue(depthTarget);
            Matrix sbMatrix = Matrix.CreateOrthographicOffCenter(0, Global.ScreenSize.X, Global.ScreenSize.Y, 0, 0, 1);
            Global.SpriteEffect.Parameters["MatrixTransform"].SetValue(sbMatrix);

            
            spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, null, null, null);
            Global.SpriteEffect.SetTechnique("Bokeh");
            foreach (var pass in Global.SpriteEffect.CurrentTechnique.Passes)
            {
                pass.Apply();
                spriteBatch.Draw(colorTarget, new Rectangle(0, 0, (int)Global.ScreenSize.X, (int)Global.ScreenSize.Y), Color.White);
            }

            Global.SpriteEffect.SetTechnique("Normal");
            foreach (var pass in Global.SpriteEffect.CurrentTechnique.Passes)
            {
                pass.Apply();
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
