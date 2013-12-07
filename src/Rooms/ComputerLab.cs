using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Content;

namespace SunsetHigh
{
    public class ComputerLab : Room
    {
        private Character npc1, npc2, npc3;

        public ComputerLab()
            : base()
        {
            npc1 = new Character(9 * TILE_SIZE, 7 * TILE_SIZE);
            this.addObject(npc1);
            npc2 = new Character(17 * TILE_SIZE, 12 * TILE_SIZE);
            this.addObject(npc2);
            npc3 = new Character(21 * TILE_SIZE, 7 * TILE_SIZE);
            this.addObject(npc3);
        }

        public override void loadContent(ContentManager content, string filename)
        {
            base.loadContent(content, filename);

            npc1.loadImage(content, Directories.CHARACTERS + "humanCalc copy");
            npc2.loadImage(content, Directories.CHARACTERS_TEMP + "nerd");
            npc3.loadImage(content, Directories.CHARACTERS_TEMP + "jock2");
        }
    }
}
