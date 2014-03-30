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
        Random rand = new Random();
        ColorManager clearColor;
        public int NoteOffset = 0;
        MusicalNote note = null;
        bool isBeingTouched = false;
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
            clearColor.Color = new Color(150,150,150);
        }
        public void Update()
        {
            bool lastIsTouched = isBeingTouched;
            clearColor.Update();
            int index = Global.PanelManager.Panels.IndexOf(this);
            if (index != -1)
            {
                bool touchOccured = false;
                foreach (var touch in Global.Touches)
                {
                    if (touch.Position.X > (Global.ScreenSize.X / Global.PanelManager.Panels.Count) * index
                        && touch.Position.X < (Global.ScreenSize.X / Global.PanelManager.Panels.Count) * (index + 1)
                        )
                    {
                        touchOccured = true;
                    }
                }
                isBeingTouched = touchOccured;
            }
            if (isBeingTouched != lastIsTouched)
            {
                IsBeingTouchedChanged(isBeingTouched);
            }
        }
        void IsBeingTouchedChanged(bool touched)
        {
            if (touched)
            {
                clearColor.Animate(Color.White, 10);
                note = MusicManager.AddNote(MusicManager.LoadUserSounds(1, NoteOffset + Global.PanelManager.NoteOffset, "bip")[rand.Next(0, 3)].CreateInstance());
            }
            else
            {
                if (note != null)
                {
                    note.FadeOut(0.04f, 0.001f);
                }
            }
        }
    }
}
