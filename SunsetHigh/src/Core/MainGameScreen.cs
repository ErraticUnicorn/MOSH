using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace SunsetHigh
{
    public class MainGameScreen : AbstractScreen
    {
        private const float SCALE_FACTOR = 1.0f;    //other factors break the panels
        Hero h1;

        public MainGameScreen()
        {
            h1 = Hero.instance;
            h1.setX(32 * 2);
            h1.setY(32 * 6);

            h1.inventory.addItem(Item.PokeBall, 3);
            h1.inventory.addItem(Item.LunchMoney, 1);
            h1.inventory.addItem(Item.Hat);
            h1.inventory.addItem(Item.Shoes);
            h1.inventory.addItem(Item.Socks);
            h1.inventory.removeItem(Item.Hat);

            InGameMenu.init();
            InGameMenu.refreshPanelLists();

            Quest.setQuestAvailable(QuestID.FoodFight);
        }

        public override void loadContent(ContentManager content)
        {
            WorldManager.loadMaps(content);

            //In the future, all Sprites will call loadContent(this.Content), and child
            //classes will override that method to automatically choose the appropriate 
            //content to load (i.e. both images and sound)
            h1.loadImage(content, "red_spritesheet", 4, 3, 0.25f);
            h1.loadContent(content);

            InGameMenu.loadContent(content);
            LocationNamePanel.instance.loadContent(content);
        }

        public override void update(float elapsed)
        {
            // Teleport character to new room
            WorldManager.handleWarp(h1);
            // Update the offset based on new room / character position
            WorldManager.updateCameraOffset(h1);

            // Keyboard listening
            if (!InGameMenu.isOpen())
            {
                KeyboardManager.handleInteractions(h1, WorldManager.m_currentRoom.Interactables);
                if (!h1.isTalking())
                {
                    KeyboardManager.handleCharacterMovement(h1, elapsed);
                    KeyboardManager.handlePickpocketing(h1, WorldManager.m_currentRoom.CharList);
                    KeyboardManager.handleShooting(h1);
                }
            }
            if (!h1.isTalking())
                KeyboardManager.handleInGameMenu();

            // Updates based on time
            InGameMenu.update(elapsed);
            if (!InGameMenu.isOpen())
            {
                LocationNamePanel.instance.update(elapsed);
                h1.update(elapsed);
                WorldManager.update(elapsed);
            }

            //DEBUG
            IInteractable interactable = CollisionManager.collisionWithInteractableAtRelative(Hero.instance, Point.Zero, Hero.instance);
            if (interactable != null)
            {
                interactable.onCollide();
            }
            //end DEBUG
        }

        public override void draw(SpriteBatch sb)
        {
            WorldManager.drawMap(sb);

            h1.draw(sb);

            LocationNamePanel.instance.draw(sb);
            InGameMenu.draw(sb);
        }

        public override void refresh()
        {
            BGMusic.transitionToSong(Directories.MUSIC + "sunset high ambient.mp3");
        }
    }
}
