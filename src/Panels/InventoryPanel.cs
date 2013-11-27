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
}
