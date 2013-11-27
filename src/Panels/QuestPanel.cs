using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace SunsetHigh
{
    public class QuestDataEntry : MenuEntry
    {
        private const string COMPLETE_STRING = "[COMPLETE!]";
        private QuestID id;
        private Dictionary<QuestState, string> data;

        public QuestDataEntry(QuestID id)
            : base(SunsetUtils.enumToString<QuestID>(id), 0, 0)
        {
            this.id = id;
        }

        public override void onPress()
        {
            throw new NotImplementedException();
        }

        public override void draw(SpriteBatch sb, int x_offset, int y_offset, SpriteFont font, Color c)
        {
            if (Quest.isQuestComplete(id) && !this.getName().EndsWith(COMPLETE_STRING))
                this.setName(this.getName() + " " + COMPLETE_STRING);
            base.draw(sb, x_offset, y_offset, font, c);
        }
    }


    public class QuestPanel : ListPanel
    {
        public QuestPanel()
            : this(0, 0, 0, 0) { }
        public QuestPanel(int x, int y, int width, int height)
            : base(x, y, width, height)
        {
        }

        public void loadEntriesFromFile(string fileName)
        {

        }
    }
}
