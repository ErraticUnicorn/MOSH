using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;

namespace SunsetHigh
{
    public abstract class AbstractScreen
    {
        public abstract void loadContent(ContentManager content);
        public abstract void update(float elapsed);
        public abstract void draw(SpriteBatch sb);

        public virtual void refresh() { }
        public virtual void moveCursor(Direction dir) { }
        public virtual void confirm() { }
        public virtual void cancel() { }
    }
}
