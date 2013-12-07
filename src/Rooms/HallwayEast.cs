﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Content;

namespace SunsetHigh
{
    public class HallwayEast : Room
    {
        private Character npc1;

        public HallwayEast()
            : base()
        {
            npc1 = new Character(19 * TILE_SIZE, 2 * TILE_SIZE);
            this.addObject(npc1);
        }

        public override void loadContent(ContentManager content, string filename)
        {
            base.loadContent(content, filename);

            npc1.loadImage(content, Directories.CHARACTERS_TEMP + "jock2");
        }
    }
}