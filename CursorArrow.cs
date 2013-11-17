using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace SunsetHigh
{
    public class CursorArrow : PopInOutSprite
    {
        private const int ARROW_X_OFFSET = -25;
        private const int ARROW_Y_OFFSET = 0;
        private const float ARROW_SCROLL_TIME = 0.25f;
        private const float POP_TIME = 0.75f;

        private Panel panel;
        private MenuEntry targetEntry;

        public CursorArrow(int x, int y, int width, int height, Panel panel)
            : base(x, y, width, height)
        {
            this.panel = panel;
            this.setSmoothMoveType(SmoothMoveType.Sqrt);
            this.setPopDuration(POP_TIME);

            WorldManager.OffsetChanged += updateOffsets;
        }
        public CursorArrow(int x, int y, int width, int height)
            : this(x, y, width, height, null) { }
        public CursorArrow(int x, int y)
            : this(x, y, 0, 0, null) { }
        public CursorArrow()
            : this(0, 0, 0, 0, null) { }

        public void moveToActivePanel(Panel panel)
        {
            if (panel == null)
                return;
            this.panel = panel;
            if (panel is IListPanel)
            {
                if (this.targetEntry != ((IListPanel)panel).getCurrentEntry())
                {
                    this.targetEntry = ((IListPanel)panel).getCurrentEntry();
                    moveToTargetEntry(POP_TIME);
                }
            }
        }

        public void updateCursor()
        {
            if (this.panel is IListPanel)
            {
                if (this.targetEntry != ((IListPanel)panel).getCurrentEntry())
                {
                    this.targetEntry = ((IListPanel)panel).getCurrentEntry();
                    moveToTargetEntry(ARROW_SCROLL_TIME);
                }
            }
        }

        private void moveToTargetEntry(float moveTime)
        {
            if (this.targetEntry == null || this.panel == null)
                return;
            this.smoothMove(this.panel.getAppearX() + this.targetEntry.getX() + ARROW_X_OFFSET,
                this.panel.getAppearY() + this.targetEntry.getY() + ARROW_Y_OFFSET,
                moveTime);
            //System.Diagnostics.Debug.WriteLine(this.getX() + " " + this.getY());
        }

        public override void loadContent(ContentManager content)
        {
            base.loadContent(content);
            this.loadImage(content, "InGameMenuArrow");
        }
        public override void update(float elapsed)
        {
            base.update(elapsed);
            if (this.targetEntry != null && this.panel != null && this.panel is IListPanel
                && !this.isSmoothMoving() && !this.isHiding() && !this.atTargetEntry())
                moveToTargetEntry(ARROW_SCROLL_TIME);  //move to newer location
        }
        private bool atTargetEntry()
        {
            if (this.targetEntry != null && this.panel != null)
            {
                if (this.getX() == this.panel.getAppearX() + this.targetEntry.getX() + ARROW_X_OFFSET &&
                    this.getY() == this.panel.getAppearY() + this.targetEntry.getY() + ARROW_Y_OFFSET)
                    return true;
            }
            return false;
        }
    }
}
