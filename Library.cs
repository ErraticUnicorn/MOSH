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
        private Character npc3;
        private Character npc4;
        public Library() : base()
        {
            npc1 = new Character(21 * 32, 6 * 32);
            npc2 = new Character(1 * 32, 11 * 32);
            npc3 = new Character(20 * 32, 12 * 32);
            npc4 = new Character(24 * 32, 14 * 32);
            npc1.getInventory().addItem(Item.PokeBall, 5);
            npc2.getInventory().addItem(Item.PokeBall, 5);
            npc3.getInventory().addItem(Item.PokeBall, 5);
            npc4.getInventory().addItem(Item.PokeBall, 5);
            CharList.Add(npc1);
            CharList.Add(npc2);
            CharList.Add(npc3);
            CharList.Add(npc4);
        }

        public override void loadContent(Microsoft.Xna.Framework.Content.ContentManager content, string filename)
        {
            base.loadContent(content, filename);
            npc1.loadImage(content, "sprite_ffwriter");
            npc2.loadImage(content, "sprite_brace_face_temp");
            npc3.loadImage(content, "humanCalc copy");
            npc4.loadImage(content, "sprite_enforcer");
        }
    }
}
