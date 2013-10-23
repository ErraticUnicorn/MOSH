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
    /// <summary>
    /// Hero extends from Character, and is used for the main character
    /// </summary>
    public class Hero : Character
    {
        private const float RECHARGE_TIME = 1.0f;   //time between shots in seconds
        private const string PROJECTILE_IMAGE_NAME = "projectile";

        private bool ppActive;  //if currently pickpocketing
        private Character ppTarget;     //the target of the pickpocket
        private PickpocketSystem ppSystem;  //the graphics associated with the pickpocket minigame
        private SoundEffect gotItemSound;
        private List<Projectile> projectiles; //List of all projectiles
        //private Texture2D paperball; //paperball texture
        private float shootTimer;   // For recharge time
        private bool canShoot; //Boolean for tinkering with parameters of how often a player can shoot

        /// <summary>
        /// Initializes a Hero at the origin which will match the dimensions
        /// of its texture (when loaded)
        /// </summary>
        /// 
        public List<Projectile> getProjectiles()
        {
            return this.projectiles;
        }
        public Hero()
            : this(0, 0, 0, 0) { }

        /// <summary>
        /// Initalizes a Hero at the given positon which will match the dimensions
        /// of its texture (when loaded)
        /// </summary>
        /// <param name="x">X coordinate of the top-left corner</param>
        /// <param name="y">Y coordinate of the top-left corner</param>
        public Hero(int x, int y)
            : this(x, y, 0, 0) { }

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
            shootTimer = 0.0f;
        }

        public void converse(Character c)
        {
        }
        
        public override void loadContent(ContentManager content)
        {
            base.loadContent(content);
            ppSystem.loadContent(content);
            this.gotItemSound = content.Load<SoundEffect>("LTTP_Rupee1");
            Sprite.loadCommonImage(content, PROJECTILE_IMAGE_NAME);
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

            shootTimer += elapsed;
            if (shootTimer >= RECHARGE_TIME)
                canShoot = true;

            foreach (Projectile p in projectiles)
            {
                p.update(elapsed);  //TODO: remove if off the screen
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
                ppSystem.randomize();
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

        /// <summary>
        /// Causes the Hero to create a projectile and fire it in the direction
        /// he is facing; Hero is then responsible for updating and drawing this projectile
        /// </summary>
        public void shoot()
        {
            if (canShoot)
            {
                int x = 0;
                int y = 0;
                if (this.getDirection().Equals(Direction.North))
                    y = -this.getHeight() / 2;
                if (this.getDirection().Equals(Direction.South))
                    y = this.getHeight() / 2;
                if (this.getDirection().Equals(Direction.East))
                    x = this.getWidth() / 2;
                if (this.getDirection().Equals(Direction.West))
                    x = -this.getWidth() / 2;

                Projectile bullet = new Projectile(this.getX() + x, this.getY() + y, 10, this.getDirection());
                bullet.setImage(Sprite.getCommonImage(PROJECTILE_IMAGE_NAME));
                projectiles.Add(bullet);
                
                canShoot = false;
                shootTimer = 0.0f;
            }
        }
      

        private class PickpocketSystem  //A container for three sprites
        {
            private const int NEGATIVE_WIDTH = 100;
            private const float DEFAULT_POSITIVE_WIDTH_FACTOR = 0.25f; 
            private const int BAR_HEIGHT = 20;
            private const int ARROW_WIDTH = 15;
            private const int ARROW_HEIGHT = 15;
            private const int ARROW_Y_OFFSET = -40;
            private const int BAR_Y_OFFSET = -27;
            private const float DEFAULT_SPEED = 31.0f;  //for linear
            private const float DEFAULT_SPEED_FACTOR = 2.5f; //for sinusoidal
            private const float SPEED_FACTOR_RANGE = 3.5f;

            private Sprite negativeBar;
            private Sprite positiveBar;
            private Sprite arrow;
            private int displacement;
            //private float speed;
            //private bool goingRight;
            private float speedFactor;
            private float arrowTimer;
            private float randomOffset;

            public PickpocketSystem()
            {
                negativeBar = new Sprite(0, 0, NEGATIVE_WIDTH, BAR_HEIGHT);
                positiveBar = new Sprite(0, 0, (int)(NEGATIVE_WIDTH * DEFAULT_POSITIVE_WIDTH_FACTOR), BAR_HEIGHT);
                arrow = new Sprite(0, 0, ARROW_WIDTH, ARROW_HEIGHT);
                displacement = 0;
                //speed = DEFAULT_SPEED;
                //goingRight = true;
                speedFactor = DEFAULT_SPEED_FACTOR;
                randomOffset = 0;
                arrowTimer = 0;
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
                //if (arrowTimer > Math.PI * 2)
                //    arrowTimer -= (float)(Math.PI * 2);     //keep timer between 0 and 2*PI
                displacement = (int)(NEGATIVE_WIDTH / 2 * Math.Sin(arrowTimer * speedFactor + randomOffset));
                arrow.setX(arrow.getX() + displacement);
            }

            public void draw(SpriteBatch sb)
            {
                negativeBar.draw(sb);
                positiveBar.draw(sb);
                arrow.draw(sb);
            }

            /// <summary>
            /// Randomizes the speed and starting position of the arrow (to keep the game unpredictable)
            /// </summary>
            public void randomize()
            {
                Random rand = new Random();
                randomOffset = (float)(rand.NextDouble() * (Math.PI * 2));
                speedFactor = (float)(DEFAULT_SPEED_FACTOR + (rand.NextDouble() * SPEED_FACTOR_RANGE));
                arrowTimer = 0.0f;
            }

            /// <summary>
            /// Sets bar width in terms of "difficulty"
            /// </summary>
            /// <param name="difficulty">0.0 to 1.0, 0.0 for impossible, 1.0 for always win</param>
            public void setBarDifficulty(float difficulty)
            {
                this.positiveBar.setWidth((int)(negativeBar.getWidth() * difficulty));
            }

            /// <summary>
            /// Checks if the arrow is within the positive region (i.e. if pickpocket is successful
            /// </summary>
            /// <returns>True if pickpocket was successful, false otherwise</returns>
            public bool success()
            {
                return arrow.getXCenter() > positiveBar.getX() && arrow.getXCenter() < positiveBar.getX() + positiveBar.getWidth();
            }
        }
    }
}
    