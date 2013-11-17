using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Graphics;

namespace SunsetHigh
{
    public class ExitEntry : MenuEntry
    {
        private bool exit;
        public ExitEntry(string name, bool exiting)
        {
            this.setName(name);
            this.setExitType(exiting);
        }
        public bool isExiting() { return this.exit; }
        public void setExitType(bool exiting) { this.exit = exiting; }
        public override void onPress()
        {
            // if this entry confirms exiting the game
            if (this.isExiting())
            {
                // Cleanup and return to title screen
            }
            // this entry cancels exiting
            else
            {
                InGameMenu.goBack();
            }
        }
    }


    public class ItemEntry : MenuEntry
    {
        private const int NEXT_COLUMN_OFFSET = 100;

        private InventoryPanel owner;
        private Item mItem;
        private int mQuantity;

        public ItemEntry(InventoryPanel owner, Item type, int quantity)
            : base("Temp", 0, 0) 
        {
            this.owner = owner;
            this.mItem = type;
            this.mQuantity = quantity;
            convertItemTypeToName();
        }

        public string getItemNameString()
        {
            return Enum.GetName(typeof(Item), mItem);
        }
        public string getQuantityString()
        {
            return "x" + mQuantity;
        }
        public Item getItemType() { return this.mItem; }
        public int getQuantity() { return this.mQuantity; }

        public void setItemType(Item itemType) { this.mItem = itemType; }
        public void setQuantity(int quantity) { this.mQuantity = quantity; }

        public override void onPress() { }  // nothing happens
        public override void onHover()
        {
            this.owner.getMessagePanel().setMessage("Information about " + getItemNameString());
        }

        public override void draw(SpriteBatch sb, int x_offset, int y_offset, SpriteFont font, Color c)
        {
            base.draw(sb, x_offset, y_offset, font, c);
            sb.DrawString(font, this.getQuantityString(),
                new Vector2(x_offset + this.getX() + NEXT_COLUMN_OFFSET, y_offset + this.getY()), c);
        }

        private void convertItemTypeToName()
        {
            this.setName(Enum.GetName(typeof(Item), mItem));
        }
    }


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

    public class QuestDataEntry : MenuEntry
    {
        private const string COMPLETE_STRING = "[COMPLETE!]";
        private QuestID id;
        private Dictionary<QuestState, string> data;

        public QuestDataEntry(QuestID id)
            : base(SunsetUtils.enumToString<QuestID>(id), 0, 0)
        {
            this.id = id;
        }

        public override void onPress()
        {
            throw new NotImplementedException();
        }

        public override void draw(SpriteBatch sb, int x_offset, int y_offset, SpriteFont font, Color c)
        {
            if (Quest.isQuestComplete(id) && !this.getName().EndsWith(COMPLETE_STRING))
                this.setName(this.getName() + " " + COMPLETE_STRING);
            base.draw(sb, x_offset, y_offset, font, c);
        }
    }

    public class SaveDataEntry : MenuEntry
    {
        private const int NEXT_COLUMN_OFFSET = 200;
        
        private SaveDataPanel owner;
        private SaveGameData saveData;

        public SaveDataEntry(SaveDataPanel owner)
            : this(owner, null) { }
        public SaveDataEntry(SaveDataPanel owner, SaveGameData data)
            : base("Temp", 0, 0)
        {
            this.owner = owner;
            this.setSaveData(data);
        }

        public void setSaveData(SaveGameData data) { this.saveData = data; }
        public SaveGameData getSaveData() { return this.saveData; }

        public override void onPress()
        {
            //if save type if true, we will save our current save data
            if (owner.isSaving())
            {
                SaveGameData newData = SaveManager.packData();
                if (this.saveData == null)
                { 
                    newData.fileName = SaveManager.generateNewFileName();
                    this.owner.loadEntries(new SaveDataEntry(this.owner, null));    //add a new blank entry
                    this.setName(newData.heroData.name);
                }
                else
                {
                    //newData.playTime = this.saveData.playTime + newData.playTime;
                    newData.fileName = this.saveData.fileName;
                }
                SaveManager.saveGame(newData.fileName, newData, false);
                this.saveData = newData;
                this.owner.getPreviousPanel().setMessage("Game saved!");
            }
            //if save type if false, we will load our current save data
            else
            {
                if (this.saveData == null)
                {
                    this.owner.getPreviousPanel().setMessage("An empty save file cannot be loaded.");
                }
                else
                {
                    this.owner.getPreviousPanel().setMessage("Game loaded!");
                    InGameMenu.reset();
                    SaveManager.unpackData(this.saveData);
                }
            }
        }

        public override void draw(SpriteBatch sb, int x_offset, int y_offset, SpriteFont font, Color c)
        {
            base.draw(sb, x_offset, y_offset, font, c);
            string playTimeStr;
            if (this.saveData != null)
                playTimeStr = GameClock.formatTimeSpan(this.saveData.playTime);
            else
                playTimeStr = GameClock.formatTimeSpan(TimeSpan.Zero);
            sb.DrawString(font, playTimeStr,
                new Vector2(x_offset + NEXT_COLUMN_OFFSET + this.getX(), y_offset + this.getY()), c);
        }
    }



    public class SaveTypeSpecifierEntry : SubMenuFocusEntry
    {
        IMessagePanel owner;
        private bool saving;
        private string newMessage;

        public SaveTypeSpecifierEntry(string name, IMessagePanel owner, SaveDataPanel next)
            : base(name, next) 
        {
            this.owner = owner;
        }
        public void setSaveType(bool saving) { this.saving = saving; }
        public bool isSaving() { return this.saving; }
        public void setNewMessage(string newMessage) { this.newMessage = newMessage; }
        public string getNewMessage() { return this.newMessage; }

        public override void onPress()
        {
            base.onPress();
            ((SaveDataPanel)(this.getNextPanel())).setSaveType(this.isSaving());
            this.owner.setMessage(this.getNewMessage());
        }
    }



    public class ScreenSpecifierEntry : SubMenuFocusEntry
    {
        IMessagePanel owner;
        private string newMessage;

        public ScreenSpecifierEntry(string name, IMessagePanel owner, Panel next)
            : base(name, next) 
        {
            this.owner = owner;
        }
        public void setNewMessage(string newMessage) { this.newMessage = newMessage; }
        public string getNewMessage() { return this.newMessage; }

        public override void onHover()
        {
            base.onHover();
            PanelGroupSorter.panelIn(this.getNextPanel());
        }

        public override void onUnhover()
        {
            base.onUnhover();
            PanelGroupSorter.panelOut(this.getNextPanel());
        }

        public override void onPress()
        {
            base.onPress();
            if (this.newMessage != null && this.newMessage.Length != 0)
                this.owner.setMessage(this.getNewMessage());
        }
    }



    public class SubMenuFocusEntry : MenuEntry
    {
        private Panel nextFocus;
        public SubMenuFocusEntry()
            : this("No name", null) { }
        public SubMenuFocusEntry(string name)
            : this(name, null) { }
        public SubMenuFocusEntry(string name, Panel next)
            : base(name, 0, 0) 
        {
            this.setNextPanel(next);
        }
        public void setNextPanel(Panel next)
        {
            this.nextFocus = next;
        }
        public Panel getNextPanel()
        {
            return this.nextFocus;
        }
        public override void onPress()
        {
            if (nextFocus != null)
                InGameMenu.pushActivePanel(this.nextFocus);
        }
    }
}
