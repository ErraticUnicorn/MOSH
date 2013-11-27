using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace SunsetHigh
{
    public abstract class MenuEntry
    {
        private string name;
        private int x;
        private int y;

        public MenuEntry(string name, int x, int y)
            : base()
        {
            this.name = name;
            this.x = x;
            this.y = y;
        }

        public MenuEntry(string name)
            : this(name, 0, 0)
        {
            this.name = name;
        }

        public MenuEntry()
            : this("N/A", 0, 0)
        {
        }

        public abstract void onPress();
        public virtual void onHover() { }
        public virtual void onUnhover() { }

        public void setName(string name) { this.name = name; }
        public void setX(int x) { this.x = x; }
        public void setY(int y) { this.y = y; }
        public string getName() { return this.name; }
        public int getX() { return this.x; }
        public int getY() { return this.y; }

        public virtual void draw(SpriteBatch sb, int x_offset, int y_offset, SpriteFont font, Color c)
        {
            sb.DrawString(font, this.getName(),
                new Vector2(x_offset + this.getX(), y_offset + this.getY()), c);
        }
    }
}
