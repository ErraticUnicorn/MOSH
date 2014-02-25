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
    public interface IMessagePanel
    {
        void setMessage(string message);
        void setMessageColor(Color color);

        string getMessage();
        Color getMessageColor();
    }

    public interface IListPanel
    {
        void setEntryColor(Color color);
        void setEntryHighlightedColor(Color color);

        Color getEntryColor();
        Color getEntryHighlightedColor();

        void loadEntries(params MenuEntry[] entries);
        void clearEntries();
        MenuEntry getCurrentEntry();
    }

    public abstract class Panel : PopInOutSprite
    {
        private const float POP_TIME = 0.75f;
        private const int DEFAULT_X_MARGIN = 50;
        private const int DEFAULT_Y_MARGIN = 50;

        private bool backgroundVisible;
        private int marginX;
        private int marginY;
        private bool highlighted;
        private BorderSystem borders;

        public Panel()
            : this(0, 0, 0, 0) { }

        public Panel(int x, int y, int width, int height)
            : base(x, y, width, height)
        {
            this.setSmoothMoveType(SmoothMoveType.Sqrt);
            this.setPopDuration(POP_TIME);
            this.setPanelBackgroundVisible(true);
            this.setXMargin(DEFAULT_X_MARGIN);
            this.setYMargin(DEFAULT_Y_MARGIN);
            this.setHighlighted(false);
            this.borders = new BorderSystem(this);
            this.setColor(Color.DarkCyan);

            WorldManager.OffsetChanged += updateOffsets;
        }

        public void setPanelBackgroundVisible(bool visible) { this.backgroundVisible = visible; }
        public void setXMargin(int margin) { this.marginX = margin; }
        public void setYMargin(int margin) { this.marginY = margin; }
        public void setHighlighted(bool highlighted) { this.highlighted = highlighted; }

        public bool isPanelBackgroundVisible() { return this.backgroundVisible; }
        public int getXMargin() { return this.marginX; }
        public int getYMargin() { return this.marginY; }
        public bool isHighlighted() { return this.highlighted; }

        /// <summary>
        /// Invoked when the user types the "Action" or "Confirm" key
        /// </summary>
        public abstract void onConfirm();
        /// <summary>
        /// Invoked when the user types a directional key
        /// </summary>
        /// <param name="dir">The direction</param>
        public virtual void onMoveCursor(Direction dir) { }
        /// <summary>
        /// Invoked when the user types any key
        /// </summary>
        /// <param name="key">The key</param>
        /// <returns>True if state changed (key was meaningful); false otherwise</returns>
        public virtual bool onKeyInput(Keys key) { return false;  }

        /// <summary>
        /// Invoked when this panel has entered the user's view
        /// </summary>
        public virtual void onEnter() { }
        /// <summary>
        /// Invoked when this panel now has focus (i.e. user is navigating this panel)
        /// </summary>
        public virtual void onFocus() { this.setHighlighted(true);  }
        /// <summary>
        /// Invoked when another panel now has the focus (but this panel may still be in view)
        /// </summary>
        public virtual void onUnfocus() { }
        /// <summary>
        /// Invoked when a newer panel has been unfocused, bringing the focus back to this panel
        /// </summary>
        public virtual void onRefocus() { }
        /// <summary>
        /// Invoked when this panel is exiting the user's view
        /// </summary>
        public virtual void onExit() { this.setHighlighted(false);  }

        public override void loadContent(ContentManager content)
        {
            base.loadContent(content);
            this.loadImage(content, Directories.SPRITES + "InGameMenuBackground");
            this.borders.loadContent(content);
        }

        public override void draw(SpriteBatch sb)
        {
            if (!this.isInFocus() && !this.isSmoothMoving())  //i.e. cannot be seen on screen
                return;
            if (!this.isPanelBackgroundVisible())
                return;
            base.draw(sb);
            if (this.isVisible())
                this.borders.draw(sb);
        }

        protected class BorderSystem
        {
            private const int DEFAULT_THICKNESS = 10;

            private Panel parent;
            private int thickness;

            private Texture2D horizontal;
            private Texture2D vertical;
            private Texture2D corner;

            public BorderSystem(Panel parent, int thickness)
            {
                this.parent = parent;
                this.setThickness(thickness);
            }
            public BorderSystem(Panel parent)
                : this(parent, DEFAULT_THICKNESS) { }

            public void setThickness(int thickness) { this.thickness = thickness; }
            
            public void loadContent(ContentManager content)
            {
                this.horizontal = content.Load<Texture2D>(Directories.SPRITES + "InGameMenuBorderHorizontal");
                this.vertical = content.Load<Texture2D>(Directories.SPRITES + "InGameMenuBorderVertical");
                this.corner = content.Load<Texture2D>(Directories.SPRITES + "InGameMenuCorner");
            }

            public void draw(SpriteBatch sb)
            {
                sb.Draw(this.horizontal, new Rectangle(parent.getX(), parent.getY(), parent.getWidth(), this.thickness), Color.White);
                sb.Draw(this.horizontal, new Rectangle(parent.getX(), parent.getY() + (parent.getHeight() - this.thickness), parent.getWidth(), this.thickness), Color.White);
                sb.Draw(this.vertical, new Rectangle(parent.getX(), parent.getY(), this.thickness, parent.getHeight()), Color.White);
                sb.Draw(this.vertical, new Rectangle(parent.getX() + (parent.getWidth() - this.thickness), parent.getY(), this.thickness, parent.getHeight()), Color.White);
                sb.Draw(this.corner, new Rectangle(parent.getX(), parent.getY(), this.thickness, this.thickness), Color.White);
                sb.Draw(this.corner, new Rectangle(parent.getX() + (parent.getWidth() - this.thickness), parent.getY(), this.thickness, this.thickness), Color.White);
                sb.Draw(this.corner, new Rectangle(parent.getX(), parent.getY() + (parent.getHeight() - this.thickness), this.thickness, this.thickness), Color.White);
                sb.Draw(this.corner, new Rectangle(parent.getX() + (parent.getWidth() - this.thickness), parent.getY() + (parent.getHeight() - this.thickness), 
                    this.thickness, this.thickness), Color.White);
            }
        }
    }
}
