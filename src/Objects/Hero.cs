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
    /// Structure for saving data about a Hero.
    /// </summary>
    public struct HeroSave
    {
        public InventorySave inventorySave;
        public int x;
        public int y;
        public Direction dir;
        public string name;
    }

    /// <summary>
    /// Hero extends from Character, and is used for the main character
    /// </summary>
    public sealed class Hero : Character
    {
        private const float RECHARGE_TIME = 1.0f;   //time between shots in seconds
        private static string PROJECTILE_IMAGE_NAME = Directories.SPRITES + "projectile";

        //pickpocket vars
        private bool ppActive;  //if currently pickpocketing
        private Character ppTarget;     //the target of the pickpocket
        private PickpocketSystem ppSystem;  //the graphics associated with the pickpocket minigame

        //Shooting vars
        private List<Projectile> projectiles; //List of all projectiles
        //private Texture2D paperball; //paperball texture
        private float shootTimer;   // For recharge time
        private bool canShoot;      // whether player can shoot a projectile
        
        //Speech vars
        private Dialogue dialogue;

        //Reputation vars
        private int nerdRep;
        private int jockRep;
        private int prepRep;
        private int bullyRep;
        private int slackerRep;

        //Static vars
        private static volatile Hero inst;
        private static object syncRoot = new Object();
        /// <summary>
        /// Returns an instance of the Hero class (singleton)
        /// </summary>
        public static Hero instance
        {
            get
            {
                if (inst == null)
                {
                    lock (syncRoot)
                    {
                        if (inst == null)
                            inst = new Hero();
                    }
                }
                return inst;
            }
        }

        /// <summary>
        /// Initializes a Hero at the origin which will match the dimensions
        /// of its texture (when loaded)
        /// </summary>
        /// 
        private Hero()
            : this(0, 0, 0, 0) { }

        /// <summary>
        /// Initalizes a Hero at the given positon which will match the dimensions
        /// of its texture (when loaded)
        /// </summary>
        /// <param name="x">X coordinate of the top-left corner</param>
        /// <param name="y">Y coordinate of the top-left corner</param>
        private Hero(int x, int y)
            : this(x, y, 0, 0) { }

        /// <summary>
        /// Initializes a Hero at the given position with the given dimensions
        /// </summary>
        /// <param name="x">X coordinate of the top-left corner</param>
        /// <param name="y">Y coordinate of the top-left corner</param>
        /// <param name="w">Width in pixels</param>
        /// <param name="h">Height in pixels</param>
        private Hero(int x, int y, int w, int h)
            : base(x, y, w, h, string.Empty)
        {
            ppSystem = new PickpocketSystem();
            ppActive = false;
            projectiles = new List<Projectile>();
            canShoot = true;
            shootTimer = 0.0f;
            dialogue = new Dialogue();
            nerdRep = 0;
            jockRep = 0;
            prepRep = 0;
            bullyRep = 0;
            slackerRep = 0;
        }

        public void setReputation(Clique clique, int repPoints)
        {
            switch (clique)
            {
                case Clique.Nerd: nerdRep += repPoints; break;
                case Clique.Jock: jockRep += repPoints; break;
                case Clique.Prep: prepRep += repPoints; break;
                case Clique.Bully: bullyRep += repPoints; break;
                case Clique.Slacker: slackerRep += repPoints; break;
            }
        }

        // input positive points to increase reputation, negative to decrease
        public void shiftReputation(Clique clique, int repPoints)
        {
            this.setReputation(clique, this.getReputation(clique) + repPoints);
        }

        public int getReputation(Clique clique)
        {
            switch (clique)
            {
                case Clique.Nerd: return nerdRep;
                case Clique.Jock: return jockRep;
                case Clique.Prep: return prepRep;
                case Clique.Bully: return bullyRep;
                case Clique.Slacker: return slackerRep;
            }
            return 0;
        }

        public void converse(Character c)
        {
            dialogue.talking = true;
            dialogue.loadInteraction(c);
            dialogue.end = false;
            System.Diagnostics.Debug.WriteLine("Spoke!");
        }

        public bool isTalking()
        {
            return dialogue.talking;
        }

        public void stopTalking()
        {
            dialogue.talking = false;
        }
        public override void loadContent(ContentManager content)
        {
            base.loadContent(content);
            ppSystem.loadContent(content);
            dialogue.loadContent(content);
            SoundFX.loadSound(content, Directories.SOUNDS + "LTTP_Rupee1");
            Sprite.loadCommonImage(content, PROJECTILE_IMAGE_NAME);
        }

        public override void draw(SpriteBatch sb)
        {
            base.draw(sb);
            if (ppActive)
            {
                ppSystem.draw(sb);
            }

            if (dialogue.talking)
            {
                dialogue.draw(sb);
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

            if (dialogue.talking)
            {
               dialogue.update();
            }

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
                    Item i = ppTarget.inventory.removeRandomItem();
                    if (!i.Equals(Item.Nothing))
                    {
                        //Got item!
                        this.inventory.addItem(i, 1);
                        SoundFX.playSound("LTTP_Rupee1");
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

                Projectile bullet = new Projectile(this.getX() + x, this.getY() + y, 300.0f, this.getDirection());
                bullet.setImage(Sprite.getCommonImage(PROJECTILE_IMAGE_NAME));
                projectiles.Add(bullet);
                
                canShoot = false;
                shootTimer = 0.0f;
            }
        }

        /// <summary>
        /// Used for loading purposes
        /// </summary>
        /// <param name="data">The save data for the Hero</param>
        public void loadSaveStructure(HeroSave data)
        {
            this.setX(data.x);
            this.setY(data.y);
            this.setDirection(data.dir);
            this.setName(data.name);
            this.inventory.loadSaveStructure(data.inventorySave);
            this.resetAnimation();
        }

        /// <summary>
        /// Used for saving purposes
        /// </summary>
        /// <returns>The save data for the Hero</returns>
        public HeroSave getSaveStructure()
        {
            HeroSave data;
            data.x = this.getX();
            data.y = this.getY();
            data.name = this.getName();
            data.dir = this.getDirection();
            data.inventorySave = this.inventory.getSaveStructure();
            return data;
        }

        /// <summary>
        /// Class to handle the dialogue box/moving through the line tree, etc. Godawful and ugly, but works for right now.
        /// </summary>
        private class Dialogue
        {
            private Interaction interaction;
            private InteractionTreeNode current;
            private SpriteFont font;
            private Sprite bg;
            private Sprite nameBg;
            private string say = "...";
            public bool end = false;
            public bool talking = false;
            private int place;
            private int lower;
            private readonly InteractionTreeNode defaultNode = new InteractionTreeNode { eventType = Events.End, line = "You're out of line!", responses = new List<Tuple<string, Events, int>>() };
            
            public Dialogue()
            {
                bg = new Sprite(0, 0, 0, 0);
                nameBg = new Sprite(0, 0, 0, 0);
                place = 0;
                lower = 0;
            }

            public string buildString()
            {
                string[] words = current.line.Split(' ');
                StringBuilder wrappedText = new StringBuilder();
                wrappedText.Append(buildString(words));
                for (int i = lower; i <= lower + 3 && i < current.responses.Count; ++i)
                {
                    var resp = current.responses[i];
                    wrappedText.Append("\n");
                    var signifier = (i == place) ? " > " : " - ";
                    wrappedText.Append(signifier + buildString(resp.Item1.Split(' ')));
                }

                return wrappedText.ToString();
            }

            private string buildString(string[] words)
            {
                StringBuilder wrappedText = new StringBuilder();
                float linewidth = 0f;
                float spaceWidth = font.MeasureString(" ").X;
                for (int i = 0; i < words.Length; ++i)
                {
                    Vector2 size = font.MeasureString(words[i]);
                    if (linewidth + size.X < 400)
                    {
                        linewidth += size.X + spaceWidth;
                    }
                    else
                    {
                        wrappedText.Append("\n");
                        linewidth = size.X + spaceWidth;
                    }
                    wrappedText.Append(words[i]);
                    wrappedText.Append(" ");
                }

                return wrappedText.ToString();
            }

            public void loadContent(ContentManager content)
            {
                font = content.Load<SpriteFont>(Directories.FONTS + "Arial");
                bg.loadImage(content, Directories.SPRITES + "bg");
                nameBg.loadImage(content, Directories.SPRITES + "bg");
                nameBg.setHeight(20);
                nameBg.setY(0);
            }

            public void draw(SpriteBatch sb)
            {
                if (interaction != null)
                {
                    int width = sb.GraphicsDevice.Viewport.Width;
                    int height = sb.GraphicsDevice.Viewport.Height;
                    say = end ? "(end)" : buildString();
                    bg.setWidth((int)font.MeasureString(say).X + 20);
                    bg.setY(height - 90);
                    bg.setX(width / 3);
                    bg.draw(sb);
                    nameBg.setWidth((int)font.MeasureString(interaction.name).X + 10);
                    nameBg.setX(width / 2);
                    nameBg.draw(sb);
                    say = end ? "(end)" : buildString();
                    sb.DrawString(font, say, new Vector2(bg.getX() + 10, height - 90), Color.Black, 0f, new Vector2(), 0.80f, SpriteEffects.None, 0f);
                    sb.DrawString(font, interaction.name, new Vector2(nameBg.getX() + 5, 0), Color.Black, 0f, new Vector2(), 0.80f, SpriteEffects.None, 0f);
                }

            }

            public void update()
            {
                Tuple<string, Events, int> next = null;

                if (KeyboardManager.isKeyPressed(Microsoft.Xna.Framework.Input.Keys.A))
                {
                    if(end)
                    {
                        talking = false;
                        return;
                    }
                    else if (current.eventType != Events.End)
                        next = current.responses[place];
                    else
                    {
                        end = true;
                        return;
                    }
                    
                }

                else if(KeyboardManager.isKeyPressed(Microsoft.Xna.Framework.Input.Keys.Down)) {
                    ++place;
                    if (place >= current.responses.Count)
                    { 
                        place = 0;
                        lower = 0;
                    }

                    if (place == lower + 4)
                        ++lower; 
                }
                else if (KeyboardManager.isKeyPressed(Microsoft.Xna.Framework.Input.Keys.Up))
                {
                    --place;
                    if (place < 0)
                    {
                        place = current.responses.Count - 1;
                        lower = current.responses.Count - 4;
                    }

                    if (place < lower)
                        --lower;

                    if (lower < 0)
                        lower = 0;
                }

                if (next != null)
                {
                    current = interaction.dialogue.ElementAtOrDefault(next.Item3 - 1) ?? defaultNode;
                    switch (next.Item2)
                    {
                        case Events.Quest:
                            Quest.setQuestAccepted((QuestID)next.Item3);
                            end = true;
                            break;
                        case Events.End:
                            end = true;
                            break;
                        default:
                            place = 0;
                            break;
                    }
                }
            }
            public void loadInteraction(Character c)
            {
                interaction = c.script;
                current = interaction.dialogue.ElementAtOrDefault(0) ?? defaultNode;
                place = 0;
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
            private const float DEFAULT_SPEED_FACTOR = 2.5f; //for sinusoidal
            private const float SPEED_FACTOR_RANGE = 3.5f;

            private Sprite negativeBar;
            private Sprite positiveBar;
            private Sprite arrow;
            private int displacement;
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
                negativeBar.loadImage(content, Directories.SPRITES + "pickpocketnegativebar");
                positiveBar.loadImage(content, Directories.SPRITES + "pickpocketpositivebar");
                arrow.loadImage(content, Directories.SPRITES + "pickpocketarrow");
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
    