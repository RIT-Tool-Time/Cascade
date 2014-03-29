using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Audio;

namespace Cascade
{
    public class PanelManager
    {
        List<MusicPanel> panels;
        public List<MusicPanel> Panels
        {
            get
            {
                return panels;
            }
        }
        public int NoteOffset = 0;
        public MusicPanel this[int i]
        {
            get 
            {
                if (i >= panels.Count) i = panels.Count - 1;
                if (i < 0) i = 0;
                return panels[i];
            }
        }
        public PanelManager()
        {
            panels = new List<MusicPanel>();
        }
        public void Update()
        {
            foreach (var panel in panels)
            {
                panel.Update();
            }
        }
        public MusicPanel Add(Color baseColor)
        {
            MusicPanel mp = new MusicPanel();
            panels.Add(mp);
            //mp.Color = baseColor;
            return mp;
        }
        public MusicPanel Add(MusicPanel mp)
        {
            panels.Add(mp);
            return mp;
        }
        public void Draw(GraphicsDevice GraphicsDevice, GraphicsDeviceManager graphics, SpriteBatch spriteBatch, RenderTarget2D defaultRenderTarget, int width, int height)
        {
            int w = width; int h = height;
            int add = 10;

            float panelWidth = ((w - add) / (float)panels.Count) - (add);

            float x = 0;
            for (int i = 0; i < panels.Count; i++)
            {
                x += add;
                var r = new PolygonRect(panelWidth, h);
                for (int o = 0; o < r.vertices.Length; o++)
                {
                    r.vertices[o].Position.X += x;
                    r.vertices[o].Color = panels[i].Color;
                }
                x += panelWidth;
                GraphicsDevice.DrawUserPrimitives<CascadeVertex>(PrimitiveType.TriangleList, r.vertices, 0, 2);
            }
        }
    }
}
