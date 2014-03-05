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
        public Color Color
        {
            get { return color; }
            set
            {
                color = value;
                for (int i = 0; i < Vertices.Length; i++)
                {
                    Vertices[i].Color = value * alpha;
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
                for (int i = 0; i < Vertices.Length; i++)
                {
                    Vertices[i].Color = color * value;
                }
            }
        }
        public CascadeVertex[] Vertices;
        protected ParticleManager manager;
        public Matrix Matrix;
        public float Gravity = 0;

        public Particle(ParticleManager Manager, Vector3 pos)
        {
            Behaviors = new List<Cascade.Behaviors.ParticleBehavior>();
            Pos = pos;
            manager = Manager;
            manager.Add(this);
            /*int add = 20;
            vertices = new VertexPositionColor[4];
            vertices[0] = new VertexPositionColor(new Vector3(0, 0, 0), Color);
            vertices[1] = new VertexPositionColor(new Vector3(add, add, 0), Color);
            vertices[2] = new VertexPositionColor(new Vector3(-add, add, 0), Color);
            vertices[3] = new VertexPositionColor(new Vector3(0, 0, 0), Color);*/
        }
        public virtual void Remove()
        {
            manager.Remove(this);
        }
        public virtual void Update()
        {
            Speed.Y += Gravity * Global.Speed;
            Pos += Speed * Global.Speed;
            SetVertexPositions();
            Matrix = Matrix.CreateTranslation(Pos);
            foreach (var b in Behaviors)
            {
                b.Update(this);
            }
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
            Global.Effect.World = Matrix.CreateScale(new Vector3(Scale, 1)) * Matrix.CreateTranslation(Pos);
            GraphicsDevice.BlendState = BlendState;
            foreach (var p in Global.Effect.CurrentTechnique.Passes)
            {
                p.Apply();
                GraphicsDevice.DrawUserPrimitives<CascadeVertex>(PrimitiveType, Vertices, 0, PrimitiveCount);
            }
        }
    }
}
