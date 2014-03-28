using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;

namespace SunsetHigh
{
    /// <summary>
    /// Specifies an item type to go in a Character's inventory,
    /// or to be associated with a Pickup
    /// </summary>
    public enum Item
    {
        Nothing = -1,   //used for showing steal failed (like a null type)

        PokeBall = 0,      //any possible items go here! (comma delimited)
        LightSaber,
        LunchMoney,
        SmallDumbell,
        Cologne,
        SomeSubstance,
    };

    public struct InventorySave
    {
        public int[] items;
        public int[] order;
        public int total;
        public int numTypes;
    }

    public class InventoryEventArgs : EventArgs
    {
        public Item type;
        public int quantity;
        //int position; // ?
    }

    /// <summary>
    /// Represents an assortment of items; each Character has an Inventory
    /// to hold his/her own collection
    /// </summary>
    public class Inventory : IEnumerable
    {
        public static int NUM_TYPE_ITEMS = Enum.GetValues(typeof(Item)).Length - 1;
                                             //number of values in Item enum excluding Nothing
                                             //don't change this variable please
        private int total;
        private int numTypes;
        private int[] items;
        private int[] order;

        public event EventHandler<InventoryEventArgs> InventoryChanged;

        /// <summary>
        /// Default constructor
        /// </summary>
        public Inventory() 
        {
            items = new int[NUM_TYPE_ITEMS];
            order = new int[NUM_TYPE_ITEMS];
            for (int i = 0; i < order.Length; i++)
                order[i] = -1;
            numTypes = 0;
            total = 0; 
        }

        /// <summary>
        /// Sum total of all the items in this Inventory
        /// </summary>
        /// <returns>Sum total</returns>
        public int numTotalItems()
        {
            return this.total;
        }

        /// <summary>
        /// Checks if this Inventory has at least one item
        /// </summary>
        /// <returns>True if the Inventory has at least one item, false if empty</returns>
        public bool hasItem()
        {
            return this.total != 0;
        }

        /// <summary>
        /// Checks if this Inventory has the given Item type
        /// </summary>
        /// <param name="item">The Item type to check for</param>
        /// <returns>True if the Inventory has at least one item of the given type, false if not</returns>
        public bool hasItem(Item item)
        {
            if((int)item >= 0)
                return this.items[(int)item] > 0;
            return false;
        }

        /// <summary>
        /// Gets the amount of a specific Item type in this Inventory
        /// </summary>
        /// <param name="item">The Item type to check for</param>
        /// <returns>The number of the specific item in the inventory</returns>
        public int numItem(Item item)
        {
            if ((int)item >= 0)
                return this.items[(int)item];
            return 0;
        }

        /// <summary>
        /// Adds a given number of the given Item type to this Inventory
        /// </summary>
        /// <param name="type">The Item type to add</param>
        /// <param name="quantity">The number of items to add</param>
        public void addItem(Item type, int quantity)
        {
            if (quantity < 0 || type.Equals(Item.Nothing))
                return;     //bad arguments
            
            if (this.items[(int)type] == 0)
            {
                this.order[numTypes++] = (int)type;
            }
            this.items[(int)type] += quantity;
            total += quantity;

            // push event
            InventoryEventArgs args = new InventoryEventArgs();
            args.type = type;
            args.quantity = quantity;
            pushEvent(args);
        }

        /// <summary>
        /// Adds one item of a given type to this Inventory
        /// </summary>
        /// <param name="type">The Item type to add</param>
        public void addItem(Item type)
        {
            this.addItem(type, 1);
        }

        /// <summary>
        /// Removes a given number of the given item type from this Inventory
        /// </summary>
        /// <param name="type">The Item type to remove</param>
        /// <param name="quantity">The number of items to remove</param>
        public void removeItem(Item type, int quantity)
        {
            if (quantity < 0 || type.Equals(Item.Nothing))
                return;     //bad arguments
            if (this.items[(int)type] <= quantity)
            {
                int i = 0;
                for (; order[i] != (int)type; i++) ;
                Array.Copy(order, i + 1, order, i, numTypes - 1 - i);
                order[numTypes - 1] = -1;
                numTypes--;     // O(n) operation
                quantity = this.items[(int)type]; //remove all items of this type
            }
            this.items[(int)type] -= quantity;
            this.total -= quantity;

            // push event
            InventoryEventArgs args = new InventoryEventArgs();
            args.type = type;
            args.quantity = -quantity;
            pushEvent(args);
        }

        /// <summary>
        /// Removes one item of a given type from this Inventory
        /// </summary>
        /// <param name="type">The Item type to remove</param>
        public void removeItem(Item type)
        {
            this.removeItem(type, 1);
        }

        /// <summary>
        /// Removes and returns a random item from this Inventory; each Item type is weighted equally
        /// (i.e. quantity does not matter); use this for pickpocketing
        /// </summary>
        /// <returns>Returns a random Item type, or Item.Nothing if this Inventory is empty</returns>
        public Item removeRandomItem()
        {
            List<Item> possible = new List<Item>();
            foreach(Item it in Enum.GetValues(typeof(Item)))
            {
                if ((int)it < 0) continue;
                if(items[(int)it] > 0)
                    possible.Add(it);
            }
            if (possible.Count == 0)
                return Item.Nothing;    //no Items to steal!

            Random rand = new Random();
            int pick = rand.Next(possible.Count);

            Item retVal = possible[pick];
            removeItem(retVal);
            return retVal;
        }

        /// <summary>
        /// Clears this Inventory of all items
        /// </summary>
        public void removeAll()
        {
            for (int i = 0; i < this.items.Length; i++)
            {
                if (this.items[i] > 0)
                {
                    this.removeItem((Item)i, items[i]); //removes all and pushes the event
                }
            } 
            for (int i = 0; i < this.order.Length; i++)
            {
                this.order[i] = -1;
            }
            this.total = 0;
        }

        /// <summary>
        /// Used for saving purposes only
        /// </summary>
        /// <returns>An struct containing primitives representing the inventory</returns>
        public InventorySave getSaveStructure()
        {
            InventorySave saveStruct;
            saveStruct.items = this.items;
            saveStruct.order = this.order;
            saveStruct.numTypes = this.numTypes;
            saveStruct.total = this.total;
            return saveStruct;
        }

        /// <summary>
        /// Used for loading in the hero's inventory when restoring a saved game 
        /// </summary>
        /// <param name="saveStruct">An int[] representation of inventory to load</param>
        public void loadSaveStructure(InventorySave saveStruct)
        {
            if (saveStruct.items.Length <= this.items.Length)
                saveStruct.items.CopyTo(this.items, 0);
            if (saveStruct.order.Length <= this.order.Length)
                saveStruct.order.CopyTo(this.order, 0);
            this.numTypes = saveStruct.numTypes;
            this.total = saveStruct.total;
        }

        public IEnumerator GetEnumerator()
        {
            for (int i = 0; i < numTypes; i++)
            {
                if (order[i] >= 0)
                    yield return (Item)order[i];
            }
        }

        private void pushEvent(InventoryEventArgs e)
        {
            if (InventoryChanged != null)
            {
                InventoryChanged(this, e);
            }
        }
    }
}
