using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace SunsetHigh
{
    public class SoundModifierEntry : MenuEntry
    {
        public const byte MUSIC_CONFIG = 0;
        public const byte SOUNDFX_CONFIG = 1;
        private const int NEXT_COLUMN_OFFSET = 200;

        private byte inputType;
        private string status;

        public SoundModifierEntry(string name, byte type)
            : base(name, 0, 0)
        {
            this.inputType = type;
            updateStatus();
        }

        public override void onPress()
        {
            if (inputType == MUSIC_CONFIG)
            {
                BGMusic.setPaused(!BGMusic.isPaused());
            }
            else if (inputType == SOUNDFX_CONFIG)
            {
                SoundFX.setMuted(!SoundFX.isMuted());
            }
            updateStatus();
        }

        public override void draw(SpriteBatch sb, int x_offset, int y_offset, SpriteFont font, Color c)
        {
            base.draw(sb, x_offset, y_offset, font, c);
            
            sb.DrawString(font, status,
                new Vector2(x_offset + NEXT_COLUMN_OFFSET + this.getX(), y_offset + this.getY()), c);
        }

        private void updateStatus()
        {
            if (inputType == MUSIC_CONFIG)
            {
                if (!BGMusic.isPaused())
                    status = "Playing";
                else
                    status = "Paused";
            }
            else if (inputType == SOUNDFX_CONFIG)
            {
                if (!SoundFX.isMuted())
                    status = "Active";
                else
                    status = "Muted";
            }
        }
    }


    public class SoundConfigPanel : ListPanel
    {
        public SoundConfigPanel()
            : this(0, 0, 0, 0) { }
        public SoundConfigPanel(int x, int y, int width, int height)
            : base(x, y, width, height)
        {
            SoundModifierEntry entry1 = new SoundModifierEntry("Background Music", 
                SoundModifierEntry.MUSIC_CONFIG);
            SoundModifierEntry entry2 = new SoundModifierEntry("Sound Effects",
                SoundModifierEntry.SOUNDFX_CONFIG);
            this.loadEntries(entry1, entry2);
            this.alignEntriesVertical();
        }
    }
}
