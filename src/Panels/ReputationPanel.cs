using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace SunsetHigh
{
    public class ReputationPanel : Panel
    {
        private const int BAR_WIDTH = 263;
        private const int BAR_HEIGHT = 30;
        private const int NEEDLE_WIDTH = 8;
        private const int NEEDLE_HEIGHT = 41;
        private const int BAR_X_OFFSET = 125;
        private const int TEXT_X_OFFSET = 50;

        private const int NUM_TYPES = 5;

        private SpriteFont font;
        private Texture2D barTexture;
        private Texture2D needleTexture;

        public ReputationPanel()
            : this(0, 0, 0, 0) { }
        public ReputationPanel(int x, int y, int width, int height)
            : base(x, y, width, height)
        {
        }

        public override void onConfirm()
        {
            InGameMenu.goBack();
        }

        public override void loadContent(ContentManager content)
        {
            base.loadContent(content);
            font = content.Load<SpriteFont>(Directories.FONTS + "BabyBlue");
            barTexture = content.Load<Texture2D>(Directories.SPRITES + "reputation_bar");
            needleTexture = content.Load<Texture2D>(Directories.SPRITES + "reputation_needle");
        }
        public override void draw(SpriteBatch sb)
        {
            base.draw(sb);
            int space = this.getHeight() - 2 * this.getYMargin();
            int spacePerEntry = space / NUM_TYPES;
            for (int i = 0; i < NUM_TYPES; i++)
            {
                int offsetY = this.getY() + this.getYMargin() +
                    (spacePerEntry * i) + (spacePerEntry / 2);
                switch (i)
                {
                    case 0:
                        drawRepHelper(sb, offsetY, Clique.Nerd);
                        break;
                    case 1:
                        drawRepHelper(sb, offsetY, Clique.Jock);
                        break;
                    case 2:
                        drawRepHelper(sb, offsetY, Clique.Prep);
                        break;
                    case 3:
                        drawRepHelper(sb, offsetY, Clique.Bully);
                        break;
                    case 4:
                        drawRepHelper(sb, offsetY, Clique.Slacker);
                        break;
                }
            }
        }

        private void drawRepHelper(SpriteBatch sb, int offsetY, Clique clique)
        {
            if (this.isInFocus() || this.isSmoothMoving())  //i.e. can be seen on screen
            {
                Hero h1 = Hero.instance;
                string cliqueHeader = SunsetUtils.enumToString<Clique>(clique) + ":";
                int offsetNeedleX = h1.getReputation(clique);
                // we need some sort of scale for rep (min / max), needle will move accordingly
                // my idea is having a log(reputation) scale - it moves quickly at first, then slowly
                sb.DrawString(font, cliqueHeader,
                    new Vector2(this.getX() + TEXT_X_OFFSET, offsetY - font.MeasureString(cliqueHeader).Y / 2), Color.Black);
                sb.Draw(barTexture, new Rectangle(this.getX() + BAR_X_OFFSET, offsetY - BAR_HEIGHT / 2,
                    BAR_WIDTH, BAR_HEIGHT), Color.White);
                sb.Draw(needleTexture, new Rectangle(this.getX() + BAR_X_OFFSET + BAR_WIDTH / 2 + offsetNeedleX - NEEDLE_WIDTH / 2,
                    offsetY - NEEDLE_HEIGHT / 2,
                    NEEDLE_WIDTH, NEEDLE_HEIGHT), Color.White);
            }
        }
    }
}
