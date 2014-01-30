using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SunsetHigh
{
    public class PanelGroup
    {
        private List<Panel> panels;
        public PanelGroup()
            : this(null) { }
        public PanelGroup(params Panel[] panels)
        {
            this.panels = new List<Panel>();
            this.addPanels(panels);
        }

        public void addPanels(params Panel[] panels)
        {
            if (panels != null && panels.Length > 0)
            {
                foreach (Panel p in panels)
                {
                    if (p != null)
                        this.panels.Add(p);
                }
            }
        }

        public List<Panel> getPanels()
        {
            return this.panels;
        }

        public bool contains(Panel p)
        {
            return this.panels.Contains(p);
        }

        public void clear()
        {
            this.panels.Clear();
        }

        public void popIn()
        {
            foreach (Panel p in panels)
            {
                p.popIn();
                p.onEnter();
            }
        }

        public void popOut()
        {
            foreach (Panel p in panels)
            {
                p.popOut();
                p.onExit();
            }
        }
    }

    public static class PanelGroupSorter
    {
        private static HashSet<PanelGroup> panelGroups;
        private static HashSet<Panel> activePanels;
        private static HashSet<PanelGroup> activeGroups;

        public static void addPanelGroups(params PanelGroup[] pGroups)
        {
            nullCheck();
            if (pGroups != null)
            {
                foreach (PanelGroup pg in pGroups)
                {
                    if (pg != null)
                        panelGroups.Add(pg);
                }
            }
        }

        public static void panelIn(Panel panel)
        {
            nullCheck();
            if (activePanels.Contains(panel))
                return;
            activePanels.Add(panel);

            PanelGroup theGroup = null;
            foreach (PanelGroup pg in panelGroups)
                if (pg.contains(panel))
                {
                    theGroup = pg;
                    break;
                }
            if (!activeGroups.Contains(theGroup))
            {
                activeGroups.Add(theGroup);
                theGroup.popIn();
            }
        }

        public static void panelOut(Panel panel)
        {
            nullCheck();
            if (!activePanels.Contains(panel))
                return;
            activePanels.Remove(panel);

            PanelGroup theGroup = null;
            foreach (PanelGroup pg in panelGroups)
                if (pg.contains(panel))
                {
                    theGroup = pg;
                    break;
                }
            foreach (Panel p in theGroup.getPanels())
            {
                if (activePanels.Contains(p))
                    return;
            }
            activeGroups.Remove(theGroup);
            theGroup.popOut();
        }

        private static void nullCheck()
        {
            if (panelGroups == null)
                panelGroups = new HashSet<PanelGroup>();
            if (activeGroups == null)
                activeGroups = new HashSet<PanelGroup>();
            if (activePanels == null)
                activePanels = new HashSet<Panel>();
        }
    }
}
