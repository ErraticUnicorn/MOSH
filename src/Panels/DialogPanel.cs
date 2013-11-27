using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace SunsetHigh
{
    public class DialogPanel : ListPanel, IMessagePanel
    {
        private string refreshMessage;
        private string message;
        private Color messageColor;
        private bool autoAdjust;

        public DialogPanel()
            : this(0, 0, 0, 0, "No message") { }

        public DialogPanel(int x, int y, int width, int height)
            : this(x, y, width, height, "No message") { }

        public DialogPanel(int x, int y, int width, int height, string message)
            : base(x, y, width, height)
        {
            this.setRefreshMessage(message);
            this.setMessage(message);
            this.setMessageColor(Color.Black);
            this.autoAdjust = true;
        }

        public void setRefreshMessage(string message) { this.refreshMessage = message; }
        public void setMessage(string message) { this.message = message; }
        public void setMessageColor(Color color) { this.messageColor = color; }

        public string getRefreshMessage() { return this.refreshMessage; }
        public string getMessage() { return this.message; }
        public Color getMessageColor() { return this.messageColor; }

        public override void loadContent(ContentManager content)
        {
            base.loadContent(content);
            if (autoAdjust)
                adjustDialogDimensions();
        }

        public override void onRefocus()
        {
            base.onRefocus();
            this.setMessage(this.getRefreshMessage());
        }

        public override void update(float elapsed)
        {
            base.update(elapsed);
        }

        public override void draw(SpriteBatch sb)
        {
            base.draw(sb);
            if (this.isInFocus() || this.isSmoothMoving())  //i.e. can be seen on screen
            {
                sb.DrawString(font, SunsetUtils.wordWrapText(this.message, font, this.getWidth() - 2 * this.getXMargin()),
                    new Vector2(this.getX() + this.getXMargin(), this.getY() + this.getYMargin()),
                    this.getMessageColor());
                //System.Diagnostics.Debug.WriteLine(this.message);
            }
        }

        public void setYDivider(int y)
        {
            alignEntriesHorizontal(this.getXMargin(), this.getWidth() - this.getXMargin(),
                y, this.getHeight() - this.getYMargin());
            autoAdjust = false;
        }

        private void adjustDialogDimensions()
        {
            if (this.message == null || this.font == null)
                return;
            string wrappedText = SunsetUtils.wordWrapText(this.getMessage(), font, this.getWidth() - 2 * this.getXMargin());
            int heightOfText = (int)font.MeasureString("A").Y * wrappedText.Split('\n').Length;
            alignEntriesHorizontal(this.getXMargin(), this.getWidth() - this.getXMargin(),
                this.getYMargin() * 3 / 2 + heightOfText, this.getHeight() - this.getYMargin());
        }
    }
}
