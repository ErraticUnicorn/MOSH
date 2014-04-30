using System;
using System.IO;
using System.Security;
using System.Security.Cryptography;
using System.Text;
using System.Xml.Serialization;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Input;

namespace SunsetHigh
{
    /// <summary>
    /// Stores all the information of a save state.
    /// </summary>
    public class SaveGameData
    {
        public string fileName;             //The file name of this save game data
        public ClockSave playTime;           //The play time logged in this file
        public HeroSave heroData;           //Data about the Hero (Inventory, position, etc.)
        public PlaceID roomName;             //The current room
        public Keys[] inputKeys;            //custom keys for input
        public QuestState[] questStates;        //triggers for how far along in quests we are
        public CharacterBankSave characterStates;   //the locations of all the NPCs
    }

    /// <summary>
    /// Static class for managing save states. Saves games by serializing SaveGameData as XML and then 
    /// encrypting it. The encryption prevents players from easily accessing and "hacking" the XML files.
    /// </summary>
    public static class SaveManager
    {
        private const string CRYPT_KEY = "m!oG0p\nL";

        /// <summary>
        /// Saves a game to an external file in the "SaveData" folder. 
        /// By default the save file is encrypted.
        /// </summary>
        /// <param name="fileName">Name to give saved data file</param>
        /// <param name="saveGame">Data to be saved</param>
        /// <param name="encrypt">Optional, specifies whether output file should be encrypted (True by default)</param>
        public static bool saveGame(string fileName, SaveGameData saveGame, bool encrypt=true)
        {
            if (!Directory.Exists(Directories.SAVEDATA))
                Directory.CreateDirectory(Directories.SAVEDATA);
            if (fileName.StartsWith(Directories.SAVEDATA))
                fileName = fileName.Substring(Directories.SAVEDATA.Length);

            try
            {
                // unencrypted method (stable)
                if (!encrypt)
                {
                    Stream fStream = new FileStream(Directories.SAVEDATA + fileName, FileMode.Create);
                    XmlSerializer serializer = new XmlSerializer(typeof(SaveGameData));
                    serializer.Serialize(fStream, saveGame);
                    fStream.Close();
                }
                // encrypted method
                else
                {
                    Stream memStream = new MemoryStream();
                    XmlSerializer serializer = new XmlSerializer(typeof(SaveGameData));
                    serializer.Serialize(memStream, saveGame);
                    encryptFile(memStream, Directories.SAVEDATA + fileName);
                    memStream.Close();
                }
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine(e.StackTrace);
                return false;
            }
            return true;
        }

        public static void saveGame(string fileName, bool encrypt = true)
        {
            saveGame(fileName, packData(), encrypt);
        }

        /// <summary>
        /// Loads a game with a given file name in the "SaveData" folder.
        /// By default the method will attempt to decrypt the file.
        /// </summary>
        /// <param name="fileName">Name of the file to load</param>
        /// <param name="encrypt">Optional, specifies whether to decrypt the file</param>
        /// <returns>The save game data to load, or null if there are errors in reading/decryption</returns>
        public static SaveGameData getSaveData(string fileName, bool encrypt=true)
        {
            SaveGameData saveGame = null;

            if (fileName.StartsWith(Directories.SAVEDATA))
                fileName = fileName.Substring(Directories.SAVEDATA.Length);

            try
            {
                // unencrypted method (stable)
                if (!encrypt)
                {
                    Stream fStream = new FileStream(Directories.SAVEDATA + fileName, FileMode.Open);
                    XmlSerializer serializer = new XmlSerializer(typeof(SaveGameData));
                    saveGame = (SaveGameData)serializer.Deserialize(fStream);
                    fStream.Close();
                }
                // encrypted method
                else
                {
                    Stream memStream = decryptFile(Directories.SAVEDATA + fileName, false);
                    if (memStream != null)
                    {
                        XmlSerializer serializer = new XmlSerializer(typeof(SaveGameData));
                        saveGame = (SaveGameData)serializer.Deserialize(memStream);
                        memStream.Flush();
                        memStream.Close();
                    }
                }
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine(e.StackTrace);
                return null;
            }

            return saveGame;
        }

        public static void loadGame(string fileName, bool encrypt = true)
        {
            SaveGameData data = getSaveData(fileName, encrypt);
            if (data != null)
                unpackData(data);
        }

        public static SaveGameData packData()
        {
            SaveGameData data = new SaveGameData();
            Hero h1 = Hero.instance;
            data.heroData = h1.getSaveStructure();
            data.inputKeys = KeyboardManager.getKeyControls();
            data.questStates = Quest.getQuestStateSave();
            data.roomName = WorldManager.m_currentRoomID;
            data.playTime = GameClock.getSaveStructure();
            data.characterStates = CharacterManager.getSaveStructure();
            return data;
        }

        public static void unpackData(SaveGameData data)
        {
            Hero h1 = Hero.instance;
            h1.loadSaveStructure(data.heroData);
            KeyboardManager.loadKeyControls(data.inputKeys);
            Quest.loadQuestStateSave(data.questStates);
            WorldManager.clearMaps();
            WorldManager.setRoomNoTransition(data.roomName);
            GameClock.loadSaveStructure(data.playTime);
            CharacterManager.loadSaveStructure(data.characterStates);
            InGameMenu.refreshPanelLists();
        }

        public static void unpackDefaultData()
        {
            HeroSave hSave = new HeroSave();
            hSave.x = 25 * 32;
            hSave.y = 4 * 32;
            hSave.dir = Direction.North;
            hSave.inventorySave = new Inventory().getSaveStructure();
            hSave.monologueSave = new InnerMonologue().getSaveStructure();
            hSave.name = "No name";
            hSave.reputationSave = new int[] { 0, 0, 0, 0, 0 };
            hSave.followerID = PersonID.None;
            Hero.instance.loadSaveStructure(hSave);

            KeyboardManager.loadDefaultKeys();

            QuestState[] defQuests = new QuestState[Quest.NUM_QUEST_IDS];   //should all be 0 when initialized
            defQuests[(int)QuestID.FoodFight] = QuestState.Available;
            Quest.loadQuestStateSave(defQuests);

            WorldManager.clearMaps();
            WorldManager.setRoomNoTransition(PlaceID.StudentLounge);

            GameClock.renewClock();

            CharacterBankSave characterSave = new CharacterBankSave();
            characterSave.char_x = new int[CharacterManager.NUM_PERSON_IDS];
            characterSave.char_y = new int[CharacterManager.NUM_PERSON_IDS];
            characterSave.char_room = new PlaceID[CharacterManager.NUM_PERSON_IDS];

            characterSave.char_x[(int)PersonID.Phil] = 23 * 32;
            characterSave.char_y[(int)PersonID.Phil] = 7 * 32;
            characterSave.char_room[(int)PersonID.Phil] = PlaceID.Cafeteria;

            characterSave.char_x[(int)PersonID.Librarian] = 4 * 32;
            characterSave.char_y[(int)PersonID.Librarian] = 4 * 32;
            characterSave.char_room[(int)PersonID.Librarian] = PlaceID.Library;

            characterSave.char_x[(int)PersonID.Artie] = 12 * 32;
            characterSave.char_y[(int)PersonID.Artie] = 8 * 32;
            characterSave.char_room[(int)PersonID.Artie] = PlaceID.Cafeteria;

            characterSave.char_x[(int)PersonID.Bill] = 16 * 32;
            characterSave.char_y[(int)PersonID.Bill] = 14 * 32;
            characterSave.char_room[(int)PersonID.Bill] = PlaceID.Cafeteria;

            characterSave.char_x[(int)PersonID.Claude] = 25 * 32;
            characterSave.char_y[(int)PersonID.Claude] = 18 * 32;
            characterSave.char_room[(int)PersonID.Claude] = PlaceID.Cafeteria;

            characterSave.char_x[(int)PersonID.Jay1] = 4 * 32;
            characterSave.char_y[(int)PersonID.Jay1] = 18 * 32;
            characterSave.char_room[(int)PersonID.Jay1] = PlaceID.Cafeteria;

            characterSave.char_x[(int)PersonID.Jay2] = 10 * 32;
            characterSave.char_y[(int)PersonID.Jay2] = 2 * 32;
            characterSave.char_room[(int)PersonID.Jay2] = PlaceID.Cafeteria;

            characterSave.char_x[(int)PersonID.Jay3] = 14 * 32;
            characterSave.char_y[(int)PersonID.Jay3] = 18 * 32;
            characterSave.char_room[(int)PersonID.Jay3] = PlaceID.Cafeteria;

            characterSave.char_x[(int)PersonID.Jay4] = 20 * 32;
            characterSave.char_y[(int)PersonID.Jay4] = 2 * 32;
            characterSave.char_room[(int)PersonID.Jay4] = PlaceID.Cafeteria;

            characterSave.char_x[(int)PersonID.BraceFace] = 10 * 32;
            characterSave.char_y[(int)PersonID.BraceFace] = 10 * 32;
            characterSave.char_room[(int)PersonID.BraceFace] = PlaceID.Math;

            characterSave.char_x[(int)PersonID.Fitz] = 19 * 32;
            characterSave.char_y[(int)PersonID.Fitz] = 13 * 32;
            characterSave.char_room[(int)PersonID.Fitz] = PlaceID.ComputerLab;

            characterSave.char_x[(int)PersonID.ROB] = 8 * 32;
            characterSave.char_y[(int)PersonID.ROB] = 11 * 32;
            characterSave.char_room[(int)PersonID.ROB] = PlaceID.Science;

            characterSave.char_x[(int)PersonID.Enforcer] = 11 * 32;
            characterSave.char_y[(int)PersonID.Enforcer] = 49 * 32;
            characterSave.char_room[(int)PersonID.Enforcer] = PlaceID.HallwayWest;

            characterSave.char_x[(int)PersonID.Tyke] = 11 * 32;
            characterSave.char_y[(int)PersonID.Tyke] = 6 * 32;
            characterSave.char_room[(int)PersonID.Tyke] = PlaceID.Entrance;

            characterSave.char_x[(int)PersonID.Dyke] = 18 * 32;
            characterSave.char_y[(int)PersonID.Dyke] = 11 * 32;
            characterSave.char_room[(int)PersonID.Dyke] = PlaceID.StudentLounge;

            characterSave.char_x[(int)PersonID.Shaq] = 23 * 32;
            characterSave.char_y[(int)PersonID.Shaq] = 3 * 32;
            characterSave.char_room[(int)PersonID.Shaq] = PlaceID.HallwayEast;

            characterSave.char_x[(int)PersonID.Herbert] = 3 * 32;
            characterSave.char_y[(int)PersonID.Herbert] = 40 * 32;
            characterSave.char_room[(int)PersonID.Herbert] = PlaceID.HallwayWest;

            characterSave.char_x[(int)PersonID.Avery] = 8 * 32;
            characterSave.char_y[(int)PersonID.Avery] = 7 * 32;
            characterSave.char_room[(int)PersonID.Avery] = PlaceID.StudentLounge;

            characterSave.char_x[(int)PersonID.Boris] = 25 * 32;
            characterSave.char_y[(int)PersonID.Boris] = 5 * 32;
            characterSave.char_room[(int)PersonID.Boris] = PlaceID.Entrance;

            characterSave.char_x[(int)PersonID.Jarvis] = 17 * 32;
            characterSave.char_y[(int)PersonID.Jarvis] = 15 * 32;
            characterSave.char_room[(int)PersonID.Jarvis] = PlaceID.Entrance;

            characterSave.char_x[(int)PersonID.Zeke] = 10 * 32;
            characterSave.char_y[(int)PersonID.Zeke] = 4 * 32;
            characterSave.char_room[(int)PersonID.Zeke] = PlaceID.Bathroom;

            /*
        Fitz,
        ROB,
        Enforcer,
        Tyke,
        Dyke,
        Shaq,
        Herbert,
        Avery,
        Boris,
        Jarvis,
        Zeke
             */

            CharacterManager.loadSaveStructure(characterSave);
        }

        public static string generateNewFileName()
        {
            if (!Directory.Exists(Directories.SAVEDATA))
                Directory.CreateDirectory(Directories.SAVEDATA);
            string prefix = "sunset";
            string suffix = ".sav";
            int num = 1;
            for (; File.Exists(Directories.SAVEDATA + prefix + num + suffix); num++) ;
            return prefix + num + suffix;
        }

        /// <summary>
        /// Loads all save files in the "SaveData" folder. Can be used
        /// for previewing save files in the title screen
        /// </summary>
        /// <returns>A list of all save game data in the "SaveData" folder. Can be empty</returns>
        public static List<SaveGameData> loadAllGames(bool encrypt = true)
        {
            List<SaveGameData> list = new List<SaveGameData>();
            if (Directory.Exists(Directories.SAVEDATA))
            {
                string[] files = Directory.GetFiles(Directories.SAVEDATA);
                for (int i = 0; i < files.Length; i++)
                {
                    SaveGameData data = getSaveData(files[i].Substring(Directories.SAVEDATA.Length), encrypt);
                    if (data != null)
                    {
                        list.Add(data);
                    }
                }
            }
            else
            {
                System.Diagnostics.Debug.WriteLine("Could not find \"SaveData\" directory!");
            }
            return list;
        }

        //Encryption prevents simple "hacking" of the xml save files
        private static bool encryptFile(Stream streamIn, string fileNameOut)
        {
            try
            {
                //Get unencrypted data from MemoryStream
                streamIn.Position = 0;
                byte[] bytearrayinput = new byte[streamIn.Length];
                streamIn.Read(bytearrayinput, 0, bytearrayinput.Length);
                streamIn.Flush();
                streamIn.Close();

                //Encrypt while writing to file
                FileStream fsEncrypted = new FileStream(fileNameOut,
                    FileMode.Create,
                    FileAccess.Write);
                DESCryptoServiceProvider DES = new DESCryptoServiceProvider();
                DES.Key = ASCIIEncoding.ASCII.GetBytes(CRYPT_KEY);
                DES.IV = ASCIIEncoding.ASCII.GetBytes(CRYPT_KEY);
                ICryptoTransform desencrypt = DES.CreateEncryptor();
                CryptoStream cryptostream = new CryptoStream(fsEncrypted,
                                    desencrypt,
                                    CryptoStreamMode.Write);
                cryptostream.Write(bytearrayinput, 0, bytearrayinput.Length);
                cryptostream.Flush();
                fsEncrypted.Flush();
                cryptostream.Close();
                fsEncrypted.Close();
                return true;
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine("ERROR in save file encryption!\n" + e.StackTrace);
                return false;
            }
        }

        private static Stream decryptFile(string fileNameIn, bool debug=false)
        {
            try
            {
                //Get encrypted data
                FileStream fsInput = new FileStream(fileNameIn,
                    FileMode.Open,
                    FileAccess.Read);
                byte[] bytearrayinput = new byte[fsInput.Length];
                fsInput.Read(bytearrayinput, 0, bytearrayinput.Length);
                fsInput.Flush();
                fsInput.Close();

                //Unencrypt while writing to a memory stream
                MemoryStream memStream = new MemoryStream();
                DESCryptoServiceProvider DES = new DESCryptoServiceProvider();
                DES.Key = ASCIIEncoding.ASCII.GetBytes(CRYPT_KEY);
                DES.IV = ASCIIEncoding.ASCII.GetBytes(CRYPT_KEY);
                ICryptoTransform desdecrypt = DES.CreateDecryptor();
                CryptoStream cryptostreamDecr = new CryptoStream(memStream,
                                                     desdecrypt,
                                                     CryptoStreamMode.Write);
                cryptostreamDecr.Write(bytearrayinput, 0, bytearrayinput.Length);
                cryptostreamDecr.Flush();
                cryptostreamDecr.FlushFinalBlock();
                //cryptostreamDecr.Close();             //causes memStream to close

                if (debug)
                {
                    memStream.Position = 0;
                    byte[] tempoutput = new byte[memStream.Length];
                    memStream.Read(tempoutput, 0, tempoutput.Length);
                    memStream.Flush();
                    FileStream fStream = new FileStream(fileNameIn.Substring(0, fileNameIn.Length - 4) + "_temp.sav", FileMode.Create);
                    fStream.Write(tempoutput, 0, tempoutput.Length);
                    fStream.Flush();
                    fStream.Close();
                }

                memStream.Position = 0;
                return memStream;
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine("ERROR in save file decryption!\n" + e.StackTrace);
                return null;
            }
        }
    }
}
