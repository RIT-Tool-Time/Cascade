using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
namespace Cascade
{
    public class Particle
    {
        PrimitiveType PrimitiveType = PrimitiveType.TriangleList;
        protected Color color = Color.White;
        public int PrimitiveCount = 1;
        public Vector3 Speed = Vector3.Zero;
        public Vector3 Pos = Vector3.Zero;
        public Vector2 Scale = Vector2.One;
        public List<Behaviors.ParticleBehavior> Behaviors;
        public BlendState BlendState = BlendState.AlphaBlend;
        public bool MotionStretch = false;
        public float Rotation = 0;
        public float Depth = 0;
        float stretchRot = 0, stretchScale = 0;
        Vector3 prevPos = Vector3.Zero;
        public Color Color
        {
            get { return color; }
            set
            {
                color = value;
                for (int i = 0; i < Vertices.Length; i++)
                {
                    Vertices[i].Color = value;
                }
            }
        }
        public ParticleManager Manager
        {
            get
            {
                return manager;
            }
        }
        float alpha = 1f;
        public float Alpha
        {
            get { return alpha; }
            set
            {
                alpha = value;
            }
        }
        public CascadeVertex[] Vertices;
        protected ParticleManager manager;
        public float Gravity = 0;

        public Particle(ParticleManager Manager, Vector3 pos)
        {
            Behaviors = new List<Cascade.Behaviors.ParticleBehavior>();
            Pos = pos;
            manager = Manager;
            manager.Add(this);
        }
        public virtual void Remove()
        {
            manager.Remove(this);
            
        }
        public virtual void Update()
        {
            prevPos = Pos;
            Speed.Y += Gravity * Global.Speed;
            Pos += Speed * Global.Speed;
            SetVertexPositions();

            foreach (var b in Behaviors)
            {
                b.Update(this);
            }
            if (MotionStretch)
            {
                stretchRot = -MathHelper.ToDegrees(MyMath.Direction(Pos.ToVector2(), prevPos.ToVector2())) + 90;
                stretchScale = MyMath.Distance((Pos - prevPos).ToVector2().ToVector3()) * 0.04f;
            }
            else
            {
                stretchRot = Rotation;
                stretchScale = 0;
            }
            Depth += 0.0075f * Global.Speed;
        }
        public virtual void SetVertexPositions()
        {
            /*Vector4 p = new Vector4(Pos, 0);
            for (int i = 0; i < Vertices.Length; i++)
            {
               Vertices[i].Transformation = p ;
            }*/
        }
        public virtual bool IsTransparent()
        {
            return alpha < 1 || color.A < 255 || BlendState != BlendState.AlphaBlend;
        }
        public void Draw(GraphicsDevice GraphicsDevice, GraphicsDeviceManager graphics, SpriteBatch spriteBatch, RenderTarget2D defaultRenderTarget, float width, float height)
        {
            Global.Effect.World = Matrix.CreateScale(new Vector3(Scale + new Vector2(0, stretchScale), 1)) * Matrix.CreateRotationZ(MathHelper.ToRadians(stretchRot)) * Matrix.CreateTranslation(Pos);
            Global.Effect.Alpha = Alpha;
            Global.Effect.Depth = Depth;
            GraphicsDevice.BlendState = BlendState;
            foreach (var p in Global.Effect.CurrentTechnique.Passes)
            {
                p.Apply();
                GraphicsDevice.DrawUserPrimitives<CascadeVertex>(PrimitiveType, Vertices, 0, PrimitiveCount);
            }
        }
    }
}
