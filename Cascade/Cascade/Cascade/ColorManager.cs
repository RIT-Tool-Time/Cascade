using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace Cascade
{
    public class ColorManager
    {
        Color col = Color.Black;
        Color aniColor = Color.White;
        Color aniColorTarget = Color.White;
        int animationState = 0;
        bool animating = false;
        int animationFrames = 0;
        int frame = 0;
        public byte R
        {
            get
            {
                return Color.R;
            }
            set
            {
                col.R = value;
            }
        }
        public byte G
        {
            get
            {
                return Color.G;
            }
            set
            {
                col.G = value;
            }
        }

        public byte B
        {
            get
            {
                return Color.B;
            }
            set
            {
                col.B = value;
            }
        }

        public byte A
        {
            get
            {
                return Color.A;
            }
            set
            {
                col.A = value;
            }
        }

        public Color Color
        {
            set
            {
                col = value;
            }
            get
            {
                if (animating)
                    return aniColor;
                else return col;
            }
        }
        public ColorManager()
        {

        }
        public virtual void Update()
        {
            if (animating)
            {
                if (animationState == 0)
                {
                    aniColor = MyMath.Between(col, aniColorTarget, (float)frame / (float)animationFrames);
                    frame++;
                    if (frame > animationFrames)
                    {
                        animationState = 1;
                    }
                }
                else if (animationState == 1)
                {
                    aniColor = MyMath.Between(col, aniColorTarget, (float)frame / (float)animationFrames);
                    frame--;
                    if (frame < 0)
                    {
                        animating = false;
                    }
                }
            }
        }
        public virtual void Animate(Color target, int frames)
        {
            animationFrames = frames;
            animationState = 0;
            animating = true;
            aniColorTarget = target;
        }

        public static implicit operator Color(ColorManager c)
        {
            return c.Color;
        }
    }
}
