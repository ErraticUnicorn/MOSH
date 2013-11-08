using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace SunsetHigh
{
    /// <summary>
    /// An extension of SmoothMovingSprite that is designed to move on and off
    /// screen between two (generally static) points. Handles transitions during moving
    /// transitions (i.e. popping out while popping in and vice-versa) with ease.
    /// Used for menu components.
    /// </summary>
    public class PopInOutSprite : SmoothMovingSprite
    {
        private const float DEFAULT_POP_TIME = 1.0f;

        private bool inFocus;
        private bool movingIn;
        private bool movingOut;
        private int hideX;
        private int hideY;
        private int appearX;
        private int appearY;
        private float popTime;

        public PopInOutSprite()
            : this(0, 0, 0, 0) { }

        public PopInOutSprite(int x, int y)
            : this(x, y, 0, 0) { }

        public PopInOutSprite(int x, int y, int width, int height)
            : base(x, y, width, height)
        {
            this.inFocus = false;
            this.movingIn = false;
            this.movingOut = false;
            this.popTime = DEFAULT_POP_TIME;
            //this.setVisible(false);
        }

        public void setHideX(int x) { this.hideX = x; }
        public void setHideY(int y) { this.hideY = y; }
        public void setAppearX(int x) { this.appearX = x; }
        public void setAppearY(int y) { this.appearY = y; }

        public int getHideX() { return this.hideX; }
        public int getHideY() { return this.hideY; }
        public int getAppearX() { return this.appearX; }
        public int getAppearY() { return this.appearY; }
        public bool isInFocus() { return this.inFocus; }
        public bool isHiding() { return !this.inFocus; }
        public bool isMovingIn() { return this.movingIn; }
        public bool isMovingOut() { return this.movingOut; }

        public void setPopLocations(int hideX, int hideY, int appearX, int appearY)
        {
            this.hideX = hideX;
            this.hideY = hideY;
            this.appearX = appearX;
            this.appearY = appearY;
        }

        public void setPopDuration(float time) {
            this.popTime = time;
        }
        public float getPopDuration() {
            return this.popTime;
        }

        public void popIn()
        {
            if (this.movingIn)
                return;

            this.inFocus = true;
            this.setVisible(true);
            if ((this.isSmoothMoving() && this.movingOut) /*|| (this.getX() != this.getHideX() || this.getY() != this.getHideY())*/)
            {
                Vector2 startToEnd = new Vector2(this.getAppearX() - this.getHideX(), this.getAppearY() - this.getHideY());
                Vector2 currentToEnd = new Vector2(this.getX() - appearX, this.getY() - appearY);
                float remainder = this.getPopDuration() * (currentToEnd.Length() / startToEnd.Length());
                this.smoothMove(this.getAppearX(), this.getAppearY(), remainder);
            }
            else
            {
                this.smoothMove(this.getAppearX(), this.getAppearY(), this.getPopDuration());
            }
            this.movingIn = true;
            this.movingOut = false;
        }

        public void popOut()
        {
            if (this.movingOut)
                return;

            this.inFocus = false;
            if ((this.isSmoothMoving() && this.movingIn) || (this.getX() != this.getAppearX() || this.getY() != this.getAppearY()))
            {
                Vector2 startToEnd = new Vector2(this.getAppearX() - this.getHideX(), this.getAppearY() - this.getHideY());
                Vector2 currentToEnd = new Vector2(this.getX() - hideX, this.getY() - hideY);
                float remainder = this.getPopDuration() * (currentToEnd.Length() / startToEnd.Length());
                this.smoothMove(this.getHideX(), this.getHideY(), remainder);
            }
            else
            {
                this.smoothMove(this.getHideX(), this.getHideY(), this.getPopDuration());
            }
            this.movingOut = true;
            this.movingIn = false;
        }

        public override void update(float elapsed)
        {
            base.update(elapsed);
            if (!this.isSmoothMoving() && !this.inFocus) //i.e. has moved out of view
            {
                this.setVisible(false);
            }
            if (!this.isSmoothMoving())
            {
                this.movingIn = false;
                this.movingOut = false;
            }
        }

    }
}
