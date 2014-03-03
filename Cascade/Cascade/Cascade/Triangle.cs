using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace Cascade
{
    class Triangle : Particle
    {
        public Triangle(ParticleManager manager, Vector3 pos)
            :base(manager, pos)
        {
            int add = 50;
            Vertices = new CascadeVertex[3];
            Vertices[0] = new CascadeVertex(new Vector3(0, 0, 0), Color);
            Vertices[1] = new CascadeVertex(new Vector3(add, add, 0), Color);
            Vertices[2] = new CascadeVertex(new Vector3(-add, add, 0), Color);
            PrimitiveCount = 1;
            //Vertices[3] = new VertexPositionColor(new Vector3(0, 0, 0), Color);
        }
    }
}
