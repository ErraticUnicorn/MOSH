using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Content;

namespace SunsetHigh
{
    public class MagazineOffice : Room
    {
        private Character npc1;

        public MagazineOffice()
            : base()
        {
            npc1 = new Character(4 * TILE_SIZE, 7 * TILE_SIZE);
            this.addObject(npc1);
        }

        public override void loadContent(ContentManager content, string filename)
        {
            base.loadContent(content, filename);

            npc1.loadImage(content, Directories.CHARACTERS + "sprite_enforcer");
        }
    }
}
