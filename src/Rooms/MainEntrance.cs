using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Content;

namespace SunsetHigh
{
    public class MainEntrance : Room
    {
        private Character npc1, npc2;

        public MainEntrance()
            : base()
        {
            npc1 = new Character(10 * TILE_SIZE, 7 * TILE_SIZE);
            npc1.setScript(Directories.INTERACTIONS + "Stoner.npc1.entrance.txt");
            this.addObject(npc1);

            npc2 = new Character(27 * TILE_SIZE, 3 * TILE_SIZE);
            this.addObject(npc2);
        }

        public override void loadContent(ContentManager content, string filename)
        {
            base.loadContent(content, filename);

            npc1.loadImage(content, Directories.CHARACTERS_TEMP + "slacker");
            npc2.loadImage(content, Directories.CHARACTERS_TEMP + "jock");
        }
    }
}
