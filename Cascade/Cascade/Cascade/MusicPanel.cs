using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using System.Timers;

namespace Cascade
{
    public class MusicPanel
    {
        Random rand = new Random();
        ColorManager clearColor;
        public int NoteOffset = 0;
        MusicalNote note = null;
        bool isBeingTouched = false;
        int numFingers = 0;
        List<int> idList = new List<int>();
        int instrument = 1;
        Timer timer;
        bool waitingForTimer = false;
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
            clearColor.Color = new Color(255,255,255);
            timer = new Timer(1000d / 32d);
            timer.Stop();
            timer.Elapsed += new ElapsedEventHandler(timer_Elapsed);
        }

        
        public void Update()
        {
            bool lastIsTouched = isBeingTouched;
            clearColor.Update();
            int index = Global.PanelManager.Panels.IndexOf(this);
            if (index != -1)
            {
                bool touchOccured = false;
                int num = 0;
                foreach (var touch in Global.Touches)
                {
                    if (touch.Position.X > (Global.ScreenSize.X / Global.PanelManager.Panels.Count) * index
                        && touch.Position.X < (Global.ScreenSize.X / Global.PanelManager.Panels.Count) * (index + 1)
                        )
                    {
                        touchOccured = true;
                        if (waitingForTimer && !idList.Contains(touch.Id))
                        {
                            idList.Add(touch.Id);
                            Global.Output += "Added " + touch.Id + " to idList";
                            
                        }
                        num++;
                    }
                }
                numFingers = num;
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
                clearColor.Animate(new Color(170, 230, 245), 10);
                timer.Stop();
                timer.Start();
                waitingForTimer = true;
            }
            else
            {
                if (note != null)
                {
                    note.FadeOut(0.04f, 0.001f);
                    //allIds.Clear();
                }
                //idList.Clear();
                //timer.Stop();
                
                numFingers = 0;
            }
        }

        void timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            timer.Stop();
            if (idList.Count >= 2)
            {
                instrument = 2;
            }
            else
            {
                instrument = 1;
            }
            Global.Output += "Number of fingers on panel: " + idList.Count;
            note = MusicManager.AddNote(MusicManager.LoadUserSounds(instrument, NoteOffset + Global.PanelManager.NoteOffset, "bip")[rand.Next(0, 3)].CreateInstance());
            if (!isBeingTouched)
            {
                note.FadeOut(0.04f, 0.001f);
            }
            idList.Clear();
            Global.Output += "idList cleared";
            waitingForTimer = false;
        }
    }
}
