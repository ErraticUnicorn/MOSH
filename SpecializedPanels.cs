using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Input;

namespace SunsetHigh
{
    public class InventoryPanel : ListPanel
    {
        private Inventory inventory;
        private IMessagePanel messagePanel;

        public InventoryPanel()
            : this(0, 0, 0, 0) { }
        public InventoryPanel(int x, int y, int width, int height)
            : base(x, y, width, height)
        {
        }

        public void setMessagePanel(IMessagePanel panel) { this.messagePanel = panel; }
        public IMessagePanel getMessagePanel() { return this.messagePanel; }

        public void registerInventory(Inventory inventory)
        {
            this.clearEntries();
            this.inventory = inventory;
            List<MenuEntry> tempEntries = new List<MenuEntry>();
            foreach (Item i in inventory)
            {
                int quantity = inventory.numItem(i);
                ItemEntry itemEntry = new ItemEntry(this, i, quantity);
                tempEntries.Add(itemEntry);
            }
            this.loadEntries(tempEntries.ToArray());
        }

        public void updateInventory(Item item, int quantity)
        {
            foreach (MenuEntry entry in this.entries)
            {
                if (entry is ItemEntry)
                {
                    ItemEntry iEntry = (ItemEntry)entry;
                    if (iEntry.getItemType() == item)
                    {
                        iEntry.setQuantity(iEntry.getQuantity() + quantity);
                        return;
                    }
                }
            }
            // doesn't exist yet
            ItemEntry itemEntry = new ItemEntry(this, item, quantity);
            this.loadEntries(itemEntry);
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

    public class QuestPanel : ListPanel
    {
        private const string FILE_DIRECTORY = @"Content\";

        public QuestPanel()
            : this(0, 0, 0, 0) { }
        public QuestPanel(int x, int y, int width, int height)
            : base(x, y, width, height)
        {
        }

        public void loadEntriesFromFile(string fileName)
        {

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
