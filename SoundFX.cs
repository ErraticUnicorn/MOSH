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
            if (soundFinder.ContainsKey(fileName))
                return; //already exists
            SoundEffect se = content.Load<SoundEffect>(fileName);
            soundFinder[fileName] = se;
        }

        public static void playSound(string fileName)
        {
            nullCheck();
            if (muted)
                return;
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
