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
    public class Sprite : IInteractable
    {
        //class objects
        private static Dictionary<string, Texture2D> textureFinder;

        //geometry
        private int spriteX, spriteY;               //top-left corner of sprite and collision box
        private int spriteWidth, spriteHeight;      //size of sprite, collision box
        //image properties
        private Texture2D image;                    //the picture or spritesheet to draw
        private Rectangle srcRect;                  //Source rectangle (if drawing from a texture with a collection of sprites)
        private int imageRows, imageColumns;        //rows and columns in spritesheet; (1, 1) for static pictures
        private int frameColumn;                    //the current frame in the sheet we are drawing; 0 for static pictures
        private int frameRow;                       //the current row of the sheet we are drawing; 0 for static pictures
        private float animationTime;                //speed at which to animate sprite; arbitrary time for static pictures
        private float frameElapsed;                 //personal timer for animation purposes
        private bool visible;                       //sprite visibility
        private Color color;                        //Color to add to this sprite (white for default color)
        private bool useTextureDimensions;          //used for matching width and height to texture's dimensions
        
        /// <summary>
        /// Initializes a Sprite at the origin which will match the dimensions
        /// of its texture when the texture is loaded
        /// </summary>
        public Sprite()
            : this(0, 0, 0, 0) {}

        /// <summary>
        /// Initializes a Sprite at the given position. It will match the dimensions
        /// of its texture when the texture is loaded.
        /// </summary>
        /// <param name="x">X coordinate of top-left corner</param>
        /// <param name="y">Y coordinate of top-left corner</param>
        public Sprite(int x, int y)
            : this(x, y, 0, 0) { }

        /// <summary>
        /// Initializes a Sprite at the given position with the given dimensions
        /// </summary>
        /// <param name="x">X coordinate of top-left corner</param>
        /// <param name="y">Y coordinate of top-left corner</param>
        /// <param name="width">Width in pixels; specify 0 to use texture dimensions</param>
        /// <param name="height">Height in pixels; specify 0 to use texture dimensions</param>
        public Sprite(int x, int y, int width, int height)
        {
            setX(x);
            setY(y);
            if (width > 0 && height > 0)
            {
                setWidth(width);
                setHeight(height);
                useTextureDimensions = false;
            }
            else
                useTextureDimensions = true;
            setVisible(true);
            setColor(Color.White);
            setImage(null); //initializes animation variables, but not image (that will come later)
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
        public Color getColor() { return this.color; }

        public void setX(int x) { this.spriteX = x; }
        public void setY(int y) { this.spriteY = y; }
        public void setXCenter(int x) { this.spriteX = x - this.spriteWidth / 2; }
        public void setYCenter(int y) { this.spriteY = y - this.spriteHeight / 2; }
        public void setWidth(int width) { this.spriteWidth = width; }
        public void setHeight(int height) { this.spriteHeight = height; }
        public void setVisible(bool visible) { this.visible = visible; }
        public void setColor(Color color) { this.color = color; }
        protected void setImageRows(int rows) { this.imageRows = rows; }
        protected void setImageColumns(int cols) { this.imageColumns = cols; }
        protected void setFrameRow(int row) { this.frameRow = row; }
        protected void setFrameColumn(int col) { this.frameColumn = col; }
        protected void setAnimationTime(float time) { this.animationTime = time; }
        protected void resetAnimation()
        {
            this.frameColumn = 0;
            this.frameElapsed = 0;
        }
        protected void matchToTextureDimensions() 
        { 
            this.useTextureDimensions = true;
            if (this.image != null)
            {
                if (!this.srcRect.IsEmpty)
                {
                    setWidth(srcRect.Width / this.getImageColumns());
                    setHeight(srcRect.Height / this.getImageRows());
                }
                else
                {
                    setWidth(image.Width / this.getImageColumns());
                    setHeight(image.Height / this.getImageRows());
                    srcRect = new Rectangle(0, 0, image.Width, image.Height);   //use whole texture
                }
            }
        }

        /// <summary>
        /// Sets an animating 2D texture that has already been loaded to be used
        /// to draw this Sprite
        /// </summary>
        /// <param name="image">The pre-loaded image</param>
        /// <param name="numRows">Number of rows in this spritesheet</param>
        /// <param name="numCols">Number of columns in this spritesheet</param>
        /// <param name="anTime">Time between frames (in seconds) when animating the sprite</param>
        public void setImage(Texture2D image, int numRows, int numCols, float anTime)
        {
            this.image = image;
            setFrameRow(0);
            setFrameColumn(0);
            setImageRows(numRows);
            setImageColumns(numCols);
            setAnimationTime(anTime);
            //if (this.image != null)
            //    setSourceRect(0, 0, this.image.Width, this.image.Height);
            if (this.useTextureDimensions)
                this.matchToTextureDimensions();
        }

        /// <summary>
        /// Sets a static 2D texture that has already been loaded to be used
        /// to draw this Sprite
        /// </summary>
        public void setImage(Texture2D image)
        {
            this.setImage(image, 1, 1, 100.0f);
        }

        /// <summary>
        /// Sets the source rectangle of a loaded texture for this Sprite (if the particular texture
        /// is a collection of different sprites).
        /// </summary>
        /// <param name="x">X coordinate of top-left corner</param>
        /// <param name="y">Y coordinate of top-left corner</param>
        /// <param name="width">Width in pixels</param>
        /// <param name="height">Height in pixels</param>
        public void setSourceRect(int x, int y, int width, int height)
        {
            this.srcRect = new Rectangle(x, y, width, height);
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
            if (fileName.EndsWith(".xnb") || fileName.EndsWith(".png"))
                fileName = fileName.Substring(0, fileName.Length - 4); //file extension is not necessary

            Texture2D nImage = searchDirectories(content, fileName);  //NOTE: throws exception here if file cannot be found in "Content" directory
                                                                      //be sure "Copy to output directory" settings are "Copy if newer"
            if (nImage == null)
            {
                throw new System.IO.FileNotFoundException("Could not find the texture \"" + fileName + "\" in Sprites," 
                    + " Characters, or CharactersTemp directories.\nMake sure the file is .png or .xnb, and \"Copy to"
                    + " output directory\" settings are set to \"Copy if newer\".");
            }
            this.setImage(nImage, numRows, numCols, anTime);
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

        private Texture2D searchDirectories(ContentManager content, string fileName)
        {
            Texture2D retVal;
            if (System.IO.File.Exists(fileName + ".png") || System.IO.File.Exists(fileName + ".xnb"))
            {
                retVal = content.Load<Texture2D>(fileName);
                return retVal;
            }
            if (System.IO.File.Exists(Directories.SPRITES + fileName + ".png")
                || System.IO.File.Exists(Directories.SPRITES + fileName + ".xnb"))
            {
                retVal = content.Load<Texture2D>(Directories.SPRITES + fileName);
                return retVal;
            }
            if (System.IO.File.Exists(Directories.CHARACTERS + fileName + ".png")
                || System.IO.File.Exists(Directories.CHARACTERS + fileName + ".xnb"))
            {
                retVal = content.Load<Texture2D>(Directories.CHARACTERS + fileName);
                return retVal;
            }
            if (System.IO.File.Exists(Directories.CHARACTERS_TEMP + fileName + ".png")
                || System.IO.File.Exists(Directories.CHARACTERS_TEMP + fileName + ".xnb"))
            {
                retVal = content.Load<Texture2D>(Directories.CHARACTERS_TEMP + fileName);
                return retVal;
            }
            return null;
        }

        // empty definitions to override later
        public virtual void onInteract()
        {
        }
        public virtual void onCollide(IInteractable other)
        {
        }
        public virtual void reset()
        {
        }
        public virtual Rectangle getBoundingRect()
        {
            return new Rectangle(this.getX(), this.getY(), this.getWidth(), this.getHeight());
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
            this.frameElapsed += elapsed;
            if (this.frameElapsed > this.animationTime)
            {
                this.frameColumn++;
                this.frameColumn = this.frameColumn % this.imageColumns;
                this.frameElapsed -= this.animationTime;
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
            if (this.getImage() == null || !this.isVisible())
            {
                //System.Diagnostics.Debug.WriteLine(this.ToString() + " is not drawing.");
                return;
            }

            Rectangle sourceRect;
            if (!this.srcRect.IsEmpty)
            {
                int frameWidth = this.srcRect.Width / this.getImageColumns();
                int frameHeight = this.srcRect.Height / this.getImageRows();
                sourceRect = new Rectangle(this.srcRect.X + frameWidth * this.frameColumn,
                    this.srcRect.Y + frameHeight * this.frameRow, 
                    frameWidth, frameHeight);
            }
            else
            {
                int frameWidth = this.getImage().Width / this.getImageColumns();
                int frameHeight = this.getImage().Height / this.getImageRows();
                sourceRect = new Rectangle(frameWidth * this.frameColumn,
                    frameHeight * this.frameRow, frameWidth, frameHeight);
            }
            Rectangle mapPosRect = new Rectangle(this.getX(), this.getY(),
                this.getWidth(), this.getHeight());
            sb.Draw(this.getImage(), mapPosRect, sourceRect, this.color);
            //more features available in sb.Draw(...);
        }

        /// <summary>
        /// Loads a 2D texture in the "Content" directory for use by multiple 
        /// Sprite objects. Call this method in the loading content cycle, and when
        /// instantiating new objects in-game, call getCommonImage()
        /// </summary>
        /// <param name="content">The content manager</param>
        /// <param name="imageFileName">The file name of the image, without the extension</param>
        public static void loadCommonImage(ContentManager content, string imageFileName)
        {
            nullCheck();
            if (textureFinder.ContainsKey(imageFileName))
                return;     //image already exists
            Texture2D nImage = content.Load<Texture2D>(imageFileName);
            textureFinder[imageFileName] = nImage;
        }

        /// <summary>
        /// Clears memory of all loaded textures. Be sure to call unload on the ContentManager as well.
        /// </summary>
        public static void unloadCommonImages()
        {
            nullCheck();
            textureFinder.Clear();
        }

        /// <summary>
        /// Retrieves a pre-loaded 2D texture to associate with an instantiated
        /// Sprite object. Good for multiple objects using the same texture
        /// </summary>
        /// <param name="imageFileName">The file name of the image that was previously loaded</param>
        /// <returns>The texture for the appropriate file; null if it was not loaded earlier</returns>
        public static Texture2D getCommonImage(string imageFileName)
        {
            nullCheck();
            if (!textureFinder.ContainsKey(imageFileName))
                return null;
            return textureFinder[imageFileName];
        }

        private static void nullCheck()
        {
            if (textureFinder == null)
                textureFinder = new Dictionary<string, Texture2D>();
        }
    }
}
