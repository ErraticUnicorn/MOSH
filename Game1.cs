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
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Game1 : Game
    {

        Hero h1;
        List<Character> npcs;
        Character c1;
        Pickup p1;
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

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
            TargetElapsedTime = TimeSpan.FromSeconds(1 / 30.0);
            h1 = new Hero(32 * 2, 32 * 6, 32, 32);
            c1 = new Character(300, 200, 32, 32, "Content\\Phil.libraryQuestInteraction.txt");
            c1.getInventory().addItem(Item.LunchMoney, 100);
            npcs = new List<Character>();
            npcs.Add(c1);
            p1 = new Pickup(500, 100, 24, 24, Item.PokeBall);

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
            c1.loadImage(this.Content, "red_spritesheet", 4, 3, 0.25f);
            c1.loadContent(this.Content);
            p1.loadImage(this.Content, "Poke_Ball", 1, 1, 100.0f); //doesn't animate
            p1.loadContent(this.Content);

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
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            WorldManager.handleWarp(h1);

            KeyboardManager.update();
            KeyboardManager.handleCharacterMovement(h1);
            KeyboardManager.handlePickpocketing(h1, npcs);
            KeyboardManager.handleShooting(h1);
            KeyboardManager.handleTalking(h1, npcs);

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

            if (h1.inRangeAction(p1))
            {
                h1.pickup(p1);
                System.Diagnostics.Debug.WriteLine("Picked up "+Enum.GetName(typeof(Item), p1.getItemType()));
            }

            // TODO: Add your update logic here
            float elapsed = (float)gameTime.ElapsedGameTime.TotalSeconds;
            c1.update(elapsed);
            h1.update(elapsed);
            p1.update(elapsed);

            base.Update(gameTime);
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

            Rectangle l_visibleArea = new Rectangle(l_cameraOffset.X, l_cameraOffset.Y, (int) (GraphicsDevice.Viewport.Width / l_scaleFactor),
                (int) (GraphicsDevice.Viewport.Height / l_scaleFactor));
            WorldManager.drawMap(spriteBatch, l_visibleArea);

            c1.draw(spriteBatch);
            h1.draw(spriteBatch);
            p1.draw(spriteBatch);

            spriteBatch.End();
            base.Draw(gameTime);
        }
    }
}
