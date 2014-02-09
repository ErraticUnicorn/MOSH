using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace SunsetHigh
{
    public class CreditsScreen : AbstractScreen
    {
        private Sprite background;
        private SpriteFont font;

        public CreditsScreen()
        {
            background = new Sprite();
        }

        public override void loadContent(ContentManager content)
        {
            background.loadImage(content, Directories.SPRITES + "InGameMenuBackground");
            background.setColor(Color.Gray);
            background.setPosition(0, 0);
            background.setDimensions(800, 600);

            font = content.Load<SpriteFont>(Directories.FONTS + "pf_ronda_seven");
        }

        public override void update(float elapsed)
        {
            KeyboardManager.handleSimpleScreen(this);
        }

        public override void draw(SpriteBatch sb)
        {
            background.draw(sb);
            sb.DrawString(font, "Look! It's a credits screen! Press 'Z' or 'X' to go back...", new Vector2(200, 300), Color.Green);
        }

        public override void confirm()
        {
            ScreenTransition.requestSimpleTransition(GameState.StartScreen);
        }
        public override void cancel()
        {
            this.confirm();
        }
    }
}
