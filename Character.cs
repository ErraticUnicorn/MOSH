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
    /// Specifies which direction a Character is facing
    /// </summary>
    public enum Direction                       
    {
        Undefined = -1,
        North = 0,
        East,
        South,
        West
    };
    
    /// <summary>
    /// Character extends from Sprite, adding behavior for movement and inventory.
    /// Each character also has an Inventory (all his/her items).
    /// </summary>
    public class Character : Sprite
    {
        private const int ACTION_OFFSET = 3;        //pixels offset between sprite and action boxes
                                                    //the "action box" being area where character can interact with environment
        private const int COLLISION_OFFSET = 0;     //pixels offset between sprite and other sprites for collisions

        //mechanics
        private Direction direction;                //which direction the character is facing
        private bool moving;                        //whether character is currently in motion
        //personal data
        private bool male;                          //male or female
        private string name;                        //character's name
        public Interaction script;                  //script given to NPCs
        private Inventory inventory;                //all the items this character has
        
        /// <summary>
        /// Initializes a default Character at the origin which matches the dimensions
        /// of its sprite (when loaded)
        /// </summary>
        public Character()
            : base()
        {
            this.inventory = new Inventory();
            setName("NAMELESS");
            setMale(true);
            setMoving(false);
            setDirection(Direction.South);
            script = null;
        }

        /// <summary>
        /// Initializes a Character at the given position which matches the dimensions
        /// of its sprite (when loaded)
        /// </summary>
        /// <param name="x">X coordinate of the top-left corner</param>
        /// <param name="y">Y coordinate of the top-left corner</param>
        public Character(int x, int y)
            : base(x, y)
        {
            this.inventory = new Inventory();
            setName("NAMELESS");
            setMale(true);
            setMoving(false);
            setDirection(Direction.South);
        }

        /// <summary>
        /// Initializes a Character at the given position with the given dimensions
        /// </summary>
        /// <param name="x">X coordinate of the top-left corner</param>
        /// <param name="y">Y coordinate of the top-left corner</param>
        /// <param name="width">Width in pixels</param>
        /// <param name="height">Height in pixels</param>
        public Character(int x, int y, int width, int height, string file)
            : base(x, y, width, height)
        {
            this.inventory = new Inventory();
            setName("NAMELESS");
            setMale(true);
            setMoving(false);
            setDirection(Direction.South);
            if(file != string.Empty)
                setScript(file);
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
        public void setScript(string file) { this.script = new Interaction(file); }

        /// <summary>
        /// Sets the direction of this Character and updates which row on the 
        /// spritesheet to animate
        /// </summary>
        /// <param name="dir">Direction of this Character</param>
        public void setDirection(Direction dir)
        {
            if (dir.Equals(Direction.Undefined)) return;

            this.direction = dir;
            //we should have a standard convention for spritesheets
            //i.e. each row is a direction
            if (this.getImageRows() >= 4)
            {
                if (this.getDirection().Equals(Direction.North))
                    this.setFrameRow(3);
                if (this.getDirection().Equals(Direction.East))
                    this.setFrameRow(2);
                if (this.getDirection().Equals(Direction.South))
                    this.setFrameRow(0);
                if (this.getDirection().Equals(Direction.West))
                    this.setFrameRow(1);
            }
        }

        /// <summary>
        /// Moves this Character a given distance in pixels; also sets Character's direction
        /// </summary>
        /// <param name="dir">Direction to move the Character</param>
        /// <param name="dist">Distance to move in pixels</param>
        public void move(Direction dir, int dist) {
            this.setDirection(dir);

            Point l_offset = new Point(0, 0);

            switch (dir) {
                case Direction.North: l_offset.Y = -dist; break;
                case Direction.South: l_offset.Y =  dist; break;
                case Direction.East:  l_offset.X =  dist; break;
                case Direction.West:  l_offset.X = -dist; break;
                default: break;
            }

            if ((l_offset.X != 0 || l_offset.Y != 0) && !CollisionManager.collisionWithSolidAtRelative(this, l_offset)) {
                this.setMoving(true);

                switch (dir) {
                    case Direction.North: this.setY(this.getY() - dist); break;
                    case Direction.South: this.setY(this.getY() + dist); break;
                    case Direction.East:  this.setX(this.getX() + dist); break;
                    case Direction.West:  this.setX(this.getX() - dist); break;
                    default: break;
                }
            }
        }
        
        /// <summary>
        /// Moves the Character in two directions (for diagonal movement)
        /// </summary>
        /// <param name="dir1">First direction to move</param>
        /// <param name="dir2">Second direction to move</param>
        /// <param name="dist1">Distance to move in first direction</param>
        /// <param name="dist2">Distance to move in second direction</param>
        public void move2D(Direction dir1, Direction dir2, int dist1, int dist2)
        {
            this.setMoving(true);
            this.move(dir1, dist1);
            this.move(dir2, dist2);
            if (dist1 > dist2) this.setDirection(dir1);
            else this.setDirection(dir2);
        }

        /// <summary>
        /// Checks in this Character is within a given range with another Sprite to perform any action.
        /// The range is another rectangle a given number of pixels thicker than the sprite's drawing rectangle.
        /// E.g. talk to another character, pickup an item, collision detection
        /// </summary>
        /// <param name="other">The other Sprite to range check against</param>
        /// <param name="offset">The pixels offset for the range check</param>
        /// <returns>True if the Character is in range, false if not</returns>
        public bool inRange(Sprite other, int offset)
        {
            return (((this.getX() <= other.getX() && this.getX() + this.getWidth() + offset >= other.getX() - offset) ||
                    (this.getX() >= other.getX() && this.getX() - offset <= other.getX() + other.getWidth() + offset)) &&
                   ((this.getY() <= other.getY() && this.getY() + this.getHeight() + offset >= other.getY() - offset) ||
                    (this.getY() >= other.getY() && this.getY() - offset <= other.getY() + other.getHeight() + offset)));
        }
        /// <summary>
        /// Checks if this Character is in a given range to perform an action such as talking or pickpocketing
        /// </summary>
        /// <param name="other">The other Sprite to range check against</param>
        /// <returns>True if the Character is in range, false if not</returns>
        public bool inRangeAction(Sprite other)
        {
            return inRange(other, ACTION_OFFSET);
        }
        /// <summary>
        /// Checks if this Character is in a given range for collisions
        /// </summary>
        /// <param name="other">The other Sprite to range check against</param>
        /// <returns>True if the Character is in range, false if not</returns>
        public bool inRangeCollide(Sprite other)
        {
            return inRange(other, COLLISION_OFFSET);
        }

        /// <summary>
        /// Checks if this Character is facing another Character; used as a check for talking action
        /// </summary>
        /// <param name="other">The other Character</param>
        /// <returns>True if this character is facing the other, false if not</returns>
        public bool facing(Character other)
        {
            return ((this.getDirection().Equals(Direction.North) && this.getY() > other.getY()) ||
                   (this.getDirection().Equals(Direction.South) && this.getY() < other.getY()) ||
                   (this.getDirection().Equals(Direction.East) && this.getX() < other.getX()) ||
                   (this.getDirection().Equals(Direction.West) && this.getX() > other.getX()));
        }

        /// <summary>
        /// Checks if both Characters are facing each other
        /// </summary>
        /// <param name="other">The other Character</param>
        /// <returns>True if both characters are facing each other, false if not</returns>
        public bool bothFacing(Character other)
        {
            return this.facing(other) && other.facing(this);
        }

        /// <summary>
        /// Returns the reverse direction of this Character's current direction; used for making
        /// Characters face each other when talking
        /// </summary>
        /// <returns>The opposite direction</returns>
        public Direction getOppositeDirection()
        {
            Direction mDir = this.getDirection();
            if (mDir == Direction.Undefined)
                return Direction.Undefined;

            return (Direction)(((int)mDir + 2) % 4);
        }

        /// <summary>
        /// Adds the given Pickup to this Character's inventory (and the Pickup disappears)
        /// </summary>
        /// <param name="p">The Pickup to add</param>
        public void pickup(Pickup p)
        {
            this.getInventory().addItem(p.getItemType());
            p.banish();
        }

        /// <summary>
        /// Updates the animation, only if the Character is moving
        /// </summary>
        /// <param name="elapsed">Time (in seconds) that has elasped since last update</param>
        public override void update(float elapsed)
        {
            if (this.isMoving()) //only update walking animation if moving
            {
                base.update(elapsed);
            }
        }
    }
}
