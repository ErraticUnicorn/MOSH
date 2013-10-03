using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;

namespace SunsetHigh
{
    /*
     * Class for objects that appear on screen that a character can pick up
     */
    public class Pickup : Sprite
    {
        private const int INF = 1000000;

        Item itemType;

        public Pickup() 
            : base() 
        {
            this.setItemType(Item.Nothing);
        }
        public Pickup(int x, int y)
            : base(x, y)
        {
            this.setItemType(Item.Nothing);
        }
        public Pickup(int x, int y, Item type)
            : base(x, y)
        {
            this.setItemType(type);
        }
        public Pickup(int x, int y, int width, int height)
            : base(x, y, width, height)  
        {
            this.setItemType(Item.Nothing);
        }
        public Pickup(int x, int y, int width, int height, Item type)
            : base(x, y, width, height) 
        { 
            this.setItemType(type);
        }

        public Item getItemType() { return this.itemType; }
        public void setItemType(Item type) { this.itemType = type; }

        /*
         * Once this item has been picked up, we call this
         * to remove it from the screen
         */
        public void banish()
        {
            this.setVisible(false);
            this.setPosition(-INF, -INF);   //inelegant, but it should work
        }
    }
}
