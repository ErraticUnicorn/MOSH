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
     * Character inherits all the Sprite methods.
     * New information includes a direction (which way it's facing), a gender,
     * and an inventory (all its items).
     */
    public class Character : Sprite
    {
        private const int ACTION_OFFSET = 3;        //pixels offset between sprite and action boxes
                                                    //the "action box" being area where character can interact with environment                                       
        //mechanics
        private Direction direction;                //which direction the character is facing
        private bool moving;                        //whether character is currently in motion
        //personal data
        private bool male;                          //male or female
        private string name;                        //character's name
        //private Dialogue script;                  //script given to NPCs
        private Inventory inventory;                //all the items this character has

        public Character()
            : base()
        {
            this.inventory = new Inventory();
            setName("NAMELESS");
            setMale(true);
            setMoving(false);
            setDirection(Direction.South);
        }
        public Character(int x, int y, int width, int height)
            : base(x, y, width, height)
        {
            this.inventory = new Inventory();
            setName("NAMELESS");
            setMale(true);
            setMoving(false);
            setDirection(Direction.South);
        }

        public Direction getDirection() { return this.direction; }
        public Inventory getInventory() { return this.inventory; }
        public string getName() { return this.name; }
        public bool isMoving() { return this.moving; }
        public bool isMale() { return this.male; }
        public bool isFemale() { return !this.male; }
       
        public void setMoving(bool moving) { this.moving = moving; }
        public void setMale(bool male) { this.male = male; }
        public void setName(string name) { this.name = name; }

        public void setDirection(Direction dir)
        {
            if (dir.Equals(Direction.Undefined)) return;

            this.direction = dir;
            //we should have a standard convention for spritesheets
            //i.e. each row is a direction
            if (this.getDirection().Equals(Direction.North))
                this.setFrameRow(3);
            if (this.getDirection().Equals(Direction.East))
                this.setFrameRow(2);
            if (this.getDirection().Equals(Direction.South))
                this.setFrameRow(0);
            if (this.getDirection().Equals(Direction.West))
                this.setFrameRow(1);

        }

        /*
         * Handles 1D movement
         */
        public void move(Direction dir, int dist)
        {
            this.setMoving(true);
            this.setDirection(dir);
            if (dir.Equals(Direction.North))
                this.setY(this.getY() - dist);
            if (dir.Equals(Direction.South))
                this.setY(this.getY() + dist);
            if (dir.Equals(Direction.East))
                this.setX(this.getX() + dist);
            if (dir.Equals(Direction.West))
                this.setX(this.getX() - dist);
        }
        
        /*
         * Handles moving diagonally
         */
        public void move2D(Direction dir1, Direction dir2, int dist1, int dist2)
        {
            this.setMoving(true);
            this.move(dir1, dist1);
            this.move(dir2, dist2);
            if (dist1 >= dist2) this.setDirection(dir1);
            else this.setDirection(dir2);
        }

        /*
         * Checks if this character is close enough with another sprite to perform any action
         * e.g. talk to other character, pickup item
         */
        public bool inRange(Sprite other)
        {
            return(((this.getX() < other.getX() && this.getX()+this.getWidth()+ACTION_OFFSET > other.getX()-ACTION_OFFSET) ||
                    (this.getX() > other.getX() && this.getX()-ACTION_OFFSET < other.getX()+other.getWidth()+ACTION_OFFSET)) &&
                   ((this.getY() < other.getY() && this.getY()+this.getHeight()+ACTION_OFFSET > other.getY()-ACTION_OFFSET) ||
                    (this.getY() > other.getY() && this.getY()-ACTION_OFFSET < other.getY()+other.getHeight()+ACTION_OFFSET)));                
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

        public void pickup(Pickup p)
        {
            this.getInventory().addItem(p.getItemType());
            p.banish();
        }

        /*
         * Updates which frame on sprite sheet to draw
         * Adapted from MSDN XNA tutorials
         * Override this in child classes as necessary
         */
        public override void update(float elapsed)
        {
            if (this.isMoving())
            {
                base.update(elapsed);
            }
        }
    }
}
