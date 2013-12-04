using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.Timers;
//must have the naudio.dll reference to build this file!
using NAudio;
using NAudio.Wave;
using NAudio.Wave.SampleProviders;

namespace SunsetHigh
{
    /// <summary>
    /// Static class for managing all the background music for the game, using the NAudio library. Currently supports playing, pausing,
    /// resuming, stopping, fading in and out, and looping. Fading effects are recommended when changing screens/songs. When providing
    /// song file names, please specify the file extension (it's not necessary, but recommended).
    /// </summary>
    public static class BGMusic
    {
        public const float FULL_VOLUME = 1.0f;
        public const float SILENCE = 0.0f;

        private const double DEFAULT_FADE_TIME = 2000;          // time to fade out = 2 seconds 
        private const double DEFAULT_LAG_BETWEEN_SONGS = 1000;  // time between songs (EXCLUDES FADE OUT TIME) = 1 second

        private static IWavePlayer wavePlayer;
        private static AudioFileReader file;
        private static FadeInOutSampleProviderAdapted fadeInOut;
        private static SampleToWaveProvider sampleToWave;
        private static bool playing = false;
        private static bool paused = false;
        private static Timer transitionTimer;
        private static string queuedSongName;
        private static double queuedFadeTime;

        /// <summary>
        /// Begins playing a song with the specified file name. Only .mp3, .wma, and .m4a are supported.
        /// </summary>
        /// <param name="fileName">File name of the song in the "Content" directory</param>
        public static void playSong(string fileName)
        {
            if (playing)
                stopSong(); //cut off current song

            if (fileName.StartsWith(Directories.MUSIC))
                fileName = fileName.Substring(Directories.MUSIC.Length);

            if (!tryAllFileTypes(fileName))
            {
                throw new System.IO.FileNotFoundException("Could not find the music file \"" + fileName + "\" in the \"Music\" directory"
                    + " in Content.\nMake sure the file is .mp3, .m4a, or .wma, and \"Copy to output directory\" settings"
                    + " are set to \"Copy if newer\".");
            }

            if (wavePlayer == null)
                wavePlayer = new WaveOutEvent(); //initialize wavePlayer

            fadeInOut = new FadeInOutSampleProviderAdapted(file);
            sampleToWave = new SampleToWaveProvider(fadeInOut);
            wavePlayer.Init(sampleToWave);

            if (!paused)
            {
                wavePlayer.Play();
                playing = true;
            }
        }

        /// <summary>
        /// Fades out the current song and starts playing the new song after a short lag
        /// </summary>
        /// <param name="fileName">File name of the new song to play in the "Content" folder</param>
        /// <param name="fadeTime">Time to fade out, in milliseconds</param>
        /// <param name="lagTime">Lag time between the two songs (excluding fade time), in milliseconds</param>
        public static void transitionToSong(string fileName, double fadeTime, double lagTime)
        {
            queuedSongName = fileName;
            fadeOut(fadeTime);
            transitionTimer = new Timer(fadeTime + lagTime);
            transitionTimer.Elapsed += new ElapsedEventHandler(OnFadeOver);
            transitionTimer.Start();
        }

        /// <summary>
        /// Fades out the current song and starts playing the new song after a short lag.
        /// Default fade time and lag times are used.
        /// </summary>
        /// <param name="fileName">File name of the new song to play in the "Content" folder</param>
        public static void transitionToSong(string fileName)
        {
            transitionToSong(fileName, DEFAULT_FADE_TIME, DEFAULT_LAG_BETWEEN_SONGS);
        }

        /// <summary>
        /// Fades out the current song and starts fading into the new song after a short lag
        /// </summary>
        /// <param name="fileName">File name of the new song to play in the "Content" folder</param>
        /// <param name="fadeTime">Time to fade out and fade in (each), in milliseconds</param>
        /// <param name="lagTime">Lag time between the two songs (excluding fade time), in milliseconds</param>
        public static void transitionToSongWithFadeIn(string fileName, double fadeTime, double lagTime)
        {
            if (lagTime < fadeTime)
                return;    //don't try this yet
            queuedSongName = fileName;
            queuedFadeTime = fadeTime;
            fadeOut(fadeTime);
            transitionTimer = new Timer(fadeTime + lagTime);
            transitionTimer.Elapsed += new ElapsedEventHandler(OnFadeOverFadeIn);
            transitionTimer.Start();
        }

        /// <summary>
        /// Fades out the current song and starts fading into the new song after a short lag.
        /// Default fade and lag times are used.
        /// </summary>
        /// <param name="fileName">File name of the new song to play in the "Content" folder</param>
        public static void transitionToSongWithFadeIn(string fileName)
        {
            transitionToSongWithFadeIn(fileName, DEFAULT_FADE_TIME, DEFAULT_LAG_BETWEEN_SONGS);
        }

        /// <summary>
        /// Stops song and disposes of it (cannot be resumed afterward)
        /// </summary>
        public static void stopSong()
        {
            if (wavePlayer != null)
            {
                wavePlayer.Stop();
                if (file != null)
                {
                    file.Dispose();
                    file = null;
                }
                playing = false;
            }
        }

        /// <summary>
        /// Disposes of all resources contained in BGMusic. Call this method at the 
        /// end of the Game's life.
        /// </summary>
        public static void dispose()
        {
            if (file != null)
            {
                file.Dispose();
                file = null;
            }
            if (wavePlayer != null)
            {
                wavePlayer.Dispose();
                wavePlayer = null;
            }
            fadeInOut = null;
        }

        /// <summary>
        /// Changes the volume of playback; BGMusic.SILENCE can be used for muting
        /// the song, while BGMusic.FULL_VOLUME can unmute.
        /// </summary>
        /// <param name="volume"></param>
        public static void setVolume(float volume)
        {
            if (file != null)
                file.Volume = volume;
        }

        /// <summary>
        /// Song starts from silence and fades in to full volume
        /// </summary>
        /// <param name="fadeTime">Time to fade in, in milliseconds</param>
        public static void fadeIn(double fadeTime)
        {
            if (fadeInOut != null)
                fadeInOut.BeginFadeIn(fadeTime);
        }
        /// <summary>
        /// Song starts from silence and fades in to full volume. Default fade in time.
        /// </summary>
        public static void fadeIn()
        {
            fadeIn(DEFAULT_FADE_TIME);
        }

        /// <summary>
        /// Song starts from full volume and fades out to silence
        /// </summary>
        /// <param name="fadeTime">Time to fade out, in milliseconds</param>
        public static void fadeOut(double fadeTime)
        {
            if (fadeInOut != null)
                fadeInOut.BeginFadeOut(fadeTime);
        }
        /// <summary>
        /// Song starts from full volume and fades out to silence. Default fade out time.
        /// </summary>
        public static void fadeOut()
        {
            fadeOut(DEFAULT_FADE_TIME);
        }

        /// <summary>
        /// Sets whether a song should loop upon ending (true by default)
        /// </summary>
        /// <param name="looping">True if the song should loop, false otherwise</param>
        public static void setLooping(bool looping)
        {
            if (fadeInOut != null)
                fadeInOut.setLooping(looping);
        }
        /// <summary>
        /// Specifies whether a song is looping (true by default)
        /// </summary>
        /// <returns>True if the song is looping, false if not</returns>
        public static bool isLooping()
        {
            if (fadeInOut != null)
                return fadeInOut.isLooping();
            return false;
        }

        /// <summary>
        /// Sets whether the soundtrack is paused or not
        /// </summary>
        /// <param name="paused"></param>
        public static void setPaused(bool pause)
        {
            if (wavePlayer != null && file != null)
            {
                if (pause)
                {
                    wavePlayer.Pause();
                    playing = false;
                }
                else
                {
                    wavePlayer.Play();
                    playing = true;
                }
                paused = pause;
            }
        }

        /// <summary>
        /// Specifies whether the soundtrack is paused (false if is playing)
        /// </summary>
        /// <returns></returns>
        public static bool isPaused()
        {
            return paused;
        }

        private static bool tryAllFileTypes(string fileName)
        {
            string filetry = fileName;
            if (System.IO.File.Exists(Directories.MUSIC + fileName))
            {
                file = new AudioFileReader(Directories.MUSIC + fileName);
                return true;
            }
            if (System.IO.File.Exists(Directories.MUSIC + fileName + ".mp3"))
            {
                file = new AudioFileReader(Directories.MUSIC + fileName + ".mp3");
                return true;
            }
            if (System.IO.File.Exists(Directories.MUSIC + fileName + ".m4a"))
            {
                file = new AudioFileReader(Directories.MUSIC + fileName + ".m4a");
                return true;
            }
            if (System.IO.File.Exists(Directories.MUSIC + fileName + ".wma"))
            {
                file = new AudioFileReader(Directories.MUSIC + fileName + ".wma");
                return true;
            }
            return true;
        }

        private static void OnFadeOver(object source, ElapsedEventArgs e)
        {
            transitionTimer.Stop(); //event is single fire
            stopSong(); //dispose of old song
            playSong(queuedSongName);
        }

        private static void OnFadeOverFadeIn(object source, ElapsedEventArgs e)
        {
            transitionTimer.Stop(); //event is single fire
            stopSong(); //dispose of old song
            playSong(queuedSongName);
            fadeInOut.BeginFadeIn(queuedFadeTime);
        }

        /// <summary>
        /// This helper class is lifted straight out of NAudio samples (with minor edits)
        /// to allow for fading/looping
        /// </summary>
        private class FadeInOutSampleProviderAdapted : ISampleProvider
        {
            enum FadeState
            {
                Silence,
                FadingIn,
                FullVolume,
                FadingOut,
            }

            private readonly object lockObject = new object();
            private readonly AudioFileReader source;
            private int fadeSamplePosition;
            private int fadeSampleCount;
            private FadeState fadeState;
            private bool looping;

            /// <summary>
            /// Creates a new FadeInOutSampleProvider
            /// </summary>
            /// <param name="source">The source stream with the audio to be faded in or out</param>
            /// <param name="initiallySilent">If true, we start faded out</param>
            public FadeInOutSampleProviderAdapted(AudioFileReader source, bool initiallySilent = false)
            {
                this.source = source;
                this.fadeState = initiallySilent ? FadeState.Silence : FadeState.FullVolume;
                this.looping = true;
            }

            /// <summary>
            /// Requests that a fade-in begins (will start on the next call to Read)
            /// </summary>
            /// <param name="fadeDurationInMilliseconds">Duration of fade in milliseconds</param>
            public void BeginFadeIn(double fadeDurationInMilliseconds)
            {
                lock (lockObject)
                {
                    fadeSamplePosition = 0;
                    fadeSampleCount = (int)((fadeDurationInMilliseconds * source.WaveFormat.SampleRate) / 1000);
                    fadeState = FadeState.FadingIn;
                }
            }

            /// <summary>
            /// Requests that a fade-out begins (will start on the next call to Read)
            /// </summary>
            /// <param name="fadeDurationInMilliseconds">Duration of fade in milliseconds</param>
            public void BeginFadeOut(double fadeDurationInMilliseconds)
            {
                lock (lockObject)
                {
                    fadeSamplePosition = 0;
                    fadeSampleCount = (int)((fadeDurationInMilliseconds * source.WaveFormat.SampleRate) / 1000);
                    fadeState = FadeState.FadingOut;
                }
            }

            /// <summary>
            /// Reads samples from this sample provider
            /// </summary>
            /// <param name="buffer">Buffer to read into</param>
            /// <param name="offset">Offset within buffer to write to</param>
            /// <param name="count">Number of samples desired</param>
            /// <returns>Number of samples read</returns>
            public int Read(float[] buffer, int offset, int count)
            {
                int sourceSamplesRead = source.Read(buffer, offset, count);

                if (looping && sourceSamplesRead == 0)
                    source.Position = 0;    //loop!

                lock (lockObject)
                {
                    if (fadeState == FadeState.FadingIn)
                    {
                        FadeIn(buffer, offset, sourceSamplesRead);
                    }
                    else if (fadeState == FadeState.FadingOut)
                    {
                        FadeOut(buffer, offset, sourceSamplesRead);
                    }
                    else if (fadeState == FadeState.Silence)
                    {
                        ClearBuffer(buffer, offset, count);
                    }
                }
                return sourceSamplesRead;
            }

            private static void ClearBuffer(float[] buffer, int offset, int count)
            {
                for (int n = 0; n < count; n++)
                {
                    buffer[n + offset] = 0;
                }
            }

            private void FadeOut(float[] buffer, int offset, int sourceSamplesRead)
            {
                int sample = 0;
                while (sample < sourceSamplesRead)
                {
                    float multiplier = 1.0f - (fadeSamplePosition / (float)fadeSampleCount);
                    for (int ch = 0; ch < source.WaveFormat.Channels; ch++)
                    {
                        buffer[offset + sample++] *= multiplier;
                    }
                    fadeSamplePosition++;
                    if (fadeSamplePosition > fadeSampleCount)
                    {
                        fadeState = FadeState.Silence;
                        // clear out the end
                        ClearBuffer(buffer, sample + offset, sourceSamplesRead - sample);
                        break;
                    }
                }
            }

            private void FadeIn(float[] buffer, int offset, int sourceSamplesRead)
            {
                int sample = 0;
                while (sample < sourceSamplesRead)
                {
                    float multiplier = (fadeSamplePosition / (float)fadeSampleCount);
                    for (int ch = 0; ch < source.WaveFormat.Channels; ch++)
                    {
                        buffer[offset + sample++] *= multiplier;
                    }
                    fadeSamplePosition++;
                    if (fadeSamplePosition > fadeSampleCount)
                    {
                        fadeState = FadeState.FullVolume;
                        // no need to multiply any more
                        break;
                    }
                }
            }

            /// <summary>
            /// WaveFormat of this SampleProvider
            /// </summary>
            public WaveFormat WaveFormat
            {
                get { return source.WaveFormat; }
            }

            /// <summary>
            /// Sets whether the current song should loop or not (true by default)
            /// </summary>
            /// <param name="looping"></param>
            public void setLooping(bool looping)
            {
                this.looping = looping;
            }

            /// <summary>
            /// Returns whether the current song is looping (true by default)
            /// </summary>
            /// <returns></returns>
            public bool isLooping()
            {
                return this.looping;
            }
        }
    }
}
