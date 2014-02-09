using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Graphics;

namespace SunsetHigh
{
    public class ExitEntry : MenuEntry
    {
        private bool exit;
        public ExitEntry(string name, bool exiting)
        {
            this.setName(name);
            this.setExitType(exiting);
        }
        public bool isExiting() { return this.exit; }
        public void setExitType(bool exiting) { this.exit = exiting; }
        public override void onPress()
        {
            // if this entry confirms exiting the game
            if (this.isExiting())
            {
                // Cleanup?
                ScreenTransition.requestTransition(delegate()
                {
                    InGameMenu.reset();
                    Game1.changeScreen(GameState.SplashScreen);
                });
            }
            // this entry cancels exiting
            else
            {
                InGameMenu.goBack();
            }
        }
    }


    public class SaveTypeSpecifierEntry : SubMenuFocusEntry
    {
        IMessagePanel owner;
        private bool saving;
        private string newMessage;

        public SaveTypeSpecifierEntry(string name, IMessagePanel owner, SaveDataPanel next)
            : base(name, next) 
        {
            this.owner = owner;
        }
        public void setSaveType(bool saving) { this.saving = saving; }
        public bool isSaving() { return this.saving; }
        public void setNewMessage(string newMessage) { this.newMessage = newMessage; }
        public string getNewMessage() { return this.newMessage; }

        public override void onPress()
        {
            base.onPress();
            ((SaveDataPanel)(this.getNextPanel())).setSaveType(this.isSaving());
            this.owner.setMessage(this.getNewMessage());
        }
    }



    public class ScreenSpecifierEntry : SubMenuFocusEntry
    {
        IMessagePanel owner;
        private string newMessage;
        private List<Panel> invisibleOthers;

        public ScreenSpecifierEntry(string name, IMessagePanel owner, Panel next)
            : base(name, next) 
        {
            this.owner = owner;
            this.invisibleOthers = new List<Panel>();
        }
        public void setNewMessage(string newMessage) { this.newMessage = newMessage; }
        public string getNewMessage() { return this.newMessage; }
        public void setOtherPanels(params Panel[] otherPanels)
        {
            if (otherPanels != null)
            {
                foreach (Panel p in otherPanels)
                    if (p != null)
                        this.invisibleOthers.Add(p);
            }
        }

        public override void onHover()
        {
            base.onHover();
            this.getNextPanel().setVisible(true);
            foreach (Panel p in invisibleOthers)
                p.setVisible(false);
        }

        public override void onUnhover()
        {
            base.onUnhover();
        }

        public override void onPress()
        {
            base.onPress();
            if (this.newMessage != null && this.newMessage.Length != 0)
                this.owner.setMessage(this.getNewMessage());
        }
    }



    public class SubMenuFocusEntry : MenuEntry
    {
        private Panel nextFocus;
        public SubMenuFocusEntry()
            : this("No name", null) { }
        public SubMenuFocusEntry(string name)
            : this(name, null) { }
        public SubMenuFocusEntry(string name, Panel next)
            : base(name, 0, 0) 
        {
            this.setNextPanel(next);
        }
        public void setNextPanel(Panel next)
        {
            this.nextFocus = next;
        }
        public Panel getNextPanel()
        {
            return this.nextFocus;
        }
        public override void onPress()
        {
            if (nextFocus != null)
                InGameMenu.pushActivePanel(this.nextFocus);
        }
    }
}
