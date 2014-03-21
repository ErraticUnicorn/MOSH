using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Content;

namespace SunsetHigh
{
    public class MathClassroom : Room
    {
        private bool isFightMode;

        public MathClassroom()
            : base()
        {
        }

        public override void onEnter()
        {
            base.onEnter();
            if (Quest.isQuestComplete(QuestID.FoodFight))
            {
                isFightMode = true;
                ((BraceFace)CharacterManager.getCharacter(PersonID.BraceFace)).reset();
                ((BraceFace)CharacterManager.getCharacter(PersonID.BraceFace)).setAggressive(true);
                ((BraceFace)CharacterManager.getCharacter(PersonID.BraceFace)).setHealthBarVisible(true);
                Hero.instance.setHealthBarVisible(true);
                BGMusic.transitionToSong("sunset techno action.mp3");
            }
        }

        public override void onExit()
        {
            base.onExit();
            if (isFightMode)
            {
                ((BraceFace)CharacterManager.getCharacter(PersonID.BraceFace)).setHealthBarVisible(false);
                Hero.instance.setHealthBarVisible(false);
                BGMusic.transitionToSong("sunset high ambient.mp3");
            }
        }

        public override void loadContent(ContentManager content, string filename)
        {
            base.loadContent(content, filename);
        }

        public override void update(float elapsed)
        {
            base.update(elapsed);
            if (isFightMode)
            {
                BraceFace b1 = (BraceFace)CharacterManager.getCharacter(PersonID.BraceFace);
                if (Hero.instance.getHealth() <= 0)
                {
                    WorldManager.setRoom(PlaceID.HallwayWest, 2 * TILE_SIZE, 43 * TILE_SIZE, Direction.East);
                }
                else if (b1.getHealth() <= 0)
                {
                    b1.setAggressive(false);
                }
            }
        }
    }
}
