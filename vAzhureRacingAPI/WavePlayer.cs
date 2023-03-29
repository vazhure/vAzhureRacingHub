using System;
using System.Windows.Media;

namespace vAzhureRacingHub
{
    public class WavePlayer
    {
        private readonly MediaPlayer m_mediaPlayer = new MediaPlayer();

        public void Play(string filename)
        {
            m_mediaPlayer.Open(new Uri(filename));
            m_mediaPlayer.Play();
        }

        public void SetVolume(int volume)
        {
            m_mediaPlayer.Volume = volume / 100.0f;
        }
    }
}
