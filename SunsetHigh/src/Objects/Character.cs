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
    /// Specifies a type of clique for certain characters to belong to.
    /// </summary>
    public enum Clique
    {
        None = -1,
        Nerd = 0,
        Jock,
        Prep,
        Bully,
        Slacker
    }

    /// <summary>
    /// Character extends from FreeMovingSprite, adding behavior for movement and inventory.
    /// Each character also has an Inventory (all his/her items).
    /// </summary>
    public class Character : FreeMovingSprite
    {
        private const int ACTION_OFFSET = 3;        //pixels offset between sprite and action boxes
                                                    //the "action box" being area where character can interact with environment
        private const int COLLISION_OFFSET = 0;     //pixels offset between sprite and other sprites for collisions

        //mechanics
        private Direction direction;                //which direction the character is facing

        //personal data
        private bool male;                          //male or female (male by default)
        private string name;                        //character's name ("NAMELESS" by default)
        private Clique clique;                      //clique type of this character (None by default)
        private PersonID personID;                  //ID for NPCs that get journal entries (Generic by default)
        
        public Interaction script;                  //script given to NPCs
        public Inventory inventory { get; set; }    //all the items this character has

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
            setClique(Clique.None);
            setPersonID(PersonID.None);
            setDirection(Direction.South);
            if(file != string.Empty)
                setScript(file);
        }

        public Direction getDirection() { return this.direction; }
        public string getName() { return this.name; }
        public Clique getClique() { return this.clique; }
        public PersonID getPersonID() { return this.personID; }
        public bool isMale() { return this.male; }
        public bool isFemale() { return !this.male; }
       
        public void setMale(bool male) { this.male = male; }
        public void setName(string name) { this.name = name; }
        public void setClique(Clique clique) { this.clique = clique; }
        public void setPersonID(PersonID id) { this.personID = id; }
        public void setScript(string file) { this.script = new Interaction(file); }

        /// <summary>
        /// Sets the direction of this Character and updates which row on the 
        /// spritesheet to animate
        /// </summary>
        /// <param name="dir">Direction of this Character</param>
        public void setDirection(Direction dir)
        {
            if (dir.Equals(Direction.Undefined)) return;

            //we should have a standard convention for spritesheets
            //i.e. each row is a direction
            if (this.getImageRows() >= 4)
            {
                switch (dir)
                {
                    case Direction.NorthEast:
                    case Direction.NorthWest:
                    case Direction.North: this.setFrameRow(3); 
                        this.direction = Direction.North; break;
                    case Direction.SouthEast:
                    case Direction.SouthWest:
                    case Direction.South: this.setFrameRow(0);
                        this.direction = Direction.South; break;
                    case Direction.East: this.setFrameRow(2);
                        this.direction = Direction.East; break;
                    case Direction.West: this.setFrameRow(1);
                        this.direction = Direction.West; break;
                    default: break;
                }
            }
        }

        public override bool move(Direction dir, float elapsed, bool collide = true)
        {
            this.setDirection(dir);
            return base.move(dir, elapsed, collide);
        }
        public override bool move(float angle, float elapsed, bool collide = true)
        {
            this.setDirection(SunsetUtils.convertAngleToDirection(angle));
            return base.move(angle, elapsed, collide);
        }

        /// <summary>
        /// Checks in this Character is within a given range with another Interactable to perform any action.
        /// The range is another rectangle a given number of pixels thicker than the sprite's drawing rectangle.
        /// E.g. talk to another character, pickup an item, collision detection
        /// </summary>
        /// <param name="other">The other Interactable to range check against</param>
        /// <param name="offset">The pixels offset for the range check</param>
        /// <returns>True if the Character is in range, false if not</returns>
        public bool inRange(IInteractable other, int offset)
        {
            Rectangle copy = other.getBoundingRect();   // not sure if it returns the reference
            Rectangle inflatedRect;
            inflatedRect.X = copy.X - offset;
            inflatedRect.Y = copy.Y - offset;
            inflatedRect.Width = copy.Width + offset * 2;
            inflatedRect.Height = copy.Height + offset * 2;
            return this.getBoundingRect().Intersects(inflatedRect);
        }

        /// <summary>
        /// Checks if this Character is in a given range to perform an action such as talking or pickpocketing
        /// </summary>
        /// <param name="other">The other Sprite to range check against</param>
        /// <returns>True if the Character is in range, false if not</returns>
        public bool inRangeAction(IInteractable other)
        {
            return inRange(other, ACTION_OFFSET);
        }
        /// <summary>
        /// Checks if this Character is in a given range for collisions
        /// </summary>
        /// <param name="other">The other Sprite to range check against</param>
        /// <returns>True if the Character is in range, false if not</returns>
        public bool inRangeCollide(IInteractable other)
        {
            return inRange(other, COLLISION_OFFSET);
        }

        /// <summary>
        /// Checks if this Character is facing another Interactable (e.g. other Character); used as a check for talking action
        /// </summary>
        /// <param name="other">The other Interactable</param>
        /// <returns>True if this character is facing the other, false if not</returns>
        public bool facing(IInteractable other)
        {
            Rectangle rect = other.getBoundingRect();
            return ((this.getDirection().Equals(Direction.North) && this.getY() > rect.Y) ||
                   (this.getDirection().Equals(Direction.South) && this.getY() < rect.Y) ||
                   (this.getDirection().Equals(Direction.East) && this.getX() < rect.X) ||
                   (this.getDirection().Equals(Direction.West) && this.getX() > rect.X));
        }

        /// <summary>
        /// Checks if two Characters are facing each other
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
            this.inventory.addItem(p.getItemType());
            p.banish();
        }

        public override void onInteract()
        {
            base.onInteract();
            Hero.instance.converse(this);
        }
    }
}
