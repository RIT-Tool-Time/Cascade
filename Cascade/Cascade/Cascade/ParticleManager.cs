using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Cascade
{
    public class ParticleManager
    {
        int primitiveCount = 0;
        List<Particle> particles;
        List<ParticleEmitter> emitters;
        int numVertices = 0;
        public ParticleManager()
        {
            particles = new List<Particle>();
            emitters = new List<ParticleEmitter>();
        }
        public int NumberofParticles
        {
            get
            {
                return particles.Count;
            }
        }
        public void Add(params Particle[] parts)
        {
            foreach (var part in parts)
            {
                particles.Add(part);
            }
        }
        public void Add(params ParticleEmitter[] parts)
        {
            foreach (var part in parts)
            {
                emitters.Add(part);
            }
        }
        public void Remove(params Particle[] parts)
        {
            foreach (var part in parts)
            {
                if (particles.Contains(part))
                {
                    particles.Remove(part);
                }
            }
        }
        public void Remove(params ParticleEmitter[] parts)
        {
            foreach (var part in parts)
            {
                if (emitters.Contains(part))
                {
                    emitters.Remove(part);
                }
            }
        }
        public virtual void Update()
        {
            for (int i = 0; i < particles.Count; i++)
            {
                particles[i].Update();
            }
            for (int i = 0; i < emitters.Count; i++)
            {
                emitters[i].Update();
            }
            Sort();
        }
        public void Sort()
        {
            for (int i = 0; i < particles.Count; i++)
            {
                if (i > 0)
                {
                    if (MyMath.Distance(Global.Camera.Pos - particles[i].Pos) > MyMath.Distance(Global.Camera.Pos - particles[i - 1].Pos))
                    {
                        var temp = particles[i - 1];
                        particles[i - 1] = particles[i];
                        particles[i] = temp;
                        i -= 2;
                    }
                }
            }
        }
        public void Draw(GraphicsDevice GraphicsDevice, GraphicsDeviceManager graphics, SpriteBatch spriteBatch, RenderTarget2D defaultRenderTarget, float width, float height)
        {
            
            //GraphicsDevice.SetVertexBuffer(new VertexBuffer(
            //GraphicsDevice.DrawIndexedPrimitives(PrimitiveType.TriangleList, 0, 0, numVertices, 0, primitiveCount);
            //GraphicsDevice.DrawUserPrimitives<CascadeVertex>(PrimitiveType.TriangleList, Vertices, 0, primitiveCount);
            foreach (var p in particles)
            {
                p.Draw(GraphicsDevice, graphics, spriteBatch, defaultRenderTarget, width, height);
            }
        }
    }
}
