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
        private DialoguePanel dialogue;

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
            dialogue = new DialoguePanel();
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
            if (c.script != null)
            {
                dialogue.talking = true;
                dialogue.loadInteraction(c);
                dialogue.end = false;
            }
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
               dialogue.update(elapsed);
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
        public void startPickpocket(Character character)
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

        public void dialogueChoiceMove(Direction dir)
        {
            this.dialogue.onMoveCursor(dir);
        }

        public void dialogueConfirm()
        {
            this.dialogue.onConfirm();
        }

        /// <summary>
        /// Class to handle the dialogue box/moving through the line tree, etc.
        /// </summary>
        private class DialoguePanel : ListPanel
        {
            private Interaction interaction;
            private InteractionTreeNode current;
            private CursorArrow cursorArrow;
            private string say = "...";
            public bool end = false;
            public bool talking = false;
            private readonly InteractionTreeNode defaultNode = new InteractionTreeNode { 
                eventType = Events.End, line = "You're out of line!", responses = new List<InteractionResponseNode>() };

            private const int DIALOGUE_X_MARGIN = 35;
            private const int DIALOGUE_Y_MARGIN = 15;
            private const int DIALOGUE_WIDTH = 600;
            private const int DIALOGUE_HEIGHT = 200;
            private const int DIALOGUE_X = 400 - (DIALOGUE_WIDTH / 2);
            private const int DIALOGUE_Y = 480 - DIALOGUE_HEIGHT;
            private const int NUM_RESPONSES_ON_PANEL = 3;
            private const int RESPONSE_START_Y = 100;

            public DialoguePanel()
                : base(DIALOGUE_X, DIALOGUE_Y, DIALOGUE_WIDTH, DIALOGUE_HEIGHT)
            {
                this.setVisible(true);
                this.setPopLocations(DIALOGUE_X, DIALOGUE_Y, DIALOGUE_X, DIALOGUE_Y);
                this.popIn();
                this.setHighlighted(true);

                this.cursorArrow = new CursorArrow(DIALOGUE_X, DIALOGUE_Y, 20, 20, this);
                this.cursorArrow.setPopLocations(DIALOGUE_X, DIALOGUE_Y + RESPONSE_START_Y, 
                    DIALOGUE_X, DIALOGUE_Y + RESPONSE_START_Y);
                this.cursorArrow.popIn();
                
                this.setXMargin(DIALOGUE_X_MARGIN);
                this.setYMargin(DIALOGUE_Y_MARGIN);
                this.setScrolling(NUM_RESPONSES_ON_PANEL, 1, this.getXMargin(), this.getWidth() - this.getXMargin(), RESPONSE_START_Y);
                scrollBar.setInitParameters(RESPONSE_START_Y, this.getHeight() - RESPONSE_START_Y - this.getYMargin());
            }

            public string buildString()
            {
                StringBuilder wrappedText = new StringBuilder();
                wrappedText.Append(SunsetUtils.wordWrapText(interaction.name + ": " + current.line, 
                    font, this.getWidth() - this.getXMargin() * 2));

                List<MenuEntry> tempEntries = new List<MenuEntry>();
                for (int i = 0; i < current.responses.Count; ++i)
                {
                    var resp = current.responses[i];
                    tempEntries.Add(new DialogueEntry(SunsetUtils.wordWrapText(current.responses[i].line, 
                        font, this.getWidth() - this.getXMargin() * 2)));
                }
                this.clearEntries();
                this.loadEntries(tempEntries.ToArray());
                this.cursor = 0;
                this.cursorArrow.updateCursor();
                this.cursorArrow.setVisible(tempEntries.Count > 0);
                this.scrollBar.setVisible(tempEntries.Count > NUM_RESPONSES_ON_PANEL);
                return wrappedText.ToString();
            }

            public override void onConfirm()
            {
                InteractionResponseNode next = null;
                if (current.eventType != Events.End && current.eventType != Events.Quest)
                {
                    next = current.responses[cursor];
                }
                else
                {
                    end = true;
                    if (current.eventType == Events.Quest)
                    {
                        Quest.addQuestState(current.questID, current.questState);
                    }
                }
                if (next != null)
                {
                    current = interaction.dialogue.ElementAtOrDefault(next.nextLine - 1) ?? defaultNode;
                    switch (next.eventType)
                    {
                        case Events.End:
                            end = true;
                            break;
                        case Events.Quest:
                            end = true;
                            Quest.addQuestState(next.questID, next.questState);
                            break;
                        default:
                            cursor = 0;
                            break;
                    }
                }
                if (end)
                {
                    talking = false;
                    return;
                }
                say = end ? "(end)" : buildString();
            }

            public override void onMoveCursor(Direction dir)
            {
                base.onMoveCursor(dir);
                cursorArrow.updateCursor();
            }

            public override void loadContent(ContentManager content)
            {
                base.loadContent(content);
                font = content.Load<SpriteFont>(Directories.FONTS + "pf_ronda_seven");
                cursorArrow.loadContent(content);
            }

            public override void draw(SpriteBatch sb)
            {
                if (interaction != null)
                {
                    base.draw(sb);
                    sb.DrawString(font, say, new Vector2(this.getX() + this.getXMargin(), this.getY() + this.getYMargin()), 
                        Color.Black, 0f, new Vector2(), 1.0f, SpriteEffects.None, 0f);
                    cursorArrow.draw(sb);                    
                }
            }

            public override void update(float elapsed)
            {
                base.update(elapsed);
                cursorArrow.update(elapsed);
            }

            public void loadInteraction(Character c)
            {
                interaction = c.script;
                if (interaction != null)
                {
                    current = interaction.getStartingLine();
                    say = buildString();
                    cursor = 0;   // puts cursor at top
                }
            }
        }

        private class DialogueEntry : MenuEntry
        {
            public DialogueEntry()
                : base() { }
            public DialogueEntry(string name)
                : base(name) { }
            public DialogueEntry(string name, int x, int y)
                : base(name, x, y) { }
            public override void onPress()
            {
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
    