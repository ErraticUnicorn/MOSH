using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SunsetHigh
{
    /// <summary>
    /// Specifies all plot-related "triggers" caused by accepting quests (or going through plot-related events)
    /// </summary>
    public enum QuestID
    {
        FoodFight = 0,
        TeacherChase,
        //and so on...
    }

    /// <summary>
    /// Specifies a state for a particular quest (i.e. if it is accepted, complete, etc.)
    /// Different states can be combined with bitwise operations (for quests with multiple
    /// parts, branching quests, etc.)
    /// </summary>
    [Flags]
    public enum QuestState
    {
        Unavailable = 0x0,
        Available = 0x1,
        Accepted = 0x2,
        Progress1 = 0x4,
        Progress2 = 0x8,
        Progress3 = 0x10,
        Progress4 = 0x20,
        Progress5 = 0x40,
        Progress6 = 0x80,
        Progress7 = 0x100,
        Progress8 = 0x200,
        Progress9 = 0x400,
        Progress10 = 0x800,     // these "progress" values specify different points in the quest
        Complete = 0x1000,
    }

    /// <summary>
    /// Static class for managing quest triggers (events that set up changes in the game)
    /// </summary>
    public static class Quest
    {
        private static int NUM_QUEST_IDS = Enum.GetValues(typeof(QuestID)).Length;
        private static QuestState[] triggers;

        public static void setQuestAvailable(QuestID id)
        {
            addQuestState(id, QuestState.Available);
        }
        public static void setQuestAccepted(QuestID id)
        {
            addQuestState(id, QuestState.Accepted);
        }
        public static void setQuestComplete(QuestID id)
        {
            addQuestState(id, QuestState.Complete);
        }

        /// <summary>
        /// Adds the given state(s) to the quest with the given ID (keeping any old states)
        /// </summary>
        /// <param name="id">The particular quest</param>
        /// <param name="state">The state(s) to add</param>
        public static void addQuestState(QuestID id, QuestState state)
        {
            nullCheck();
            triggers[(int)id] |= state;
        }
        /// <summary>
        /// Removes the given state(s) from the quest with the given ID (keeping other states)
        /// </summary>
        /// <param name="id">The particular quest</param>
        /// <param name="state">The state(s) to remove</param>
        public static void removeQuestState(QuestID id, QuestState state)
        {
            nullCheck();
            triggers[(int)id] &= ~state;
        }
        /// <summary>
        /// Sets the quest with the given ID to a given state (overwrites any old states!)
        /// </summary>
        /// <param name="id">The particular quest</param>
        /// <param name="state">The new state(s) of the quest</param>
        public static void overwriteQuestState(QuestID id, QuestState state)
        {
            nullCheck();
            triggers[(int)id] = state;
        }

        public static bool isQuestAvailable(QuestID id)
        {
            return isQuestStateActive(id, QuestState.Available);
        }
        public static bool isQuestAccepted(QuestID id)
        {
            return isQuestStateActive(id, QuestState.Accepted);
        }
        public static bool isQuestComplete(QuestID id)
        {
            return isQuestStateActive(id, QuestState.Complete);
        }

        /// <summary>
        /// Checks a given quest for the given quest state(s)
        /// </summary>
        /// <param name="id">The particular quest</param>
        /// <param name="state">The quest state(s) to check</param>
        /// <returns>True if the state(s) are active, false otherwise</returns>
        public static bool isQuestStateActive(QuestID id, QuestState state)
        {
            nullCheck();
            return (triggers[(int)id] & state) == state;
        }

        /// <summary>
        /// Checks a given quest for whether the given state(s) is switched off
        /// </summary>
        /// <param name="id">The particular quest</param>
        /// <param name="state">The quest state(s) to check</param>
        /// <returns>True if the state(s) are inactive, false otherwise</returns>
        public static bool isQuestStateInactive(QuestID id, QuestState state)
        {
            nullCheck();
            return (~triggers[(int)id] & state) == state;
        }
        /// <summary>
        /// Returns the state(s) of the given quests. Check what states are active
        /// using bitwise operations (&).
        /// </summary>
        /// <param name="id">The particular quest</param>
        /// <returns>The state(s), as a combination of flags (bits)</returns>
        public static QuestState getQuestState(QuestID id)
        {
            nullCheck();
            return triggers[(int)id];
        }

        /// <summary>
        /// Used for saving purposes only
        /// </summary>
        /// <returns>A QuestState[] representation of the game's triggers</returns>
        public static QuestState[] getQuestStateSave()
        {
            nullCheck();
            return triggers;
        }

        /// <summary>
        /// Used for loading in quest states when restoring a saved game
        /// </summary>
        /// <param name="loadableTriggers">A QuestState[] representation of the states to load</param>
        public static void loadQuestStateSave(QuestState[] loadableTriggers)
        {
            nullCheck();
            if (loadableTriggers.Length <= triggers.Length)
                loadableTriggers.CopyTo(triggers, 0);
            else
                System.Diagnostics.Debug.WriteLine("Invalid array input; size must be same");
        }

        private static void nullCheck()
        {
            if (triggers == null || triggers.Length == 0)
            {
                triggers = new QuestState[NUM_QUEST_IDS];
                for (int i = 0; i < triggers.Length; i++)
                {
                    triggers[i] = QuestState.Unavailable;
                }
            }
        }
    }
}
