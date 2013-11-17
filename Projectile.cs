using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace SunsetHigh
{
    public class Projectile : FreeMovingSprite, IInteractable
    {
        private const float DEFAULT_ANGLE = 0.0f;
        private float angle;

        /// <summary>
        /// Initializes a Projectile at the origin with a default speed and heading east.
        /// </summary>
        public Projectile()
            : this(0, 0, DEFAULT_SPEED, DEFAULT_ANGLE) { }

        /// <summary>
        /// Initializes a Projectile with the given speed and angle
        /// </summary>
        /// <param name="speed">A positive speed (in pixels per second)</param>
        /// <param name="angle">An angle in radians (from 0 to 2 PI)</param>
        public Projectile(float speed, float angle)
            : this(0, 0, speed, angle) { }

        /// <summary>
        /// Initializes a Projectile with the given speed and direction
        /// </summary>
        /// <param name="speed">A positive speed (in pixels per second)</param>
        /// <param name="dir">A direction in which it will move</param>
        public Projectile(float speed, Direction dir)
            : this(0, 0, speed, dir) { }

        /// <summary>
        /// Initializes a Projectile with a given position, speed, and angle
        /// </summary>
        /// <param name="x">X coordinate of the top-left corner</param>
        /// <param name="y">Y coordinate of the top-left corner</param>
        /// <param name="speed">A positive speed (in pixels per second)</param>
        /// <param name="angle">An angle in radians (from 0 to 2 PI)</param>
        public Projectile(int x, int y, float speed, float angle)
            : this(x, y, 0, 0, speed, angle) { }

        /// <summary>
        /// Initializes a Projectile with a given position, speed, and direction
        /// </summary>
        /// <param name="x">X coordinate of the top-left corner</param>
        /// <param name="y">Y coordinate of the top-left corner</param>
        /// <param name="speed">A positive speed (in pixels per second)</param>
        /// <param name="dir">A direction in which it will move</param>
        public Projectile(int x, int y, float speed, Direction dir)
            : this(x, y, 0, 0, speed, dir) { }

        /// <summary>
        /// Initializes a Projectile with a given position and dimensions, a default speed heading east
        /// </summary>
        /// <param name="x">X coordinate of the top-left corner</param>
        /// <param name="y">Y coordinate of the top-left corner</param>
        /// <param name="width">Width in pixels</param>
        /// <param name="height">Height in pixels</param>
        public Projectile(int x, int y, int width, int height)
            : this(x, y, width, height, DEFAULT_SPEED, DEFAULT_ANGLE) { }

        /// <summary>
        /// Initializes a Projectile with a given position, dimensions, speed, and angle
        /// </summary>
        /// <param name="x">X coordinate of the top-left corner</param>
        /// <param name="y">Y coordinate of the top-left corner</param>
        /// <param name="width">Width in pixels</param>
        /// <param name="height">Height in pixels</param>
        /// <param name="speed">A positive speed (in pixels per second)</param>
        /// <param name="angle">An angle in radians (from 0 to 2 PI)</param>
        public Projectile(int x, int y, int width, int height, float speed, float angle)
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
        /// <param name="speed">A positive speed (in pixels per second)</param>
        /// <param name="dir">A direction in which it will move</param>
        public Projectile(int x, int y, int width, int height, float speed, Direction dir)
            : base(x, y, width, height)
        {
            this.setSpeed(speed);
            this.setDirection(dir);
        }

        public Direction getDirection() 
        { 
            return convertAngleToDirection(this.angle); 
        }
        public float getAngle()
        {
            return this.angle;
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
            this.move(this.getAngle(), elapsed, false);
        }

        private float convertDirectionToAngle(Direction dir)
        {
            switch (dir)
            {
                case Direction.North: return (float)Math.PI / 2;
                case Direction.NorthEast: return (float)Math.PI / 4;
                case Direction.East: return 0.0f;
                case Direction.SouthEast: return (float)Math.PI * 7 / 4;
                case Direction.South: return (float)Math.PI * 3 / 2;
                case Direction.SouthWest: return (float)Math.PI * 5 / 4;
                case Direction.West: return (float)Math.PI;
                case Direction.NorthWest: return (float)Math.PI * 3 / 4;
            }
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
