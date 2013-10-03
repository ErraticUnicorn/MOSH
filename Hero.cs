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
        private PickpocketSystem ppSystem;

        public Hero()
            : base()

        {
            ppSystem = new PickpocketSystem();
            ppActive = false;
        }

        public Hero(int x, int y)
            : base(x, y)
        {
            ppSystem = new PickpocketSystem();
            ppActive = false;
        }

        public Hero(int x, int y, int w, int h)
            : base(x, y, w, h)
        {
            ppSystem = new PickpocketSystem();
            ppActive = false;
        }

        public void converse(Character c)
        {
        }

        public override void loadContent(ContentManager content)
        {
            base.loadContent(content);
            ppSystem.loadContent(content);
            this.gotItemSound = content.Load<SoundEffect>("LTTP_Rupee1");
        }

        public override void draw(SpriteBatch sb)
        {
            base.draw(sb);
            if (ppActive)
            {
                ppSystem.draw(sb);
            }
        }

        public override void update(float elapsed) 
        {
            base.update(elapsed);
            if (ppActive)
            {
                ppSystem.update(this, elapsed);
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

                if (ppSystem.success())
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
                        //Character c has nothing to steal! Cry...
                        return i;
                    }
                }
                else
                {
                    //if fail, do something here (play sound)
                }
            }
            return Item.Nothing;
        }

        private class PickpocketSystem  //A container for three sprites
        {
            private const int NEGATIVE_WIDTH = 100;
            private const float POSITIVE_WIDTH_FACTOR = 0.25f;
            private const int BAR_HEIGHT = 20;
            private const int ARROW_WIDTH = 15;
            private const int ARROW_HEIGHT = 15;
            private const int ARROW_Y_OFFSET = -40;
            private const int BAR_Y_OFFSET = -27;
            private const float DEFAULT_SPEED = 31.0f;  //for linear
            private const float DEFAULT_SPEED_FACTOR = 3.0f; //for sinusoidal

            private Sprite negativeBar;
            private Sprite positiveBar;
            private Sprite arrow;
            private int displacement;
            private float speed;
            private bool goingRight;
            private float arrowTimer;

            public PickpocketSystem() : base()
            {
                negativeBar = new Sprite(0, 0, NEGATIVE_WIDTH, BAR_HEIGHT);
                positiveBar = new Sprite(0, 0, (int)(NEGATIVE_WIDTH * POSITIVE_WIDTH_FACTOR), BAR_HEIGHT);
                arrow = new Sprite(0, 0, ARROW_WIDTH, ARROW_HEIGHT);
                displacement = 0;
                speed = DEFAULT_SPEED;
                goingRight = true;
            }

            public void loadContent(ContentManager content)
            {
                negativeBar.loadImage(content, "pickpocketnegativebar");
                positiveBar.loadImage(content, "pickpocketpositivebar");
                arrow.loadImage(content, "pickpocketarrow");
            }

            public void update(Hero h, float elapsed)
            {
                //set initial positions
                arrow.setXCenter(h.getXCenter());
                arrow.setYCenter(h.getYCenter() + ARROW_Y_OFFSET);
                negativeBar.setXCenter(h.getXCenter());
                negativeBar.setYCenter(h.getYCenter() + BAR_Y_OFFSET);
                positiveBar.setXCenter(h.getXCenter());
                positiveBar.setYCenter(h.getYCenter() + BAR_Y_OFFSET);

                //update moving arrow

                //moves linearly
                /*
                float velocity = 0;
                if (displacement >= negativeBar.getWidth() / 2)
                {
                    goingRight = false;
                }
                else if (displacement <= -(negativeBar.getWidth() / 2))
                {
                    goingRight = true;
                }
                if (goingRight) 
                    velocity = speed;
                if (!goingRight) 
                    velocity = -speed;
                displacement += (int)(velocity * elapsed);
                arrow.setX(arrow.getX() + displacement);
                 */

                //moves sinusoidally
                arrowTimer += elapsed;
                if (arrowTimer > Math.PI * 2)
                    arrowTimer -= (float)(Math.PI * 2);
                displacement = (int)(NEGATIVE_WIDTH / 2 * Math.Sin(arrowTimer * DEFAULT_SPEED_FACTOR));
                arrow.setX(arrow.getX() + displacement);
            }

            public void draw(SpriteBatch sb)
            {
                negativeBar.draw(sb);
                positiveBar.draw(sb);
                arrow.draw(sb);
            }

            public bool success()
            {
                return arrow.getXCenter() > positiveBar.getX() && arrow.getXCenter() < positiveBar.getX() + positiveBar.getWidth();
            }
        }
    }
}
    