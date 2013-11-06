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
        FoodFight1 = 1,
        FoodFight2,
        TeacherChase = 1,
        //and so on...
    }

    /// <summary>
    /// Static class for managing quest triggers (events that set up changes in the game)
    /// </summary>
    public static class Quest
    {
        private static int NUM_QUEST_IDS = Enum.GetValues(typeof(QuestID)).Length;
        private static bool[] triggers;

        /// <summary>
        /// Sets a trigger with the given ID to be true
        /// </summary>
        /// <param name="id">The trigger name</param>
        public static void setTrigger(QuestID id)
        {
            nullCheck();
            triggers[(int)id] = true;
        }
        /// <summary>
        /// Sets a trigger with the given ID to be false
        /// </summary>
        /// <param name="id">The trigger name</param>
        public static void unsetTrigger(QuestID id)
        {
            nullCheck();
            triggers[(int)id] = false;
        }
        /// <summary>
        /// Sets or unsets a trigger depending on its state. I.e. if the trigger is inactive,
        /// it will be set active (true), and vice-versa
        /// </summary>
        /// <param name="id">The trigger name</param>
        public static void toggleTrigger(QuestID id)
        {
            nullCheck();
            triggers[(int)id] = !triggers[(int)id];
        }

        /// <summary>
        /// Checks if a given trigger is active (i.e. true)
        /// </summary>
        /// <param name="id">The trigger name</param>
        /// <returns>True if the given trigger is active, false if not</returns>
        public static bool isTriggered(QuestID id)
        {
            nullCheck();
            return triggers[(int)id];
        }

        /// <summary>
        /// Used for saving purposes only
        /// </summary>
        /// <returns>A bool[] representation of the game's triggers</returns>
        public static bool[] getTriggers()
        {
            nullCheck();
            return triggers;
        }

        /// <summary>
        /// Used for loading in quest triggers when restoring a saved game
        /// </summary>
        /// <param name="loadableTriggers">A bool[] representation of the triggers to load</param>
        public static void loadTriggers(bool[] loadableTriggers)
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
                triggers = new bool[NUM_QUEST_IDS]; //initializes the array
        }
    }
}
