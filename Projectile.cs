using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SunsetHigh
{
    public class Projectile : Sprite
    {
        private const int DEFAULT_SPEED = 0;
        private const float DEFAULT_ANGLE = 0.0f;

        private int speed;
        private float angle;

        /// <summary>
        /// Initializes a Projectile at the origin with zero speed and angle.
        /// </summary>
        public Projectile()
            : this(0, 0, DEFAULT_SPEED, DEFAULT_ANGLE) { }

        /// <summary>
        /// Initializes a Projectile with the given speed and angle
        /// </summary>
        /// <param name="speed">A positive speed (in pixels per frame update)</param>
        /// <param name="angle">An angle in radians (from 0 to 2 PI)</param>
        public Projectile(int speed, float angle)
            : this(0, 0, speed, angle) { }

        /// <summary>
        /// Initializes a Projectile with the given speed and direction
        /// </summary>
        /// <param name="speed">A positive speed (in pixels per frame update)</param>
        /// <param name="dir">A direction in which it will move</param>
        public Projectile(int speed, Direction dir)
            : this(0, 0, speed, dir) { }

        /// <summary>
        /// Initializes a Projectile with a given position, speed, and angle
        /// </summary>
        /// <param name="x">X coordinate of the top-left corner</param>
        /// <param name="y">Y coordinate of the top-left corner</param>
        /// <param name="speed">A positive speed (in pixels per frame update)</param>
        /// <param name="angle">An angle in radians (from 0 to 2 PI)</param>
        public Projectile(int x, int y, int speed, float angle)
            : this(x, y, 0, 0, speed, angle) { }

        /// <summary>
        /// Initializes a Projectile with a given position, speed, and direction
        /// </summary>
        /// <param name="x">X coordinate of the top-left corner</param>
        /// <param name="y">Y coordinate of the top-left corner</param>
        /// <param name="speed">A positive speed (in pixels per frame update)</param>
        /// <param name="dir">A direction in which it will move</param>
        public Projectile(int x, int y, int speed, Direction dir)
            : this(x, y, 0, 0, speed, dir) { }

        /// <summary>
        /// Initializes a Projectile with a given position and dimensions, and zero speed and angle
        /// </summary>
        /// <param name="x">X coordinate of the top-left corner</param>
        /// <param name="y">Y coordinate of the top-left corner</param>
        /// <param name="width">Width in pixels</param>
        /// <param name="height">Height in pixels</param>
        public Projectile(int x, int y, int width, int height)
            : this(x, y, width, height, DEFAULT_SPEED, DEFAULT_ANGLE) { }

        public bool inRange(Sprite other, int offset)
        {
            return (((this.getX() <= other.getX() && this.getX() + this.getWidth() + offset >= other.getX() - offset) ||
                    (this.getX() >= other.getX() && this.getX() - offset <= other.getX() + other.getWidth() + offset)) &&
                   ((this.getY() <= other.getY() && this.getY() + this.getHeight() + offset >= other.getY() - offset) ||
                    (this.getY() >= other.getY() && this.getY() - offset <= other.getY() + other.getHeight() + offset)));
        }
        /// <summary>
        /// Initializes a Projectile with a given position, dimensions, speed, and angle
        /// </summary>
        /// <param name="x">X coordinate of the top-left corner</param>
        /// <param name="y">Y coordinate of the top-left corner</param>
        /// <param name="width">Width in pixels</param>
        /// <param name="height">Height in pixels</param>
        /// <param name="speed">A positive speed (in pixels per frame update)</param>
        /// <param name="angle">An angle in radians (from 0 to 2 PI)</param>
        public Projectile(int x, int y, int width, int height, int speed, float angle)
            : base(x, y, width, height)
        {
            this.setSpeed(speed);
            this.setAngle(angle);
        }

        /// <summary>
        /// Initializes a Projectile with a given position, dimensions, speed, and angle
        /// </summary>
        /// <param name="x">X coordinate of the top-left corner</param>
        /// <param name="y">Y coordinate of the top-left corner</param>
        /// <param name="width">Width in pixels</param>
        /// <param name="height">Height in pixels</param>
        /// <param name="speed">A positive speed (in pixels per frame update)</param>
        /// <param name="dir">A direction in which it will move</param>
        public Projectile(int x, int y, int width, int height, int speed, Direction dir)
            : base(x, y, width, height)
        {
            this.setSpeed(speed);
            this.setDirection(dir);
        }

        public int getSpeed() 
        { 
            return this.speed; 
        }
        public Direction getDirection() 
        { 
            return convertAngleToDirection(this.angle); 
        }
        public float getAngle()
        {
            return this.angle;
        }

        public void setSpeed(int speed)
        {
            if (speed < 0)
                speed = speed * -1;
            this.speed = speed;
        }
        public void setDirection(Direction dir)
        {
            this.angle = convertDirectionToAngle(dir);
        }
        public void setAngle(float angle)   // Angle in radians!
        {
            while (angle < 0.0f)
                angle += (float)Math.PI * 2;
            while (angle >= Math.PI * 2)
                angle -= (float)Math.PI * 2;
            this.angle = angle;
        }

        public override void update(float elapsed)
        {
            base.update(elapsed);
            this.setX(this.getX() + (int)Math.Round(this.speed * Math.Cos(this.angle)));
            this.setY(this.getY() - (int)Math.Round(this.speed * Math.Sin(this.angle)));  // y-axis is reversed
        }

        private float convertDirectionToAngle(Direction dir)
        {
            if (dir.Equals(Direction.North))
                return (float)Math.PI / 2;
            if (dir.Equals(Direction.East))
                return 0.0f;
            if (dir.Equals(Direction.South))
                return (float)Math.PI * 3 / 2;
            if (dir.Equals(Direction.West))
                return (float)Math.PI;
            return 0.0f;
        }

        private Direction convertAngleToDirection(float angle)
        {
            if ((angle <= Math.PI * 2 && angle >= Math.PI * 7 / 4) 
                || (angle < Math.PI * 1 / 4 && angle >= 0.0f))
                return Direction.East;
            if (angle < Math.PI * 3 / 4 && angle >= Math.PI * 1 / 4)
                return Direction.North;
            if (angle < Math.PI * 5 / 4 && angle >= Math.PI * 3 / 4)
                return Direction.West;
            if (angle < Math.PI * 7 / 4 && angle >= Math.PI * 5 / 4)
                return Direction.South;
            return Direction.Undefined;
        }
    }
}
