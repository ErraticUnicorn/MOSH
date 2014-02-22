using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Content;

namespace SunsetHigh
{
    /// <summary>
    /// Specifies a certain person (used for linking external text files to objects in the code)
    /// </summary>
    public enum PersonID
    {
        None = -1,
        Phil = 0,
        Librarian,
        Bill,
        Artie,
        Claude,
        Jay1,
        Jay2,
        Jay3,
        Jay4
    }

    public struct CharacterBankSave
    {
        public int[] char_x;
        public int[] char_y;
        public PlaceID[] char_room;
    }

    public static class CharacterManager
    {
        public static int NUM_PERSON_IDS = Enum.GetValues(typeof(PersonID)).Length - 1;

        private static Dictionary<PersonID, Character> mCharMap;
        private static Dictionary<PersonID, PlaceID> mRoomMap;

        public static void loadContent(ContentManager content)
        {
            mCharMap = new Dictionary<PersonID, Character>();
            mRoomMap = new Dictionary<PersonID, PlaceID>();

            //initialize all the games characters
            //NOTE: their initial positions are specified in SaveManager's unpackDefaultData class
            Character phil = new Character();
            phil.loadImage(content, Directories.CHARACTERS + "sprite_ffwriter");
            phil.setScript(Directories.INTERACTIONS + "Phil.txt");
            mCharMap[PersonID.Phil] = phil;

            Character librarian = new Character();
            librarian.loadImage(content, Directories.CHARACTERS_TEMP + "teacher");
            librarian.setScript(Directories.INTERACTIONS + "Librarian.txt");
            mCharMap[PersonID.Librarian] = librarian;

            Character bill = new Character();
            bill.loadImage(content, Directories.CHARACTERS_TEMP + "jock");
            bill.setScript(Directories.INTERACTIONS + "Bill.txt");
            mCharMap[PersonID.Bill] = bill;

            Character artie = new Character();
            artie.loadImage(content, Directories.CHARACTERS_TEMP + "prep");
            artie.setScript(Directories.INTERACTIONS + "Artie.txt");
            mCharMap[PersonID.Artie] = artie;

            Character claude = new Character();
            claude.loadImage(content, Directories.CHARACTERS_TEMP + "slacker");
            claude.setScript(Directories.INTERACTIONS + "Claude.txt");
            mCharMap[PersonID.Claude] = claude;

            Character jay1 = new Character();
            jay1.loadImage(content, Directories.CHARACTERS_TEMP + "bully");
            jay1.setScript(Directories.INTERACTIONS + "Jay1.txt");
            mCharMap[PersonID.Jay1] = jay1;
            Character jay2 = new Character();
            jay2.loadImage(content, Directories.CHARACTERS_TEMP + "bully");
            jay2.setScript(Directories.INTERACTIONS + "Jay2.txt");
            mCharMap[PersonID.Jay2] = jay2;
            Character jay3 = new Character();
            jay3.loadImage(content, Directories.CHARACTERS_TEMP + "bully");
            jay3.setScript(Directories.INTERACTIONS + "Jay3.txt");
            mCharMap[PersonID.Jay3] = jay3;
            Character jay4 = new Character();
            jay4.loadImage(content, Directories.CHARACTERS_TEMP + "bully");
            jay4.setScript(Directories.INTERACTIONS + "Jay4.txt");
            mCharMap[PersonID.Jay4] = jay4;
        }

        public static Character getCharacter(PersonID id)
        {
            return mCharMap[id];
        }

        public static void moveCharacterToRoom(PersonID person, PlaceID place, int x, int y)
        {
            Character c1 = mCharMap[person];
            WorldManager.removeObjectFromRoom(c1, mRoomMap[person]);
            WorldManager.addObjectToRoom(c1, place);
            mRoomMap[person] = place;
            c1.setX(x);
            c1.setY(y);
            c1.reset();  
        }

        public static void moveCharacterToRoom(PersonID person, PlaceID place)
        {
            moveCharacterToRoom(person, place, mCharMap[person].getX(), mCharMap[person].getY());
        }

        public static CharacterBankSave getSaveStructure()
        {
            CharacterBankSave data = new CharacterBankSave();
            data.char_x = new int[NUM_PERSON_IDS];
            data.char_y = new int[NUM_PERSON_IDS];
            data.char_room = new PlaceID[NUM_PERSON_IDS];
            for (int i = 0; i < NUM_PERSON_IDS; i++)
            {
                Character c1 = mCharMap[(PersonID)i];
                data.char_x[i] = c1.getX();
                data.char_y[i] = c1.getY();
                data.char_room[i] = mRoomMap[(PersonID)i];
            }
            return data;
        }

        public static void loadSaveStructure(CharacterBankSave save)
        {
            for (int i = 0; i < NUM_PERSON_IDS && i < save.char_x.Length; i++)
            {
                Character c1 = mCharMap[(PersonID)i];
                c1.setX(save.char_x[i]);
                c1.setY(save.char_y[i]);
                c1.reset();
                if (mRoomMap.ContainsKey((PersonID)i))
                    WorldManager.removeObjectFromRoom(c1, mRoomMap[(PersonID)i]);
                WorldManager.addObjectToRoom(c1, save.char_room[i]);
                mRoomMap[(PersonID)i] = save.char_room[i];
            }
        }
    }
}
