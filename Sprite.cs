using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;

namespace SunsetHigh
{
    /// <summary>
    /// Class for anything that can be drawn with a sprite.
    /// Characters and Pickups (and other drawable game 
    /// components) can extend from this class to add behavior 
    /// to the sprite. Alternative, just use this class for 
    /// simple or static drawings.
    /// </summary>
    public class Sprite
    {
        //geometry
        private int spriteX, spriteY;               //top-left corner of sprite and collision box
        private int spriteWidth, spriteHeight;      //size of sprite, collision box
        //image properties
        private Texture2D image;                    //the picture or spritesheet to draw
        private Rectangle sourceRect;               //Source rectangle (if drawing from a texture with a collection of sprites)
        private int imageRows, imageColumns;        //rows and columns in spritesheet; (1, 1) for static pictures
        private int frameColumn;                    //the current frame in the sheet we are drawing; 0 for static pictures
        private int frameRow;                       //the current row of the sheet we are drawing; 0 for static pictures
        private float animationTime;                //speed at which to animate sprite; arbitrary time for static pictures
        private float totalElapsed;                 //personal timer for animation purposes
        private bool visible;                       //sprite visibility
        private bool useTextureDimensions;          //used for matching width and height to texture's dimensions
        
        /// <summary>
        /// Initializes a Sprite at the origin which will match the dimensions
        /// of its texture when the texture is loaded
        /// </summary>
        public Sprite()
        {
            setX(0); setY(0);
            setWidth(1); setHeight(1); //a tiny pixel until the image loads
            useTextureDimensions = true;
            setVisible(true);
        }

        /// <summary>
        /// Initializes a Sprite at the given position. It will match the dimensions
        /// of its texture when the texture is loaded.
        /// </summary>
        /// <param name="x">X coordinate of top-left corner</param>
        /// <param name="y">Y coordinate of top-left corner</param>
        public Sprite(int x, int y)
        {
            setX(x);
            setY(y);
            useTextureDimensions = true;
            setVisible(true);
        }

        /// <summary>
        /// Initializes a Sprite at the given position with the given dimensions
        /// </summary>
        /// <param name="x">X coordinate of top-left corner</param>
        /// <param name="y">Y coordinate of top-left corner</param>
        /// <param name="width">Width in pixels</param>
        /// <param name="height">Height in pixels</param>
        public Sprite(int x, int y, int width, int height)
        {
            setX(x);
            setY(y);
            setWidth(width);
            setHeight(height);
            useTextureDimensions = false;
            setVisible(true);
        }

        /// <summary>
        /// Initializes a Sprite at the given position with the 
        /// </summary>
        /// <returns></returns>

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
        public void setFrameRow(int row) { this.frameRow = row; }
        public void setFrameColumn(int col) { this.frameColumn = col; }
        public void setAnimationTime(float time) { this.animationTime = time; }
        public void matchToTextureDimensions() 
        { 
            this.useTextureDimensions = true;
            if (this.image != null)
            {
                setWidth(image.Width / this.getImageColumns());
                setHeight(image.Height / this.getImageRows());
            }
        }

        /// <summary>
        /// Sets the position of this Sprite with given center coordinates
        /// </summary>
        /// <param name="x">X coordinate of center</param>
        /// <param name="y">Y coordinate of center</param>
        public void setCenter(int x, int y)
        {
            this.setXCenter(x);
            this.setYCenter(y);
        }

        /// <summary>
        /// Sets the position of this Sprite given the coordinates of its top-left corner
        /// </summary>
        /// <param name="x">X coordinate of top-left corner</param>
        /// <param name="y">Y coordinate of top-left corner</param>
        public void setPosition(int x, int y)
        {
            this.setX(x);
            this.setY(y);
        }

        /// <summary>
        /// Sets the dimensions of this Sprite
        /// </summary>
        /// <param name="width">Width in pixels</param>
        /// <param name="height">Height in pixels</param>
        public void setDimensions(int width, int height)
        {
            this.setWidth(width);
            this.setHeight(height);
        }

        /// <summary>
        /// Loads a 2D spritesheet with the given filename in the "Content" directory
        /// </summary>
        /// <param name="content">ContentManager passed in through Game</param>
        /// <param name="fileName">File name of XNB image in "Content" directory</param>
        /// <param name="numRows">Number of rows in this spritesheet</param>
        /// <param name="numCols">Number of columns in this spritesheet</param>
        /// <param name="anTime">Time between frames (in seconds) when animating the sprite</param>
        public void loadImage(ContentManager content, string fileName, int numRows, int numCols, float anTime)
        {
            if (fileName.EndsWith(".xnb"))
                fileName = fileName.Substring(0, fileName.Length - 4); //.xnb extension is not neccesary
            try
            {
                this.image = content.Load<Texture2D>(fileName);
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine("Load texture failed!\n" + e.StackTrace);
            }
            this.frameRow = 0;
            this.frameColumn = 0;
            this.imageRows = numRows;
            this.imageColumns = numCols;
            this.animationTime = anTime;
            if (this.useTextureDimensions)
                matchToTextureDimensions();
        }

        /// <summary>
        /// Loads a static 2D texture with the given filename in the "Content" directory
        /// </summary>
        /// <param name="content">ContentManager passed in through Game</param>
        /// <param name="fileName">File name of XNB image in "Content" directory</param>
        public void loadImage(ContentManager content, string fileName)
        {
            this.loadImage(content, fileName, 1, 1, 100.0f);
        }

        /// <summary>
        /// Sets a static 2D texture that has already been loaded
        /// </summary>
        public void setImage(Texture2D image)
        {
            this.image = image;
            this.frameColumn = 0;
            this.frameRow = 0;
            this.animationTime = 100.0f;
            this.imageColumns = 1;
            this.imageRows = 1;
            this.matchToTextureDimensions();
        }

        /// <summary>
        /// Loads all content necessary for this Sprite. Call this method on every Sprite
        /// in the Game's load cycle. Override this method when writing the classes that must
        /// load specific sprites and sound effects.
        /// </summary>
        /// <param name="content">ContentManager passed in through Game</param>
        public virtual void loadContent(ContentManager content)
        {
            //Child classes call loadImage() from here
        }

        /// <summary>
        /// Updates this sprite based on time. If the sprite can animate, then
        /// it will do so here. Call this method on every Sprite in the Game's update
        /// cycle. Override this method in child classes as necessary.
        /// </summary>
        /// <param name="elapsed">Time (in seconds) that has elapsed since the last update</param>
        public virtual void update(float elapsed)
        {
            this.totalElapsed += elapsed;
            if (this.totalElapsed > this.animationTime)
            {
                this.frameColumn++;
                this.frameColumn = this.frameColumn % this.imageColumns;
                this.totalElapsed -= this.animationTime;
            }
        }

        /// <summary>
        /// Draws this sprite. Call this method on every Sprite in the Game's 
        /// draw cycle. Override this method in child classes as necessary (i.e.
        /// if drawing additional components)
        /// </summary>
        /// <param name="sb">SpriteBatch passed in through Game</param>
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
