using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
namespace Cascade
{
    public static class MyMath
    {
        static Random random = new Random();
        public static Color Between(Color col1, Color col2, float val)
        {
            Vector4 v1 = col1.ToVector4(), v2 = col2.ToVector4();
            return new Color(v1 + ((v2 - v1) * val));
        }
        public static float Between(float num1, float num2, float val)
        {
            return num1 + ((num2 - num1) * val);
        }

        public static Vector2 Between(Vector2 num1, Vector2 num2, float val)
        {
            return num1 + ((num2 - num1) * val);
        }
        public static float BetweenValue(float val1, float val2, float between)
        {
            return (between - val1) / (val2 - val1);
        }
        public static float RandomRange(float num1, float num2)
        {
            return num1 + ((num2 - num1) * (float)random.NextDouble());
        }
        public static Vector3 RandomRange(Vector3 vec1, Vector3 vec2)
        {
            return new Vector3(RandomRange(vec1.X, vec2.X), RandomRange(vec1.Y, vec2.Y), RandomRange(vec1.Z, vec2.Z));
        }
        public static float Random()
        {
            return (float)random.NextDouble();
        }
        public static float Direction(float x1, float y1, float x2, float y2)
        {
            return (float)Math.Atan2(y1 - y2, x2 - x1);
        }
        public static float Direction(Vector2 vec1, Vector2 vec2)
        {
            return (float)Direction(vec1.X, vec1.Y, vec2.X, vec2.Y);
        }
        public static float AngleDistance(Vector2 vec1, Vector2 vec2, float target)
        {
            float dir = MathHelper.ToDegrees(Direction(vec1, vec2));
            float d1 = Math.Abs(target - dir);
            float d2 = Math.Abs(target - (dir + 360));
            float d3 = Math.Abs(target - (dir - 360));
            return Math.Min(Math.Min(d1, d2), d3);
        }
        public static Vector2 Direction(Vector3 vec1, Vector3 vec2)
        {
            return new Vector2(Direction(vec1.Z, vec1.X, vec2.Z, vec2.X), Direction(vec1.Z, vec1.Y, vec2.Z, vec2.Y));
        }
        public static float Distance(Vector3 pos1, Vector3 pos2)
        {
            Vector3 pos3 = pos1 - pos2;
            return (float)Math.Sqrt((pos3.X * pos3.X) + (pos3.Y * pos3.Y) + (pos3.Z * pos3.Z));
        }
        public static float Distance(Vector3 pos)
        {
            return (float)Math.Sqrt((pos.X * pos.X) + (pos.Y * pos.Y) + (pos.Z * pos.Z));
        }
        public static float LengthDirX(float Length, float Direction)
        {
            return (float)Math.Cos(MathHelper.ToRadians(Direction)) * Length;
        }
        public static float LengthDirY(float Length, float Direction)
        {
            return -(float)Math.Sin(MathHelper.ToRadians(Direction)) * Length;
        }
        public static Vector2 LengthDir(float Length, float Direction)
        {
            return new Vector2(LengthDirX(Length, Direction), LengthDirY(Length, Direction));
        }
        public static Vector2 RandomVectorRange(this Vector2 baseVector, Vector2 rangeVector)
        {
            return baseVector + new Vector2(MyMath.RandomRange(rangeVector.X, -rangeVector.X), MyMath.RandomRange(rangeVector.Y, -rangeVector.Y));
        }
        public static Vector3 RandomVectorRange(this Vector3 baseVector, Vector3 rangeVector)
        {
            return baseVector + new Vector3(MyMath.RandomRange(rangeVector.X, -rangeVector.X), MyMath.RandomRange(rangeVector.Y, -rangeVector.Y), MyMath.RandomRange(rangeVector.Z, -rangeVector.Z));
        }
        public static Vector4 RandomVectorRange(this Vector4 baseVector, Vector4 rangeVector)
        {
            return baseVector + new Vector4(MyMath.RandomRange(rangeVector.X, -rangeVector.X), MyMath.RandomRange(rangeVector.Y, -rangeVector.Y), MyMath.RandomRange(rangeVector.Z, -rangeVector.Z), MyMath.RandomRange(rangeVector.W, -rangeVector.W));
        }

    }
}
