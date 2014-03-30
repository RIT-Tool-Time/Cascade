using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Audio;

namespace Cascade
{
    public static class MusicManager
    {
        public static readonly int[] MajorScale = { 0, 2, 4, 5, 6, 9, 11, 12 };
        public static readonly int[] PentatonicScale = { 0, 2, 4, 7, 9, 12 };
        public static List<MusicalNote> SoundEffects = new List<MusicalNote>();
        
        public static SoundEffect LoadUserSound(int UserNumber, int MidiNumber, int Version, string InstrumentName)
        {
            try
            {
                string path = "Sound Samples/USER" + UserNumber + "/USER" + UserNumber + "_" + MidiNumber + "_V" + Version + "_" + InstrumentName;

                return Global.Game.Content.Load<SoundEffect>(path);
            }
            catch
            {
                return null;
            }
        }

        public static SoundEffect[] LoadUserSounds(int UserNumber, int MidiNumber, string InstrumentName)
        {
            SoundEffect[] effects = new SoundEffect[3];

            for (int i = 0; i < 3; i++)
            {
                effects[i] = LoadUserSound(UserNumber, MidiNumber, i + 1, InstrumentName);
            }

            return effects;
        }
        public static MusicalNote AddNote(SoundEffectInstance i)
        {
            var m = new MusicalNote(i);
            SoundEffects.Add(m);
            return m;
        }
        public static void Update()
        {
            for (int i = 0; i < SoundEffects.Count; i++)
            {
                SoundEffects[i].Update();
                if (SoundEffects[i].State == SoundState.Stopped)
                {
                    SoundEffects.RemoveAt(i);
                    i--;
                }
            }
        }
    }
    public class MusicalNote
    {
        bool fade = false, fadeIn = true;
        float fadeSpeed = 0.01f;
        float fadeSpeed2 = 0.01f;
        SoundEffectInstance s;
        public float Volume
        {
            get
            {
                return s.Volume;
            }
            set
            {
                s.Volume = value;
            }
        }
        public SoundState State
        {
            get
            {
                return s.State;
            }
        }
        public MusicalNote(SoundEffectInstance instance)
        {
            s = instance;
            s.Volume = 0;
            fadeIn = true;
            s.Play();
        }
       
        public void FadeOut(float fadeSpeed, float fadeSpeed2)
        {
            this.fadeSpeed = fadeSpeed;
            this.fadeSpeed2 = fadeSpeed2;
            fade = true;
        }
        public void Update()
        {
            if (fade && !fadeIn)
            {
                float vol = Volume - (Volume > 0.1f ? fadeSpeed : fadeSpeed2);
                if (vol > 0)
                {
                    s.Volume = vol;
                }
                else
                {
                    s.Stop();

                }
            }
            else if (fadeIn)
            {
                float vol = Volume + 0.1f;
                if (vol < 1)
                {
                    s.Volume = vol;
                }
                else 
                {
                    s.Volume = 1;
                    fadeIn = false;
                }
            }
        }
    }
}
