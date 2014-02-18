using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace SunsetHigh
{
    public class StartScreen : AbstractScreen
    {
        private Sprite background;
        private Sprite buttonNewGame;
        private Sprite buttonContinue;
        private Sprite buttonCredits;
        private SpriteFont font;    //temporary stuff until we get art

        private int cursorPosition = 0;
        private GameState stateChange = GameState.StartScreen;

        public StartScreen()
        {
            background = new Sprite();
            buttonNewGame = new Sprite();
            buttonContinue = new Sprite();
            buttonCredits = new Sprite();
        }

        public override void loadContent(ContentManager content)
        {
            background.loadImage(content, Directories.SPRITES + "InGameMenuBackground");
            background.setColor(Color.Gray);
            background.setPosition(0, 0);
            background.setDimensions(800, 600);

            buttonNewGame.loadImage(content, Directories.SPRITES + "InGameMenuBackground");
            buttonNewGame.setColor(Color.Red);

            buttonContinue.loadImage(content, Directories.SPRITES + "InGameMenuBackground");
            buttonContinue.setColor(Color.Blue);
            
            buttonCredits.loadImage(content, Directories.SPRITES + "InGameMenuBackground");
            buttonCredits.setColor(Color.Green);

            font = content.Load<SpriteFont>(Directories.FONTS + "pf_ronda_seven");

            updateButtonHighlighted();
        }

        public override void update(float elapsed)
        {
            background.update(elapsed);
            buttonNewGame.update(elapsed);
            buttonContinue.update(elapsed);
            buttonCredits.update(elapsed);

            KeyboardManager.handleSimpleScreen(this);
        }

        public override void draw(SpriteBatch sb)
        {
            background.draw(sb);
            buttonNewGame.draw(sb);
            buttonContinue.draw(sb);
            buttonCredits.draw(sb);

            sb.DrawString(font, "Sunset High", new Vector2(200, 200), Color.Black);
            sb.DrawString(font, "New Game", new Vector2(360, 305), Color.Black);
            sb.DrawString(font, "Continue", new Vector2(360, 355), Color.Black);
            sb.DrawString(font, "Credits", new Vector2(360, 405), Color.Black);
        }

        public override void confirm()
        {
            switch (cursorPosition)
            {
                case 0:
                    stateChange = GameState.InGame;
                    SaveManager.unpackDefaultData();
                    break;
                case 1:
                    stateChange = GameState.LoadScreen;
                    break;
                case 2:
                    stateChange = GameState.CreditsScreen;
                    break;
            }

            ScreenTransition.requestSimpleTransition(stateChange);
        }

        public override void moveCursor(Direction dir)
        {
            if (dir == Direction.North)
            {
                cursorPosition--;
                if (cursorPosition < 0) cursorPosition = 2;
            }
            if (dir == Direction.South)
            {
                cursorPosition++;
                if (cursorPosition > 2) cursorPosition = 0;
            }
            updateButtonHighlighted();
        }

        private void updateButtonHighlighted()
        {
            buttonNewGame.setPosition(350, 300);
            buttonNewGame.setDimensions(100, 30);
            buttonContinue.setPosition(350, 350);
            buttonContinue.setDimensions(100, 30);
            buttonCredits.setPosition(350, 400);
            buttonCredits.setDimensions(100, 30);
            switch (cursorPosition)
            {
                case 0:
                    buttonNewGame.setPosition(325, 300);
                    buttonNewGame.setDimensions(150, 30);
                    break;
                case 1:
                    buttonContinue.setPosition(325, 350);
                    buttonContinue.setDimensions(150, 30);
                    break;
                case 2:
                    buttonCredits.setPosition(325, 400);
                    buttonCredits.setDimensions(150, 30);
                    break;
            }
        }

    }
}
