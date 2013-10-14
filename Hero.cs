﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Audio;

namespace SunsetHigh
{
    /// <summary>
    /// Hero extends from Character, and is used for the main character
    /// </summary>
    public class Hero : Character
    {
        private bool ppActive;  //if currently pickpocketing
        private Character ppTarget;     //the target of the pickpocket
        private PickpocketSystem ppSystem;  //the graphics associated with the pickpocket minigame
        private SoundEffect gotItemSound;
        private List<Projectile> projectiles; //List of all projectiles
        private Texture2D paperball; //paperball texture
        private bool canShoot; //Boolean for tinkering with parameters of how often a player can shoot

        /// <summary>
        /// Initializes a Hero at the origin which will match the dimensions
        /// of its texture (when loaded)
        /// </summary>
        public Hero()
            : base()

        {
            ppSystem = new PickpocketSystem();
            ppActive = false;
            projectiles = new List<Projectile>();
            canShoot = true;
        }

        /// <summary>
        /// Initalizes a Hero at the given positon which will match the dimensions
        /// of its texture (when loaded)
        /// </summary>
        /// <param name="x">X coordinate of the top-left corner</param>
        /// <param name="y">Y coordinate of the top-left corner</param>
        public Hero(int x, int y)
            : base(x, y)
        {
            ppSystem = new PickpocketSystem();
            ppActive = false;
            projectiles = new List<Projectile>();
            canShoot = true;
        }

        /// <summary>
        /// Initializes a Hero at the given position with the given dimensions
        /// </summary>
        /// <param name="x">X coordinate of the top-left corner</param>
        /// <param name="y">Y coordinate of the top-left corner</param>
        /// <param name="w">Width in pixels</param>
        /// <param name="h">Height in pixels</param>
        public Hero(int x, int y, int w, int h)
            : base(x, y, w, h)
        {
            ppSystem = new PickpocketSystem();
            ppActive = false;
            projectiles = new List<Projectile>();
            canShoot = true;
        }

        public void converse(Character c)
        {
        }

        public override void loadContent(ContentManager content)
        {
            base.loadContent(content);
            ppSystem.loadContent(content);
            this.gotItemSound = content.Load<SoundEffect>("LTTP_Rupee1");
            this.paperball = content.Load<Texture2D>("projectile");
        }

        public override void draw(SpriteBatch sb)
        {
            base.draw(sb);
            if (ppActive)
            {
                ppSystem.draw(sb);
            }
            foreach(Projectile p in projectiles)
            {
                p.draw(sb);
            }
        }

        public override void update(float elapsed) 
        {
            base.update(elapsed);
            if (ppActive)
            {
                ppSystem.update(this, elapsed);
            }

            Projectile temp = new Projectile(); //Dummy projectile to figure out most recent projectile fired
            foreach (Projectile p in projectiles)
            {
                p.update(elapsed);
                temp = p; //makes temp the last projectile fired
            }

            if (temp.getX() > this.getX() + 100 || temp.getY() > this.getY() + 100 
                || temp.getX() < this.getX() - 100 || temp.getY() < this.getY() - 100) //allows for shooting again once a certain range has been met
            {
                canShoot = true;
            }
        }

        public bool isPickpocketing()
        {
            return ppActive;
        }

        /// <summary>
        /// Begins the pickpocketing minigame with the given character as the target
        /// </summary>
        /// <param name="character">The targeted Character</param>
        public void startPickpocket(Character character) //need content manager?
        {
            if (!ppActive)
            {
                ppActive = true;
                ppTarget = character;   //assigns pointer
            }
        }

        /// <summary>
        /// Stops the pickpocketing minigame and checks for success. If successful,
        /// an Item is returned; if not, Item.Nothing is returned
        /// </summary>
        /// <returns>An Item, or Item.Nothing if the pickpocket fails</returns>
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

        public void shoot()
        {
            int x = 0;
            int y = 0;
            if (canShoot)
            {
                if (this.getDirection().Equals(Direction.North))
                {
                    y = -10;
                    canShoot = false;
                }
                if (this.getDirection().Equals(Direction.South))
                {
                    y = 10;
                    canShoot = false;
                }
                if (this.getDirection().Equals(Direction.East))
                {
                    x = 10;
                    canShoot = false;
                }
                if (this.getDirection().Equals(Direction.West))
                {
                    x = -10;
                    canShoot = false;
                }

                Projectile bullet = new Projectile(this.getX() + x, this.getY() + y);
                bullet.setImage(paperball);
                bullet.setSpeed(5);
                bullet.setDirection(this.getDirection());
                projectiles.Add(bullet);
            }

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
            //private float speed;
            //private bool goingRight;
            private float speedFactor;
            private float arrowTimer;

            public PickpocketSystem() : base()
            {
                negativeBar = new Sprite(0, 0, NEGATIVE_WIDTH, BAR_HEIGHT);
                positiveBar = new Sprite(0, 0, (int)(NEGATIVE_WIDTH * POSITIVE_WIDTH_FACTOR), BAR_HEIGHT);
                arrow = new Sprite(0, 0, ARROW_WIDTH, ARROW_HEIGHT);
                displacement = 0;
                //speed = DEFAULT_SPEED;
                //goingRight = true;
                speedFactor = DEFAULT_SPEED_FACTOR;
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
                    arrowTimer -= (float)(Math.PI * 2);     //keep timer between 0 and 2*PI
                displacement = (int)(NEGATIVE_WIDTH / 2 * Math.Sin(arrowTimer * speedFactor));
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
    