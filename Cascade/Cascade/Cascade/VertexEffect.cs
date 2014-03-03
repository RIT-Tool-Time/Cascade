using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace Cascade
{
    public class VertexEffect : Effect
    {
        Matrix view, projection, world;
        public Matrix View
        {
            get
            {
                return view;
            }
            set
            {
                if (view != value)
                {
                    view = value;
                    Parameters["View"].SetValue(value);
                }
            }
        }
        public Matrix Projection
        {
            get
            {
                return projection;
            }
            set
            {
                if (projection != value)
                {
                    projection = value;
                    Parameters["Projection"].SetValue(value);
                }
            }
        }
        public Matrix World
        {
            get
            {
                return world;
            }
            set
            {
                world = value;
                Parameters["World"].SetValue(value);
            }
        }
        public VertexEffect()
            : base(Global.Game.Content.Load<Effect>("Shader"))
        {
            CurrentTechnique = Techniques["Normal"];
        }
    }
}
