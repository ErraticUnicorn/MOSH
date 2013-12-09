using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using TiledLib;

namespace SunsetHigh
{
    /// <summary>
    /// Specifies which direction a free moving sprite can face/move
    /// </summary>
    public enum Direction
    {
        Undefined = -1,
        North = 0,
        East,
        South,
        West,
        NorthWest,
        NorthEast,
        SouthWest,
        SouthEast
    };

    /// <summary>
    /// A child of Sprite that has the ability to move around the scene through preprogrammed 
    /// directions or through user control. Can move with speed and acceleration independent of 
    /// framerate - the units for these quantities are pixels/sec and pixels/(sec^2).
    /// </summary>
    public class FreeMovingSprite : Sprite
    {
        protected const float RAD2BY2 = 0.70710678118f;
        protected const float DEFAULT_SPEED = 120f;
        protected const float DEFAULT_ACCELERATION = 0f;  //constant speed

        private float mStartSpeed;
        private float mSpeed;
        private float mAcceleration;
        private float pixelsRemainderX;
        private float pixelsRemainderY;

        public FreeMovingSprite()
            : this(0, 0, 0, 0) { }
        public FreeMovingSprite(int x, int y)
            : this(x, y, 0, 0) { }
        public FreeMovingSprite(int x, int y, int width, int height)
            : this(x, y, width, height, DEFAULT_SPEED, DEFAULT_ACCELERATION) { }
        public FreeMovingSprite(int x, int y, float speed, float accel)
            : this(x, y, 0, 0, DEFAULT_SPEED, DEFAULT_ACCELERATION) { }
        public FreeMovingSprite(int x, int y, int width, int height, float speed, float accel)
            : base(x, y, width, height)
        {
            setAcceleration(accel);
            setSpeed(speed);
            pixelsRemainderX = 0;
            pixelsRemainderY = 0;
        }

        //Note: acceleration should primarily be for projectiles
        public void setAcceleration(float accel) 
        {
            if (accel < 0)
                accel = accel * -1;
            mAcceleration = accel; 
        }
        public float getAcceleration() { return mAcceleration; }
        public void setSpeed(float speed)
        {
            if (speed < 0)
                speed = speed * -1;
            mSpeed = speed;
            mStartSpeed = speed;
        }
        public float getSpeed() { return mSpeed; }

        /// <summary>
        /// Moves this Sprite a given Direction. Optional parameter sets it to
        /// collide with the scene and characters in the scene.
        /// </summary>
        /// <param name="dir">Direction to move the Character</param>
        /// <param name="elapsed">Time that has elapsed since last update, in seconds</param>
        /// <param name="collide">Specifies whether the Sprite should collide with objects in the scene</param>
        /// <returns>True if the sprite moved; false otherwise</returns>
        public virtual bool move(Direction dir, float elapsed, bool collide = true)
        {
            this.setSpeed(this.getSpeed() + this.getAcceleration() * elapsed);
            bool moved = this.moveDirectionHelper(dir, this.getSpeed(), elapsed, collide);
            if (!moved) this.setSpeed(mStartSpeed);
            return moved;
        }

        /// <summary>
        /// Moves this Sprite a given angle (0 to 2 pi). Optional parameter sets it to
        /// collide with the scene and characters in the scene.
        /// </summary>
        /// <param name="angle">Angle in radians, from 0 to 2 pi, to move the Character</param>
        /// <param name="elapsed">Time that has elapsed since last update, in seconds</param>
        /// <param name="collide">Specifies whether the Sprite should collide with objects in the scene</param>
        /// <returns>True if the sprite moved; false otherwise</returns>
        public virtual bool move(float angle, float elapsed, bool collide = true)
        {
            this.setSpeed(this.getSpeed() + this.getAcceleration() * elapsed);
            bool moved = this.moveAngleHelper(angle, this.getSpeed(), elapsed, collide);
            if (!moved) this.setSpeed(mStartSpeed);
            return moved;
        }

        private bool moveDirectionHelper(Direction dir, float speed, float elapsed, bool collide = true)
        {
            Point l_offset = new Point(0, 0);
            int distX = 0;
            int distY = 0;
            bool a, b;

            switch (dir)
            {
                case Direction.North:
                    pixelsRemainderY -= speed * elapsed;
                    distY = (int)pixelsRemainderY;
                    pixelsRemainderY -= distY;
                    break;
                case Direction.South:
                    pixelsRemainderY += speed * elapsed;
                    distY = (int)pixelsRemainderY;
                    pixelsRemainderY -= distY;
                    break;
                case Direction.East:
                    pixelsRemainderX += speed * elapsed;
                    distX = (int)pixelsRemainderX;
                    pixelsRemainderX -= distX;
                    break;
                case Direction.West:
                    pixelsRemainderX -= speed * elapsed;
                    distX = (int)pixelsRemainderX;
                    pixelsRemainderX -= distX;
                    break;

                case Direction.NorthWest:
                    a = this.moveDirectionHelper(Direction.North, speed * RAD2BY2, elapsed);
                    b = this.moveDirectionHelper(Direction.West, speed * RAD2BY2, elapsed);
                    return a || b;
                case Direction.NorthEast:
                    a = this.moveDirectionHelper(Direction.North, speed * RAD2BY2, elapsed);
                    b = this.moveDirectionHelper(Direction.East, speed * RAD2BY2, elapsed);
                    return a || b;
                case Direction.SouthWest:
                    a = this.moveDirectionHelper(Direction.South, speed * RAD2BY2, elapsed);
                    b = this.moveDirectionHelper(Direction.West, speed * RAD2BY2, elapsed);
                    return a || b;
                case Direction.SouthEast:
                    a = this.moveDirectionHelper(Direction.South, speed * RAD2BY2, elapsed);
                    b = this.moveDirectionHelper(Direction.East, speed * RAD2BY2, elapsed);
                    return a || b;
                default: break;
            }
            l_offset.Y = distY;
            l_offset.X = distX;

            if (!collide)
            {
                this.setX(this.getX() + l_offset.X);
                this.setY(this.getY() + l_offset.Y);
                return true;
            }

            IInteractable interactObj = CollisionManager.collisionWithInteractableAtRelative(this, l_offset, this);
            if (interactObj != null)
            {
                interactObj.onCollide();
                if (interactObj is Character)
                {
                    if ((l_offset.X == 0) != (l_offset.Y == 0))     //only one offset is zero (multiple cannot be handled yet)
                    {
                        return CollisionManager.setSpriteOutsideRectangle(this, interactObj.getBoundingRect());
                    }
                }
            }

            // collision checking here
            MapObject mapObj = CollisionManager.collisionWithObjectAtRelative(this, l_offset);
            if (mapObj != null)
            {
                if ((l_offset.X == 0) != (l_offset.Y == 0))     //only one offset is zero (multiple cannot be handled yet)
                {
                    return CollisionManager.setSpriteOutsideRectangle(this, mapObj.Bounds);
                }
            }

            //no collisions detected!
            this.setX(this.getX() + l_offset.X);
            this.setY(this.getY() + l_offset.Y);
            return true;
        }

        private bool moveAngleHelper(float angle, float speed, float elapsed, bool collide = true)
        {
            float x_speed = (float)(Math.Cos(angle) * speed);
            float y_speed = (float)(Math.Sin(angle) * speed);
            bool a;
            bool b;

            if (y_speed >= 0)
                a = this.moveDirectionHelper(Direction.North, Math.Abs(y_speed), elapsed, collide);
            else
                a = this.moveDirectionHelper(Direction.South, Math.Abs(y_speed), elapsed, collide);

            if (x_speed >= 0)
                b = this.moveDirectionHelper(Direction.East, Math.Abs(x_speed), elapsed, collide);
            else
                b = this.moveDirectionHelper(Direction.West, Math.Abs(x_speed), elapsed, collide);
            return a || b;
        }
    }
}
