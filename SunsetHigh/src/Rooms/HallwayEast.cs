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

        public override void updateState()
        {
            base.updateState();
            if (Quest.isQuestStateActive(QuestID.FoodFight, QuestState.Progress2))
            {
                Character c1 = CharacterManager.getCharacter(PersonID.Phil);
                if (this.CharList.Contains(c1))
                    c1.moveToDestination(20 * TILE_SIZE, -2 * TILE_SIZE, null);
            }
            if (Quest.isQuestStateActive(QuestID.FoodFight, QuestState.Progress4))
            {
                Character c1 = CharacterManager.getCharacter(PersonID.Artie);
                if (this.CharList.Contains(c1))
                    c1.moveToDestination(20 * TILE_SIZE, -2 * TILE_SIZE, null);
            }
            if (Quest.isQuestStateActive(QuestID.FoodFight, QuestState.Progress6))
            {
                Character c1 = CharacterManager.getCharacter(PersonID.Bill);
                if (this.CharList.Contains(c1))
                    c1.moveToDestination(20 * TILE_SIZE, -2 * TILE_SIZE, null);
            }
            if (Quest.isQuestStateActive(QuestID.FoodFight, QuestState.Progress8))
            {
                Character c1 = CharacterManager.getCharacter(PersonID.Claude);
                if (this.CharList.Contains(c1))
                    c1.moveToDestination(20 * TILE_SIZE, -2 * TILE_SIZE, null);
            }
        }

        public override void onEnter()
        {
            base.onEnter();
            if (Quest.isQuestStateActive(QuestID.FoodFight, QuestState.Progress1)
                && Quest.isQuestStateInactive(QuestID.FoodFight, QuestState.Progress2))
            {
                CharacterManager.moveCharacterToRoom(PersonID.Phil, PlaceID.HallwayEast, 20 * TILE_SIZE, 3 * TILE_SIZE);
                Character c1 = CharacterManager.getCharacter(PersonID.Phil);
                Hero.instance.stopFollower();
                Hero.instance.converse(c1);
            }
            if (Quest.isQuestStateActive(QuestID.FoodFight, QuestState.Progress3)
                && Quest.isQuestStateInactive(QuestID.FoodFight, QuestState.Progress4))
            {
                CharacterManager.moveCharacterToRoom(PersonID.Artie, PlaceID.HallwayEast, 20 * TILE_SIZE, 3 * TILE_SIZE);
                Character c1 = CharacterManager.getCharacter(PersonID.Artie);
                Hero.instance.stopFollower();
                Hero.instance.converse(c1);
            }
            if (Quest.isQuestStateActive(QuestID.FoodFight, QuestState.Progress5)
                && Quest.isQuestStateInactive(QuestID.FoodFight, QuestState.Progress6))
            {
                CharacterManager.moveCharacterToRoom(PersonID.Bill, PlaceID.HallwayEast, 20 * TILE_SIZE, 3 * TILE_SIZE);
                Character c1 = CharacterManager.getCharacter(PersonID.Bill);
                Hero.instance.stopFollower();
                Hero.instance.converse(c1);
            }
            if (Quest.isQuestStateActive(QuestID.FoodFight, QuestState.Progress7)
                && Quest.isQuestStateInactive(QuestID.FoodFight, QuestState.Progress8))
            {
                CharacterManager.moveCharacterToRoom(PersonID.Claude, PlaceID.HallwayEast, 20 * TILE_SIZE, 3 * TILE_SIZE);
                Character c1 = CharacterManager.getCharacter(PersonID.Claude);
                Hero.instance.stopFollower();
                Hero.instance.converse(c1);
            }
        }

        public override void onExit()
        {
            base.onExit();
            if (Quest.isQuestStateActive(QuestID.FoodFight, QuestState.Progress2)
                && this.CharList.Contains(CharacterManager.getCharacter(PersonID.Phil)))
                CharacterManager.moveCharacterToRoom(PersonID.Phil, PlaceID.Library, 22 * TILE_SIZE, 7 * TILE_SIZE);
        }
    }
}
