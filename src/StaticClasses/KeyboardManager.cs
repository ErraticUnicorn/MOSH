using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Input;

namespace SunsetHigh
{
    /// <summary>
    /// Specifies game functions that are controlled through keyboard input
    /// </summary>
    public enum KeyInputType
    {
        MoveNorth = 0,
        MoveEast,
        MoveSouth,
        MoveWest,
        Pickpocket,
        Shoot,
        Talk,
        Action,
        Cancel,
        MenuToggle,
        //others
    }

    /// <summary>
    /// Static class that has three purposes: 1) To easily tell if a key is
    /// held down or has been just pressed, 2) To handle complex keyboard input for the game
    /// with simple method calls, 3) To store custom key controls for the game.
    /// </summary>
    public static class KeyboardManager
    {
        private static int NUM_KEY_TYPES = Enum.GetValues(typeof(KeyInputType)).Length;
        private static Keys[] keyTypes;

        private static KeyboardState priorState;
        private static KeyboardState currentState;

        /// <summary>
        /// Updates the keyboard input; call this method in the Game's update cycle
        /// </summary>
        public static void update()
        {
            priorState = currentState; 
            currentState = Keyboard.GetState();
        }

        /// <summary>
        /// Checks if the given key is currently down (pressed)
        /// </summary>
        /// <param name="key">The key to check for</param>
        /// <returns>True if the key is down, false if not</returns>
        public static bool isKeyDown(Keys key)
        {
            if (currentState == null || priorState == null)
                return false;
            return currentState.IsKeyDown(key);
        }
        /// <summary>
        /// Checks if the given key is currently up (not pressed)
        /// </summary>
        /// <param name="key">The key to check for</param>
        /// <returns>True if the key is up, false if not</returns>
        public static bool isKeyUp(Keys key)
        {
            if (currentState == null || priorState == null)
                return false;
            return currentState.IsKeyUp(key);
        }
        /// <summary>
        /// Checks if the given key has just been pressed (i.e. it is down now and was
        /// not pressed in the previous update)
        /// </summary>
        /// <param name="key">The key to check for</param>
        /// <returns>True if the key has just been pressed, false if not</returns>
        public static bool isKeyPressed(Keys key)
        {
            if (currentState == null || priorState == null)
                return false;
            return currentState.IsKeyDown(key) && !priorState.IsKeyDown(key);
        }
        
        /// <summary>
        /// Checks if any new key has just been pressed (and was not down previously)
        /// </summary>
        /// <returns>True if any key has just been pressed, false if not</returns>
        public static bool isNewKeyPressed()
        {
            if (currentState == null || priorState == null)
                return false;
            return currentState.GetPressedKeys().Length - 1 == priorState.GetPressedKeys().Length;
        }

        /// <summary>
        /// Returns one key that has just been pressed (assuming it's the only key pressed)
        /// </summary>
        /// <returns>The only key that has just been pressed</returns>
        public static Keys getNewKeyPressed()
        {
            if (isNewKeyPressed())
            {
                foreach (Keys key in currentState.GetPressedKeys())
                {
                    if (!priorState.GetPressedKeys().Contains(key))
                    {
                        return key;
                    }
                }
            }
            System.Diagnostics.Debug.WriteLine("Only call this method if isOneKeyPressed() is valid!");
            return currentState.GetPressedKeys()[0];
        }

        /// <summary>
        /// Checks if the given key has just been released (i.e. it is up now and was
        /// pressed in the previous update)
        /// </summary>
        /// <param name="key">The key to check for</param>
        /// <returns>True if the key has just been released, false if not</returns>
        public static bool isKeyUnpressed(Keys key)
        {
            if (currentState == null || priorState == null)
                return false;
            return currentState.IsKeyUp(key) && !priorState.IsKeyUp(key);
        }

        /// <summary>
        /// A check for whether the given key is already registered with a certain
        /// type of input. Called when customizing key controls
        /// </summary>
        /// <param name="key">The possible key to shift the input type to</param>
        /// <param name="exclude">Input type to exclude (i.e. the input being considered)</param>
        /// <returns>True if another input type uses the key, false otherwise</returns>
        public static bool keyControlExists(Keys key, KeyInputType exclude)
        {
            for (int i = 0; i < keyTypes.Length; i++)
            {
                if ((int)exclude == i) continue;
                if (keyTypes[i] == key) return true;
            }
            return false;
        }

        /// <summary>
        /// Change the key input needed to trigger a given game function; used for 
        /// customizing key controls
        /// </summary>
        /// <param name="inputType">The game function</param>
        /// <param name="key">The new key to press</param>
        public static void changeKeyControl(KeyInputType inputType, Keys key)
        {
            nullCheck();
            keyTypes[(int)inputType] = key;
        }

        /// <summary>
        /// Returns the current Key mapped to this KeyInputType
        /// </summary>
        public static Keys getKeyControl(KeyInputType inputType)
        {
            nullCheck();
            return keyTypes[(int)inputType];
        }

        /// <summary>
        /// Used for loading in custom key controls when restoring a saved game
        /// </summary>
        /// <param name="loadableKeys">A Keys[] representation of the key controls</param>
        public static void loadKeyControls(Keys[] loadableKeys)
        {
            nullCheck();
            if (loadableKeys.Length == keyTypes.Length)
                loadableKeys.CopyTo(keyTypes, 0);
            else
                System.Diagnostics.Debug.WriteLine("Invalid array input; size must be same");
        }
        
        /// <summary>
        /// Used for loading the default key controls when starting a game (or resetting controls)
        /// </summary>
        public static void loadDefaultKeys()
        {
            nullCheck();
            keyTypes[(int)KeyInputType.MoveNorth] = Keys.Up;
            keyTypes[(int)KeyInputType.MoveEast] = Keys.Right;
            keyTypes[(int)KeyInputType.MoveSouth] = Keys.Down;
            keyTypes[(int)KeyInputType.MoveWest] = Keys.Left;
            keyTypes[(int)KeyInputType.Pickpocket] = Keys.S;
            keyTypes[(int)KeyInputType.Shoot] = Keys.A;
            keyTypes[(int)KeyInputType.Talk] = Keys.F;
            keyTypes[(int)KeyInputType.Action] = Keys.Z;
            keyTypes[(int)KeyInputType.Cancel] = Keys.X;
            keyTypes[(int)KeyInputType.MenuToggle] = Keys.Space;
        }

        /// <summary>
        /// Used for saving purposes only
        /// </summary>
        /// <returns>A Keys[] representation of the key controls</returns>
        public static Keys[] getKeyControls()
        {
            nullCheck();
            return keyTypes;
        }

        /// <summary>
        /// Ts Character movement in 8 directions; call this method in the Game's update
        /// cycle (AFTER calling KeyboardManager.update())
        /// </summary>
        /// <param name="character">The character to control (presumably main character)</param>
        /// <param name="elapsed">The time that elapsed since the last update</param>
        public static void handleCharacterMovement(Character character, float elapsed)
        {
            nullCheck();
            int dirFlags = 0;
            if (KeyboardManager.isKeyDown(keyTypes[(int)KeyInputType.MoveNorth]))
                dirFlags |= 1;
            if (KeyboardManager.isKeyDown(keyTypes[(int)KeyInputType.MoveSouth]))
                dirFlags |= 2;
            if (KeyboardManager.isKeyDown(keyTypes[(int)KeyInputType.MoveEast]))
                dirFlags |= 4;
            if (KeyboardManager.isKeyDown(keyTypes[(int)KeyInputType.MoveWest]))
                dirFlags |= 8;
            Direction xaxis = Direction.Undefined;
            Direction yaxis = Direction.Undefined;
            if ((dirFlags & 1) > 0 ^ (dirFlags & 2) > 0)
            {
                if ((dirFlags & 1) > 0) yaxis = Direction.North;
                else yaxis = Direction.South;
            }
            if ((dirFlags & 4) > 0 ^ (dirFlags & 8) > 0)
            {
                if ((dirFlags & 4) > 0) xaxis = Direction.East;
                else xaxis = Direction.West;
            }
            if (!xaxis.Equals(Direction.Undefined) && !yaxis.Equals(Direction.Undefined))
            {
                if (yaxis.Equals(Direction.North))
                    if (xaxis.Equals(Direction.East))
                        character.move(Direction.NorthEast, elapsed);
                    else
                        character.move(Direction.NorthWest, elapsed);
                else
                    if (xaxis.Equals(Direction.East))
                        character.move(Direction.SouthEast, elapsed);
                    else
                        character.move(Direction.SouthWest, elapsed);
            }
            else if (!xaxis.Equals(Direction.Undefined))
            {
                character.move(xaxis, elapsed);
            }
            else if (!yaxis.Equals(Direction.Undefined))
            {
                character.move(yaxis, elapsed);
            }
            else
            {
                character.setMoving(false);
            }
        }

        /// <summary>
        /// Handles Hero's "weapon" ability; call this method in the Game's update cycle (AFTER
        /// calling KeyboardManager.update())
        /// </summary>
        /// <param name="character">The main character</param>
        public static void handleShooting(Hero hero)
        {
            if (KeyboardManager.isKeyPressed(keyTypes[(int)KeyInputType.Shoot]))
            {
                hero.shoot();
            }
        }

        /// <summary>
        /// Handles the Hero's pickpocketing ability; call this method in the Game's update
        /// cycle (AFTER calling KeyboardManager.update())
        /// </summary>
        /// <param name="hero">The main character</param>
        /// <param name="targets">List of characters in the scene; one that is in range is chosen randomly</param>
        public static void handlePickpocketing(Hero hero, List<Character> targets)
        {
            nullCheck();
            if (KeyboardManager.isKeyPressed(keyTypes[(int)KeyInputType.Pickpocket]))
            {
                if (hero.isPickpocketing())
                {
                    Item item = hero.stopPickpocket();
                    System.Diagnostics.Debug.WriteLine("Stole " + Enum.GetName(typeof(Item), item));
                }
                else if (!hero.isPickpocketing())
                {
                    //naive search through a list of characters
                    // We should probably do this on a per-room basis later, for efficiency.
                    // And/or use some sort of "expanding circle" search.
                    for(int i = 0; i < targets.Count; i++)
                    {
                        if (hero.inRangeAction(targets[i]))
                        {
                            hero.startPickpocket(targets[i]);
                            break;
                        }
                    }
                }
            }
        }

        public static void handleInteractions(Hero hero, List<IInteractable> targets)
        {
            nullCheck();

            if (KeyboardManager.isKeyPressed(keyTypes[(int)KeyInputType.Talk])) //we will replace this with "action" key later
            {
                for (int i = 0; i < targets.Count; i++)
                {
                    if (hero.inRangeAction(targets[i]) && hero.facing(targets[i]))
                    {
                        targets[i].onInteract();
                        break;
                    }
                }
            }
        }

        public static void handleInGameMenu()
        {
            if (!InGameMenu.isOpen())
            {
                if (KeyboardManager.isKeyPressed(keyTypes[(int)KeyInputType.MenuToggle]))
                {
                    InGameMenu.open();
                }
            }
            else
            {
                bool blockKey = false;
                if (KeyboardManager.isNewKeyPressed())
                {
                    blockKey = InGameMenu.sendOneKeyInput(KeyboardManager.getNewKeyPressed()); //for custom key controls..
                    if (!blockKey)
                    {
                        if (KeyboardManager.isKeyPressed(keyTypes[(int)KeyInputType.MenuToggle]))
                            InGameMenu.close();
                        else if (KeyboardManager.isKeyPressed(keyTypes[(int)KeyInputType.MoveNorth]))
                            InGameMenu.moveCursor(Direction.North);
                        else if (KeyboardManager.isKeyPressed(keyTypes[(int)KeyInputType.MoveWest]))
                            InGameMenu.moveCursor(Direction.West);
                        else if (KeyboardManager.isKeyPressed(keyTypes[(int)KeyInputType.MoveSouth]))
                            InGameMenu.moveCursor(Direction.South);
                        else if (KeyboardManager.isKeyPressed(keyTypes[(int)KeyInputType.MoveEast]))
                            InGameMenu.moveCursor(Direction.East);
                        else if (KeyboardManager.isKeyPressed(keyTypes[(int)KeyInputType.Action]))
                            InGameMenu.confirm();
                        else if (KeyboardManager.isKeyPressed(keyTypes[(int)KeyInputType.Cancel]))
                            InGameMenu.goBack();
                    }
                }
            }
        }

        private static void nullCheck()
        {
            if (keyTypes == null || keyTypes.Length == 0)
            {
                keyTypes = new Keys[NUM_KEY_TYPES]; //initializes the array               
                loadDefaultKeys();
            }
        }
    }
}
