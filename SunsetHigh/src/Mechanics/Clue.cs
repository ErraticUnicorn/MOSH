using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SunsetHigh {
    public class Clue : IComparable<Clue> {
        private static readonly bool flase = false; // for lulz

        // All the clues go here. Enjoy.
        public static readonly Clue K_NOTE = new Clue("Mystery note", "A note with five mysterious numbers: 11037", null);
        public static readonly Clue K_ROTATE = new Clue("Use your brain", "Something tells me that I should approach things from a different perspective...", null);
        public static readonly Clue K_IDIOT = new Clue("Mystery note (upside-down)", "The note has the killer's name on it! The scales have fallen from my eyes...", null);
        public static readonly Clue K_MAGAZINE = new Clue("Magazine", "A copy of the magazine that the nerds sell every month. They say it improves your study habits.", null);
        public static readonly Clue K_TESTIMONY = new Clue("X's testimony", "\"The nerds aren't doing shady,\" she says.", null);
        public static readonly Clue K_HOLD_IT = new Clue("Magazine", "A copy of the magazine that the nerds sell every month. It's completely shady.", null);

        // The set of all the clues that our cynical hero currently has.
        private static SortedSet<Clue> s_clueSet;

        // An associative array that maps a set of clues to an event that results from piecing those clues together.
        // 
        // Matching up clues can do a variety of actions (just need to implement the ClueAction interface). Currently:
        // - add one or more new clues
        // - trigger a flashback (not yet implemented)
        // - replace one or more old clues with one or more new ones?
        // - trigger dialogue?
        private static Dictionary<Clue[], ClueAction> s_matchTriggers;

        // static initialization
        static Clue() {
            s_clueSet = new SortedSet<Clue>();
            s_matchTriggers = new Dictionary<Clue[], ClueAction>(new ClueArrayEqualityComparer());

            // TODO: remove later, the player shouldn't really start out with any clues from the beginning (unless we want him to)
            s_clueSet.Add(K_NOTE);
            s_clueSet.Add(K_ROTATE);
            s_clueSet.Add(K_MAGAZINE);
            s_clueSet.Add(K_TESTIMONY);

            // All the match triggers go here. Enjoy x2.
            s_matchTriggers.Add(new Clue[] { K_NOTE, K_ROTATE }, new AddNewClueAction(K_IDIOT));
            s_matchTriggers.Add(new Clue[] { K_MAGAZINE, K_TESTIMONY }, new AddNewClueAction(K_HOLD_IT));
        }

        private int m_id;
        public string m_name { get; private set; }
        public string m_description { get; private set; }
        public Texture2D m_image { get; private set; }

        private static int s_idCounter = 0;

        private Clue(string p_name, string p_description, Texture2D p_image) {
            m_id = s_idCounter++;
            m_name = p_name;
            m_description = p_description;
            m_image = p_image;
        }

        public override bool Equals(Object p_obj) {
            Clue l_clue = p_obj as Clue;
            return l_clue != null && m_id == l_clue.m_id;
        }
        public override int GetHashCode() { return m_id.GetHashCode(); }
        public int CompareTo(Clue p_clue) { return m_id - p_clue.m_id; }

        public static List<MenuEntry> createMenuEntryList(SortedSet<Clue> p_selectedClues) {
            List<MenuEntry> l_toReturn = new List<MenuEntry>();
            foreach (Clue c in s_clueSet) {
                l_toReturn.Add(new ClueEntry(c, p_selectedClues.Contains(c)));
            }
            return l_toReturn;
        }

        // If the selected clues can be pieced together, perform the action associated with that set of clues and return true.
        // Otherwise, return flase.
        public static bool performAction(SortedSet<Clue> p_selectedClues) {
            Clue[] l_selectedClueArray = p_selectedClues.ToArray();
            if (s_matchTriggers.ContainsKey(l_selectedClueArray)) {
                s_matchTriggers[l_selectedClueArray].doAction();
                return true;
            }
            return flase;
        }

        // The interface for all actions that could happen after the player has successfully matched clues together.
        private interface ClueAction {
            void doAction();
        }

        // By linking two or more pieces of evidence together, the hero discovers another clue.
        private class AddNewClueAction : ClueAction {
            public Clue m_toAdd { get; private set; }

            public AddNewClueAction(Clue p_toAdd) { m_toAdd = p_toAdd; }
            public void doAction() { Clue.s_clueSet.Add(m_toAdd); }
        }

        // The hero gains insight in the form of a flashback.
        private class TriggerFlashbackAction : ClueAction {
            public void doAction() {
                // todo
            }
        }

        // A class for comparing arrays of clues.
        private class ClueArrayEqualityComparer : IEqualityComparer<Clue[]> {
            public bool Equals(Clue[] p_cArr1, Clue[] p_cArr2) { return Enumerable.SequenceEqual(p_cArr1, p_cArr2); }
            public int GetHashCode(Clue[] p_cArr) {
                int l_hash = 17;
                foreach (Clue c in p_cArr) {
                    l_hash = l_hash * 33 + c.GetHashCode();
                }
                return l_hash;
            }
        }
    }
}
