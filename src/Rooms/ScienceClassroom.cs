using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Content;

namespace SunsetHigh
{
    public class ScienceClassroom : Room
    {
        private Character npc1, npc2;

        public ScienceClassroom()
            : base()
        {
            npc1 = new Character(1 * TILE_SIZE, 3 * TILE_SIZE);
            npc1.setScript(Directories.INTERACTIONS + "ShadyNerd.npc1.science.txt");
            this.addObject(npc1);

            npc2 = new Character(11 * TILE_SIZE, 7 * TILE_SIZE);
            npc2.setScript(Directories.INTERACTIONS + "Jock.npc2.science.txt");
            this.addObject(npc2);
        }

        public override void loadContent(ContentManager content, string filename)
        {
            base.loadContent(content, filename);

            npc1.loadImage(content, Directories.CHARACTERS_TEMP + "nerd");
            npc2.loadImage(content, Directories.CHARACTERS_TEMP + "jock");
        }
    }
}
