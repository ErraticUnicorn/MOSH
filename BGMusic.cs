using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.Timers;
//must have the naudio.dll reference to build this file!
using NAudio;
using NAudio.Wave;

namespace SunsetHigh
{
    /*
     * Controls all the background music for the game. The whole class is static;
     * there are no instantiations. To play a song, call BGMusic.playSong(<filename in Content directory>)
     * When providing the filename, the extension is preferred (but not necessary, the class will automatically
     * check for .mp3, .m4a, and .wma files of the given name). This is different from ContentManager.LoadContent,
     * which does NOT want the file extension (.xnb)
     * 
     * The class currently supports pausing and resuming music, stopping, fading in and out, and looping (true by default).
     * That should be all that's necessary for this game. Fading effects are recommended when changing screens/songs.
     */
    public static class BGMusic
    {
        public const float FULL_VOLUME = 1.0f;
        public const float SILENCE = 0.0f;

        private const string CONTENT_DIRECTORY = @"Content\";   // Looks in "Content" directory by default
        private const double DEFAULT_FADE_TIME = 2000;          // time to fade out = 2 seconds 
        private const double DEFAULT_LAG_BETWEEN_SONGS = 1000;  // time between songs (EXCLUDES FADE OUT TIME) = 1 second

        private static IWavePlayer wavePlayer;
        private static AudioFileReader file;
        private static FadeInOutSampleProvider fadeInOut;
        private static bool playing = false;
        private static Timer transitionTimer;
        private static string queuedSongName;

        /*
         * Starts playing the song.
         * Assumed to be in the "Content" directory
         */
        public static void playSong(string fileName)
        {
            if (playing)
                stopSong(); //cut off current song

            if (!tryAllFileTypes(fileName))
                return; //file could not be found, AudioFileReader did not load

            if (wavePlayer == null)
                wavePlayer = new WaveOutEvent(); //initialize wavePlayer

            fadeInOut = new FadeInOutSampleProvider(file);
            wavePlayer.Init(fadeInOut);
            
            wavePlayer.Play();
            playing = true;
        }

        /*
         * Fades out the current song and starts playing the 
         * new song after a little wait
         */
        public static void transitionToSong(string fileName, double fadeTime, double lagTime)
        {
            if (lagTime < fadeTime)
                return;    //don't try this yet
            queuedSongName = fileName;
            fadeOut(fadeTime);
            transitionTimer = new Timer(fadeTime + lagTime);
            transitionTimer.Elapsed += new ElapsedEventHandler(OnFadeOver);
            transitionTimer.Start();
        }

        public static void transitionToSong(string fileName)
        {
            transitionToSong(fileName, DEFAULT_FADE_TIME, DEFAULT_LAG_BETWEEN_SONGS);
        }

        /*
         * Fades out the current song and starts fading into the
         * new song after a little wait
         */
        public static void transitionToSongWithFadeIn(string fileName, double fadeTime, double lagTime)
        {
            if (lagTime < fadeTime)
                return;    //don't try this yet
            queuedSongName = fileName;
            fadeOut(fadeTime);
            transitionTimer = new Timer(fadeTime + lagTime);
            transitionTimer.Elapsed += new ElapsedEventHandler(OnFadeOverFadeIn);
            transitionTimer.Start();
        }

        public static void transitionToSongWithFadeIn(string fileName)
        {
            transitionToSongWithFadeIn(fileName, DEFAULT_FADE_TIME, DEFAULT_LAG_BETWEEN_SONGS);
        }

        /*
         * Switches between pause and play
         */
        public static void togglePlay()
        {
            if (wavePlayer != null && file != null)
            {
                if (playing)
                    wavePlayer.Pause();
                else
                    wavePlayer.Play();
                playing = !playing;
            }
        }

        /*
         * Stops song and disposes of it (cannot be resumed afterward)
         */ 
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

        /*
         * Disposes of everything. Call this at end of application's life
         */
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

        /*
         * Changes the volume of this file
         */
        public static void setVolume(float volume)
        {
            if(file != null)
                file.Volume = volume;
        }

        /*
         * Song starts from silence and fades in to full volume
         */
        public static void fadeIn(double fadeTime)
        {
            if(fadeInOut != null)
                fadeInOut.BeginFadeIn(fadeTime);
        }

        public static void fadeIn()
        {
            fadeIn(DEFAULT_FADE_TIME);
        }

        /*
         * Song starts from full volume and fades out to silence
         */
        public static void fadeOut(double fadeTime)
        {
            if(fadeInOut != null)
                fadeInOut.BeginFadeOut(fadeTime);
        }

        public static void fadeOut()
        {
            fadeOut(DEFAULT_FADE_TIME);
        }

        /*
         * Sets whether Song should loop upon ending
         */
        public static void setLooping(bool looping)
        {
            if(fadeInOut != null)
                fadeInOut.setLooping(looping);
        }

        public static bool isLooping()
        {
            if(fadeInOut != null)
                return fadeInOut.isLooping();
            return false;
        }

        private static bool tryAllFileTypes(string fileName)
        {
            string filetry = fileName;
            try
            {
                filetry = CONTENT_DIRECTORY + fileName;
                file = new AudioFileReader(filetry);
            }
            catch (System.IO.FileNotFoundException)
            {
                try
                {
                    filetry = CONTENT_DIRECTORY + fileName + ".mp3";
                    file = new AudioFileReader(filetry);
                }
                catch (System.IO.FileNotFoundException)
                {
                    try
                    {
                        filetry = CONTENT_DIRECTORY + fileName + ".m4a";
                        file = new AudioFileReader(filetry);
                    }
                    catch (System.IO.FileNotFoundException)
                    {
                        try
                        {
                            filetry = CONTENT_DIRECTORY + fileName + ".wma";
                            file = new AudioFileReader(filetry);
                        }
                        catch (System.IO.FileNotFoundException)
                        {
                            Debug.WriteLine("Music File not found or unsupported extension.");
                            Debug.WriteLine("Only use .mp3, .wma, or .m4a; don't include \"Content\" in file path.");
                            return false;
                        }
                    }
                }
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
            fadeInOut.BeginFadeIn(DEFAULT_FADE_TIME);
        }

        /// <summary>
        /// This helper class is lifted straight out of NAudio samples (with minor edits)
        /// to allow for fading/looping
        /// </summary>
        private class FadeInOutSampleProvider : ISampleProvider
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
            public FadeInOutSampleProvider(AudioFileReader source, bool initiallySilent = false)
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

            /*
             * Sets looping 
             */
            public void setLooping(bool looping)
            {
                this.looping = looping;
            }
            public bool isLooping()
            {
                return this.looping;
            }
        }

    }
}
