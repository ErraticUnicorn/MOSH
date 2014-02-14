using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace SunsetHigh
{
    public class SplashScreen : AbstractScreen
    {
        private Sprite background;
        private SpriteFont font;

        private const float SHOW_TIME = 1.0f;
        private float showTimer = 0.0f;

        public SplashScreen()
        {
            background = new Sprite(0, 0, 800, 600);
        }

        public override void loadContent(ContentManager content)
        {
            background.loadImage(content, Directories.SPRITES + "InGameMenuBackground");
            background.setColor(Color.Black);

            font = content.Load<SpriteFont>(Directories.FONTS + "pf_ronda_seven");
        }

        public override void update(float elapsed)
        {
            showTimer += elapsed;
            if (showTimer > SHOW_TIME)
            {
                ScreenTransition.requestSimpleTransition(GameState.StartScreen);
            }
        }

        public override void draw(SpriteBatch sb)
        {
            background.draw(sb);
            sb.DrawString(font, "SPLASH SCREEN!", new Vector2(350, 275), Color.White);
        }

        public override void refresh()
        {
            showTimer = 0.0f;
            // play the title track
            BGMusic.transitionToSong("sunset high menu track.mp3");
        }
    }
}
