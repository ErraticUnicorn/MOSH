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
    /// Structure for saving data about a Hero.
    /// </summary>
    public struct HeroSave
    {
        public InventorySave inventorySave;
        public int x;
        public int y;
        public Direction dir;
        public bool[] monologueSave;
        public int[] reputationSave;
        public string name;
        public PersonID followerID;
    }

    /// <summary>
    /// Hero extends from Character, and is used for the main character
    /// </summary>
    public sealed class Hero : Character
    {
        //pickpocket vars
        private bool ppActive;  //if currently pickpocketing
        private Character ppTarget;     //the target of the pickpocket
        private PickpocketSystem ppSystem;  //the graphics associated with the pickpocket minigame
        //private const float MAX_PICKPOCKET_DISTANCE = 50;

        //Shooting vars
        private const float RECHARGE_TIME = 0.5f;   //time between shots in seconds
        private static string PROJECTILE_IMAGE_NAME = Directories.SPRITES + "projectile";
        //private Texture2D paperball; //paperball texture
        private float shootTimer;   // For recharge time
        private bool canShoot;      // whether player can shoot a projectile
        
        //Speech vars
        private DialoguePanel dialogue;
        private InnerMonologue monologue;

        //Reputation vars
        private int nerdRep;
        private int jockRep;
        private int prepRep;
        private int bullyRep;
        private int slackerRep;

        //Follow vars (special cases where a character follows you)
        private PersonID follower;
        private const float FOLLOW_TIME = 0.2f;

        //Battle vars
        private const int DEFAULT_MAX_HEALTH = 300;
        private int health;
        private bool healthBarVisible;
        private HealthBarTexture healthBar;

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
            canShoot = true;
            shootTimer = 0.0f;
            dialogue = new DialoguePanel();
            monologue = new InnerMonologue();
            nerdRep = 0;
            jockRep = 0;
            prepRep = 0;
            bullyRep = 0;
            slackerRep = 0;
            healthBar = new HealthBarTexture();
            this.setHealth(DEFAULT_MAX_HEALTH);
            follower = PersonID.None;
        }

        //public override Rectangle getBoundingRect()
        //{
        //   return new Rectangle(this.getX(), this.getY() + Room.TILE_SIZE, Room.TILE_SIZE, Room.TILE_SIZE);
        //}

        public override void loadContent(ContentManager content)
        {
            base.loadContent(content);
            ppSystem.loadContent(content);
            dialogue.loadContent(content);
            healthBar.loadContent(content);
            SoundFX.loadSound(content, Directories.SOUNDS + "LTTP_Rupee1");
            Sprite.loadCommonImage(content, PROJECTILE_IMAGE_NAME);
            monologue.loadEntriesFromFile(Directories.TEXTDATA + "MonologueLines.txt");
        }

        public override void draw(SpriteBatch sb)
        {
            base.draw(sb);
            if (ppActive)
            {
                ppSystem.draw(sb);
            }

            if (this.isTalking())
            {
                dialogue.draw(sb);
            }
            if (healthBarVisible)
                healthBar.draw(sb);
        }

        public override void update(float elapsed) 
        {
            if (!this.isTalking())
            {
                base.update(elapsed);
            }
            else
            {
                dialogue.update(elapsed);
            }

            if (ppActive)
            {
                ppSystem.update(this, elapsed);
            }
            shootTimer += elapsed;
            if (shootTimer >= RECHARGE_TIME)
                canShoot = true;
        }

        public override void reset()
        {
            base.reset();
            dialogue.talking = false;
            dialogue.selfTalking = false;
            this.setHealth(DEFAULT_MAX_HEALTH);
        }

        public override bool move(Direction dir, float elapsed, bool collide = true)
        {
            if (follower != PersonID.None)
            {
                bool retVal;
                retVal = base.move(dir, elapsed, collide);
                if (retVal)
                {
                    Vector2 destPos = new Vector2(this.getX(), this.getY());
                    switch (this.getDirection())
                    {
                        case Direction.North: destPos.Y += 32; break;
                        case Direction.South: destPos.Y -= 32; break;
                        case Direction.East: destPos.X -= 32; break;
                        case Direction.West: destPos.X += 32; break;
                    }
                    Character c1 = CharacterManager.getCharacter(follower);
                    c1.moveToDestination((int)destPos.X, (int)destPos.Y, null);
                }
                return retVal;
            }
            else
                return base.move(dir, elapsed, collide);
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

        public void setHealthBarVisible(bool visible) { healthBarVisible = visible; }
        public void setHealth(int health) { 
            this.health = health;
            this.healthBar.setFullRatio((float)health / DEFAULT_MAX_HEALTH);
        }
        public int getHealth() { return this.health; }
        public void shiftHealth(int dhp) { this.setHealth(this.getHealth() + dhp); }

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
            return dialogue.talking || dialogue.selfTalking;
        }

        public bool hasFollower()
        {
            return this.follower != PersonID.None;
        }
        public PersonID getFollowerID()
        {
            return this.follower;
        }
        public void setFollower(PersonID follower)
        {
            this.follower = follower;
            if (this.follower != PersonID.None)
                CollisionManager.excludeInteractableCollision(CharacterManager.getCharacter(follower));
            else
                CollisionManager.resetInteractableCollisions();
        }
        public void stopFollower()
        {
            if (this.follower != PersonID.None)
                CollisionManager.includeInteractableCollision(CharacterManager.getCharacter(follower));
            this.follower = PersonID.None;
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
                bullet.addCollideEvent(new ProjectileCollideEvent(heroBulletCollideEvent));
                WorldManager.enqueueObjectToCurrentRoom(bullet);

                canShoot = false;
                shootTimer = 0.0f;
            }
        }

        private void heroBulletCollideEvent(IInteractable collider)
        {
            if (collider is BraceFace)
            {
                BraceFace b1 = (BraceFace)collider;
                b1.shiftHealth(-1);
                //dec health
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
            this.monologue.loadSaveStructure(data.monologueSave);
            if (data.reputationSave.Length >= 5)
            {
                this.nerdRep = data.reputationSave[0];
                this.jockRep = data.reputationSave[1];
                this.prepRep = data.reputationSave[2];
                this.bullyRep = data.reputationSave[3];
                this.slackerRep = data.reputationSave[4];
            }
            this.setFollower(data.followerID);
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
            data.monologueSave = this.monologue.getSaveStructure();
            data.reputationSave = new int[] { nerdRep, jockRep, prepRep, bullyRep, slackerRep };
            data.followerID = follower;
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

        public void dialogueCancel()
        {
            // nothing much yet
            if (dialogue.selfTalking)
                this.dialogue.onConfirm();
        }

        public void talkToSelf()
        {
            dialogue.loadSelfThought(monologue.chooseLine());
            dialogue.selfTalking = true;
        }

        /// <summary>
        /// Class to handle the dialogue box/moving through the line tree, etc.
        /// </summary>
        private class DialoguePanel : ListPanel
        {
            private Interaction interaction;
            private InteractionLinkedTreeNode current;
            private CursorArrow cursorArrow;

            private string say = "...";
            private int scrollTextIndex = 0;
            private float responseShowPauseTimer = 0.0f;
            private Texture2D advanceArrowTexture;
            private float advanceArrowFlashTimer = 0.0f;
            private float characterAdvanceTimer = 0.0f;
            public bool end = false;
            public bool talking = false;
            public bool selfTalking = false;
            private readonly InteractionLinkedTreeNode defaultNode = new InteractionLinkedTreeNode { 
                eventType = Events.End, line = "You're out of line!", responses = new List<InteractionTreeNode>() };

            private const int DIALOGUE_X_MARGIN = 35;
            private const int DIALOGUE_Y_MARGIN = 15;
            private const int DIALOGUE_WIDTH = 800;
            private const int DIALOGUE_HEIGHT = 175;
            private const int DIALOGUE_X = 400 - (DIALOGUE_WIDTH / 2);
            private const int DIALOGUE_Y = 480 - DIALOGUE_HEIGHT;
            private const int NUM_RESPONSES_ON_PANEL = 2;
            private const int RESPONSE_START_Y = 100;
            private const float SCROLL_TEXT_TIME = 0.020f;   //seconds for one character
            private const float PUNCTUATION_TIME_FACTOR = 8.0f;
            private const float RESPONSE_SHOW_PAUSE_TIME = 0.2f;    //pause between when text is displayed and responses appear
            private const int ADVANCE_ARROW_DISPLACEMENT = 50;
            private const float ADVANCE_ARROW_FLASH_SPEED = 3.0f;
            private const string PUNCTUATION = ".!?,";

            public DialoguePanel()
                : base(DIALOGUE_X, DIALOGUE_Y, DIALOGUE_WIDTH, DIALOGUE_HEIGHT)
            {
                this.setVisible(true);
                this.setPopLocations(DIALOGUE_X, DIALOGUE_Y, DIALOGUE_X, DIALOGUE_Y);
                this.popIn();
                this.setHighlighted(true);

                this.cursorArrow = new CursorArrow(DIALOGUE_X, DIALOGUE_Y + RESPONSE_START_Y, 20, 20, this);
                this.cursorArrow.setPopLocations(DIALOGUE_X, DIALOGUE_Y + RESPONSE_START_Y, 
                    DIALOGUE_X, DIALOGUE_Y + RESPONSE_START_Y);
                this.cursorArrow.popIn();
                
                this.setXMargin(DIALOGUE_X_MARGIN);
                this.setYMargin(DIALOGUE_Y_MARGIN);
                this.setScrolling(NUM_RESPONSES_ON_PANEL, 1, this.getXMargin(), this.getWidth() - this.getXMargin(), RESPONSE_START_Y);
                scrollBar.setInitParameters(RESPONSE_START_Y, this.getHeight() - RESPONSE_START_Y - this.getYMargin());
            }

            private string renewPanelContent()
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
                this.hideEntries();
                this.cursor = 0;
                this.cursorArrow.updateCursor();
                this.cursorArrow.setVisible(false);
                this.scrollTextIndex = 2 + interaction.name.Length;
                this.responseShowPauseTimer = 0.0f;
                this.advanceArrowFlashTimer = 0.0f;
                return wrappedText.ToString();
            }

            public override void onConfirm()
            {
                if (!selfTalking)   //having dialogue instead of thinking
                {
                    if (scrollTextIndex < say.Length)
                    {
                        scrollTextIndex = say.Length;
                    }
                    else if (responseShowPauseTimer == RESPONSE_SHOW_PAUSE_TIME)
                    {
                        if (current.responses.Count == 0)   //there are no responses to NPC's line
                        {
                            gameStateUpdateHelper(current);
                        }
                        else
                        {
                            InteractionTreeNode next = current.responses[cursor];
                            gameStateUpdateHelper(next);
                        }
                        if (end)
                        {
                            talking = false;
                            // !! update the room state based on what happened in the dialogue
                            WorldManager.m_currentRoom.updateState();
                        }
                        else
                        {
                            say = renewPanelContent();
                        }
                    }
                }
                else
                {
                    if (scrollTextIndex < say.Length)
                    {
                        scrollTextIndex = say.Length;
                    }
                    else if (responseShowPauseTimer == RESPONSE_SHOW_PAUSE_TIME)
                    {
                        selfTalking = false;
                        end = true;
                    }
                }
            }

            private void gameStateUpdateHelper(InteractionTreeNode node)
            {
                if ((node.eventType & Events.Quest) > 0)
                {
                    Quest.addQuestState(node.questID, node.questState);
                }
                if ((node.eventType & Events.Reputation) > 0)
                {
                    Hero.instance.shiftReputation(node.repClique, node.repChange);
                }
                if ((node.eventType & Events.Inventory) > 0)
                {
                    if (node.itemChange > 0)
                        Hero.instance.inventory.addItem(node.item, node.itemChange);
                    else
                        Hero.instance.inventory.removeItem(node.item, -1 * node.itemChange);
                }
                if ((node.eventType & Events.Special) > 0)
                {
                    DialogueEventParser.parseEvent(node.specialEvent);
                }
                if ((node.eventType & Events.End) > 0 || (node.eventType & Events.NextLine) == 0)
                {
                    end = true;
                }
                if ((node.eventType & Events.NextLine) > 0)
                {
                    current = interaction.dialogue.ElementAtOrDefault(node.nextLine - 1) ?? defaultNode;
                }
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
                advanceArrowTexture = content.Load<Texture2D>(Directories.SPRITES + "black_arrow_right");
            }

            public override void draw(SpriteBatch sb)
            {
                if (interaction != null || selfTalking)
                {
                    base.draw(sb);
                    sb.DrawString(font, say.Substring(0, scrollTextIndex), new Vector2(this.getX() + this.getXMargin(), this.getY() + this.getYMargin()), 
                        Color.Black, 0f, new Vector2(), 1.0f, SpriteEffects.None, 0f);
                    cursorArrow.draw(sb);
                    if (this.entries.Count == 0 && responseShowPauseTimer == RESPONSE_SHOW_PAUSE_TIME)
                    {
                        sb.Draw(advanceArrowTexture, 
                            new Vector2(this.getX() + this.getWidth() - ADVANCE_ARROW_DISPLACEMENT, this.getY() + this.getHeight() - ADVANCE_ARROW_DISPLACEMENT), 
                            new Color(Color.White, 0.3f*(float)(Math.Cos(advanceArrowFlashTimer * ADVANCE_ARROW_FLASH_SPEED) + 0.8f)));
                    }
                }
            }

            public override void update(float elapsed)
            {
                base.update(elapsed);
                cursorArrow.update(elapsed);
                if (scrollTextIndex < say.Length)
                {
                    characterAdvanceTimer += elapsed;
                    float factor = 1.0f;
                    if (scrollTextIndex > 0 && scrollTextIndex < say.Length - 2 &&
                        PUNCTUATION.Contains(say[scrollTextIndex - 1])) factor = PUNCTUATION_TIME_FACTOR;
                    if (characterAdvanceTimer > SCROLL_TEXT_TIME * factor)
                    {
                        scrollTextIndex++;
                        characterAdvanceTimer = 0.0f;
                    }
                    if (scrollTextIndex > say.Length) scrollTextIndex = say.Length;
                }
                if (scrollTextIndex == say.Length && responseShowPauseTimer < RESPONSE_SHOW_PAUSE_TIME)
                {
                    responseShowPauseTimer += elapsed;
                    if (responseShowPauseTimer > RESPONSE_SHOW_PAUSE_TIME)
                    {
                        responseShowPauseTimer = RESPONSE_SHOW_PAUSE_TIME;
                        this.unhideEntries();
                        this.cursorArrow.setVisible(this.entries.Count > 0);
                    }
                }
                if (this.entries.Count == 0 && responseShowPauseTimer == RESPONSE_SHOW_PAUSE_TIME)
                {
                    advanceArrowFlashTimer += elapsed;
                }
            }

            public void loadInteraction(Character c)
            {
                interaction = c.script;
                if (interaction != null)
                {
                    current = interaction.getStartingLine();
                    say = renewPanelContent();
                    cursor = 0;   // puts cursor at top
                    scrollTextIndex = 0;
                    responseShowPauseTimer = 0.0f;
                    advanceArrowFlashTimer = 0.0f;
                }
            }

            public void loadSelfThought(string str)
            {
                say = SunsetUtils.wordWrapText(str, font, this.getWidth() - this.getXMargin() * 2);

                this.clearEntries();
                this.cursor = 0;
                this.scrollTextIndex = 0;
                this.responseShowPauseTimer = 0.0f;
                this.advanceArrowFlashTimer = 0.0f;
                this.cursorArrow.updateCursor();
                this.cursorArrow.setVisible(false);
                //this.scrollBar.setVisible(false);
            }
        }

        //dummy class, has no real use
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

        private class HealthBarTexture  //A container for the health bar texture
        {
            private const int X_OFFSET = 50;
            private const int Y_OFFSET = 50;
            private const int OUTLINE_THICKNESS = 5;
            private const int BAR_WIDTH = 100;
            private const int BAR_HEIGHT = 100 / 3;
            private Texture2D box;
            private int mX = 0;
            private int mY = 0;
            private float mFullRatio = 1.0f;

            public HealthBarTexture()
            {
                WorldManager.OffsetChanged += updateOffsets;
            }
            public void loadContent(ContentManager content)
            {
                box = content.Load<Texture2D>(Directories.SPRITES + "InGameMenuBackground");
            }
            public void setFullRatio(float fullRatio)
            {
                mFullRatio = fullRatio;
            }
            public void draw(SpriteBatch sb)
            {
                sb.Draw(box, new Rectangle(mX + X_OFFSET, mY + Y_OFFSET, (int)(mFullRatio * BAR_WIDTH), BAR_HEIGHT), Color.Green);
                sb.Draw(box, new Rectangle(mX + X_OFFSET - OUTLINE_THICKNESS, mY + Y_OFFSET - OUTLINE_THICKNESS, BAR_WIDTH + OUTLINE_THICKNESS * 2, OUTLINE_THICKNESS), Color.Black);
                sb.Draw(box, new Rectangle(mX + X_OFFSET- OUTLINE_THICKNESS, mY + Y_OFFSET - OUTLINE_THICKNESS, OUTLINE_THICKNESS, BAR_HEIGHT + OUTLINE_THICKNESS * 2), Color.Black);
                sb.Draw(box, new Rectangle(mX + X_OFFSET + BAR_WIDTH, mY + Y_OFFSET - OUTLINE_THICKNESS, OUTLINE_THICKNESS, BAR_HEIGHT + OUTLINE_THICKNESS * 2), Color.Black);
                sb.Draw(box, new Rectangle(mX + X_OFFSET - OUTLINE_THICKNESS, mY + Y_OFFSET + BAR_HEIGHT, BAR_WIDTH + OUTLINE_THICKNESS * 2, OUTLINE_THICKNESS), Color.Black);
            }
            private void updateOffsets(object sender, CameraOffsetEventArgs e)
            {
                mX += e.dx_offset;
                mY += e.dy_offset;
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
    