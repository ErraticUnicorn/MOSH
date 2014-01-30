using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace SunsetHigh
{
    public class KeyModifierEntry : MenuEntry
    {
        private const int NEXT_COLUMN_OFFSET = 200;
        private KeyConfigPanel owner;
        private KeyInputType inputType;

        public KeyModifierEntry(string name, KeyConfigPanel owner, KeyInputType inputType)
            : base(name, 0, 0)
        {
            this.owner = owner;
            this.inputType = inputType;
        }

        public KeyInputType getKeyInputType() { return inputType; }
        public void setKeyInputType(KeyInputType inputType) { this.inputType = inputType; }

        public string getKeyInputString()
        {
            return Enum.GetName(typeof(KeyInputType), inputType);
        }

        public string getKeyString()
        {
            return "[" + Enum.GetName(typeof(Keys), KeyboardManager.getKeyControl(inputType)) + "]";
        }

        public void sendNewKey(Keys key)
        {
            KeyboardManager.changeKeyControl(inputType, key);
        }

        public override void onPress()
        {
            this.owner.getPreviousPanel().setMessage("Configuring " + this.getName() + "... Press the new key, or [Esc] to cancel.");
        }

        public override void draw(SpriteBatch sb, int x_offset, int y_offset, SpriteFont font, Color c)
        {
            base.draw(sb, x_offset, y_offset, font, c);
            //Draw the key associated with this input type
            sb.DrawString(font, this.getKeyString(),
                new Vector2(x_offset + NEXT_COLUMN_OFFSET + this.getX(), y_offset + this.getY()), c);
        }
    }


    public class KeyConfigPanel : ListPanel
    {
        private IMessagePanel prevPanel;
        private bool openToNewKey;

        public KeyConfigPanel()
            : this(0, 0, 0, 0) { }
        public KeyConfigPanel(int x, int y, int width, int height)
            : base(x, y, width, height)
        {
            openToNewKey = false;
        }

        public void setPreviousPanel(IMessagePanel prev) { this.prevPanel = prev; }
        public IMessagePanel getPreviousPanel() { return this.prevPanel; }

        public override void onConfirm()
        {
            if (!openToNewKey)
            {
                base.onConfirm();
                openToNewKey = true;
            }
        }

        public override bool onKeyInput(Keys key)
        {
            if (openToNewKey && this.getCurrentEntry() is KeyModifierEntry)
            {
                KeyModifierEntry entry = (KeyModifierEntry)this.getCurrentEntry();
                string prefix = "";

                if (KeyboardManager.keyControlExists(key, entry.getKeyInputType()))
                {
                    this.prevPanel.setMessage("Key [" + Enum.GetName(typeof(Keys), key) +
                        "] is already being used! Try a different key. ([Esc] to cancel)");
                    return true;
                }

                if (key == Keys.Escape)
                {
                    prefix = "Cancelled key config; to continue, ";
                }
                else
                {
                    entry.sendNewKey(key);
                    prefix = "Changed " + entry.getKeyInputString() + " to " + entry.getKeyString() + "; to continue, ";
                }
                this.prevPanel.setMessage(prefix + "press [" +
                    Enum.GetName(typeof(Keys), KeyboardManager.getKeyControl(KeyInputType.Action)) +
                    "] to modify a key.");
                openToNewKey = false;
                return true;
            }
            return false;
        }
    }
}
