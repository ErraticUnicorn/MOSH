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
        private struct ConditionalText
        {
            public string statement;
            public string text;
        }

        private const string COMPLETE_STRING = "[COMPLETE]";
        private string baseText;
        private QuestID id;
        private List<ConditionalText> condTextList;

        public QuestDataEntry(QuestID id)
            : base(SunsetUtils.enumToString<QuestID>(id), 0, 0)
        {
            this.id = id;
            condTextList = new List<ConditionalText>();
        }

        public QuestID getQuestID()
        {
            return this.id;
        }

        public void setBaseText(string text)
        {
            this.baseText = text;
        }

        public void pushConditionalText(string statement, string text)
        {
            ConditionalText condText = new ConditionalText();
            condText.statement = statement;
            condText.text = text;
            condTextList.Add(condText);
        }

        public override void onPress()
        {
            string entryText = baseText;
            foreach (ConditionalText condText in condTextList)
            {
                if (ConditionalParser.evaluateStatement("Quest " + SunsetUtils.enumToString<QuestID>(id) + " " + condText.statement))
                    entryText += condText.text;
            }
            JournalInfoPanel.instance.setMessage(entryText);
            InGameMenu.pushActivePanel(JournalInfoPanel.instance);
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
        private List<QuestDataEntry> allEntries;

        public QuestPanel()
            : this(0, 0, 0, 0) { }
        public QuestPanel(int x, int y, int width, int height)
            : base(x, y, width, height)
        {
            allEntries = new List<QuestDataEntry>();
            Quest.QuestStateChanged += updateQuestList;
        }

        public void updateQuestList(object sender, QuestEventArgs e)
        {
            if (e == null || e.questStateChange == QuestState.Accepted || e.questStateChange == QuestState.Complete)
            {
                List<QuestDataEntry> tempEntries = new List<QuestDataEntry>();
                foreach (QuestDataEntry entry in allEntries)
                {
                    if (Quest.isQuestAccepted(entry.getQuestID()) && !Quest.isQuestComplete(entry.getQuestID()))
                    {
                        tempEntries.Add(entry);
                    }
                }
                foreach (QuestDataEntry entry in allEntries)
                {
                    if (Quest.isQuestComplete(entry.getQuestID()))
                    {
                        tempEntries.Add(entry);
                    }
                }
                this.clearEntries();
                this.loadEntries(tempEntries.ToArray());
            }
        }

        public void loadEntriesFromFile()
        {
            // somewhat ugly parser...
            string[] lines = System.IO.File.ReadAllLines(Directories.TEXTDATA + "QuestJournalInfo.txt");
            
            for (int i = 0; i < lines.Length; i++)
            {
                if (lines[i].StartsWith("#entry "))
                {
                    QuestID questID = SunsetUtils.parseEnum<QuestID>(lines[i].Substring("#entry ".Length));
                    QuestDataEntry entry = new QuestDataEntry(questID);
                    string baseText = "";

                    for (int j = i + 1; j < lines.Length; j++)
                    {
                        if (lines[j].StartsWith("#title "))
                        {
                            entry.setName(lines[j].Substring("#title ".Length));
                        }
                        else if (lines[j].StartsWith("#if "))
                        {
                            string ifText = "";
                            string ifStatement = lines[j].Substring("#if ".Length);

                            for (int k = j + 1; k < lines.Length; k++)
                            {
                                if (lines[k].StartsWith("#end"))
                                {
                                    //finished if statement
                                    entry.pushConditionalText(ifStatement, ifText);
                                    j = k;
                                    break;
                                }
                                else
                                {
                                    ifText += lines[k] + "\n";
                                }
                            }
                        }
                        else if (lines[j].StartsWith("#end"))
                        {
                            //finished entry
                            entry.setBaseText(baseText);
                            allEntries.Add(entry);
                            i = j;
                            break;
                        }
                        else
                        {
                            baseText += lines[j] + "\n";
                        }
                    }
                }
            }

            this.updateQuestList(null, null);
        }
    }
}
