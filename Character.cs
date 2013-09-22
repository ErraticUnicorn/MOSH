using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;

namespace SunsetHigh    //adjust as needed
{
    /*
     * Specifies which direction a character is facing.
     */
    public enum Direction                       
    {
        Undefined = -1,
        North = 0,
        East,
        South,
        West
    };
    
    /*
     * Every character has a sprite drawing box and collision box (which are the same, size-wise).
     * 
     * Every character also contains a sprite atlas and information about what drawing/animation to draw
     * depending on the situation.
     */
    public class Character
    {
        private const int ACTION_OFFSET = 5;        //pixels offset between sprite and action boxes
                                                    //the "action box" being area where character can interact with environment                                       
        //geometry
        private int spriteX, spriteY;                   //top-left corner of sprite and collision box
        private int spriteWidth, spriteHeight;      //size of sprite, collision box
        private Direction direction;                      //which direction the character is facing
        private bool moving;
        //sprite image
        private Texture2D image;                    //the picture or spritesheet to draw
        private int imageRows, imageColumns;        //rows and columns in spritesheet
        private int frameColumn;                       //the current frame in the sheet we are drawing
        private int frameRow;                       //the current row of the sheet we are drawing
        private float animationTime;               //speed at which to animate sprite
        private float totalElapsed;                 //personal timer for animation purposes
        //private bool visible;                       //sprite visibility
        //personal data
        private bool male;                          //male or female
        private string name;                      //character's name
        //private Dialogue script;                  //script given to NPCs

        public Character() { }                      //everything is zero/null
        public Character(int x, int y, int w, int h)
        {
            spriteX = x;
            spriteY = y;
            spriteWidth = w;
            spriteHeight = h;
        }

        public int getX() { return this.spriteX; }
        public int getY() { return this.spriteY; }
        public int getWidth() { return this.spriteWidth; }
        public int getHeight() { return this.spriteHeight; }
        public Direction getDirection() { return this.direction; }
        public bool isMale() { return this.male; }
        public bool isFemale() { return !this.male; }

        public void setPosition(int x, int y)
        {
            this.spriteX = x;
            this.spriteY = y;
        }
        public void setDimensions(int width, int height)
        {
            this.spriteWidth = width;
            this.spriteHeight = height;
        }
        public void setDirection(Direction dir)
        {
            if (dir.Equals(Direction.Undefined)) return;

            this.direction = dir;
            //we should have a standard convention for spritesheets
            //i.e. each row is a direction
            if (this.getDirection().Equals(Direction.North))
                this.frameRow = 3;
            if (this.getDirection().Equals(Direction.East))
                this.frameRow = 2;
            if (this.getDirection().Equals(Direction.South))
                this.frameRow = 0;
            if (this.getDirection().Equals(Direction.West))
                this.frameRow = 1;

        }
        public void setPersonalData(bool male, string name)
        {
            this.male = male;
            this.name = name;
        }
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
         * Handles 1D movement
         */
        public void move(Direction dir, int speed)
        {
            this.moving = true;
            this.setDirection(dir);
            if (dir.Equals(Direction.North))
                spriteY -= speed;
            if (dir.Equals(Direction.South))
                spriteY += speed;
            if (dir.Equals(Direction.East))
                spriteX += speed;
            if (dir.Equals(Direction.West))
                spriteX -= speed;
        }
        
        /*
         * Handles moving diagonally
         */
        public void move2D(Direction dir1, Direction dir2, int speed1, int speed2)
        {
            this.moving = true;
            this.move(dir1, speed1);
            this.move(dir2, speed2);
            if (speed1 >= speed2) this.setDirection(dir1);
            else this.setDirection(dir2);
        }

        /*
         * Stops moving
         */
        public void stop()
        {
            this.moving = false;
        }

        /*
         * Checks if this character is close enough with another character to perform any action
         */
        public bool inRange(Character other)
        {
            return((this.getX()+this.getWidth()+ACTION_OFFSET > other.getX()-ACTION_OFFSET ||
                    this.getX()-ACTION_OFFSET < other.getX()+other.getWidth()+ACTION_OFFSET) &&
                   (this.getY()+this.getHeight()+ACTION_OFFSET > other.getY()-ACTION_OFFSET ||
                    this.getY()-ACTION_OFFSET < other.getY()+other.getHeight()+ACTION_OFFSET));                
        }

        /*
         * Checks if this character is facing another character
         */
        public bool facing(Character other)
        {
            return ((this.getDirection().Equals(Direction.North) && this.getY() > other.getY()) ||
                   (this.getDirection().Equals(Direction.South) && this.getY() < other.getY()) ||
                   (this.getDirection().Equals(Direction.East) && this.getX() < other.getX()) ||
                   (this.getDirection().Equals(Direction.West) && this.getX() > other.getX()));
        }
        
        /*
         * Checks if both characters are facing each other
         */
        public bool bothFacing(Character other)
        {
            return this.facing(other) && other.facing(this);
        }


        /*
         * Updates which frame on sprite sheet to draw
         * Adapted from MSDN XNA tutorials
         */
        public void updateFrame(float elapsed)
        {
            if (this.moving)
            {
                this.totalElapsed += elapsed;
                if (this.totalElapsed > this.animationTime)
                {
                    this.frameColumn++;
                    this.frameColumn = this.frameColumn % this.imageColumns;
                    this.totalElapsed %= this.animationTime;
                }
            }
        }

        /*
         * Draws itself!
         * Adapted from MSDN XNA tutorials
         */
        public void draw(SpriteBatch sb)
        {
            if (image == null)
                return;

            int frameWidth = this.image.Width / this.imageColumns;
            int frameHeight = this.image.Height / this.imageRows;
            Rectangle sourceRect = new Rectangle(frameWidth * frameColumn, 
                frameHeight * frameRow, frameWidth, frameHeight);
            Rectangle mapPosRect = new Rectangle(this.getX(), this.getY(), 
                this.getWidth(), this.getHeight());
            sb.Draw(this.image, mapPosRect, sourceRect, Color.White);
                //more features available in sb.Draw(...);
        }
    }
}
