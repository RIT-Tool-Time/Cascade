
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
namespace Cascade
{
    public class PolygonRect
    {
        public Vector2 pos;
        public VertexPositionColor[] vertices;
        ColorManager color;

        float x = 0, y = 0, z = 0, width = 0, height = 0;

        public Vector3 Origin = Vector3.Zero;
        public float Width
        {
            get
            { return width; }
        }
        public float Height
        {
            get
            { return height; }
        }
        public PolygonRect(float Width, float Height)
        {
            width = Width; height = Height;
            color = new ColorManager();
            color.Color = Color.Black;
            vertices = new VertexPositionColor[6];
            vertices[0] = new VertexPositionColor(new Vector3(0, 0, 0),  Color.Red);
            vertices[1] = new VertexPositionColor(new Vector3(Width, 0, 0), Color.Red);
            vertices[2] = new VertexPositionColor(new Vector3(Width, Height, 0), Color.Red);
            vertices[3] = new VertexPositionColor(new Vector3(Width, Height, 0), Color.Red);
            vertices[4] = new VertexPositionColor(new Vector3(0, Height, 0), Color.Red);
            vertices[5] = new VertexPositionColor(new Vector3(0, 0, 0), Color.Red);
            Origin = Vector3.Zero;
        }
        /// <summary>
        /// Places the origin at the center of the polygons
        /// </summary>
        public void CenterOrigin()
        {
            Origin = new Vector3(width / 2, height / 2, 0);
        }
        //reset Rectangle to a default state
        public static PolygonRect Default
        {
            get
            {
                return new PolygonRect(100, 100);
            }
        }
        // return positions of where the vertices would be if they were rotated
        public Vector3[] PositionsWithRotation(Vector3 rot, float scale)
        {
            var pos = new Vector3[4];
            pos[0] = Vector3.Zero;
            float l = vertices[1].Position.X;
            pos[1] = new Vector3(MyMath.LengthDirX(l, rot.Z), MyMath.LengthDirY(l, rot.Z), MyMath.LengthDirY(l, rot.Y)) * scale;
            l = vertices[4].Position.Y;
            pos[3] = new Vector3(-MyMath.LengthDirY(l, rot.Z), MyMath.LengthDirX(l, rot.Z), MyMath.LengthDirY(l, rot.X)) * scale;
            pos[2] = pos[3] + (pos[1] - pos[0]);
            return pos;
        }
    }
}
