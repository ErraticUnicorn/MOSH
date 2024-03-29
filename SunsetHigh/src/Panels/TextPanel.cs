﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace SunsetHigh
{
    public class TextPanel : Panel, IMessagePanel
    {
        private string message;
        private Color messageColor;
        protected SpriteFont font;

        public TextPanel()
            : this(0, 0, 0, 0, "No message") { }

        public TextPanel(int x, int y, int width, int height)
            : this(x, y, width, height, "No message") { }

        public TextPanel(int x, int y, int width, int height, string message)
            : base(x, y, width, height)
        {
            this.setMessage(message);
            this.setMessageColor(Color.Black);
        }

        public virtual void setMessage(string message) { this.message = message; }
        public virtual void setMessageColor(Color color) { this.messageColor = color; }

        public virtual string getMessage() { return this.message; }
        public virtual Color getMessageColor() { return this.messageColor; }

        public override void onConfirm() { }

        public override void loadContent(ContentManager content)
        {
            base.loadContent(content);
            font = content.Load<SpriteFont>(Directories.FONTS + "BabyBlue");
        }

        public override void draw(SpriteBatch sb)
        {
            base.draw(sb);
            if ((this.isInFocus() || this.isSmoothMoving()) && this.isVisible())  //i.e. can be seen on screen
            {
                sb.DrawString(font, SunsetUtils.wordWrapText(this.message, font, this.getWidth() - 2 * this.getXMargin()),
                    new Vector2(this.getX() + this.getXMargin(), this.getY() + this.getYMargin()),
                    this.getMessageColor());
            }
        }
    }
}
