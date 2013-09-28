using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SunsetHigh
{
    public enum Item
    {
        Nothing = -1,   //used for showing steal failed (like a null type)

        Shoes = 0,      //any possible items go here! (comma delimited)
        Socks,
        Hat,
        LunchMoney,
        PokeBall
    };

    public class Inventory
    {
        private static int NUM_TYPE_ITEMS = Enum.GetValues(typeof(Item)).Length - 1;
                                             //number of values in Item enum excluding Nothing
                                             //don't change this variable please
        private int total;
        private int[] items;
        
        public Inventory() 
        {
            items = new int[NUM_TYPE_ITEMS];
            total = 0; 
        }

        /*
         * Sum total of all the items a character has
         */
        public int numTotalItems()
        {
            return this.total;
        }

        /*
         * Checks if a character has any items
         */
        public bool hasItem()
        {
            return this.total != 0;
        }

        /*
         * Check for a specific type of item
         */
        public bool hasItem(Item item)
        {
            if((int)item >= 0)
                return this.items[(int)item] > 0;
            return false;
        }

        /*
         * Get number of a specific type of item
         */
        public int numItem(Item item)
        {
            if ((int)item >= 0)
                return this.items[(int)item];
            return 0;
        }

        /*
         * Adds given number of certain item
         */
        public void addItem(Item type, int quantity)
        {
            if (quantity < 0 || type.Equals(Item.Nothing))
                return;     //bad arguments
            this.items[(int)type] += quantity;
            total += quantity;
        }

        public void addItem(Item type)
        {
            this.addItem(type, 1);
        }

        /*
         * Removes given number of certain item
         */
        public void removeItem(Item type, int quantity)
        {
            if (quantity < 0 || type.Equals(Item.Nothing))
                return;     //bad arguments
            if (this.items[(int)type] < quantity)
                quantity = this.items[(int)type]; //prevents negative quantities

            this.items[(int)type] -= quantity;
            this.total -= quantity;
        }

        public void removeItem(Item type)
        {
            this.removeItem(type, 1);
        }

        /*
         * Used for stealing; removes and returns a random item 
         * out of this character's inventory
         * Returns Item.Nothing if inventory is empty
         */
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

            this.items[(int)possible[pick]] -= 1;  //removes one of this type
            this.total -= 1;
            return possible[pick];
        }

        /*
         * Clears inventory
         */
        public void removeAll()
        {
            for (int i = 0; i < this.items.Length; i++)
            {
                this.items[i] = 0;
            }
            this.total = 0;
        }

    }
}
