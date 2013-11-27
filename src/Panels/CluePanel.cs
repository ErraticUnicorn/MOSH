using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SunsetHigh {
    public class ClueEntry : MenuEntry {
        public Clue m_clue { get; private set; }

        public ClueEntry(Clue p_clue) : base(p_clue.m_name) { m_clue = p_clue; }
        public override void onPress() { }
    }

    public class CluePanel : ListPanel {
        private IMessagePanel m_messagePanel;

        // In order for the player to match clues together, he must first select which clues to match.
        // This set contains all the currently selected clues.
        private HashSet<ClueEntry> m_selectedClues;
        private Color m_entrySelectColor;

        public CluePanel(int p_x, int p_y, int p_width, int p_height, IMessagePanel p_messagePanel) : base(p_x, p_y, p_width, p_height) {
            m_messagePanel = p_messagePanel;
            m_selectedClues = new HashSet<ClueEntry>();
            m_entrySelectColor = Color.AliceBlue;
        }

        public override void onEnter() {
            base.onEnter();
            updateClues();
            m_selectedClues.Clear();
        }

        public override void onConfirm() {
            base.onConfirm();
            if (this.entries.Count == 0) { return; }

            ClueEntry l_entry = getCurrentEntry() as ClueEntry;
            if (l_entry != null) {
                // toggle whether or not the clue has been selected
                if (m_selectedClues.Contains(l_entry)) {
                    m_selectedClues.Remove(l_entry);
                    System.Diagnostics.Debug.WriteLine("Deselected " + l_entry.m_clue.m_name);
                } else {
                    m_selectedClues.Add(l_entry);
                    System.Diagnostics.Debug.WriteLine("Selected " + l_entry.m_clue.m_name);
                }
            }
        }

        public override void onMoveCursor(Direction p_dir) {
            base.onMoveCursor(p_dir);

            updateClueDescription();

            // TODO: until we agree on a better keyboard input for the actual clue matching part, this should suffice
            if (p_dir == Direction.East) {
                List<Clue> l_selectedCluesList = new List<Clue>();
                foreach (ClueEntry ce in m_selectedClues) { l_selectedCluesList.Add(ce.m_clue); }

                // if the clues were successfully matched and an action was performed, update the menu entries
                if (Clue.performAction(l_selectedCluesList)) { updateClues(); }

                m_selectedClues.Clear();
            }
        }

        private void updateClues() {
            clearEntries();
            loadEntries(Clue.createMenuEntryList().ToArray());
            alignEntriesVertical();

            updateClueDescription();
        }

        private void updateClueDescription() {
            ClueEntry l_entry = getCurrentEntry() as ClueEntry;
            if (l_entry != null) {
                m_messagePanel.setMessage(l_entry.m_clue.m_description);
            }
        }
    }
}
