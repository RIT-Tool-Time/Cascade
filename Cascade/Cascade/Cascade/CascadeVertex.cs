using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Cascade
{
    public struct CascadeVertex : IVertexType
    {
        public Vector3 Position;
        public Color Color;
        public static VertexDeclaration VertexDeclaration
        {
            get
            {
                return new VertexDeclaration(
                new VertexElement(0, VertexElementFormat.Vector3, VertexElementUsage.Position, 0),
                new VertexElement(12, VertexElementFormat.Color, VertexElementUsage.Color, 0)
                );
            }
        }
        public CascadeVertex(Vector3 pos, Color c)
        {
            Position = pos;
            Color = c;
        }
        VertexDeclaration IVertexType.VertexDeclaration
        {
            get { return VertexDeclaration; }
        }
    }
}
