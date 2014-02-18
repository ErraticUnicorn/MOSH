using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Content;

namespace SunsetHigh
{
    public class HallwayEast : Room
    {
        public HallwayEast()
            : base()
        {
        }

        public override void loadContent(ContentManager content, string filename)
        {
            base.loadContent(content, filename);
        }

        public override void onWarpEnter()
        {
            if (Quest.isQuestStateActive(QuestID.FoodFight, QuestState.Progress1)
                && Quest.isQuestStateInactive(QuestID.FoodFight, QuestState.Progress2))
            {
                CharacterManager.moveCharacterToRoom(PersonID.Phil, PlaceID.HallwayEast);
                Character c1 = CharacterManager.getCharacter(PersonID.Phil);
                c1.setPosition(20 * TILE_SIZE, 3 * TILE_SIZE);
                c1.moveToDestination(20 * TILE_SIZE, -2 * TILE_SIZE);
                Hero.instance.stopFollower();
                Hero.instance.converse(c1);
            }
        }
    }
}
