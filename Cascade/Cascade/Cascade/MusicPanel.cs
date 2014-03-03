using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace Cascade
{
    public class MusicPanel
    {
        ColorManager clearColor;
        public ColorManager ColorManager
        {
            get
            {
                return clearColor;
            }
        }
        public Color Color
        {
            get
            {
                return clearColor;
            }
            set
            {
                clearColor.Color = value;
            }
        }
        public MusicPanel()
        {
            clearColor = new ColorManager();
            clearColor.Color = Color.Green;
        }
        public void Update()
        {
            clearColor.Update();
        }
    }
}
