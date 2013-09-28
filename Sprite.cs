using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;

namespace SunsetHigh
{
    /*
     * Abstract class for anything that can be drawn 
     * with a sprite
     * 
     * Characters and Pickups (and other drawable game 
     * components) can extend from this class
     */
    public abstract class Sprite
    {
        //geometry
        private int spriteX, spriteY;               //top-left corner of sprite and collision box
        private int spriteWidth, spriteHeight;      //size of sprite, collision box
        //image properties
        private Texture2D image;                    //the picture or spritesheet to draw
        private int imageRows, imageColumns;        //rows and columns in spritesheet
        private int frameColumn;                    //the current frame in the sheet we are drawing
        private int frameRow;                       //the current row of the sheet we are drawing
        private float animationTime;                //speed at which to animate sprite
        private float totalElapsed;                 //personal timer for animation purposes
        private bool visible;                       //sprite visibility

        public Sprite()
        {
            setX(0); setY(0);
            setWidth(1); setHeight(1); //a tiny pixel..
            setVisible(true);
        }

        public Sprite(int x, int y, int width, int height)
        {
            setX(x);
            setY(y);
            setWidth(width);
            setHeight(height);
            setVisible(true);
        }

        public int getX() { return this.spriteX; }
        public int getY() { return this.spriteY; }
        public int getWidth() { return this.spriteWidth; }
        public int getHeight() { return this.spriteHeight; }
        public Texture2D getImage() { return this.image; }
        public int getImageRows() { return this.imageRows; }
        public int getImageColumns() { return this.imageColumns; }
        public float getAnimationTime() { return this.animationTime; }
        public bool isVisible() { return this.visible;  }

        public void setX(int x) { this.spriteX = x; }
        public void setY(int y) { this.spriteY = y; }
        public void setWidth(int width) { this.spriteWidth = width; }
        public void setHeight(int height) { this.spriteHeight = height; }
        public void setVisible(bool visible) { this.visible = visible; }
        protected void setFrameRow(int row) { this.frameRow = row; }
        protected void setFrameColumn(int col) { this.frameColumn = col; }
        protected void setImage(Texture2D image) { this.image = image; }
        protected void setAnimationTime(float time) { this.animationTime = time; }

        public void setPosition(int x, int y)
        {
            this.setX(x);
            this.setY(y);
        }

        public void setDimensions(int width, int height)
        {
            this.setWidth(width);
            this.setHeight(height);
        }

        /*
         * Call this for all sprites in a given room
         * in the application's load cycle
         */
        public void loadImage(ContentManager content, string fileName, int numRows, int numCols, float anTime)
        {
            this.image = content.Load<Texture2D>(fileName);
            this.frameRow = 0;
            this.frameColumn = 0;
            this.imageRows = numRows;
            this.imageColumns = numCols;
            this.animationTime = anTime;
        }

        /*
         * Call this for all sprites in the application's
         * update cycle
         * 
         * Updates which frame on sprite sheet to draw, given
         * the time that has elapsed since last frame
         * Adapted from MSDN XNA tutorials
         * Override this in child classes as necessary
         */
        public virtual void update(float elapsed)
        {
            this.totalElapsed += elapsed;
            if (this.totalElapsed > this.animationTime)
            {
                this.frameColumn++;
                this.frameColumn = this.frameColumn % this.imageColumns;
                this.totalElapsed %= this.animationTime;
            }
        }

        /*
         * Call this for all sprites in the application's
         * draw cycle
         * 
         * Draws itself!
         * Adapted from MSDN XNA tutorials
         * Override this in child classes as necessary
         */
        public virtual void draw(SpriteBatch sb)
        {
            if (image == null || !this.isVisible())
                return;

            int frameWidth = this.getImage().Width / this.getImageColumns();
            int frameHeight = this.getImage().Height / this.getImageRows();
            Rectangle sourceRect = new Rectangle(frameWidth * this.frameColumn,
                frameHeight * this.frameRow, frameWidth, frameHeight);
            Rectangle mapPosRect = new Rectangle(this.getX(), this.getY(),
                this.getWidth(), this.getHeight());
            sb.Draw(this.getImage(), mapPosRect, sourceRect, Color.White);
            //more features available in sb.Draw(...);
        }
    }
}
