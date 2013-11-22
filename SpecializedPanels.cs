using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;

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
            Hero.instance.inventory.InventoryChanged += updateInventory;
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

        /// <summary>
        /// Listens for inventory changed events and updates the entries.
        /// Inefficient - O(n) - refactor if it's an issue with LARGE amounts of items
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void updateInventory(object sender, InventoryEventArgs e)
        {
            Item item = e.type;
            int quantity = e.quantity;

            bool found = false;
            MenuEntry removeEntry = null;
            foreach (MenuEntry entry in this.entries)
            {
                if (entry is ItemEntry)
                {
                    ItemEntry iEntry = (ItemEntry)entry;
                    if (iEntry.getItemType() == item)
                    {
                        iEntry.setQuantity(iEntry.getQuantity() + quantity);
                        if (iEntry.getQuantity() == 0)
                            removeEntry = iEntry;
                        found = true;
                        break;
                    }
                }
            }
            if (found && removeEntry != null)
            {
                this.entries.Remove(removeEntry);
                this.loadEntries(); // !! this automatically resizes the list of entries on the panel
            }
            if (!found)
            {
                ItemEntry itemEntry = new ItemEntry(this, item, quantity);
                this.loadEntries(itemEntry);
            }
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

    public class ReputationPanel : Panel
    {
        private const int BAR_WIDTH = 263;
        private const int BAR_HEIGHT = 30;
        private const int NEEDLE_WIDTH = 8;
        private const int NEEDLE_HEIGHT = 41;
        private const int BAR_X_OFFSET = 125;
        private const int TEXT_X_OFFSET = 50;

        private const int NUM_TYPES = 5;

        private SpriteFont font;
        private Texture2D barTexture;
        private Texture2D needleTexture;

        public ReputationPanel()
            : this(0, 0, 0, 0) { }
        public ReputationPanel(int x, int y, int width, int height)
            : base(x, y, width, height)
        {
        }
        
        public override void onConfirm()
        {
            InGameMenu.goBack();
        }

        public override void loadContent(ContentManager content)
        {
            base.loadContent(content);
            font = content.Load<SpriteFont>("BabyBlue");
            barTexture = content.Load<Texture2D>("reputation_bar");
            needleTexture = content.Load<Texture2D>("reputation_needle");
        }
        public override void draw(SpriteBatch sb)
        {
            base.draw(sb);
            int space = this.getHeight() - 2 * this.getYMargin();
            int spacePerEntry = space / NUM_TYPES;
            for (int i = 0; i < NUM_TYPES; i++)
            {
                int offsetY = this.getY() + this.getYMargin() + 
                    (spacePerEntry * i) + (spacePerEntry / 2);
                switch (i)
                {
                    case 0:
                        drawRepHelper(sb, offsetY, Clique.Nerd);
                        break;
                    case 1:
                        drawRepHelper(sb, offsetY, Clique.Jock);
                        break;
                    case 2:
                        drawRepHelper(sb, offsetY, Clique.Prep);
                        break;
                    case 3:
                        drawRepHelper(sb, offsetY, Clique.Bully);
                        break;
                    case 4:
                        drawRepHelper(sb, offsetY, Clique.Slacker);
                        break;
                }
            }
        }

        private void drawRepHelper(SpriteBatch sb, int offsetY, Clique clique)
        {
            if (this.isInFocus() || this.isSmoothMoving())  //i.e. can be seen on screen
            {
                Hero h1 = Hero.instance;
                string cliqueHeader = SunsetUtils.enumToString<Clique>(clique) + ":";
                int offsetNeedleX = h1.getReputation(clique);
                                    // we need some sort of scale for rep (min / max), needle will move accordingly
                                    // my idea is having a log(reputation) scale - it moves quickly at first, then slowly
                sb.DrawString(font, cliqueHeader,
                    new Vector2(this.getX() + TEXT_X_OFFSET, offsetY - font.MeasureString(cliqueHeader).Y / 2), Color.Black);
                sb.Draw(barTexture, new Rectangle(this.getX() + BAR_X_OFFSET, offsetY - BAR_HEIGHT / 2, 
                    BAR_WIDTH, BAR_HEIGHT), Color.White);
                sb.Draw(needleTexture, new Rectangle(this.getX() + BAR_X_OFFSET + BAR_WIDTH / 2 + offsetNeedleX - NEEDLE_WIDTH / 2, 
                    offsetY - NEEDLE_HEIGHT / 2,
                    NEEDLE_WIDTH, NEEDLE_HEIGHT), Color.White);
            }
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
