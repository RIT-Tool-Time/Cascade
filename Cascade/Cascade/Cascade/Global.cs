using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace Cascade
{
    public static class Global
    {
        public static OutputString Output;
        public static Game1 Game;
        static float speed = 1, framespeed = 1, gamespeed = 1, speedtarget = 1, speedspeed = 1;
        public static VertexEffect Effect;
        public static SpriteShader SpriteEffect;
        public static ParticleManager ParticleManager;
        public static Camera Camera;
        public static Vector2 ScreenSize = new Vector2(1280, 720);
        static Controls controls;
        public static float Speed
        {
            get
            {
                return speed;
            }
        }
        public static void init()
        {
            Effect = new VertexEffect();
            SpriteEffect = new SpriteShader();
            Output = new OutputString("\n");
            ParticleManager = new ParticleManager();
            Camera = new Camera();
            controls = new Controls();
        }
        public static void Update(GameTime time)
        {
            controls.update();
            framespeed = (60f) / (1f / (float)time.ElapsedGameTime.TotalSeconds);
            if (!float.IsNaN(framespeed))
            {
                gamespeed += (speedtarget - gamespeed) * speedspeed * framespeed;
                speed = MathHelper.Clamp(framespeed * gamespeed, 0, 5);
            }
            Effect.View = Matrix.CreateLookAt(Global.Camera.Pos, Global.Camera.LookAtPos, Vector3.Up);
            Effect.Projection = Matrix.CreatePerspectiveFieldOfView(MathHelper.ToRadians(45), 16f / 9f, 1, 1000000);
            Effect.World = Matrix.CreateTranslation(0, 0, 0);
            Controls.ScreenSize.X = Game.GraphicsDevice.PresentationParameters.BackBufferWidth;
            Controls.ScreenSize.Y = Game.GraphicsDevice.PresentationParameters.BackBufferHeight;
            //speed = 10;
            ParticleManager.Update();
        }
        public static void SetSpeed(float target, float speed)
        {
            speedtarget = target;
            speedspeed = speed;
        }
    }
    public static class Fonts
    {
        public static SpriteFont Output = Global.Game.Content.Load<SpriteFont>("OutputFont");
    }
    public struct OutputString
    {
        string output;
        string separator;
        public OutputString(string sep)
        {
            output = "";
            separator = sep;
        }
        public static implicit operator String(OutputString o)
        {
            return o.output;
        }
        public static OutputString operator +(OutputString o, object s)
        {
            o.output += s + o.separator;
            return o;
        }
    }
}
