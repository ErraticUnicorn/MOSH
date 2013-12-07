using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Content;

namespace SunsetHigh
{
    public class Bathroom : Room
    {
        private Character npc1;

        public Bathroom() 
            : base()
        {
            npc1 = new Character(13 * TILE_SIZE, 1 * TILE_SIZE);
            this.addObject(npc1);
        }

        public override void loadContent(ContentManager content, string filename)
        {
            base.loadContent(content, filename);

            npc1.loadImage(content, Directories.CHARACTERS_TEMP + "slacker2");
        }
    }
}
