using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Audio;

namespace SunsetHigh
{
    public static class SoundFX
    {
        private static Dictionary<string, SoundEffect> soundFinder;
        private static bool muted;

        public static void setMuted(bool mute)
        {
            muted = mute;
        }
        public static bool isMuted()
        {
            return muted;
        }

        public static void loadSound(ContentManager content, string fileName)
        {
            nullCheck();
            if (fileName.StartsWith(Directories.SOUNDS))
                fileName = fileName.Substring(Directories.SOUNDS.Length);

            if (soundFinder.ContainsKey(fileName))
                return; //already exists

            if (System.IO.File.Exists(Directories.SOUNDS + fileName))
            {
                throw new System.IO.FileNotFoundException("Could not find the sound file \"" + fileName + "\" in the \"Sounds\" directory"
                    + " in Content.\nMake sure the file is .wav, and \"Copy to output directory\" settings"
                    + " are set to \"Copy if newer\".");
            }

            SoundEffect se = content.Load<SoundEffect>(Directories.SOUNDS + fileName);
            soundFinder[fileName] = se;
        }

        public static void playSound(string fileName)
        {
            nullCheck();
            if (muted)
                return;
            if (fileName.StartsWith(Directories.SOUNDS))
                fileName = fileName.Substring(Directories.SOUNDS.Length);

            if (!soundFinder.ContainsKey(fileName))
            {
                System.Diagnostics.Debug.WriteLine("Sound file " + fileName + " was not loaded!");
                return;
            }
            soundFinder[fileName].Play();
        }

        private static void nullCheck()
        {
            if (soundFinder == null)
            {
                soundFinder = new Dictionary<string, SoundEffect>();
                muted = false;
            }
        }
    }
}
