using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;

namespace SunsetHigh
{
    /// <summary>
    /// Pickup extends from Sprite, adding information for an Item attribute
    /// </summary>
    public class Pickup : Sprite
    {
        private const int INF = 1000000;

        Item itemType;

        /// <summary>
        /// Initializes a type-less Pickup at the origin which matches the dimensions of 
        /// its sprite (when loaded)
        /// </summary>
        public Pickup() 
            : base() 
        {
            this.setItemType(Item.Nothing);
        }
        /// <summary>
        /// Initializes a type-less Pickup at the given position which matches the dimensions
        /// of its sprite (when loaded)
        /// </summary>
        /// <param name="x">X coordinate of the top-left corner</param>
        /// <param name="y">Y coordinate of the top-left corner</param>
        public Pickup(int x, int y)
            : base(x, y)
        {
            this.setItemType(Item.Nothing);
        }
        /// <summary>
        /// Initializes a Pickup with a given Item type at the given position which matches the 
        /// dimensions of its sprite (when loaded)
        /// </summary>
        /// <param name="x">X coordinate of the top-left corner</param>
        /// <param name="y">Y coordinate of the top-left corner</param>
        /// <param name="type">Item type of this Pickup</param>
        public Pickup(int x, int y, Item type)
            : base(x, y)
        {
            this.setItemType(type);
        }
        /// <summary>
        /// Initializes a type-less Pickup with the given position and dimensions
        /// </summary>
        /// <param name="x">X coordinate of the top-left corner</param>
        /// <param name="y">Y coordinate of the top-left corner</param>
        /// <param name="width">Width in pixels</param>
        /// <param name="height">Height in pixels</param>
        public Pickup(int x, int y, int width, int height)
            : base(x, y, width, height)  
        {
            this.setItemType(Item.Nothing);
        }
        /// <summary>
        /// Initializes a Pickup with the given position, dimensions, and Item type
        /// </summary>
        /// <param name="x">X coordinate of the top-left corner</param>
        /// <param name="y">Y coordinate of the top-left corner</param>
        /// <param name="width">Width in pixels</param>
        /// <param name="height">Height in pixels</param>
        /// <param name="type">Item type of this Pickup</param>
        public Pickup(int x, int y, int width, int height, Item type)
            : base(x, y, width, height) 
        { 
            this.setItemType(type);
        }

        public Item getItemType() { return this.itemType; }
        public void setItemType(Item type) { this.itemType = type; }

        /// <summary>
        /// Once the Pickup has been claimed, this method removes
        /// it from the screen
        /// </summary>
        public void banish()
        {
            this.setVisible(false);
            this.setPosition(-INF, -INF);   //inelegant, but it should work
        }

        /// <summary>
        /// We assume whenever the Hero collides with a pickup he picks it up.
        /// </summary>
        public override void onCollide(IInteractable other)
        {
            base.onCollide(other);
            Hero.instance.pickup(this);     //roundabout.. change later
        }
    }
}
