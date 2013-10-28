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
    /// Character extends from FreeMovingSprite, adding behavior for movement and inventory.
    /// Each character also has an Inventory (all his/her items).
    /// </summary>
    public class Character : FreeMovingSprite
    {
        private const int ACTION_OFFSET = 3;        //pixels offset between sprite and action boxes
                                                    //the "action box" being area where character can interact with environment
        private const int COLLISION_OFFSET = 0;     //pixels offset between sprite and other sprites for collisions
        //private const float RAD2_OVER_2 = (float)Math.Sqrt(2.0) / 2.0f;

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
            : this(0, 0, 0, 0, "") { }

        /// <summary>
        /// Initializes a Character at the given position which matches the dimensions
        /// of its sprite (when loaded)
        /// </summary>
        /// <param name="x">X coordinate of the top-left corner</param>
        /// <param name="y">Y coordinate of the top-left corner</param>
        public Character(int x, int y)
            : this(x, y, 0, 0, "") { }

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
                switch (dir)
                {
                    case Direction.NorthEast:
                    case Direction.NorthWest:
                    case Direction.North: this.setFrameRow(3); break;
                    case Direction.SouthEast:
                    case Direction.SouthWest:
                    case Direction.South: this.setFrameRow(0); break;
                    case Direction.East: this.setFrameRow(2); break;
                    case Direction.West: this.setFrameRow(1); break;
                    default: break;
                }
            }
        }

        public override bool move(Direction dir, float elapsed, bool collide = true)
        {
            this.setDirection(dir);
            bool retVal = base.move(dir, elapsed, collide);
            this.setMoving(retVal);
            return retVal;
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
            switch (this.getDirection())
            {
                case Direction.North: return Direction.South;
                case Direction.South: return Direction.North;
                case Direction.East: return Direction.West;
                case Direction.West: return Direction.East;
                case Direction.NorthWest: return Direction.SouthEast;
                case Direction.NorthEast: return Direction.SouthWest;
                case Direction.SouthWest: return Direction.NorthEast;
                case Direction.SouthEast: return Direction.NorthWest;
            }
            return Direction.Undefined;
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
