using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SunsetHigh
{
    public sealed class JournalInfoPanel : TextPanel
    {
        private const int PANEL_WIDTH = 440;
        private const int PANEL_HEIGHT = 480;
        private const int PANEL_X = 200;
        private const int PANEL_Y = 480;
        private const int PANEL_X_MARGIN = 40;
        private const int PANEL_Y_MARGIN = 40;

        //Singleton implementation
        private static volatile JournalInfoPanel inst;
        private static object syncRoot = new Object();
        /// <summary>
        /// Returns an instance of LocationNamePanel
        /// </summary>
        public static JournalInfoPanel instance
        {
            get
            {
                if (inst == null)
                {
                    lock (syncRoot)
                    {
                        if (inst == null)
                            inst = new JournalInfoPanel();
                    }
                }
                return inst;
            }
        }

        private JournalInfoPanel()
            : base(PANEL_X, PANEL_Y, PANEL_WIDTH, PANEL_HEIGHT)
        {
            this.setPopLocations(PANEL_X, PANEL_Y, PANEL_X, 0);
            this.setXMargin(PANEL_X_MARGIN);
            this.setYMargin(PANEL_Y_MARGIN);
        }

        public override void onConfirm()
        {
            InGameMenu.goBack();
        }
    }
}
