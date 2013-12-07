using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Content;

namespace SunsetHigh
{
    public class HallwayWest : Room
    {
        private Character npc1, npc2, npc3;

        public HallwayWest()
            : base()
        {
            npc1 = new Character(10 * TILE_SIZE, 11 * TILE_SIZE);
            //npc1.setScript(Directories.INTERACTIONS + "Lucas.cafeteriaInformationInteraction.txt");
            this.addObject(npc1);

            npc2 = new Character(3 * TILE_SIZE, 35 * TILE_SIZE);
            this.addObject(npc2);

            npc3 = new Character(20 * TILE_SIZE, 49 * TILE_SIZE);
            this.addObject(npc3);
        }

        public override void loadContent(ContentManager content, string filename)
        {
            base.loadContent(content, filename);
            npc1.loadImage(content, Directories.CHARACTERS_TEMP + "bully2");
            npc2.loadImage(content, Directories.CHARACTERS_TEMP + "prep2");
            npc3.loadImage(content, Directories.CHARACTERS_TEMP + "bully");
        }
    }
}
