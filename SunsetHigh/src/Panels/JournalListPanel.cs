using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SunsetHigh
{
    public class JournalDataEntry<T> : MenuEntry
    {
        protected struct ConditionalText
        {
            public string statement;
            public string text;
        }

        protected string baseText;
        protected T id;
        protected List<ConditionalText> condTextList;

        public JournalDataEntry(T id)
            : base(SunsetUtils.enumToString<T>(id), 0, 0)
        {
            this.id = id;
            condTextList = new List<ConditionalText>();
        }

        public T getID() { return this.id; }
        public void setBaseText(string text) { this.baseText = text; }

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
                if (ConditionalParser.evaluateStatement(condText.statement))
                    entryText += "\n" + condText.text;
            }
            JournalInfoPanel.instance.setMessage(entryText);
            InGameMenu.pushActivePanel(JournalInfoPanel.instance);
        }
    }

    public abstract class JournalListPanel<T> : ListPanel
    {
        protected JournalDataEntry<T>[] allEntries;

        public JournalListPanel()
            : this(0, 0, 0, 0) { }
        public JournalListPanel(int x, int y, int width, int height)
            : base(x, y, width, height) { }

        public virtual void refreshJournalList(object sender, EventArgs e)
        {
            //todo: add only when you talk to the person
            this.clearEntries();
            this.loadEntries(allEntries);
        }
        
        public virtual void loadEntriesFromFile(string filename, string prependConditional = "")
        {
            string[] lines = System.IO.File.ReadAllLines(filename);
            for (int i = 0; i < lines.Length; i++)
            {
                if (lines[i].StartsWith("#entry "))
                {
                    T id = SunsetUtils.parseEnum<T>(lines[i].Substring("#entry ".Length));
                    allEntries[Convert.ToInt32(id)] = new JournalDataEntry<T>(id);
                    JournalDataEntry<T> entry = allEntries[Convert.ToInt32(id)];
                    string baseText = "";

                    for (int j = i + 1; j < lines.Length; j++)
                    {
                        if (lines[j].Trim().Length == 0) continue;

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
                                if (lines[k].Trim().Length == 0) continue;

                                if (lines[k].StartsWith("#end"))
                                {
                                    if (id is QuestID)      //special case for quest text, prepends name automatically
                                    {
                                        ifStatement = "Quest " + SunsetUtils.enumToString<T>(id) + " " + ifStatement;
                                    }

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

            for (int i = 0; i < allEntries.Length; i++)
            {
                if (allEntries[i] == null)
                {
                    allEntries[i] = new JournalDataEntry<T>((T)Enum.ToObject(typeof(T), i));
                    allEntries[i].setBaseText("No description loaded.");
                }
            }
            refreshJournalList(this, null);
        }
    }

    public class QuestPanel : JournalListPanel<QuestID>
    {
        private const string COMPLETE_STRING = "[COMPLETE]";
        public QuestPanel()
            : this(0, 0, 0, 0) { }
        public QuestPanel(int x, int y, int width, int height)
            : base(x, y, width, height)
        {
            allEntries = new JournalDataEntry<QuestID>[Quest.NUM_QUEST_IDS];
            Quest.QuestStateChanged += refreshJournalList;
        }
        public override void refreshJournalList(object sender, EventArgs e)
        {
            if (e == null || (e is QuestEventArgs && 
                (((QuestEventArgs)e).questStateChange == QuestState.Accepted) ||
                (((QuestEventArgs)e).questStateChange == QuestState.Complete) ))
            {
                List<JournalDataEntry<QuestID>> tempEntries = new List<JournalDataEntry<QuestID>>();
                foreach (JournalDataEntry<QuestID> entry in allEntries)
                {
                    if (entry != null && Quest.isQuestAccepted(entry.getID()) && !Quest.isQuestComplete(entry.getID()))
                    {
                        tempEntries.Add(entry);
                    }
                }
                foreach (JournalDataEntry<QuestID> entry in allEntries)
                {
                    if (entry != null && Quest.isQuestComplete(entry.getID()))
                    {
                        tempEntries.Add(entry);
                        if (!entry.getName().EndsWith(COMPLETE_STRING))
                            entry.setName(entry.getName() + " " + COMPLETE_STRING);
                    }
                }
                this.clearEntries();
                this.loadEntries(tempEntries.ToArray());
            }
        }
    }

    public class PeoplePanel : JournalListPanel<PersonID>
    {
        public PeoplePanel()
            : this(0, 0, 0, 0) { }
        public PeoplePanel(int x, int y, int width, int height)
            : base(x, y, width, height)
        {
            allEntries = new JournalDataEntry<PersonID>[CharacterManager.NUM_PERSON_IDS];
        }
    }

    public class PlacesPanel : JournalListPanel<PlaceID>
    {
        public PlacesPanel()
            : this(0, 0, 0, 0) { }
        public PlacesPanel(int x, int y, int width, int height)
            : base(x, y, width, height)
        {
            allEntries = new JournalDataEntry<PlaceID>[Room.NUM_PLACE_IDS];
        }
    }
}
