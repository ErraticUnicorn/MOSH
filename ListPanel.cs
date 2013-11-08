using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace SunsetHigh
{
    public class ListPanel : Panel, IListPanel
    {
        protected List<MenuEntry> entries;
        protected int cursor;
        protected SpriteFont font;
        private Color entryColor;
        private Color entryHighlightColor;

        //alignment variables
        private bool tempWillAlign;
        private int rows;
        private int cols;
        private int startX;
        private int startY;
        private int endX;
        private int endY;
        private bool rowFirst;

        //scrolling variables
        private ScrollBar scrollBar;
        private bool scrolling;
        private int scrollOffsetBetweenEntries;
        private int scrollRowsOnPanel;
        private int scrollTopRow;

        public ListPanel()
            : this(0, 0, 0, 0) { }

        public ListPanel(int x, int y, int width, int height)
            : base(x, y, width, height)
        {
            this.entries = new List<MenuEntry>();
            this.setEntryColor(Color.Black);
            this.setEntryHighlightedColor(Color.Yellow);

            this.cursor = 0;
            this.tempWillAlign = false;
            this.rows = 1;
            this.cols = 1;
            this.rowFirst = true;
            
            this.scrolling = false;
            this.scrollRowsOnPanel = 0;
            this.scrollBar = new ScrollBar(this);
            this.scrollBar.setVisible(false);
        }

        public void setEntryColor(Color color) { this.entryColor = color; }
        public void setEntryHighlightedColor(Color color) { this.entryHighlightColor = color; }

        public Color getEntryColor() { return this.entryColor; }
        public Color getEntryHighlightedColor() { return this.entryHighlightColor; }

        public void loadEntries(params MenuEntry[] entries)
        {
            foreach (MenuEntry i in entries)
            {
                if (i != null)
                {
                    this.entries.Add(i);
                }
            }
            if (this.isScrolling() && this.entries.Count > 0)
            {
                this.rows = ((this.entries.Count - 1) / this.cols) + 1;
                this.alignEntriesTable(this.rows, this.cols, this.getXMargin(), this.getWidth() - this.getXMargin(),
                    this.getYMargin(), this.getYMargin() + scrollOffsetBetweenEntries * this.rows, this.rowFirst);
                this.scrollBar.adjustHeight(this.scrollRowsOnPanel, this.rows);
            }
        }

        public void clearEntries()
        { 
            this.entries.Clear();
        }

        // only vertical scrolling enabled now
        public void setScrolling(int numRowsVisible, int numCols)
        {
            this.scrolling = true;
            this.scrollBar.setVisible(true);
            this.scrollRowsOnPanel = numRowsVisible;
            this.cols = numCols;
            if (this.cols == 1) this.rowFirst = false;
            else this.rowFirst = true;
            this.scrollTopRow = 0;
            this.scrollOffsetBetweenEntries = (this.getHeight() - this.getYMargin() * 2) / this.scrollRowsOnPanel;
            //int xoffset = (this.getWidth() - this.getXMargin() * 2) / this.cols;
        }
        public bool isScrolling() { return this.scrolling; }


        public void alignEntriesVertical()
        {
            alignEntriesTable(this.entries.Count, 1, 0 + this.getXMargin(), this.getWidth() - this.getXMargin(),
                0 + this.getYMargin(), this.getHeight() - this.getYMargin(), false);
        }
        public void alignEntriesVertical(int startX, int endX, int startY, int endY)
        {
            alignEntriesTable(this.entries.Count, 1, startX, endX, startY, endY, false);
        }
        public void alignEntriesHorizontal()
        {
            alignEntriesTable(1, this.entries.Count, 0 + this.getXMargin(), this.getWidth() - this.getXMargin(),
                0 + this.getYMargin(), this.getHeight() - this.getYMargin(), true);
        }
        public void alignEntriesHorizontal(int startX, int endX, int startY, int endY)
        {
            alignEntriesTable(1, this.entries.Count, startX, endX, startY, endY, true);
        }
        public void alignEntriesTable(int rows, int cols, bool rowFirst = true)
        {
            alignEntriesTable(rows, cols, 0 + this.getXMargin(), this.getWidth() - this.getXMargin(),
                0 + this.getYMargin(), this.getHeight() - this.getYMargin(), rowFirst);
        }
        public void alignEntriesTable(int rows, int cols, int startX, int endX, int startY, int endY, bool rowFirst = true)
        {
            this.rows = rows;
            this.cols = cols;
            this.rowFirst = rowFirst;
            this.startX = startX;
            this.startY = startY;
            this.endX = endX;
            this.endY = endY;

            if (this.getHeight() == 0 || this.getWidth() == 0 || this.font == null)
            {
                this.tempWillAlign = true;
                return;             // if content has not been loaded yet, we can't align yet
            }
            if (this.entries.Count > rows * cols || rows == 0 || cols == 0)
                return; // bad arguments/uninitialized

            int offsetX = (endX - startX) / cols;
            int offsetY = (endY - startY) / rows;
            
            int middleOffsetX = 0;
            int middleOffsetY = 0;
            if (rows == 1 && rowFirst)
                middleOffsetX = offsetX / 2 - ((int)font.MeasureString(entries[0].getName()).X / 2);
            middleOffsetY = offsetY / 2 - ((int)font.MeasureString(entries[0].getName()).Y / 2);

            for (int i = 0; i < this.entries.Count; i++)
            {
                if (rowFirst)
                {
                    this.entries[i].setX(startX + offsetX * (i % cols) + middleOffsetX);
                    this.entries[i].setY(startY + offsetY * (i / cols) + middleOffsetY);
                }
                else
                {
                    this.entries[i].setX(startX + offsetX * (i / rows) + middleOffsetX);
                    this.entries[i].setY(startY + offsetY * (i % rows) + middleOffsetY);
                }
            }
        }

        public MenuEntry getCurrentEntry()
        {
            if (this.entries == null || this.entries.Count == 0)
                return null;
            return this.entries[this.cursor];
        }
        public List<MenuEntry> getEntries()
        {
            return this.entries;
        }

        public override void onFocus()
        {
            base.onFocus();
            if (this.entries.Count > 0)
                this.entries[this.cursor].onHover();
        }
        public override void onRefocus()
        {
            base.onRefocus();
            if (this.entries.Count > 0)
                this.entries[this.cursor].onHover();
        }
        public override void onExit()
        {
            base.onExit();
            if (this.entries.Count > 0)
                this.entries[this.cursor].onUnhover();
        }

        public override void onMoveCursor(Direction dir)
        {
            if (this.entries.Count == 0)
                return;

            int oldCursor = this.cursor;

            if (dir.Equals(Direction.North))
            {
                if (this.rowFirst)
                {
                    this.cursor -= this.cols;
                    if (this.cursor < 0) this.cursor += this.rows * this.cols;
                    while (this.cursor >= this.entries.Count) this.cursor -= this.cols;
                }
                else
                {
                    this.cursor -= 1;
                    if ((this.cursor + 1) % this.rows == 0) this.cursor += this.rows;
                    while (this.cursor >= this.entries.Count) this.cursor -= 1;
                }
            }
            if (dir.Equals(Direction.East))
            {
                if (this.rowFirst)
                {
                    this.cursor += 1;
                    if (this.cursor >= this.entries.Count) this.cursor = (this.rows - 1) * this.cols;
                    //else if (this.cursor % this.cols == 0) this.cursor -= this.cols;
                }
                else
                {
                    this.cursor += this.rows;
                    if (this.cursor >= this.rows * this.cols) this.cursor %= (this.rows * this.cols);
                    else if (this.cursor >= this.entries.Count) this.cursor %= this.rows;
                }
            }
            if (dir.Equals(Direction.South))
            {
                if (this.rowFirst)
                {
                    this.cursor += this.cols;
                    if (this.cursor >= this.rows * this.cols) this.cursor %= (this.rows * this.cols);
                    else if (this.cursor >= this.entries.Count) this.cursor %= this.cols;
                }
                else
                {
                    this.cursor += 1;
                    if (this.cursor >= this.entries.Count) this.cursor = (this.cols - 1) * this.rows;
                    else if (this.cursor % this.rows == 0) this.cursor -= this.rows;
                }
            }
            if (dir.Equals(Direction.West))
            {
                if (this.rowFirst)
                {
                    this.cursor -= 1;
                    if (this.cursor < 0) this.cursor += this.cols;
                    //if ((this.cursor + 1) % this.cols == 0) this.cursor += this.cols;
                    //while (this.cursor >= this.entries.Count) this.cursor -= 1;
                }
                else
                {
                    this.cursor -= this.rows;
                    if (this.cursor < 0) this.cursor += this.rows * this.cols;
                    while (this.cursor >= this.entries.Count) this.cursor -= this.rows;
                }
            }

            if (this.isScrolling())
            {
                if (this.cursor >= (this.scrollTopRow + this.scrollRowsOnPanel) * this.cols
                    || this.cursor < this.scrollTopRow * this.cols)
                {
                    int oldScrollCurrentRow = this.scrollTopRow;
                    int newScrollCurrentRow = this.cursor / this.cols;
                    if (newScrollCurrentRow > oldScrollCurrentRow)
                    {
                        if (newScrollCurrentRow - (this.scrollRowsOnPanel - 1) >= 0)
                            this.scrollTopRow = newScrollCurrentRow - (this.scrollRowsOnPanel - 1);
                        else
                            this.scrollTopRow = 0;
                    }
                    else
                        this.scrollTopRow = newScrollCurrentRow;

                    this.scrollBar.adjustY(this.scrollTopRow, this.rows);
                    foreach (MenuEntry entry in this.entries)
                    {
                        entry.setY(entry.getY() - (this.scrollTopRow - oldScrollCurrentRow) * scrollOffsetBetweenEntries);
                    }
                }
            }

            if (this.cursor != oldCursor)
            {
                this.entries[oldCursor].onUnhover();
                this.entries[this.cursor].onHover();
            }
        }

        public override void onConfirm()
        {
            if (this.entries.Count == 0)
                return;
            this.entries[this.cursor].onPress();
        }

        public override void loadContent(ContentManager content)
        {
            base.loadContent(content);
            font = content.Load<SpriteFont>("BabyBlue");
            scrollBar.loadContent(content);
            if (this.tempWillAlign)
                alignEntriesTable(this.rows, this.cols, this.startX, this.endX, this.startY, this.endY, this.rowFirst);
        }
        public override void update(float elapsed)
        {
            base.update(elapsed);
            if (this.isScrolling())
            {
                scrollBar.update(elapsed);
            }
        }
        public override void draw(SpriteBatch sb)
        {
            base.draw(sb);
            if (this.isInFocus() || this.isSmoothMoving())  //i.e. can be seen on screen
            {
                if (this.isScrolling())
                {
                    for (int i = scrollTopRow * cols; i < (scrollTopRow + scrollRowsOnPanel) * cols && i < entries.Count; i++)
                    {
                        drawHelper(sb, i);
                    }
                }
                else
                {
                    //draw all list entries
                    for (int i = 0; i < this.entries.Count; i++)
                    {
                        drawHelper(sb, i);
                    }
                }
                if (this.isScrolling() && this.rows > this.scrollRowsOnPanel)
                {
                    scrollBar.draw(sb);
                }
            }
        }

        private void drawHelper(SpriteBatch sb, int i)
        {
            if (this.entries[i].getX() < this.getWidth() && this.entries[i].getY() < this.getHeight()
                && this.entries[i].getX() > 0 && this.entries[i].getY() > 0)
            {
                Color c = this.getEntryColor();
                if (this.cursor == i && this.isHighlighted()) c = this.getEntryHighlightedColor();
                this.entries[i].draw(sb, this.getX(), this.getY(), font, c);
            }
        }

        private class ScrollBar : SmoothMovingSprite
        {
            private const int DEFAULT_WIDTH = 20;
            private const int DEFAULT_X_OFFSET_FROM_RIGHT_MARGIN = 0;
            private const float SMOOTH_MOVE_TIME = 0.25f;

            private int x_relative;
            private int y_relative;
            private int x_offset;
            private int y_offset;
            private Panel owner;

            public ScrollBar(Panel owner, int width)
                : base(0, 0, width, width)
            {
                this.owner = owner;
                x_relative = owner.getWidth() - owner.getXMargin() + DEFAULT_X_OFFSET_FROM_RIGHT_MARGIN;
                y_relative = owner.getYMargin();
                x_offset = owner.getX();
                y_offset = owner.getY();
                this.setX(x_offset + x_relative);
                this.setY(y_offset + y_relative);
            }
            public ScrollBar(Panel owner)
                : this(owner, DEFAULT_WIDTH) { }

            public void adjustHeight(int rowsOnScreen, int totalRows)
            {
                int space = owner.getHeight() - 2 * owner.getYMargin();
                this.setHeight((int)(space * (1.0f * rowsOnScreen / totalRows)));
            }

            public void adjustY(int topRow, int totalRows)
            {
                int spacePerRow = (owner.getHeight() - 2 * owner.getYMargin()) / totalRows;
                y_relative = owner.getYMargin() + (spacePerRow * topRow);
                if (owner.isMovingIn())
                {
                    x_offset = owner.getAppearX();
                    y_offset = owner.getAppearY();
                }
                else if (owner.isMovingOut())
                {
                    x_offset = owner.getHideX();
                    y_offset = owner.getHideY();
                }
                if (!owner.isSmoothMoving())
                    this.smoothMove(x_offset + x_relative, y_offset + y_relative, SMOOTH_MOVE_TIME);
            }

            public override void loadContent(ContentManager content)
            {
                this.loadImage(content, "scrollbar");
                this.setColor(Color.Black);
            }

            public override void update(float elapsed)
            {
                base.update(elapsed);
                if (owner.isSmoothMoving())
                {
                    x_offset = owner.getX();
                    y_offset = owner.getY();
                    this.setX(x_offset + x_relative);
                    this.setY(y_offset + y_relative);
                }
            }
        }
    }
}
