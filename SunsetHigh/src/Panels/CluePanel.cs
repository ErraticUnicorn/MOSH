using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SunsetHigh {
    public class ClueEntry : MenuEntry {
        public Clue m_clue { get; private set; }

        public ClueEntry(Clue p_clue, bool p_selected) : base((p_selected ? "  " : "") + p_clue.m_name) { m_clue = p_clue; }
        public override void onPress() { }
    }

    public class CluePanel : ListPanel {
        private IMessagePanel m_messagePanel;

        // In order for the player to match clues together, he must first select which clues to match.
        // This set contains all the currently selected clues.
        private SortedSet<Clue> m_selectedClues;
        private Color m_entrySelectColor;

        public CluePanel(int p_x, int p_y, int p_width, int p_height, IMessagePanel p_messagePanel) : base(p_x, p_y, p_width, p_height) {
            m_messagePanel = p_messagePanel;
            m_selectedClues = new SortedSet<Clue>();
            m_entrySelectColor = Color.AliceBlue;
        }

        public override void onEnter() {
            base.onEnter();
            m_selectedClues.Clear();
            updateClues();
        }

        public override void onConfirm() {
            base.onConfirm();

            Clue.performAction(m_selectedClues);

            m_selectedClues.Clear();
            updateClues();
        }

        public override void onMoveCursor(Direction p_dir) {
            base.onMoveCursor(p_dir);
            if (this.entries.Count == 0) { return; }

            updateClueDescription();

            if (p_dir == Direction.East) {
                ClueEntry l_entry = getCurrentEntry() as ClueEntry;
                if (l_entry != null && !m_selectedClues.Contains(l_entry.m_clue)) {
                    m_selectedClues.Add(l_entry.m_clue); // pressing right selects the clue
                    updateClues();
                }
            } else if (p_dir == Direction.West) {
                ClueEntry l_entry = getCurrentEntry() as ClueEntry;
                if (l_entry != null && m_selectedClues.Contains(l_entry.m_clue)) {
                    m_selectedClues.Remove(l_entry.m_clue); // pressing left deselects the clue
                    updateClues();
                }
            }
        }

        private void updateClues() {
            clearEntries();
            loadEntries(Clue.createMenuEntryList(m_selectedClues).ToArray());
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
