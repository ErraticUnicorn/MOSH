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
        private const float SCALE_FACTOR = 1.0f;    //other factors break the panels

        Hero h1;
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        GameState gameState;

        public Game1()
            : base()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = ""; //Note: no "Content" !!
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


            h1.inventory.addItem(Item.PokeBall, 3);
            h1.inventory.addItem(Item.LunchMoney, 1);
            h1.inventory.addItem(Item.Hat);
            h1.inventory.addItem(Item.Shoes);
            h1.inventory.addItem(Item.Socks);
            h1.inventory.removeItem(Item.Hat);

            InGameMenu.init();
            InGameMenu.loadInventoryPanel(h1.inventory);

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
            InGameMenu.loadContent(Content);
            LocationNamePanel.instance.loadContent(Content);

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
                float elapsed = (float)gameTime.ElapsedGameTime.TotalSeconds;   //time elapsed since last update
                KeyboardManager.update();                                       //updates current and previous frames of key input

                // Teleport character to new room
                WorldManager.handleWarp(h1);
                // Update the offset based on new room / character position
                WorldManager.updateCameraOffset(h1, GraphicsDevice, SCALE_FACTOR);
               
                // Keyboard listening
                KeyboardManager.handleInGameMenu();
                if (!InGameMenu.isOpen())
                {
                    KeyboardManager.handleCharacterMovement(h1, elapsed);
                    KeyboardManager.handlePickpocketing(h1, WorldManager.m_currentRoom.CharList);
                    KeyboardManager.handleShooting(h1);
                    //KeyboardManager.handleTalking(h1, WorldManager.m_currentRoom.CharList);
                    KeyboardManager.handleInteractions(h1, WorldManager.m_currentRoom.Interactables);
                } 
                
                // Updates based on time
                InGameMenu.update(elapsed);
                if (!InGameMenu.isOpen())
                {
                    LocationNamePanel.instance.update(elapsed);
                    h1.update(elapsed);
                    WorldManager.update(elapsed);
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

            Point l_cameraOffset = WorldManager.m_currentCameraOffset;
            Matrix l_cameraMatrix = Matrix.CreateTranslation(-l_cameraOffset.X, -l_cameraOffset.Y, 0) * Matrix.CreateScale(SCALE_FACTOR);

            spriteBatch.Begin(0, null, null, null, null, null, l_cameraMatrix);

            if (gameState == GameState.StartScreen)
            {
                StartScreen.draw(spriteBatch);

            }

            else
            {
                Rectangle l_visibleArea = new Rectangle(l_cameraOffset.X, l_cameraOffset.Y, (int)(GraphicsDevice.Viewport.Width / SCALE_FACTOR),
                    (int)(GraphicsDevice.Viewport.Height / SCALE_FACTOR));
                WorldManager.drawMap(spriteBatch, l_visibleArea);

                h1.draw(spriteBatch);

                LocationNamePanel.instance.draw(spriteBatch);
                InGameMenu.draw(spriteBatch);

            }

            spriteBatch.End();
            base.Draw(gameTime);
        }
    }
}
