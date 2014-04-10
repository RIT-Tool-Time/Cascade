using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Audio;
using System.Diagnostics;

namespace Cascade
{
    public enum MusicalScaleType { Major, Minor, Pentatonic }
    public enum MusicState { Unknown, WaitingToStartAnalysis, Analysis, Level1, Level2, Level3 }
    public static class MusicManager
    {
        public static readonly int[] MajorScale = { 0, 2, 4, 5, 6, 9, 11, 12 };

        public static readonly int[] PentatonicScale = { 0, 2, 4, 7, 9, 12 };

        public static List<MusicalNote> SoundEffects = new List<MusicalNote>();

        public static List<PanelManager> PanelManagers = new List<PanelManager>();

        static float timeForAnalysis = 30;

        static MusicState state = MusicState.Unknown;
        static Stopwatch stopWatch = new Stopwatch();
        static List<NoteTimeInfo> notesPlayed = new List<NoteTimeInfo>();
        static NoteTimeAnalysis[] analysis;
        
        public static SoundEffect LoadUserSound(int UserNumber, int MidiNumber, int Version, string InstrumentName)
        {
            try
            {
                string path = "Sound Samples/USER" + UserNumber + "/USER" + "_V" + Version + "_" + MidiNumber + "_" + InstrumentName + " 1";

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

        public static PanelManager AddPanelManager(MusicalScaleType musicalScaleType)
        {
            int[] array;
            switch (musicalScaleType)
            {
                default:
                    array = PentatonicScale;
                    break;
            }
            PanelManager pm = new PanelManager();
            for (int i = 0; i < array.Length; i++)
            {
                pm.Add(new MusicPanel(pm)
                    {
                        NoteOffset = array[i]
                    });
            }
            PanelManagers.Add(pm);
            return pm;
        }
        public static void StartAnalysis()
        {
            Global.Output += "Waiting to start analysis";
            state = MusicState.WaitingToStartAnalysis;
        }
        public static void NotePlayedByUser(int midiNumber, int playerIndex, int instrumentNumber)
        {
            if (state == MusicState.WaitingToStartAnalysis)
            {
                state = MusicState.Analysis;
                stopWatch.Restart();
                notesPlayed.Clear();
                Global.Output += "Fist note played, starting analysis";
                timeForAnalysis = MyMath.RandomRange(20, 30);
            }
            if (state == MusicState.Analysis)
            {
                Global.Output += "Adding note for analysis of player " + (playerIndex + 1) + ": " + stopWatch.Elapsed;
                notesPlayed.Add(new NoteTimeInfo(midiNumber, instrumentNumber, playerIndex, stopWatch.Elapsed));
            }
        }

        public static MusicalNote AddNote(SoundEffectInstance i)
        {
            
            var m = new MusicalNote(i);
            SoundEffects.Add(m);
            return m;
        }
        public static void Update()
        {
            foreach (var p in PanelManagers)
            {
                p.Update();
            }
            for (int i = 0; i < SoundEffects.Count; i++)
            {
                SoundEffects[i].Update();
                if (SoundEffects[i].State == SoundState.Stopped)
                {
                    SoundEffects.RemoveAt(i);
                    i--;
                }
            }
            if (state == MusicState.Analysis)
            {
                if (stopWatch.Elapsed.TotalSeconds >= timeForAnalysis)
                {
                    stopWatch.Stop();
                    stopWatch.Reset();
                    state = MusicState.Level1;
                    analysis = new NoteTimeAnalysis[PanelManagers.Count];
                    for (int i = 0; i < PanelManagers.Count; i++)
                    {
                        analysis[i] = NoteTimeAnalysis.Analyze(i, notesPlayed, timeForAnalysis);
                        Global.Output += "Analysis for player " + (i + 1) + " - " + analysis[i];
                    }
                    
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
    public struct NoteTimeInfo
    {
        public int MidiNumber;
        public TimeSpan Time;
        public int PlayerIndex;
        public int InstrumentNumber;
        public NoteTimeInfo(int midiNumber, int instrumentNumber, int playerIndex, TimeSpan time)
        {
            MidiNumber = midiNumber;
            Time = time;
            PlayerIndex = playerIndex;
            InstrumentNumber = instrumentNumber;
        }
    }
    public struct NoteTimeAnalysis
    {
        public float BPM;
        public NoteTimeAnalysis(float bpm)
        {
            BPM = bpm;
        }
        public static NoteTimeAnalysis Analyze(int playerIndex, List<NoteTimeInfo> info, float numberOfSeconds )
        {
            double lastTime = 0;
            double addedTime = 0;
            int num = 0;
           // Global.Output += "Analyzing for player " + (playerIndex + 1) + "...";
            foreach (var i in info)
            {
                //Global.Output += i.PlayerIndex;
                if (i.PlayerIndex == playerIndex)
                {
                    addedTime += i.Time.TotalSeconds - lastTime;
                    lastTime = i.Time.TotalSeconds;
                    num++;
                }
            }
            Global.Output += "Analysis complete: Time: " + numberOfSeconds + ", AddedTime: " + addedTime + ", Number: " + num;
            float averageNoteTime = (float)(addedTime / num);
            Global.Output += "Average time to next note: " + averageNoteTime;
            float averageNotesPerMin = num * (60f / numberOfSeconds);
            Global.Output += "Average notes per min: " + averageNotesPerMin;

            float averageBPMFromNumberOfNotes = averageNotesPerMin / 4;

            Global.Output += "Average BPM from number of notes: " + averageBPMFromNumberOfNotes;

            float averageBPMFromTimes = (60 / averageNoteTime) / 4;

            Global.Output += "Average BPM from time to next note: " + averageBPMFromTimes;

            return new NoteTimeAnalysis(averageBPMFromNumberOfNotes);
        }
        public override string ToString()
        {
            return "BPM: " + BPM;
        }
    }
}
