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
     * Class for anything that can be drawn 
     * with a sprite
     * 
     * Characters and Pickups (and other drawable game 
     * components) can extend from this class (or just use this
     * class for simple or static drawings)
     */
    public class Sprite
    {
        //geometry
        private int spriteX, spriteY;               //top-left corner of sprite and collision box
        private int spriteWidth, spriteHeight;      //size of sprite, collision box
        //image properties
        private Texture2D image;                    //the picture or spritesheet to draw
        private int imageRows, imageColumns;        //rows and columns in spritesheet; (1, 1) for static pictures
        private int frameColumn;                    //the current frame in the sheet we are drawing; 0 for static pictures
        private int frameRow;                       //the current row of the sheet we are drawing; 0 for static pictures
        private float animationTime;                //speed at which to animate sprite; arbitrary time for static pictures
        private float totalElapsed;                 //personal timer for animation purposes
        private bool visible;                       //sprite visibility
        private bool useTextureDimensions;          //used for matching width and height to texture's dimensions
        
        public Sprite()
        {
            setX(0); setY(0);
            setWidth(1); setHeight(1); //a tiny pixel until the image loads
            useTextureDimensions = true;
            setVisible(true);
        }

        public Sprite(int x, int y, int width, int height)
        {
            setX(x);
            setY(y);
            setWidth(width);
            setHeight(height);
            useTextureDimensions = false;
            setVisible(true);
        }

        public int getX() { return this.spriteX; }
        public int getY() { return this.spriteY; }
        public int getXCenter() { return this.spriteX + this.spriteWidth / 2; }
        public int getYCenter() { return this.spriteY + this.spriteHeight / 2; }
        public int getWidth() { return this.spriteWidth; }
        public int getHeight() { return this.spriteHeight; }
        public Texture2D getImage() { return this.image; }
        public int getImageRows() { return this.imageRows; }
        public int getImageColumns() { return this.imageColumns; }
        public float getAnimationTime() { return this.animationTime; }
        public bool isVisible() { return this.visible;  }

        public void setX(int x) { this.spriteX = x; }
        public void setY(int y) { this.spriteY = y; }
        public void setXCenter(int x) { this.spriteX = x - this.spriteWidth / 2; }
        public void setYCenter(int y) { this.spriteY = y - this.spriteHeight / 2; }
        public void setWidth(int width) { this.spriteWidth = width; }
        public void setHeight(int height) { this.spriteHeight = height; }
        public void setVisible(bool visible) { this.visible = visible; }
        protected void setFrameRow(int row) { this.frameRow = row; }
        protected void setFrameColumn(int col) { this.frameColumn = col; }
        protected void setImage(Texture2D image) { this.image = image; }
        protected void setAnimationTime(float time) { this.animationTime = time; }
        protected void matchToTextureDimensions() 
        { 
            this.useTextureDimensions = true;
            if (this.image != null)
            {
                setWidth(image.Width);
                setHeight(image.Height);
            }
        }

        public void setCenter(int x, int y)
        {
            this.setXCenter(x);
            this.setYCenter(y);
        }

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
         * Call this method from child classes when you know
         * which spritesheets to load
         */
        public void loadImage(ContentManager content, string fileName, int numRows, int numCols, float anTime)
        {
            this.image = content.Load<Texture2D>(fileName);
            this.frameRow = 0;
            this.frameColumn = 0;
            this.imageRows = numRows;
            this.imageColumns = numCols;
            this.animationTime = anTime;
            if (this.useTextureDimensions)
                matchToTextureDimensions();
        }

        public void loadImage(ContentManager content, string fileName)
        {
            this.image = content.Load<Texture2D>(fileName);
            this.frameRow = 0;
            this.frameColumn = 0;
            this.imageRows = 1;
            this.imageColumns = 1;
            this.animationTime = 100.0f;
            if (this.useTextureDimensions)
                matchToTextureDimensions();
        }

        /*
         * Call this for all sprites in a given room
         * in the application's load cycle
         */
        public virtual void loadContent(ContentManager content)
        {
            //Child classes call loadImage() from here
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
