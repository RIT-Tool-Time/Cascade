﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace Cascade
{
    public class Ellipse : Particle
    {
        public Ellipse(ParticleManager m, Vector3 p, int accuracy)
            :base(m, p)
        {
            Vertices = new CascadeVertex[accuracy * 3];
            PrimitiveCount = accuracy;
            int o = 0;
            for (float i = 0; i < 360; i += 360f / (float)accuracy)
            {
                Vertices[o] = new CascadeVertex(Vector3.Zero,Color.White);
                Vertices[o + 1] = new CascadeVertex(new Vector3(MyMath.LengthDirX(50, i), MyMath.LengthDirY(50, i), 0), Color.White);
                Vertices[o + 2] = new CascadeVertex(new Vector3(MyMath.LengthDirX(50, i + (360f / accuracy)), MyMath.LengthDirY(50, i + (360f / accuracy)), 0), Color.White);
                o += 3;
            }
        }
    }
    public class GradientEllipse : Ellipse
    {
        public GradientEllipse(ParticleManager man, Vector3 pos, int accuracy)
            : base(man, pos, accuracy)
        {
            for (int i = 0; i < Vertices.Length; i+= 3)
            {
                Vertices[i].Color = Color.Transparent;
            }
        }
    }
}
