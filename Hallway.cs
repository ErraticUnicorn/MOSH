using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using TiledLib;

namespace SunsetHigh
{
    class Hallway : Room
    {
        private bool isChasedQuest;
        private Map hall1;
        private Map hall2;
        public Hallway()
            : base()
        {
            if(Quest.isTriggered(QuestID.TeacherChase)){
                isChasedQuest = true;
            }

            else{
                isChasedQuest = false;
            }
        }

        public override void loadContent(ContentManager content, string filename)
        {
            base.loadContent(content, filename);
        }

        /*
        public override void loadContent(ContentManager content, string filename, string filename2)
        {
            base.loadContent(content, filename);
            base.loadContent(content, filename2);
        }
         */

        public override void updateState()
        {
            base.updateState();
            if (!isChasedQuest)
            {

            }
            else if (isChasedQuest)
            {

            }
        }

        public override void update(float elapsed)
        {
            updateState();
            base.update(elapsed);
        }
    }
}
