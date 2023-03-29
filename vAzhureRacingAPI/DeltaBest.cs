using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Speech.Synthesis;

namespace vAzhureRacingAPI
{
    public class DeltaBest
    {
        private int[] best_lap_data = { 0 };
        private int[] current_lap_data = { 0 };
        private int dist_last = 0;

        /// <summary>
        /// Current Track length
        /// </summary>
        public int TrackLength { get; private set; } = 0;
        /// <summary>
        /// Current Track Name
        /// </summary>
        public string TrackName { get; private set; } = "";
        /// <summary>
        /// Current Car Name
        /// </summary>
        public string CarName { get; private set; } = "";
        /// <summary>
        /// Best Lap time in milliseconds
        /// </summary>
        public int BestLapTime { get; private set; } = 0;
        /// <summary>
        /// Last Lap time in milliseconds
        /// </summary>
        public int LastLapTime { get; private set; } = 0;

        public void Initialize()
        {
            Reset();
            CarName = TrackName = "";
            TrackLength = 0;
        }

        /// <summary>
        /// Reset best lap data
        /// </summary>
        public void Reset()
        {
            dist_last = 0;
            for (int i = 0; i < TrackLength; i++)
            {
                best_lap_data[i] = int.MaxValue;
                current_lap_data[i] = int.MaxValue;
            }
            BestLapTime = int.MaxValue;
            LastLapTime = int.MaxValue;
        }

        public bool IsStarted => dist_last > 0 || BestLapTime != int.MaxValue;

        /// <summary>
        /// Start new lap
        /// </summary>
        /// <param name="trackLength"></param>
        /// <param name="trackName"></param>
        /// <param name="carName"></param>
        public void BeginLap(int trackLength, string trackName, string carName)
        {
            if (TrackLength < trackLength || trackName != TrackName || carName != CarName)
            {
                TrackName = trackName;
                CarName = carName;
                best_lap_data = new int[(TrackLength = trackLength) + 1];
                current_lap_data = new int[(TrackLength = trackLength) + 1];
                Reset();
            }
            else
                dist_last = 0;
        }

        static DateTime tmLastSound = DateTime.Now;
        const int cMinInterval = 5000; // ms

        public void SpeakText(string text, int volume = 100)
        {
            TimeSpan ts = DateTime.Now - tmLastSound;

            if (ts.TotalMilliseconds < cMinInterval)
                return; // not 

            tmLastSound = DateTime.Now;

            Task.Run(delegate
            {
                using (var synthesizer = new SpeechSynthesizer())
                {
                    var t = synthesizer.GetInstalledVoices().Where(o => o.VoiceInfo.Gender == VoiceGender.Male).FirstOrDefault();
                    if (t is InstalledVoice v)
                        synthesizer.SelectVoice(v.VoiceInfo.Name);
                    else
                        synthesizer.SelectVoiceByHints(VoiceGender.Male, VoiceAge.Adult);
                    synthesizer.SetOutputToDefaultAudioDevice();
                    synthesizer.Rate = -3;
                    synthesizer.Volume = Math2.Clamp(volume, 0, 100); 
                    synthesizer.Speak(text);
                }
            });
        }

        public event EventHandler OnBestLap;

        /// <summary>
        /// Finish current lap time
        /// </summary>
        /// <param name="lap_time"></param>
        public void EndLap(int lap_time)
        {
            if (lap_time > 0)
            {
                // Ending last lap
                for (int t = dist_last + 1; t <= TrackLength; t++)
                    current_lap_data[t] = (int)Math2.Mapf(t, dist_last, TrackLength, current_lap_data[dist_last], lap_time);

                for (int t = 0; t <= TrackLength; t++)
                    best_lap_data[t] = Math.Min(best_lap_data[t], current_lap_data[t]);

                if (lap_time < BestLapTime)
                {
                    BestLapTime = lap_time;
                    OnBestLap?.Invoke(this, new EventArgs());
                }

                LastLapTime = lap_time;
            }

            dist_last = 0;
        }

        public bool DataValid => BestLapTime != int.MaxValue;

        /// <summary>
        /// Best lap data array
        /// </summary>
        /// <param name="index">distance in meters</param>
        /// <returns></returns>
        public int this[int index]
        {
            get => (index < 0 || index > TrackLength || !DataValid) ? 0 : best_lap_data[index];
            set => Update(index, value);
        }

        /// <summary>
        /// Update telemetry data
        /// </summary>
        /// <param name="meters">current distance, meters</param>
        /// <param name="milliseconds">current lap time, milliseconds</param>
        private void Update(int meters, int milliseconds)
        {
            if (meters >= 0 && meters <= TrackLength)
            {
                if (meters <= dist_last)
                {
                    for (int t = dist_last; t <= meters; t++)
                        current_lap_data[t] = milliseconds;
                }
                else
                {
                    for (int t = dist_last + 1; t <= meters; t++)
                        current_lap_data[t] = (int)Math2.Mapf(t, dist_last, meters, current_lap_data[dist_last], milliseconds);
                }
                dist_last = meters;
            }
        }
    }
}
