using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;

namespace SunsetHigh
{
    public class Hero : Character
    {
        private bool ppActive;  //if currently pickpocketing
        private Character ppTarget;     //the target of the pickpocket
        //other variables

        //maybe make this a singleton? (there won't be multiple instances of this class)

        public Hero()
            : base()
        {
        }

        public Hero(int x, int y, int w, int h)
            : base(x, y, w, h)
        {
        }

        public void converse(Character c)
        {
        }

        public override void draw(SpriteBatch sb)
        {
            base.draw(sb);
            if (ppActive)
            {
                //draw the bar and needle
            }
        }

        public override void update(float elapsed)
        {
            base.update(elapsed);
            if (ppActive)
            {
                //update needle location based on time elapsed
            }
        }

        public bool isPickpocketing()
        {
            return ppActive;
        }

        /*
         * Triggered by a keystroke when close to Character c
         */
        public void startPickpocket(Character c) //need content manager?
        {
            if (!ppActive)
            {
                ppActive = true;
                ppTarget = c;   //assigns pointer
            }
        }

        /*
         * Triggered by a keystroke to stop the pickpocket needle
         */
        public Item stopPickpocket()
        {
            if (ppActive)
            {
                ppActive = false;
                
                //test for success; if successful...
                
                Item i = ppTarget.getInventory().removeRandomItem();
                if (!i.Equals(Item.Nothing))
                {
                    //Got item!
                    this.getInventory().addItem(i, 1);
                    return i;
                }
                else
                {
                    //Character c has nothing! Cry...
                    return i;
                }

                //if fail, do something here
            }
            return Item.Nothing;
        }
    }
}
