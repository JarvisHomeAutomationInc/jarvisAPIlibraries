using NAudio.Wave;
using NAudio.Lame;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PAD_Console_2024.util
{
    /// <summary>
    /// single functions that are used in a static way around PAD's code
    /// </summary>
    public class Speaker
    {
        /// <summary>
        /// play an audio file, usually from a tmp file in the tmp folder from TTS api
        /// </summary>
        /// <param name="fileName">filename to play audio</param>
        public static void PlayAudio(string fileName)
        {
            using (var audioFile = new AudioFileReader(fileName))
            using (var outputDevice = new WaveOutEvent())
            {
                outputDevice.Init(audioFile);
                outputDevice.Play();

                while (outputDevice.PlaybackState == PlaybackState.Playing)
                {
                    System.Threading.Thread.Sleep(1000);
                }
            }
        }

        /// <summary>
        /// save byte data to wav file
        /// </summary>
        /// <param name="audioData">byte data</param>
        /// <param name="fileName">wav file name</param>
        public static void SaveWavFile(byte[] audioData, string fileName)
        {
            using (var memStream = new MemoryStream(audioData))
            {
                using (var reader = new RawSourceWaveStream(memStream, new WaveFormat()))
                {
                    WaveFileWriter.CreateWaveFile(fileName, reader);
                }
            }
        }

        /// <summary>
        /// convert byte data to mp3 file
        /// </summary>
        /// <param name="pcmData">byte data</param>
        /// <param name="sampleRate">rate</param>
        /// <param name="channels">mono or more</param>
        /// <param name="mp3FileName">filename for mp3</param>
        public static void SavePcmToMp3(byte[] pcmData, int sampleRate, int channels, string mp3FileName)
        {
            // Create a WaveFormat for the PCM data
            var waveFormat = new WaveFormat(sampleRate, channels);

            // Create a wave stream for the PCM data
            using var pcmStream = new RawSourceWaveStream(new MemoryStream(pcmData), waveFormat);

            // Create an MP3 file
            using var mp3File = new LameMP3FileWriter(mp3FileName, waveFormat, LAMEPreset.ABR_128);

            // Copy the PCM data to the MP3 file
            pcmStream.CopyTo(mp3File);
        }
    }

   
}
