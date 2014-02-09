using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace SunsetHigh
{
    public class ItemEntry : MenuEntry
    {
        private const int NEXT_COLUMN_OFFSET = 100;

        private InventoryPanel owner;
        private Item mItem;
        private string mDescription;

        public ItemEntry(InventoryPanel owner, Item type)
            : base(SunsetUtils.enumToString<Item>(type), 0, 0)
        {
            this.owner = owner;
            this.mItem = type;
        }

        public string getQuantityString()
        {
            return "x" + Hero.instance.inventory.numItem(mItem);
        }
        public Item getItemType() { return this.mItem; }
        public void setItemType(Item itemType) { this.mItem = itemType; }
        public string getDescription() { return this.mDescription; }
        public void setDescription(string description) { this.mDescription = description; }

        public override void onPress() { }  // nothing happens
        public override void onHover()
        {
            this.owner.getMessagePanel().setMessage(this.mDescription);
        }

        public override void draw(SpriteBatch sb, int x_offset, int y_offset, SpriteFont font, Color c)
        {
            base.draw(sb, x_offset, y_offset, font, c);
            sb.DrawString(font, this.getQuantityString(),
                new Vector2(x_offset + this.getX() + NEXT_COLUMN_OFFSET, y_offset + this.getY()), c);
        }
    }

    public class InventoryPanel : ListPanel
    {
        private IMessagePanel messagePanel;
        private ItemEntry[] allEntries;

        public InventoryPanel()
            : this(0, 0, 0, 0) { }
        public InventoryPanel(int x, int y, int width, int height)
            : base(x, y, width, height)
        {
            Hero.instance.inventory.InventoryChanged += updateInventory;
            allEntries = new ItemEntry[Inventory.NUM_TYPE_ITEMS];
        }

        public void setMessagePanel(IMessagePanel panel) { this.messagePanel = panel; }
        public IMessagePanel getMessagePanel() { return this.messagePanel; }

        public void refreshList()
        {
            Inventory inventory = Hero.instance.inventory;
            this.clearEntries();
            List<MenuEntry> tempEntries = new List<MenuEntry>();
            foreach (Item i in inventory)
            {
                ItemEntry itemEntry = allEntries[(int)i];
                tempEntries.Add(itemEntry);
            }
            this.loadEntries(tempEntries.ToArray());
        }

        /// <summary>
        /// Listens for inventory changed events and updates the entries.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void updateInventory(object sender, InventoryEventArgs e)
        {
            Item item = e.type;
            int quantity = e.quantity;

            if (Hero.instance.inventory.numItem(item) == 0)     // no more items of this type
            {
                this.entries.Remove(allEntries[(int)item]);
                this.loadEntries(); // !! this automatically resizes the list of entries on the panel
            }
            else if (Hero.instance.inventory.numItem(item) == quantity)
            {
                ItemEntry itemEntry = allEntries[(int)item];
                this.loadEntries(itemEntry);
            }
        }

        public void loadEntriesFromFile(string filename)
        {
            string[] lines = System.IO.File.ReadAllLines(filename);
            for (int i = 0; i < lines.Length; i++)
            {
                if (lines[i].StartsWith("#entry "))
                {
                    Item type = SunsetUtils.parseEnum<Item>(lines[i].Substring("#entry ".Length));
                    allEntries[(int)type] = new ItemEntry(this, type);
                    string baseText = "";

                    for (int j = i + 1; j < lines.Length; j++)
                    {
                        if (lines[j].Trim().Length == 0) continue;

                        if (lines[j].StartsWith("#title "))
                        {
                            allEntries[(int)type].setName(lines[j].Substring("#title ".Length));
                        }
                        else if (lines[j].StartsWith("#end"))
                        {
                            //finished entry
                            allEntries[(int)type].setDescription(baseText);
                            i = j;
                            break;
                        }
                        else
                        {
                            baseText += lines[j] + "\n";
                        }
                    }
                }
            }

            for (int i = 0; i < allEntries.Length; i++)
            {
                if (allEntries[i] == null)
                {
                    allEntries[i] = new ItemEntry(this, (Item)i);
                    allEntries[i].setDescription("No description loaded.");
                }
            }
        }
    }
}
