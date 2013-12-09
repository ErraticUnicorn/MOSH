using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Content;

namespace SunsetHigh
{
    public class MathClassroom : Room
    {
        private Character npc1, npc2, npc3, npc4;

        public MathClassroom()
            : base()
        {
            npc1 = new Character(8 * TILE_SIZE, 4 * TILE_SIZE);
            npc1.setScript(Directories.INTERACTIONS + "BoringTeacher.npc1.math.txt");
            this.addObject(npc1);

            npc2 = new Character(17 * TILE_SIZE, 16 * TILE_SIZE);
            this.addObject(npc2);

            npc3 = new Character(8 * TILE_SIZE, 8 * TILE_SIZE);
            this.addObject(npc3);

            npc4 = new Character(12 * TILE_SIZE, 14 * TILE_SIZE);
            npc4.setScript(Directories.INTERACTIONS + "ConfusedDude.npc4.math.txt");
            this.addObject(npc4);
        }

        public override void loadContent(ContentManager content, string filename)
        {
            base.loadContent(content, filename);

            npc1.loadImage(content, Directories.CHARACTERS_TEMP + "scienceteacher");
            npc2.loadImage(content, Directories.CHARACTERS_TEMP + "slacker2");
            npc3.loadImage(content, Directories.CHARACTERS + "sprite_boxChildAll");
            npc3.setSourceRect(72, 60, 32, 50);
            npc3.setDimensions(32, 50);
            npc4.loadImage(content, Directories.CHARACTERS_TEMP + "jock");
        }
    }
}
