using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using System.Threading;
using NAudio;
using NAudio.CoreAudioApi;
namespace Cascade
{
    public static class VolumeMeter
    {
        [DllImport("winmm.dll")]
        public static extern int waveOutGetVolume(IntPtr hwo, out uint dwVolume);

        static Thread thread;
        static ThreadStart ts;
        static bool threadRunning = false;
        static float vol = 0;
        static MMDevice device;
        public static float Volume
        {
            get
            {
                return vol;
            }
        }
        public static void Update()
        {
            if (device == null)
            {
                MMDeviceEnumerator de = new MMDeviceEnumerator();
                device = de.GetDefaultAudioEndpoint(DataFlow.Render, Role.Multimedia);
            }
            if (ts == null)
            {
                ts = new ThreadStart(getVolume);
            }
            if (!threadRunning)
            {
                thread = new Thread(ts);
                threadRunning = true;
                thread.Start();
            }
        }
        static void getVolume()
        {
            uint v = 0;
            waveOutGetVolume(IntPtr.Zero, out v);
            ushort vv = (ushort)(v & 0x0000ffff);
            vol = device.AudioMeterInformation.MasterPeakValue;
            threadRunning = false;
        }
    }
}
