using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;

namespace SunsetHigh
{
    public class LoadGameScreen : AbstractScreen
    {
        private Sprite background;
        private Texture2D saveFileBG;
        private SpriteFont font;

        private const int NUM_ENTRIES_ON_SCREEN = 5;
        private int cursorPosition = 0;
        private int topRowPosition = 0;
        private List<SaveGameData> saveList;

        public LoadGameScreen()
        {
            background = new Sprite();
            saveList = new List<SaveGameData>();
        }

        public override void loadContent(ContentManager content)
        {
            background.loadImage(content, Directories.SPRITES + "InGameMenuBackground");
            background.setColor(Color.Gray);
            background.setPosition(0, 0);
            background.setDimensions(800, 600);

            saveFileBG = content.Load<Texture2D>(Directories.SPRITES + "InGameMenuBackground");
            font = content.Load<SpriteFont>(Directories.FONTS + "pf_ronda_seven");
        }

        public override void update(float elapsed)
        {
            KeyboardManager.handleSimpleScreen(this);
        }

        public override void draw(SpriteBatch sb)
        {
            background.draw(sb);
            sb.DrawString(font, "Load your save file!", new Vector2(400, 20), Color.Black);

            for (int i = topRowPosition; i < topRowPosition + 5; i++)
            {
                Vector2 textPosition = new Vector2(200, 70 + 50 * i);
                Color textColor = Color.Black;
                if (i == cursorPosition) textColor = Color.Yellow;
                sb.Draw(saveFileBG, new Rectangle(20, (int)textPosition.Y - 5, 800 - 40, 40), Color.Red);
                if (i >= saveList.Count || i < 0)
                {
                    sb.DrawString(font, "No save file", textPosition, textColor);
                }
                else
                {
                    sb.DrawString(font, saveList[i].heroData.name, textPosition, textColor);
                    sb.DrawString(font, GameClock.formatTimeSpan(saveList[i].playTime), new Vector2(textPosition.X + 200, textPosition.Y), textColor);
                }
            }
        }

        public override void refresh()
        {
            saveList = SaveManager.loadAllGames(false);
            cursorPosition = 0;
            topRowPosition = 0;
        }

        public override void moveCursor(Direction dir)
        {
            if (saveList.Count > 0)
            {
                if (dir == Direction.North)
                {
                    cursorPosition--;
                    if (cursorPosition < 0) cursorPosition = saveList.Count - 1;
                }
                if (dir == Direction.South)
                {
                    cursorPosition++;
                    if (cursorPosition >= saveList.Count) cursorPosition = 0;
                }
                if (topRowPosition > cursorPosition)
                    topRowPosition = cursorPosition;
                if (topRowPosition + NUM_ENTRIES_ON_SCREEN <= cursorPosition)
                    topRowPosition = cursorPosition - NUM_ENTRIES_ON_SCREEN + 1;
            }
        }

        public override void confirm()
        {
            if (saveList.Count > 0)
            {
                ScreenTransition.requestTransition(delegate()
                {
                    SaveManager.unpackData(saveList[cursorPosition]);
                    Game1.changeScreen(GameState.InGame);
                    WorldManager.updateCameraOffset(Hero.instance);
                });
            }
        }

        public override void cancel()
        {
            ScreenTransition.requestSimpleTransition(GameState.StartScreen);
        }
    }
}
