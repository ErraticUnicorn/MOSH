using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace SunsetHigh
{
    /// <summary>
    /// Static class for handling the in-game menu.
    /// </summary>
    public static class InGameMenu
    {
        private static bool initialized = false;
        private static bool menuOpen;

        private static List<Sprite> components; // used to store all the sprites to draw and update easily
        private static Stack<Panel> panelStack; // stack of panels
        private static Panel activePanel;       // pointer to the panel with our current focus (e.g. cursor control)
        private static ListPanel menuBody;      // the main menu panel
        private static CursorArrow menuArrow;   // the cursor

        //Panels with dynamic content
        private static InventoryPanel inventoryScreen;
        private static PlacesPanel placesScreen;
        private static PeoplePanel peopleScreen;
        private static QuestPanel questScreen;

        /// <summary>
        /// Initializes all the menu panels and links them togther. Call this in the Game's
        /// create cycle. Lots of hard coding here! - we'll have to adjust it if the game's
        /// viewport size changes.
        /// </summary>
        public static void init()
        {
            if (initialized)
                return;
            
            //initialize static variables
            components = new List<Sprite>();
            panelStack = new Stack<Panel>();
            activePanel = menuBody;
            menuOpen = false;
            initialized = true;

            // places list
            placesScreen = new PlacesPanel(200, 480, 440, 380);
            placesScreen.setPopLocations(200, 480, 200, 100);
            placesScreen.setScrolling(7, 1);
            placesScreen.loadEntriesFromFile(Directories.TEXTDATA + "PlacesJournalInfo.txt");
            components.Add(placesScreen);

            // people list
            peopleScreen = new PeoplePanel(200, 480, 440, 380);
            peopleScreen.setPopLocations(200, 480, 200, 100);
            peopleScreen.setScrolling(7, 1);
            peopleScreen.loadEntriesFromFile(Directories.TEXTDATA + "PeopleJournalInfo.txt");
            components.Add(peopleScreen);

            // quest list
            questScreen = new QuestPanel(200, 480, 440, 380);
            questScreen.setPopLocations(200, 480, 200, 100);
            questScreen.setScrolling(7, 1);
            questScreen.loadEntriesFromFile(Directories.TEXTDATA + "QuestJournalInfo.txt");
            components.Add(questScreen);

            // journal prompt
            DialogPanel journalPrompt = new DialogPanel(200, -100, 440, 100);
            journalPrompt.setMessage("What type of info do you need?");
            journalPrompt.setRefreshMessage(journalPrompt.getMessage());
            ScreenSpecifierEntry questEntry = new ScreenSpecifierEntry("Quests", journalPrompt, questScreen);
            questEntry.setNewMessage("Looking up past and current quests...");
            questEntry.setOtherPanels(peopleScreen, placesScreen);
            ScreenSpecifierEntry peopleEntry = new ScreenSpecifierEntry("People", journalPrompt, peopleScreen);
            peopleEntry.setNewMessage("Looking up intel on individuals...");
            peopleEntry.setOtherPanels(questScreen, placesScreen);
            ScreenSpecifierEntry placesEntry = new ScreenSpecifierEntry("Places", journalPrompt, placesScreen);
            placesEntry.setNewMessage("Looking up intel on locations...");
            placesEntry.setOtherPanels(questScreen, peopleScreen);
            journalPrompt.loadEntries(questEntry, peopleEntry, placesEntry);
            journalPrompt.setPopLocations(200, -100, 200, 0);
            journalPrompt.setYMargin(20);
            journalPrompt.setYDivider(journalPrompt.getHeight() - 30);
            components.Add(journalPrompt);

            // clues info screen
            TextPanel cluesInfoScreen = new TextPanel(400, -200, 240, 200);
            cluesInfoScreen.setMessage("Info about the clue here.");
            cluesInfoScreen.setPopLocations(400, -200, 400, 0);
            components.Add(cluesInfoScreen);

            // clues list
            ListPanel cluesScreen = new CluePanel(200, 480, 200, 480, cluesInfoScreen);
            cluesScreen.setPopLocations(200, 480, 200, 0);
            components.Add(cluesScreen);

            // inventory item info screen
            TextPanel inventoryInfoScreen = new TextPanel(200, 480, 440, 150);
            inventoryInfoScreen.setMessage("This is where information about the item will appear.");
            inventoryInfoScreen.setPopLocations(200, 480, 200, 330);
            components.Add(inventoryInfoScreen);

            // inventory screen
            inventoryScreen = new InventoryPanel(200, -330, 440, 330);
            inventoryScreen.setMessagePanel(inventoryInfoScreen);
            inventoryScreen.loadEntriesFromFile(Directories.TEXTDATA + "ItemInfo.txt");
            inventoryScreen.setScrolling(5, 2);
            inventoryScreen.setPopLocations(200, -330, 200, 0);
            components.Add(inventoryScreen);

            // reputation screen
            ReputationPanel reputationScreen = new ReputationPanel(200, 480, 440, 480);
            reputationScreen.setPopLocations(200, 480, 200, 0);
            components.Add(reputationScreen);
            
            // sound config screen
            SoundConfigPanel soundConfigScreen = new SoundConfigPanel(200, 480, 440, 380);
            soundConfigScreen.setPopLocations(200, 480, 200, 100);
            components.Add(soundConfigScreen);

            // key config screen
            KeyConfigPanel keyConfigScreen = new KeyConfigPanel(200, 480, 440, 380);
            KeyModifierEntry keyAction = new KeyModifierEntry("Talk/Action/Confirm", keyConfigScreen, KeyInputType.Action);
            KeyModifierEntry keyCancel = new KeyModifierEntry("Go Back/Cancel", keyConfigScreen, KeyInputType.Cancel);
            KeyModifierEntry keyShoot = new KeyModifierEntry("Shoot", keyConfigScreen, KeyInputType.Shoot);
            KeyModifierEntry keyPickpocket = new KeyModifierEntry("Pickpocket", keyConfigScreen, KeyInputType.Pickpocket);
            KeyModifierEntry keyUp = new KeyModifierEntry("Move Up", keyConfigScreen, KeyInputType.MoveNorth);
            KeyModifierEntry keyDown = new KeyModifierEntry("Move Down", keyConfigScreen, KeyInputType.MoveSouth);
            KeyModifierEntry keyLeft = new KeyModifierEntry("Move Left", keyConfigScreen, KeyInputType.MoveWest);
            KeyModifierEntry keyRight = new KeyModifierEntry("Move Right", keyConfigScreen, KeyInputType.MoveEast);
            KeyModifierEntry keyToggleMenu = new KeyModifierEntry("Toggle Menu", keyConfigScreen, KeyInputType.MenuToggle);
            keyConfigScreen.loadEntries(keyAction, keyCancel, keyShoot, keyPickpocket, keyUp, keyDown, keyLeft, keyRight, keyToggleMenu);
            keyConfigScreen.alignEntriesVertical();
            keyConfigScreen.setPopLocations(200, 480, 200, 100);
            components.Add(keyConfigScreen);

            // config prompt
            DialogPanel configPrompt = new DialogPanel(200, -100, 440, 100);
            keyConfigScreen.setPreviousPanel(configPrompt);
            configPrompt.setMessage("What would you like to configure?");
            configPrompt.setRefreshMessage(configPrompt.getMessage());
            ScreenSpecifierEntry keyEntry = new ScreenSpecifierEntry("Key Controls", configPrompt, keyConfigScreen);
            keyEntry.setNewMessage("Configuring key controls... Press [" + 
                Enum.GetName(typeof(Keys), KeyboardManager.getKeyControl(KeyInputType.Action)) + 
                "] to modify the key.");
            keyEntry.setOtherPanels(soundConfigScreen);
            ScreenSpecifierEntry soundEntry = new ScreenSpecifierEntry("Sound", configPrompt, soundConfigScreen);
            soundEntry.setNewMessage("Configuring sound settings...");
            soundEntry.setOtherPanels(keyConfigScreen);
            configPrompt.loadEntries(keyEntry, soundEntry);            
            configPrompt.setPopLocations(200, -100, 200, 0);
            configPrompt.setYMargin(20);
            configPrompt.setYDivider(configPrompt.getHeight() - 30);
            components.Add(configPrompt);

            // save file data screen
            SaveDataPanel saveScreen = new SaveDataPanel(200, 480, 440, 380);
            saveScreen.setScrolling(5, 1);
            saveScreen.loadSaveData();
            saveScreen.setPopLocations(200, 480, 200, 100);
            components.Add(saveScreen);

            // save/load prompt
            DialogPanel savePrompt = new DialogPanel(200, -100, 440, 100);
            saveScreen.setPreviousPanel(savePrompt);
            savePrompt.setMessage("Would you like to save the current game or load an older game?");
            savePrompt.setRefreshMessage(savePrompt.getMessage());
            SaveTypeSpecifierEntry saveEntry = new SaveTypeSpecifierEntry("Save", savePrompt, saveScreen);
            saveEntry.setSaveType(true);
            saveEntry.setNewMessage("Saving current progress...select a file to overwrite.");
            SaveTypeSpecifierEntry loadEntry = new SaveTypeSpecifierEntry("Load", savePrompt, saveScreen);
            loadEntry.setSaveType(false);
            loadEntry.setNewMessage("Loading another save file...select a file to load.");
            savePrompt.loadEntries(saveEntry, loadEntry);
            //savePrompt.alignEntriesHorizontal();
            savePrompt.setPopLocations(200, -100, 200, 0);
            savePrompt.setYMargin(20);
            savePrompt.setYDivider(savePrompt.getHeight() - 30);
            components.Add(savePrompt);

            // exit game prompt
            DialogPanel exitPrompt = new DialogPanel(200, 480, 350, 200);
            exitPrompt.setMessage("Are you sure you want to exit the game? All unsaved progress will be lost.");
            ExitEntry cancelEntry = new ExitEntry("Cancel", false);
            ExitEntry exitEntry = new ExitEntry("Quit Game", true);
            exitPrompt.loadEntries(cancelEntry, exitEntry);
            exitPrompt.setPopLocations(200, 480, 200, 280);
            components.Add(exitPrompt);

            // main menu
            menuBody = new ListPanel(-200, 0, 200, 480);
            SubMenuFocusEntry menuBodyJournal = new SubMenuFocusEntry("Journal", journalPrompt);
            SubMenuFocusEntry menuBodyClues = new SubMenuFocusEntry("Clues", cluesScreen);
            SubMenuFocusEntry menuBodyInventory = new SubMenuFocusEntry("Inventory", inventoryScreen);
            //SubMenuFocusEntry menuBodyEquip = new SubMenuFocusEntry("Equip", null);
            SubMenuFocusEntry menuBodyReputation = new SubMenuFocusEntry("Reputation", reputationScreen);
            SubMenuFocusEntry menuBodyConfig = new SubMenuFocusEntry("Config", configPrompt);
            SubMenuFocusEntry menuBodySave = new SubMenuFocusEntry("Save/Load", savePrompt);
            SubMenuFocusEntry menuBodyExit = new SubMenuFocusEntry("Exit Game", exitPrompt);
            menuBody.loadEntries(menuBodyJournal, menuBodyClues, menuBodyInventory, /*menuBodyEquip,*/ 
                menuBodyReputation, menuBodyConfig, menuBodySave, menuBodyExit);
            menuBody.alignEntriesVertical();
            menuBody.setPopLocations(-200, 0, 0, 0);
            components.Add(menuBody);

            // menu cursor
            menuArrow = new CursorArrow(-175, 67);
            menuArrow.setPopLocations(menuBody.getHideX() + 25, menuBody.getHideY() + 67,
                menuBody.getAppearX() + 25, menuBody.getAppearY() + 67);
            components.Add(menuArrow);

            // journal entry info screen
            // this panel "covers" the menu cursor, so it goes last
            components.Add(JournalInfoPanel.instance);

            //group panels that pop in and out together
            PanelGroup pg0 = new PanelGroup(menuBody);
            PanelGroup pg1 = new PanelGroup(journalPrompt, questScreen, peopleScreen, placesScreen);
            PanelGroup pg1a = new PanelGroup(JournalInfoPanel.instance);
            PanelGroup pg2 = new PanelGroup(cluesScreen, cluesInfoScreen);
            PanelGroup pg3 = new PanelGroup(inventoryScreen, inventoryInfoScreen);
            PanelGroup pg4 = new PanelGroup(reputationScreen);
            PanelGroup pg5 = new PanelGroup(configPrompt, keyConfigScreen, soundConfigScreen);
            PanelGroup pg6 = new PanelGroup(saveScreen, savePrompt);
            PanelGroup pg7 = new PanelGroup(exitPrompt);

            PanelGroupSorter.addPanelGroups(pg0, pg1, pg1a, pg2, pg3, pg4, pg5, pg6, pg7);
        }

        /// <summary>
        /// We'll need a cleaner method later of resetting the game.
        /// </summary>
        public static void reset()
        {
            foreach (Sprite sprite in components)
            {
                sprite.setVisible(false);
                if (sprite is PopInOutSprite)
                {
                    PopInOutSprite pSprite = (PopInOutSprite)sprite;
                    pSprite.setX(pSprite.getHideX());
                    pSprite.setY(pSprite.getHideY());
                }
                if (sprite is Panel)
                {
                    Panel panel = (Panel)sprite;
                    panel.reset();
                }
            }            
            while (panelStack.Count > 0 && menuOpen)
            {
                goBack();
            }
            menuArrow.setX(-175);
            menuArrow.setY(67);
        }

        public static void refreshPanelLists()
        {
            nullCheck();
            inventoryScreen.refreshList();
            placesScreen.refreshJournalList(null, null);
            peopleScreen.refreshJournalList(null, null);
            questScreen.refreshJournalList(null, null);
        }

        public static void loadContent(ContentManager content)
        {
            nullCheck();
            foreach (Sprite sprite in components)
            {
                sprite.loadContent(content);
            }
        }
        public static void update(float elapsed)
        {
            nullCheck();
            foreach (Sprite sprite in components)
            {
                sprite.update(elapsed);
            }
        }

        public static void draw(SpriteBatch sb)
        {
            nullCheck();
            foreach (Sprite sprite in components)
            {
                sprite.draw(sb);
            }
        }

        public static void open()
        {
            if (menuOpen)
                return;
            nullCheck();
            panelStack.Clear();
            activePanel = menuBody;
            panelStack.Push(activePanel);
            menuArrow.moveToActivePanel(activePanel);
            activePanel.setHighlighted(true);
            menuOpen = true;
            popIn();
        }

        private static void popIn()
        {
            nullCheck();
            menuBody.popIn();
            menuArrow.setPopLocations(menuBody.getHideX() + menuBody.getCurrentEntry().getX() - 25,
                menuBody.getHideY() + menuBody.getCurrentEntry().getY(),
                menuBody.getAppearX() + menuBody.getCurrentEntry().getX() - 25, 
                menuBody.getAppearY() + menuBody.getCurrentEntry().getY());        //NOTE: constants
            menuArrow.popIn();
        }

        public static void close()
        {
            if (!menuOpen)
                return;
            nullCheck();
            while (panelStack.Count > 0)
            {
                goBack();
            }
            menuOpen = false;
            //menuArrow.moveToActivePanel(menuBody);
            popOut();
        }

        private static void popOut()
        {
            menuBody.popOut();
            menuArrow.setPopLocations(menuBody.getHideX() + menuBody.getCurrentEntry().getX() - 25,
                menuBody.getHideY() + menuBody.getCurrentEntry().getY(),
                menuArrow.getX(), menuArrow.getY());        //NOTE: constants
            menuArrow.popOut();
        }

        public static bool isOpen()
        {
            return menuOpen;
        }
        public static bool isExiting()
        {
            return menuBody.isMovingOut();
        }

        public static void moveCursor(Direction dir)
        {
            if (!menuOpen)
                return;
            nullCheck();
            activePanel.onMoveCursor(dir);
            menuArrow.updateCursor();
        }

        public static void confirm()
        {
            if (!menuOpen)
                return;
            nullCheck();
            activePanel.onConfirm();
        }

        public static void goBack()
        {
            if (!menuOpen)
                return;
            nullCheck();
            Panel popped = panelStack.Pop();
            popped.onExit();
            PanelGroupSorter.panelOut(popped);
            if (panelStack.Count == 0)
            {
                close();
            }
            else
            {
                activePanel = panelStack.Peek();
                activePanel.onRefocus();
                menuArrow.moveToActivePanel(activePanel);
            }
        }

        /// <summary>
        /// Used for changing key controls
        /// </summary>
        /// <param name="key"></param>
        public static bool sendOneKeyInput(Keys key)
        {
            nullCheck();
            if (!menuOpen)
                return false;
            return activePanel.onKeyInput(key);
        }

        public static void pushActivePanel(Panel panel)
        {
            nullCheck();
            if (panel == null)
                return;

            activePanel.onUnfocus();
            activePanel = panel;
            panelStack.Push(activePanel);
            PanelGroupSorter.panelIn(activePanel);
            activePanel.onFocus();
            menuArrow.moveToActivePanel(activePanel);
        }

        private static void nullCheck()
        {
            if (!initialized)
            {
                System.Diagnostics.Debug.WriteLine("Call InGameMenu.init() on the creation cycle!");
                InGameMenu.init();
            }
        }
    }
}
