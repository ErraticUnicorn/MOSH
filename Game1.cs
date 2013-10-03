﻿#region Using Statements
using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Storage;
using Microsoft.Xna.Framework.Media;
using Microsoft.Xna.Framework.Audio;
#endregion

namespace SunsetHigh
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Game1 : Game
    {

        Hero h1;
        Character c1;
        Pickup p1;
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        KeyboardState priorKeyboardState;

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
            h1 = new Hero(100, 100, 32, 32);
            c1 = new Character(300, 200, 32, 32);
            c1.getInventory().addItem(Item.LunchMoney, 100);
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

            //BGMusic.dispose();
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

            arrowKeyUpdate();       //controls main character

            //debug debug debug
            if (Keyboard.GetState().IsKeyDown(Keys.P) && !priorKeyboardState.IsKeyDown(Keys.P))
            {
                if (!h1.isPickpocketing() && h1.inRangeAction(c1))
                {
                    h1.startPickpocket(c1);
                }
                else if (h1.isPickpocketing())
                {
                    Item item = h1.stopPickpocket();
                    System.Diagnostics.Debug.WriteLine("Stole " + Enum.GetName(typeof(Item), item));
                }
            }

            if (h1.inRangeAction(p1))
            {
                h1.pickup(p1);
                System.Diagnostics.Debug.WriteLine("Picked up "+Enum.GetName(typeof(Item), p1.getItemType()));
            }

            priorKeyboardState = Keyboard.GetState();
            //end debug

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

            // TODO: Add your drawing code here
            spriteBatch.Begin();

            c1.draw(spriteBatch);
            h1.draw(spriteBatch);
            p1.draw(spriteBatch);

            spriteBatch.End();
            base.Draw(gameTime);
        }

        private void arrowKeyUpdate()
        {
            int dirFlags = 0;
            if (Keyboard.GetState().IsKeyDown(Keys.Up))
                dirFlags |= 1;
            if (Keyboard.GetState().IsKeyDown(Keys.Down))
                dirFlags |= 2;
            if (Keyboard.GetState().IsKeyDown(Keys.Right))
                dirFlags |= 4;
            if (Keyboard.GetState().IsKeyDown(Keys.Left))
                dirFlags |= 8;
            Direction xaxis = Direction.Undefined;
            Direction yaxis = Direction.Undefined;
            if ((dirFlags & 1) > 0 ^ (dirFlags & 2) > 0)
            {
                if ((dirFlags & 1) > 0) yaxis = Direction.North;
                else yaxis = Direction.South;
            }
            if ((dirFlags & 4) > 0 ^ (dirFlags & 8) > 0)
            {
                if ((dirFlags & 4) > 0) xaxis = Direction.East;
                else xaxis = Direction.West;
            }
            if (!xaxis.Equals(Direction.Undefined) && !yaxis.Equals(Direction.Undefined))
            {
                h1.move2D(xaxis, yaxis, 3, 4);
            }
            else if (!xaxis.Equals(Direction.Undefined))
            {
                h1.move(xaxis, 5);
            }
            else if (!yaxis.Equals(Direction.Undefined))
            {
                h1.move(yaxis, 5);
            }
            else
            {
                h1.setMoving(false);
            }
        }
    }
}
