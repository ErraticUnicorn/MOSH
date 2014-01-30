using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SunsetHigh
{
    public class Library : Room
    {
        private Character npc1;
        private Character npc2;
        public Library() : base()
        {
            npc1 = new Character(23 * TILE_SIZE, 5 * TILE_SIZE);
            npc2 = new Character(6 * TILE_SIZE, 13 * TILE_SIZE);
            npc1.inventory.addItem(Item.PokeBall, 5);
            npc2.inventory.addItem(Item.PokeBall, 5);
            npc1.setScript(Directories.INTERACTIONS + "Phil.libraryQuestInteraction.txt");
            npc1.setDirection(Direction.West);
            this.addObject(npc1);
            this.addObject(npc2);
        }

        public override void loadContent(Microsoft.Xna.Framework.Content.ContentManager content, string filename)
        {
            base.loadContent(content, filename);
            npc1.loadImage(content, Directories.CHARACTERS + "sprite_ffwriter");
            npc2.loadImage(content, Directories.CHARACTERS + "sprite_brace_face_temp");
        }
    }
}
