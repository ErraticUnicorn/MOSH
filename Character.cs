using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;

namespace SunsetHigh    //adjust as needed
{
    /*
     * Every character has a sprite drawing box and collision box (which are the same, size-wise).
     * 
     * Every character also contains a sprite atlas and information about what drawing/animation to draw
     * dependeing on the situation.
     */
    public class Character
    {
        public static enum Direction                //used to specify where characters are facing
        {
            North = 0,
            East,
            South,
            West
        };   

        private static const int ACTION_OFFSET = 5; //pixels offset between sprite and action boxes
                                                    //the "action box" being area where character can interact with environment                                       

        private int spriteX, spriteY;               //top-left corner of sprite and collision box
        private int spriteWidth, spriteHeight;      //size of sprite, collision box
        private int direction;                      //which direction the character is facing
        private Texture2D image;                    //the picture or spritesheet to draw
        private int imageRows, imageColumns;        //rows and columns in spritesheet
        //private int animationSpeed;                 //speed at which to animate sprite
        //private bool visible;                       //sprite visibility
        private bool male;                          //male or female
        //private Dialogue script;                  //script given to NPCs

        //
        //Some constructors here
        //

        public int getX() { return this.spriteX; }
        public int getY() { return this.spriteY; }
        public int getWidth() { return this.spriteWidth; }
        public int getHeight() { return this.spriteHeight; }
        public int getDirection() { return this.direction; }
        public bool isMale() { return this.male; }
        public bool isFemale() { return !this.male; }

        public void setPosition(int x, int y)
        {
            this.spriteX = x;
            this.spriteY = y;
        }
        public void setDirection(int dir)
        {
            if(dir >= 0 && dir < 4)
                direction = dir;
        }
        
        public void move(int dir, int speed)
        {
            setDirection(dir);
            //other stuff
        }
        
        public void move2D(int dir1, int dir2, int speed1, int speed2) //for moving diagonally
        {
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
    }
}
