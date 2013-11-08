using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace SunsetHigh
{
    /// <summary>
    /// Specifies a type of movement for a Sprite
    /// </summary>
    public enum SmoothMoveType
    {
        Linear,
        Sqrt,
        Square
    }

    /// <summary>
    /// An extension of Sprite that is designed to move automatically
    /// to any given location. Different types of movement (specified in
    /// the SmoothMoveType enum) can be used for different effects.
    /// </summary>
    public class SmoothMovingSprite : Sprite
    {
        private const float DEFAULT_SMOOTH_TIME = 1.0f;
        //private Point cameraOffset;
        private Vector2 smoothStart;
        private Vector2 smoothEnd;
        private float smoothDuration;
        private float smoothTimer;
        private bool smoothMoving;
        private SmoothMoveType smoothType;

        /// <summary>
        /// Initializes a SmoothMovingSprite at the origin which will match the dimensions
        /// of its texture when the texture is loaded
        /// </summary>
        public SmoothMovingSprite()
            : this(0, 0, 0, 0) { }

        /// <summary>
        /// Initializes a SmoothMovingSprite at the given position. It will match the dimensions
        /// of its texture when the texture is loaded.
        /// </summary>
        /// <param name="x">X coordinate of top-left corner</param>
        /// <param name="y">Y coordinate of top-left corner</param>
        public SmoothMovingSprite(int x, int y)
            : this(x, y, 0, 0) { }

        /// <summary>
        /// Initializes a SmoothMovingSprite at the given position with the given dimensions
        /// </summary>
        /// <param name="x">X coordinate of top-left corner</param>
        /// <param name="y">Y coordinate of top-left corner</param>
        /// <param name="width">Width in pixels</param>
        /// <param name="height">Height in pixels</param>
        public SmoothMovingSprite(int x, int y, int width, int height)
            : base(x, y, width, height)
        {
            //cameraOffset = new Point();
            smoothStart = new Vector2(this.getX(), this.getY());
            smoothEnd = new Vector2();
            smoothDuration = DEFAULT_SMOOTH_TIME;
            smoothTimer = 0.0f;
            smoothMoving = false;
            smoothType = SmoothMoveType.Linear;
        }

        /// <summary>
        /// Requests that the Sprite begin smoothly moving to a specified point within 
        /// the given amount of time.
        /// </summary>
        /// <param name="x">X coordinate of the new location (upper-left corner)</param>
        /// <param name="y">Y coordinate of the new location (upper-left corner)</param>
        /// <param name="time">Time taken to move in seconds</param>
        public void smoothMove(int x, int y, float time)
        {
            smoothStart.X = this.getX();
            smoothStart.Y = this.getY();
            smoothEnd.X = x;
            smoothEnd.Y = y;
            smoothDuration = time;
            smoothTimer = 0.0f;
            smoothMoving = true;
        }

        /// <summary>
        /// Queues the Sprite to begin smoothly moving to a specified point within 
        /// a default amount of time.
        /// </summary>
        /// <param name="x">X coordinate of the new location (upper-left corner)</param>
        /// <param name="y">Y coordinate of the new location (upper-left corner)</param>
        public void smoothMove(int x, int y)
        {
            this.smoothMove(x, y, DEFAULT_SMOOTH_TIME);
        }

        /// <summary>
        /// Requests that the Sprite begin smoothly moving its center to a specified point within
        /// the given amount of time
        /// </summary>
        /// <param name="x">X coordinate of the new location (center)</param>
        /// <param name="y">Y coordinate of the new location (center)</param>
        /// <param name="time">Time taken to move in seconds</param>
        public void smoothMoveCenter(int x, int y, float time)
        {
            this.smoothMove(x - this.getWidth() / 2, y - this.getHeight() / 2, time);
        }

        /// <summary>
        /// Requests that the Sprite begin smoothly moving its center to a specified point within
        /// the default amount of time
        /// </summary>
        /// <param name="x">X coordinate of the new location (center)</param>
        /// <param name="y">Y coordinate of the new location (center)</param>
        public void smoothMoveCenter(int x, int y)
        {
            this.smoothMoveCenter(x, y, DEFAULT_SMOOTH_TIME);
        }

        public void smoothMoveCameraAdjust(int delta_x, int delta_y)
        {
            //if ((cameraOffset.X == delta_x && cameraOffset.Y == delta_y) || !this.isSmoothMoving())
            //    return;
            //int delta_x = (int)(delta_x - cameraOffset.X);
            //int delta_y = (int)(delta_y - cameraOffset.Y);
            this.smoothEnd.X += delta_x;
            this.smoothEnd.Y += delta_y;
            //System.Diagnostics.Debug.WriteLine(delta_x + " " + delta_y);
            //cameraOffset.X = delta_x;
            //cameraOffset.Y = delta_y;
        }

        //public void setSmoothMoveDuration(float time) {
        //    this.smoothDuration = time;
        //}

        //public float getSmoothMoveDuration() {
        //    return this.smoothDuration;
        //}

        public void setSmoothMoveType(SmoothMoveType type) {
            this.smoothType = type;
        }

        public SmoothMoveType getSmoothMoveType() {
            return this.smoothType;
        }

        public bool isSmoothMoving() {
            return this.smoothMoving;
        }

        //public float getSmoothTimer() {
        //    return this.smoothTimer;
        //}

        /// <summary>
        /// Returns a number between 0.0 and 1.0 representing the distance between
        /// the start and end points of this object's current travel.
        /// </summary>
        /// <returns>A number between 0.0 (inclusive) and 1.0 (exclusive). 0.0 if the object is not moving.</returns>
        public float getFractionTraveled()
        {
            if (!this.isSmoothMoving())
                return 0.0f;
            return (new Vector2(this.getX(), this.getY()) - this.smoothStart).Length()
                / (this.smoothEnd - this.smoothStart).Length();
        }

        public override void update(float elapsed)
        {
            base.update(elapsed);
            if (this.smoothMoving)
            {
                this.smoothTimer += elapsed;
                float param = smoothTimer/smoothDuration;
                if (param > 1) //0 = @start, 1 = @end
                {
                    this.setX((int)smoothEnd.X);
                    this.setY((int)smoothEnd.Y);
                    this.smoothMoving = false;
                    this.smoothTimer = 0.0f;
                }
                else
                {
                    Vector2 currentPos = new Vector2();
                    if(this.smoothType.Equals(SmoothMoveType.Linear))
                        currentPos = Vector2.Lerp(smoothStart, smoothEnd, param);
                    else if (this.smoothType.Equals(SmoothMoveType.Sqrt))
                        currentPos = Vector2.Lerp(smoothStart, smoothEnd, (float)Math.Pow(param, 0.5));
                    else if (this.smoothType.Equals(SmoothMoveType.Square))
                        currentPos = Vector2.Lerp(smoothStart, smoothEnd, (float)Math.Pow(param, 2.0));
                    this.setX((int)currentPos.X);
                    this.setY((int)currentPos.Y);
                }
            }
        }
    }
}
