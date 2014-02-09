using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace SunsetHigh
{
    /// <summary>
    /// Handles the fade in and out between different screens or scenes.
    /// </summary>
    public static class ScreenTransition
    {
        private const float FADE_TIME = 1.5f;   //in seconds

        private static Texture2D fadeBG;
        private static float fadeTimer = 0.0f;
        private static bool fading = false;
        private static Action endAction;
        private static bool actionTaken = false;

        public static bool isTransitioning()
        {
            return fading;
        }

        /// <summary>
        /// Starts a fade out. The given Action is a delegate for whatever should
        /// be executed between the fade out and fade in (e.g. load a new screen).
        /// </summary>
        /// <param name="action"></param>
        public static void requestTransition(Action action)
        {
            fading = true;
            fadeTimer = 0.0f;
            endAction = action;
            actionTaken = false;
        }

        public static void requestSimpleTransition(GameState stateChange)
        {
            ScreenTransition.requestTransition(delegate()
            {
                Game1.changeScreen(stateChange);
            });
        }

        public static void loadContent(ContentManager content)
        {
            fadeBG = content.Load<Texture2D>(Directories.SPRITES + "InGameMenuBackground");
        }

        public static void update(float elapsed)
        {
            if (fading)
            {
                fadeTimer += elapsed;
                if (fadeTimer > FADE_TIME)
                {
                    fading = false;
                    fadeTimer = 0.0f;
                }
                if (fadeTimer > FADE_TIME / 2 && !actionTaken)
                {
                    if (endAction != null)
                        endAction();
                    actionTaken = true;
                }
            }
        }

        public static void draw(SpriteBatch sb)
        {
            if (fading)
            {
                if (fadeTimer < FADE_TIME / 2)
                {
                    float fadeFactor = fadeTimer / (FADE_TIME / 2);
                    Point offset = Point.Zero;
                    if (Game1.getGameState() == GameState.InGame) offset = WorldManager.m_currentCameraOffset;
                    sb.Draw(fadeBG, new Rectangle(offset.X, offset.Y, 800, 600), new Color(Color.Black, fadeFactor));
                }
                if (fadeTimer >= FADE_TIME / 2)
                {
                    float fadeFactor = 1.0f - ((fadeTimer - (FADE_TIME / 2)) / (FADE_TIME / 2));
                    Point offset = Point.Zero;
                    if (Game1.getGameState() == GameState.InGame) offset = WorldManager.m_currentCameraOffset;
                    sb.Draw(fadeBG, new Rectangle(offset.X, offset.Y, 800, 600), new Color(Color.Black, fadeFactor));
                }

            }
        }
    }
}
