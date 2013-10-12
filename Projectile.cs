using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SunsetHigh
{
    public class Projectile : Sprite
    {
        private int speed;

        private Direction direction;

        public Projectile() : base()
        {
            speed = 0;

            direction = Direction.Undefined;
        }

        public Projectile(int x, int y, int width, int height)
            : base(x, y, width, height)
        {
            speed = 0;

            direction = Direction.Undefined;
        }

        public Projectile(int x, int y)
            : base(x, y)
        {
            speed = 0;

            direction = Direction.Undefined;
        }

        public int getSpeed()
        {
            return speed;
        }


        public Direction getDirection()
        {
            return direction;
        }


        public void setSpeed(int x)
        {
            speed = x;
        }


        public void setDirection(Direction x)
        {
            direction = x;
        }

        public override void update(float elapsed)
        {
            base.update(elapsed);
            if(direction.Equals(Direction.North)){
                this.setY(this.getY() - speed);
            }
            if(direction.Equals(Direction.South)){
                this.setY(this.getY() + speed);
            }
            if(direction.Equals(Direction.East)){
                this.setX(this.getX() + speed);
            }
            if(direction.Equals(Direction.West)){
                this.setX(this.getX() - speed);
            }
            //System.Diagnostics.Debug.WriteLine(this.getY());
        }
    }
}
