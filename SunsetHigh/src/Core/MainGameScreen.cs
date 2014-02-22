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

            InGameMenu.init();
            InGameMenu.refreshPanelLists();
        }

        public override void loadContent(ContentManager content)
        {
            WorldManager.loadMaps(content);
            CharacterManager.loadContent(content);

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
            //DEBUG
            if (KeyboardManager.isKeyPressed(Microsoft.Xna.Framework.Input.Keys.Q))
                Hero.instance.setSpeed(Hero.instance.getSpeed() * 2);
            if (KeyboardManager.isKeyPressed(Microsoft.Xna.Framework.Input.Keys.W))
                Hero.instance.setSpeed(Hero.instance.getSpeed() / 2);
            //END DEBUG

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
            IInteractable interactable2 = null;
            if (Hero.instance.hasFollower()) 
                interactable2 = CollisionManager.collisionWithInteractableAtRelative(
                    CharacterManager.getCharacter(Hero.instance.getFollowerID()), Point.Zero, CharacterManager.getCharacter(Hero.instance.getFollowerID()));
            if (interactable != null)
            {
                interactable.onCollide();
            }
            else if (interactable2 != null)
            {
                interactable2.onCollide();
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
