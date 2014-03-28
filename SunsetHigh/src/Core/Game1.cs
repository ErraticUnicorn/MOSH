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
        SplashScreen = 0,
        StartScreen = 1, 
        LoadScreen = 2,
        CreditsScreen = 3,
        InGame = 4,
    }

    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Game1 : Game
    {
        private static GameState gameState;
        private static bool refreshRequested;

        SpriteBatch spriteBatch;
        private List<AbstractScreen> gameScreens;

        public Game1()
            : base()
        {
            new GraphicsDeviceManager(this);
        }

        public static void changeScreen(GameState newState)
        {
            gameState = newState;
            refreshRequested = true;
        }

        public static GameState getGameState()
        {
            return gameState;
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

            gameScreens = new List<AbstractScreen>();
            //The order we add these screens is important!!
            //It must correspond with the order of the GameState enum
            gameScreens.Add(new SplashScreen());
            gameScreens.Add(new StartScreen());
            gameScreens.Add(new LoadGameScreen());
            gameScreens.Add(new CreditsScreen());
            gameScreens.Add(new MainGameScreen());

            gameState = GameState.SplashScreen;

            //initialize misc stuff
            WorldManager.init(GraphicsDevice, 1.0f);

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

            //play title song first
            BGMusic.playSong("sunset high menu track.mp3");
            //BGMusic.setPaused(true);

            ScreenTransition.loadContent(Content);
            foreach (AbstractScreen screen in gameScreens)
            {
                screen.loadContent(Content);
            }
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
            KeyboardManager.update();
            float elapsed = (float)gameTime.ElapsedGameTime.TotalSeconds;   //time elapsed since last update
            //debug
            if (KeyboardManager.isKeyDown(Keys.E))
            {
                BGMusic.fadeOut();
                BGMusic.fadeIn();
            }
            //end debug
            if (refreshRequested)
            {
                refreshRequested = false;
                gameScreens[(int)gameState].refresh();
            }

            ScreenTransition.update(elapsed);
            if (!ScreenTransition.isTransitioning())
            {
                gameScreens[(int)gameState].update(elapsed);
            }
            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            if (gameState == GameState.InGame)  //apply the appropriate transformations if in-game
            {
                Point l_cameraOffset = WorldManager.m_currentCameraOffset;
                Matrix l_cameraMatrix = Matrix.CreateTranslation(-l_cameraOffset.X, -l_cameraOffset.Y, 0) * Matrix.CreateScale(1.0f);
                spriteBatch.Begin(0, null, null, null, null, null, l_cameraMatrix);
            }
            else
            {
                spriteBatch.Begin();
            }

            GraphicsDevice.Clear(Color.CornflowerBlue);
            gameScreens[(int)gameState].draw(spriteBatch);
            ScreenTransition.draw(spriteBatch);

            spriteBatch.End();
            base.Draw(gameTime);
        }
    }
}
