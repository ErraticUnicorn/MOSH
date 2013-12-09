using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Content;

namespace SunsetHigh
{
    public class StudentLounge : Room
    {
        private Character npc1, npc2, npc3;

        public StudentLounge()
            : base()
        {
            npc1 = new Character(23 * TILE_SIZE, 2 * TILE_SIZE);
            npc1.setScript(Directories.INTERACTIONS + "Pianist.npc1.lounge.txt");
            this.addObject(npc1);

            npc2 = new Character(12 * TILE_SIZE, 8 * TILE_SIZE);
            npc2.setScript(Directories.INTERACTIONS + "Slacker.npc2.lounge.txt");
            this.addObject(npc2);

            npc3 = new Character(7 * TILE_SIZE, 16 * TILE_SIZE);
            this.addObject(npc3);
        }

        public override void loadContent(ContentManager content, string filename)
        {
            base.loadContent(content, filename);

            npc1.loadImage(content, Directories.CHARACTERS_TEMP + "nerd2");
            npc2.loadImage(content, Directories.CHARACTERS_TEMP + "slacker");
            npc3.loadImage(content, Directories.CHARACTERS_TEMP + "prep");
        }
    }
}
