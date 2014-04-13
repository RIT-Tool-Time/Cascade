using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Audio;
using System.Diagnostics;
using Microsoft.Xna.Framework;
using System.Timers;

namespace Cascade
{
    public enum MusicalScaleType { Major, Minor, Pentatonic }

    public enum MusicState { Unknown, WaitingToStartAnalysis, Analysis, Level1, Level2, Level3 }
    public enum Tempo { T100 = 100, T120 = 120, T140 = 140 }
    public static class MusicManager
    {
        public static readonly int[] MajorScale = { 0, 2, 4, 5, 6, 9, 11, 12 };

        public static readonly int[] PentatonicScale = { 0, 2, 4, 7, 9, 12 };

        public static readonly int[,] Chords = 
        {
            { 0, 2, 4 },
            { 1, 3, 5 },
            { 2, 4, 6 },
            { 3, 5, 0 },
            { 4, 6, 1 },
            { 5, 0, 2 }
        };
        static int[] chordProgression = null;

        static Tempo tempo = Tempo.T100;

        public static List<MusicalNote> SoundEffects = new List<MusicalNote>();

        public static List<PanelManager> PanelManagers = new List<PanelManager>();

        static float timeForAnalysis = 30;

        static MusicState state = MusicState.Unknown;
        static Stopwatch stopWatch = new Stopwatch();
        static List<NoteTimeInfo> notesPlayed = new List<NoteTimeInfo>();
        static NoteTimeAnalysis[] analysis;

        static int numberOfUnits = 0;

        static int unit = 0;
        static int bar = 0;
        static int beat = 0;
        static int note = 0;
        static Timer timer =null;

        
        
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

        public static SoundEffect LoadPadSound(int padNumber, int midiNumber)
        {
            string path = "Sound Samples/PAD1/PAD_" + padNumber + "_" + midiNumber + "_bip 1";
            return Global.Game.Content.Load<SoundEffect>(path);
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
        public static void CreateChordProgression()
        {
            Random rand = new Random();
            chordProgression = new int[4];
            int lastVal = 0;
            chordProgression[0] = 0;
            for (int i = 1; i < chordProgression.Length; i++)
            {
                int val = 0;
                while (chordProgression.Contains(val))
                {
                    val = rand.Next(Chords.GetLength(0));
                }
                chordProgression[i] = val;
            }
            string s = "";
            foreach (var c in chordProgression)
            {
                s += c + ", ";
            }
            Global.Output += "Chord progression: " + s;
        }
        public static void NotePlayedByUser(int midiNumber, int playerIndex, int instrumentNumber)
        {
            if (state == MusicState.WaitingToStartAnalysis)
            {
                SetState(MusicState.Analysis);
                stopWatch.Restart();
                notesPlayed.Clear();

                timeForAnalysis = MyMath.RandomRange(20, 30);
                Global.Output += "First note played, running analysis for " + timeForAnalysis + " seconds";
                var pad1 = LoadPadSound(0, midiNumber + MajorScale[Chords[3, 0]]);
                var pad2 = LoadPadSound(0, midiNumber + MajorScale[Chords[3, 2]]);
                var n1 = AddNote(new QuaveringVolumeNote(pad1.CreateInstance(), MyMath.RandomRange(0.01f, 0.03f), MyMath.RandomRange(0.5f, 0.2f)));
                var n2 = AddNote(new QuaveringVolumeNote(pad2.CreateInstance(), MyMath.RandomRange(0.01f, 0.03f), MyMath.RandomRange(0.5f, 0.2f)));

                n1.Volume = n2.Volume = 0.25f;
                //pad1.Play();
                //pad2.Play();
            }
            if (state == MusicState.Analysis)
            {
                //Global.Output += "Adding note for analysis of player " + (playerIndex + 1) + ": " + stopWatch.Elapsed;
                notesPlayed.Add(new NoteTimeInfo(midiNumber, instrumentNumber, playerIndex, stopWatch.Elapsed));
            }
        }

        public static MusicalNote AddNote(SoundEffectInstance i)
        {
            
            var m = new MusicalNote(i);
            SoundEffects.Add(m);
            return m;
        }
        public static MusicalNote AddNote(MusicalNote m)
        {
            SoundEffects.Add(m);
            return m;
        }
        public static void Update()
        {
            if (timer == null)
            {
                timer = new Timer();
                timer.Elapsed += new ElapsedEventHandler(timer_Elapsed);
            }
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
                    //Global.Output += "Removed sound";
                    i--;
                }
            }
            if (state == MusicState.Analysis)
            {
                if (stopWatch.Elapsed.TotalSeconds >= timeForAnalysis)
                {
                    stopWatch.Stop();
                    stopWatch.Reset();
                    analysis = new NoteTimeAnalysis[PanelManagers.Count];
                    for (int i = 0; i < PanelManagers.Count; i++)
                    {
                        analysis[i] = NoteTimeAnalysis.Analyze(i, notesPlayed, timeForAnalysis);
                        Global.Output += "Analysis for player " + (i + 1) + " - " + analysis[i];
                    }
                    CreateChordProgression();
                    SetState(MusicState.Level1);
                }
            }
        }

        static void timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            note++;
            NoteIncremented();
            if (note % 4 == 0)
            {
                beat++;
                BeatIncremented();
                Global.Output += "Beat " + beat;
                if (beat % 4 == 0)
                {
                    bar++;
                    BarIncremented();
                    Global.Output += "Bar " + bar;
                    if (bar % 4 == 0)
                    {
                        unit++;
                        UnitIncremented();
                        Global.Output += "Unit " + unit;
                    }
                }
            }
        }
        static void NoteIncremented()
        {

        }
        static void BeatIncremented()
        {

        }
        static void BarIncremented()
        {

        }
        static void UnitIncremented()
        {
            if (unit >= numberOfUnits)
            {
                if (state == MusicState.Level1)
                {
                    SetState(MusicState.Level2);
                }
                else if (state == MusicState.Level2)
                {
                    SetState(MusicState.Level3);
                }
            }
        }
        public static void SetState(MusicState s)
        {
            if (s != state)
            {
                Global.Output += "State being set to " + s;
                switch (s)
                {
                        
                    case MusicState.Analysis:

                        break;
                    case MusicState.Level1:
                        if (analysis != null && analysis.Length > 0)
                        {
                            float bpm = 0;
                            for (int i = 0; i < analysis.Length; i++)
                            {
                                bpm += analysis[0].BPM;
                            }
                            bpm /= analysis.Length;
                            Global.Output += "Average BPM of all players: " + bpm;
                            tempo = Tempo.T100;
                            SetTimer(s, tempo);
                        }
                        break;
                    case MusicState.Level2:
                        SetTimer(s, tempo);
                        break;
                    case MusicState.Level3:
                        SetTimer(s, tempo);
                        break;
                }
                state = s;
                
            }
        }
        public static void SetTimer(MusicState s, Tempo t)
        {
            Random r = new Random();
            switch (s)
            {
                case MusicState.Level1:
                    if (t == Tempo.T140)
                    {
                        numberOfUnits = 3;
                    }
                    else
                    {
                        numberOfUnits = 2;
                    }
                    break;
                case MusicState.Level2:
                    if (t == Tempo.T100)
                    {
                        numberOfUnits = r.Next(6, 8);
                    }
                    else if (t == Tempo.T120)
                    {
                        numberOfUnits = r.Next(7, 10);
                    }
                    else if (t == Tempo.T140)
                    {
                        numberOfUnits = r.Next(9, 12);
                    }
                    break;
                case MusicState.Level3:
                    if (t == Tempo.T100)
                    {
                        numberOfUnits = r.Next(4, 6);
                    }
                    else if (t == Tempo.T120)
                    {
                        numberOfUnits = r.Next(4, 7);
                    }
                    else if (t == Tempo.T140)
                    {
                        numberOfUnits = r.Next(5, 8);
                    }
                    break;
            }
            note = beat = bar = unit = 0;
            timer.Interval = (60d / (int)tempo) * 1000d / 4d;
            timer.Stop();
            timer.Start();
        }
    }
    
    public class MusicalNote
    {
        bool fade = false, fadeIn = true;
        float fadeSpeed = 0.01f;
        float fadeSpeed2 = 0.01f;
        SoundEffectInstance s;
        float volume = 1, fadeVolume = 1;
        public float Volume
        {
            get
            {
                return GetVolume();
            }
            set
            {
                volume = value;
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
            fadeVolume = 0;
            fadeIn = true;
            s.Play();
            
        }
       
        public void FadeOut(float fadeSpeed, float fadeSpeed2)
        {
            this.fadeSpeed = fadeSpeed;
            this.fadeSpeed2 = fadeSpeed2;
            fade = true;
        }
        public virtual void Update()
        {
            if (fade && !fadeIn)
            {
                float vol = fadeVolume - (fadeVolume > 0.1f ? fadeSpeed : fadeSpeed2);
                if (vol > 0)
                {
                    fadeVolume = vol;
                }
                else
                {
                    s.Stop();

                }
            }
            else if (fadeIn)
            {
                float vol = fadeVolume + 0.1f;
                if (vol < 1)
                {
                    fadeVolume = vol;
                }
                else 
                {
                    fadeVolume = 1;
                    fadeIn = false;
                }
            }
            s.Volume = MathHelper.Clamp(GetVolume(), 0, 1);
            
        }
        protected virtual float GetVolume()
        {
            return volume * fadeVolume;
        }
    }
    public class QuaveringVolumeNote : MusicalNote
    {
        float range, speed;
        float vol = 1;
        public QuaveringVolumeNote(SoundEffectInstance i, float speed, float range)
            :base(i)
        {
            this.speed = speed;
            this.range = range;
        }
        public override void Update()
        {
            base.Update();
            if (vol < 0 && speed < 0)
            {
                speed *= -1;
            }
            else if (vol > 1 && speed > 0)
            {
                speed *= -1;
            }
            vol += speed;
            //Global.Output += GetVolume();
        }
        protected override float GetVolume()
        {
            return base.GetVolume() * MyMath.Between(1 + range, 1 - range, vol);
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
