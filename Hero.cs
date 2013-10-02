using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Audio;

namespace SunsetHigh
{
    public class Hero : Character
    {
        private bool ppActive;  //if currently pickpocketing
        private Character ppTarget;     //the target of the pickpocket
        private SoundEffect gotItemSound;
        private PickpocketItem pickpocketnegbar, pickpocketposbar;
        private PickpocketItem pickpocketarrow;
        private int arrowYdist = -30;
        private int arrowXdist = 10;
        private int barYdist = -17;
        private int barXdist = -33;
        private int displacement = 0;
        private float speed = 0;
        private bool goingRight = true;
        //other content (i.e. Texture2D for pickpocket graphics)

        public Hero()
            : base()

        {
            pickpocketnegbar = new PickpocketItem(this.getX() + barXdist, this.getY() + barYdist, 100, 20);
            pickpocketposbar = new PickpocketItem(this.getX() + barXdist, this.getY() + barYdist, 33, 20);
            pickpocketarrow = new PickpocketItem(this.getX() + arrowXdist, this.getY() + arrowYdist, 15, 15);
        }

        public Hero(int x, int y, int w, int h)
            : base(x, y, w, h)
        {
            pickpocketnegbar = new PickpocketItem(this.getX() + barXdist, this.getY() + barYdist, 100, 20);
            pickpocketposbar = new PickpocketItem(this.getX() + barXdist, this.getY() + barYdist, 33, 20);
            pickpocketarrow = new PickpocketItem(this.getX() + arrowXdist, this.getY() + arrowYdist, 15, 15); //difference from the bar is 43, and 15
        }

        public void converse(Character c)
        {
        }

        public override void loadContent(ContentManager content)
        {
            base.loadContent(content);
            this.gotItemSound = content.Load<SoundEffect>("LTTP_Rupee1");
            //this.loadImage( ... );
            this.pickpocketnegbar.loadImage(content, "pickpocketnegativebar", 1, 1, 100);
            this.pickpocketposbar.loadImage(content, "pickpocketpositivebar", 1, 1, 100);
            this.pickpocketarrow.loadImage(content, "pickpocketarrow", 1, 1, 100.0f);
        }

        public override void draw(SpriteBatch sb)
        {
            base.draw(sb);
            if (ppActive)
            {
                this.pickpocketnegbar.draw(sb);
                this.pickpocketposbar.draw(sb);
                this.pickpocketarrow.draw(sb);
            }
        }

        public override void update(float elapsed) 
        {
            base.update(elapsed);
            this.pickpocketarrow.setXCenter(this.getX());
            this.pickpocketarrow.setYCenter(this.getY() + arrowYdist/2);
            this.pickpocketnegbar.setXCenter(this.getX());
            this.pickpocketnegbar.setYCenter(this.getY() + barYdist/2);
            this.pickpocketposbar.setXCenter(this.getX());
            this.pickpocketposbar.setYCenter(this.getY() + barYdist / 2);
            if (ppActive)
            {
                //update needle location based on time elapsed
                //speed = pickpocketarrow.getX() / elapsed
                if (displacement >= pickpocketnegbar.getWidth() / 2)
                {
                    goingRight = false;
                }
                else if (displacement <= -(pickpocketnegbar.getWidth() / 2))
                {
                    goingRight = true;
                }
                if (goingRight) speed = 1.0f;
                if (!goingRight) speed = -1.0f;
                displacement += (int)speed;
                this.pickpocketarrow.setX(this.pickpocketarrow.getX() + displacement);
                
                


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
                if (pickpocketarrow.getXCenter() > pickpocketposbar.getX() && pickpocketarrow.getXCenter() < pickpocketposbar.getX() + pickpocketposbar.getWidth())
                {
                    Item i = ppTarget.getInventory().removeRandomItem();
                    if (!i.Equals(Item.Nothing))
                    {
                        //Got item!
                        this.getInventory().addItem(i, 1);
                        this.gotItemSound.Play();
                        return i;
                    }
                    else
                    {
                        //Character c has nothing! Cry...
                        return i;
                    }
                }
                //if fail, do something here
            }
            return Item.Nothing;
        }
    }

    public class PickpocketItem : Sprite
    {
        public PickpocketItem() : base()
        {
        }

        public PickpocketItem(int x, int y, int width, int height)
            : base(x, y, width, height)
        {

        }

        public Boolean collision(PickpocketItem a)
        {
            if (this.getX() == a.getX())
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
    