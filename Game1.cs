#region Using Statements
using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Storage;
using Microsoft.Xna.Framework.Media;
using Microsoft.Xna.Framework.Audio;
using TiledLib;
#endregion

namespace SunsetHigh
{
    public enum GameState
    {
        StartScreen, 
        InGame
    }

    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Game1 : Game
    {
        Hero h1;
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        GameState gameState;
        public Game1()
            : base()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            // TODO: Add your initialization logic here
            TargetElapsedTime = TimeSpan.FromSeconds(1 / 45.0);

            StartScreen.init();

            h1 = Hero.instance;
            h1.setX(32 * 2);
            h1.setY(32 * 6);

            /*
            h1.getInventory().addItem(Item.PokeBall, 3);
            h1.getInventory().addItem(Item.LunchMoney, 1);
            h1.getInventory().addItem(Item.filler1);
            h1.getInventory().addItem(Item.filler2);
            h1.getInventory().addItem(Item.filler3);
            h1.getInventory().addItem(Item.Hat);
            h1.getInventory().addItem(Item.filler4);
            h1.getInventory().addItem(Item.filler5);
            h1.getInventory().addItem(Item.filler6);
            h1.getInventory().addItem(Item.filler7);
            h1.getInventory().addItem(Item.filler8);
            h1.getInventory().addItem(Item.filler9);
            h1.getInventory().addItem(Item.Shoes);
            h1.getInventory().addItem(Item.Socks);

            InGameMenu.init();
            InGameMenu.loadInventoryPanel(h1.getInventory());
             */
            //Quest.setTrigger(QuestID.FoodFight1);

            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            // TODO: use this.Content to load your game content here
            WorldManager.loadMaps(Content);

            //In the future, all Sprites will call loadContent(this.Content), and child
            //classes will override that method to automatically choose the appropriate 
            //content to load (i.e. both images and sound)
            h1.loadImage(this.Content, "red_spritesheet", 4, 3, 0.25f);
            h1.loadContent(this.Content);

            StartScreen.loadContent(Content);
            //InGameMenu.loadContent(Content);

           //BGMusic.playSong("Stickerbrush_Symphony.m4a"); 
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here

            BGMusic.dispose();
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            //if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
            //    Exit();

            if (gameState == GameState.StartScreen)
            {
                KeyboardManager.update();
                if (KeyboardManager.isNewKeyPressed())
                    gameState = GameState.InGame;   //annoying to click
            }

            else
            {
                float elapsed = (float)gameTime.ElapsedGameTime.TotalSeconds;
                double l_scaleFactor = 1.0;
                Point l_cameraOffset = WorldManager.getCameraOffset(h1, GraphicsDevice, l_scaleFactor);

                WorldManager.handleWarp(h1);

                KeyboardManager.update();
                
                //KeyboardManager.handleInGameMenu(l_cameraOffset.X, l_cameraOffset.Y);
                //if (InGameMenu.isExiting())
                //    InGameMenu.updateMovingOffsets(l_cameraOffset.X, l_cameraOffset.Y);
                //if (!InGameMenu.isOpen())
                //{
                    KeyboardManager.handleCharacterMovement(h1, elapsed);
                    KeyboardManager.handlePickpocketing(h1, WorldManager.m_currentRoom.CharList);
                    KeyboardManager.handleShooting(h1);
                    KeyboardManager.handleTalking(h1, WorldManager.m_currentRoom.CharList);
                //}
                
                /*
                if (KeyboardManager.isKeyPressed(Keys.S))
                {
                    SaveGameData data = new SaveGameData();
                    data.fileName = "savegame.sav";
                    data.heroInventory = h1.getInventory().toIntArray();
                    data.heroName = "JAY";
                    data.heroX = h1.getX();
                    data.heroY = h1.getY();
                    data.heroDirection = h1.getDirection();
                    data.inputKeys = KeyboardManager.getKeyControls();
                    data.questTriggers = Quest.getTriggers();
                    SaveManager.saveGame("savegame.sav", data);
                }

                if (KeyboardManager.isKeyPressed(Keys.D))
                {
                    SaveGameData data = SaveManager.loadGame("savegame.sav");
                    h1.getInventory().loadIntArray(data.heroInventory);
                    h1.setName(data.heroName);
                    h1.setX(data.heroX);
                    h1.setY(data.heroY);
                    h1.setDirection(data.heroDirection);
                    KeyboardManager.loadKeyControls(data.inputKeys);
                    Quest.loadTriggers(data.questTriggers);
                }

                if (KeyboardManager.isKeyPressed(Keys.F))
                {
                    SaveManager.loadAllGames();
                }
                */

                //CollisionManager.CollisionWithCharacter(h1, c1);
                //CollisionManager.CollisionWithProjectiles(h1, c1);

                //if (h1.inRangeCollide(p1))
                //{
                //    h1.pickup(p1);
                //    System.Diagnostics.Debug.WriteLine("Picked up "+Enum.GetName(typeof(Item), p1.getItemType()));
                //}

                // TODO: Add your update logic here

                //InGameMenu.update(elapsed);
                //if (!InGameMenu.isOpen())
                //{
                    h1.update(elapsed);
                    WorldManager.update(elapsed);
                //}
                foreach (Sprite s in WorldManager.m_currentRoom.Interactables)
                {
                    if (h1.inRangeCollide(s))
                    {
                        WorldManager.setRoom("map_Hallway");     //NOTE HARD CODED
                        h1.setX(12 * 32);
                        h1.setY(3 * 32);
                    }
                }

                base.Update(gameTime);

            }
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            double l_scaleFactor = 1.0;
            Point l_cameraOffset = WorldManager.getCameraOffset(h1, GraphicsDevice, l_scaleFactor);
            Matrix l_cameraMatrix = Matrix.CreateTranslation(-l_cameraOffset.X, -l_cameraOffset.Y, 0) * Matrix.CreateScale((float) l_scaleFactor);

            spriteBatch.Begin(0, null, null, null, null, null, l_cameraMatrix);

            if (gameState == GameState.StartScreen)
            {
                StartScreen.draw(spriteBatch);

            }

            else
            {
                Rectangle l_visibleArea = new Rectangle(l_cameraOffset.X, l_cameraOffset.Y, (int)(GraphicsDevice.Viewport.Width / l_scaleFactor),
                    (int)(GraphicsDevice.Viewport.Height / l_scaleFactor));
                WorldManager.drawMap(spriteBatch, l_visibleArea);

                h1.draw(spriteBatch);

                //InGameMenu.draw(spriteBatch);

            }

            spriteBatch.End();
            base.Draw(gameTime);
        }
    }
}
