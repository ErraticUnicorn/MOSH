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
        public string roomName;             //The current room
        public Keys[] inputKeys;            //custom keys for input
        public QuestState[] questStates;        //triggers for how far along in quests we are
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
            data.roomName = WorldManager.m_currentRoomName;
            data.playTime = GameClock.getSaveStructure();
            return data;
        }

        public static void unpackData(SaveGameData data)
        {
            Hero h1 = Hero.instance;
            h1.loadSaveStructure(data.heroData);
            KeyboardManager.loadKeyControls(data.inputKeys);
            Quest.loadQuestStateSave(data.questStates);
            WorldManager.setRoomNoTransition(data.roomName);
            GameClock.loadSaveStructure(data.playTime);
            InGameMenu.refreshPanelLists();
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
