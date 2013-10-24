using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using TiledLib;

namespace SunsetHigh
{
    public class Room
    {
        public Map background;
        public List<Character> CharList;
        public List<Sprite> Interactables;

        public Room()
        {
            CharList = new List<Character>();
            Interactables = new List<Sprite>();
        }

        public virtual void loadContent(ContentManager content, String filename)
        {
            background = content.Load<Map>(filename);
        }

        public virtual void updateState()
        {

        }

        public virtual void update(float elapsed)
        {
            foreach (Sprite a in Interactables)
            {
                a.update(elapsed);
            }
            foreach (Character c in CharList)
            {
                c.update(elapsed);
            }
        }

        public virtual void draw(SpriteBatch sb)
        {
            foreach (Sprite a in Interactables)
            {
                a.draw(sb);
            }
            foreach (Character c in CharList)
            {
                c.draw(sb);
            }
        }

    }
}
