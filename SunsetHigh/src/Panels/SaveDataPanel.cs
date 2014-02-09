using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace SunsetHigh
{
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
                    ScreenTransition.requestTransition(delegate()
                    {
                        InGameMenu.reset();
                        SaveManager.unpackData(this.saveData);
                        WorldManager.updateCameraOffset(Hero.instance);
                    });
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


    public class SaveDataPanel : ListPanel
    {
        private bool saving;
        private IMessagePanel prevPanel;

        public SaveDataPanel()
            : this(0, 0, 0, 0) { }
        public SaveDataPanel(int x, int y, int width, int height)
            : base(x, y, width, height)
        {
            this.saving = true;
        }

        public void setSaveType(bool saving) { this.saving = saving; }
        public bool isSaving() { return this.saving; }
        public void setPreviousPanel(IMessagePanel prevPanel) { this.prevPanel = prevPanel; }
        public IMessagePanel getPreviousPanel() { return this.prevPanel; }

        public void loadSaveData()
        {
            List<SaveGameData> listSave = SaveManager.loadAllGames(false);
            List<MenuEntry> tempEntries = new List<MenuEntry>();
            this.clearEntries();
            for (int i = 0; i < listSave.Count; i++)
            {
                SaveDataEntry entry = new SaveDataEntry(this, listSave[i]);
                entry.setName(listSave[i].heroData.name);
                tempEntries.Add(entry);
            }
            tempEntries.Add(new SaveDataEntry(this, null)); // the "blank file" for saving a new game
            this.loadEntries(tempEntries.ToArray());
        }
    }
}
